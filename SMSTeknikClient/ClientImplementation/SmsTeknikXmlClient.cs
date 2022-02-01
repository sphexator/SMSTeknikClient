using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SMSTeknikClient.Config;
using SMSTeknikClient.Messages;
using static SMSTeknikClient.ClientImplementation.Validation;
using XE = System.Xml.Linq.XElement;

namespace SMSTeknikClient.ClientImplementation;

public class SmsTeknikXmlClient : ISmsTeknikClient
{
    private static readonly HttpClient Client = new();

    private readonly SmsTeknikConfiguration _config;

    public SmsTeknikXmlClient(SmsTeknikConfiguration config) => _config = config;

    void IDisposable.Dispose() =>
        GC.SuppressFinalize(this);

    public async Task<MessageResponse> SendMessage(OutgoingSmsMessage message) =>
        (await Send(new SendRequest(message))).MessageResponses.Single();

    public Task<SendResponse> SendMessageToMultipleRecipients(OutgoingSmsMessage message, string[] toMultipleRecipients)
    {
        var l = toMultipleRecipients.Select(x =>
        {
            var m = message.Clone();
            m.To = x;
            return m;
        }).ToArray();
        return Send(new SendRequest(l));
    }

    public Task<SendResponse> SendMessages(params OutgoingSmsMessage[] messages) =>
        Send(new SendRequest(messages));

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public async Task<SendResponse> Send(SendRequest sendRequest)
    {
        var validationErrors = GetValidationErrors(sendRequest);
        if (validationErrors.Any())
            throw new ArgumentException("Invalid SMS send request: " + string.Join(", ", validationErrors));

        if (sendRequest.OutgoingSmsMessages.Length > 1)
        {
            throw new NotImplementedException("Not yet implemented: Multiple recipients");
        }

        XE xmlItems = new XE("items");

        var req = sendRequest.OutgoingSmsMessages.Single();

        var xml = new XE("sms-teknik",
            new XE("operationtype", 0),
            new XE("smssender", req.From),
            new XE("multisms", 1),
            new XE("maxmultisms", 0),
            new XE("send_date", req.SendAt?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? ""),
            new XE("send_time", req.SendAt?.ToString("HH:dd:ss", CultureInfo.InvariantCulture) ?? ""),
            new XE("udmessage", req.Body),
            xmlItems
        );

        if (!string.IsNullOrEmpty(req.StatusCallBackUrl))
            xml.Add(new XE("deliverystatustype", "3"),
                new XE("deliverystatusaddress", req.StatusCallBackUrl));

        xmlItems.Add(sendRequest.OutgoingSmsMessages.Select(outgoingMessage => new XE("recipient", new XE("nr", outgoingMessage.To))));

        Client.DefaultRequestHeaders.Add("Authorization", $"Basic {Utils.Base64Encode($"{_config.Username}:{_config.Password}")}");

        using var content = new StringContent(xml.ToString(), System.Text.Encoding.UTF8, "application/xml");
        using var responseMessage = await Client.PostAsync("https://api.smsteknik.se/send/xml/", content);
        responseMessage.EnsureSuccessStatusCode();
        var result = await responseMessage.Content.ReadAsStringAsync();
        return ParseResult(result, sendRequest.OutgoingSmsMessages);
    }


    private SendResponse ParseResult(string result, OutgoingSmsMessage[] outgoingSmsMessages)
    {
        var arrResult = result.Split(';');

        if (outgoingSmsMessages.Length > 1 && arrResult.Length == 1)
        {
            // We expected to receive multiple response codes (one for each message), but we only receive one.
            // This means that the entire request failed (eg. wrong credentials, parse error or quota exceeded).
            // We have two options: 1) throw exception, or 2) populate the same error message for each of the message response.
            // This scenario only applies when sending to multiple receivers.
            // Example error codes: 0:Access denied, 0:No Valid recipients
            var errorCode = arrResult.Single()[2..];
            throw new Exception("Error: " + errorCode);
        }

        if (outgoingSmsMessages.Length != arrResult.Length)
            throw new Exception("This should not have happened...");

        var messageResponses = new MessageResponse[outgoingSmsMessages.Length];

        for (var i = 0; i < messageResponses.Length; i++)
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
                smsId = Convert.ToInt64(resultItem);
            }

            messageResponses[i] = new MessageResponse(outgoingSmsMessages[i], success, errorMessage, smsId);
        }

        return new SendResponse(messageResponses);
    }
}