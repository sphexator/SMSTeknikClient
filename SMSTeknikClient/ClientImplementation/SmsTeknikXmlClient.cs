using System.Globalization;
using System.Xml;
using SMSTeknikClient.Config;
using SMSTeknikClient.Messages;
using XE = System.Xml.Linq.XElement;

namespace SMSTeknikClient.ClientImplementation;

public class SmsTeknikXmlClient : ISmsTeknikClient
{

    private static HttpClient _client = new HttpClient();
    
    private readonly SmsTeknikConfiguration _config;

    public SmsTeknikXmlClient(SmsTeknikConfiguration config) => _config = config;

    void IDisposable.Dispose() => 
        GC.SuppressFinalize(this);

    public async Task<MessageResponse> SendMessage(OutgoingSmsMessage message) =>
        (await SendRequest(new SendRequest(message))).MessageResponses.Single();

    public Task<SendResponse> SendMessageToMultipleRecipients(OutgoingSmsMessage message, string[] toMultipleRecipients)
    {
        var l = toMultipleRecipients.Select(x =>
        {
            var m = message.Clone();
            m.To = x;
            return m;
        }).ToArray();
        return SendRequest(new SendRequest(l));
    }

    public Task<SendResponse> SendMessages(params OutgoingSmsMessage[] messages) =>
        SendRequest(new SendRequest(messages));

    public async Task<SendResponse> SendRequest(SendRequest sendRequest)
    {
        // Validate required paramters
        if (sendRequest.OutgoingSmsMessages.Length == 0 || 
            sendRequest.OutgoingSmsMessages.Any(i => string.IsNullOrEmpty(i.Body)) ||
            sendRequest.OutgoingSmsMessages.Any(i => string.IsNullOrEmpty(i.From)) ||
            sendRequest.OutgoingSmsMessages.Any(i => string.IsNullOrEmpty(i.To)))
            throw new Exception("Required parameter is missing");

        XE xmlItems;

        var req = sendRequest.OutgoingSmsMessages.First();
        
        var xml = new XE("sms-teknik",
            new XE("operationtype", 0),
            new XE("smssender", req.From),
            new XE("multisms", 1),
            new XE("maxmultisms", 0),
            new XE("send_date", req.SendAt?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? ""),
            new XE("send_time", req.SendAt?.ToString("HH:dd:ss", CultureInfo.InvariantCulture) ?? ""),
            new XE("udmessage", req.Body),
            xmlItems = new XE("items")
        );

        if (!string.IsNullOrEmpty(req.StatusCallBackUrl))
            xml.Add(new XE("deliverystatustype", "3"),
                new XE("deliverystatusaddress", req.StatusCallBackUrl));

        xmlItems.Add(sendRequest.OutgoingSmsMessages.Select(outgoingMessage => new XE("recipient", new XE("nr", outgoingMessage.To))));

        _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Base64Encode($"{_config.Username}:{_config.Password}")}");


        var responseMessage = await _client.PostAsync("https://api.smsteknik.se/send/xml/", 
            new StringContent(xml.ToString(), System.Text.Encoding.UTF8, "application/xml"));

        string result = "";

        if (responseMessage.IsSuccessStatusCode)
            result = await responseMessage.Content.ReadAsStringAsync();
        else
            throw new Exception("The HTTP request was not successful!");

        
        return ParseResult(result, sendRequest.OutgoingSmsMessages);


    }

    private SendResponse ParseResult(string result, OutgoingSmsMessage[] outgoingSmsMessages)
    {

        var arrResult = result.Split(';');

        if(outgoingSmsMessages.Length > 1 && arrResult.Length == 1)
        {
            // We expected to receive multiple response codes (one for each message), but we only receive one.
            // This means that the entire request failed (eg. wrong credentials, parse error or quota exceeded).
            // We have two options: 1) throw exception, or 2) populate the same error message for each of the message response.
            // This scenario only applies when sending to multiple receivers.
            // Example error codes: 0:Access denied, 0:No Valid recipients
            
            throw new Exception("Error: " + arrResult.First().Substring(2));
        }
        
        if (outgoingSmsMessages.Length != arrResult.Length)
            throw new Exception("This should not have happened...");
        
        var messageResponses = new MessageResponse[outgoingSmsMessages.Length];

        for (int i = 0;i<messageResponses.Length; i++)
        {
            var resultItem = arrResult[i];

            bool success;
            string? errorMessage = null;
            long? smsId = null;

            if (resultItem.StartsWith("0:"))
            {
                // Error
                success = false;
                errorMessage = resultItem.Substring(2);
            }
            else
            {
                // Success
                success = true;
                long smsid = Convert.ToInt64(resultItem);
            }

            messageResponses[i] = new MessageResponse(outgoingSmsMessages[i], success, errorMessage, smsId);
        }

        return new SendResponse(messageResponses);

    }

    // Move this method to a helper class
    private  string Base64Encode(string textToEncode)
    {
        byte[] textAsBytes = System.Text.Encoding.UTF8.GetBytes(textToEncode);
        return Convert.ToBase64String(textAsBytes);
    }
}