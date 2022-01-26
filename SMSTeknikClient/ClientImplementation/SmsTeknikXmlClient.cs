using System.Xml;
using SMSTeknikClient.Config;
using SMSTeknikClient.Messages;

namespace SMSTeknikClient.ClientImplementation;

public class SmsTeknikXmlClient : ISmsTeknikClient
{

    private static HttpClient _client = new HttpClient();
    
    private readonly ISmsTeknikConfiguration _config;

    public SmsTeknikXmlClient(ISmsTeknikConfiguration config) => _config = config;

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

    public Task<SendResponse> SendRequest(SendRequest sendRequest)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            CloseOutput = false
        };

        MemoryStream localMemoryStream = new MemoryStream();  
        using XmlWriter writer = XmlWriter.Create(localMemoryStream, writerSettings);
        
        writer.WriteStartElement("message");  
        writer.WriteElementString("title", "Graphics Programming using GDI+");  
        writer.WriteElementString("author", "Mahesh Chand");  
        writer.WriteElementString("publisher", "Addison-Wesley");  
        writer.WriteElementString("price", "64.95");  
        writer.WriteEndElement();  
        writer.Flush();


        throw new NotImplementedException();
    }
}