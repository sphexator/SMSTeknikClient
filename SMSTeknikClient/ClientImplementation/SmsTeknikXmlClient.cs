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
        XE xmlItems;

        var req = sendRequest.OutgoingSmsMessages.First();
        
        var xml = new XE("sms-teknik",
            new XE("operationtype", 0),
            new XE("smssender", req.From),
            new XE("multisms", 1),
            new XE("maxmultisms", 0),
            new XE("send_date", ""),
            new XE("send_time", ""),
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

        string result;

        if(responseMessage.IsSuccessStatusCode)
            result = await responseMessage.Content.ReadAsStringAsync();

        // Todo: handle result.

        throw new NotImplementedException();
    }

    // Move this method to a helper class
    private  string Base64Encode(string textToEncode)
    {
        byte[] textAsBytes = System.Text.Encoding.UTF8.GetBytes(textToEncode);
        return Convert.ToBase64String(textAsBytes);
    }
}