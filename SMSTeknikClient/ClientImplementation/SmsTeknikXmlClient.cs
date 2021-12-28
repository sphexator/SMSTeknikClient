using SMSTeknikClient.Config;
using SMSTeknikClient.Messages;

namespace SMSTeknikClient.ClientImplementation;

public class SmsTeknikXmlClient : ISmsTeknikClient
{
    private readonly SmsTeknikConfiguration _config;

    public SmsTeknikXmlClient(SmsTeknikConfiguration config) => 
        _config = config;

    void IDisposable.Dispose() => 
        GC.SuppressFinalize(this);

    public MessageResponse SendMessage(OutgoingSmsMessage message) =>
        SendRequest(new SendRequest(message)).MessageResponses.Single();

    public SendResponse SendMessages(params OutgoingSmsMessage[] messages) =>
        SendRequest(new SendRequest(messages));

    public SendResponse SendRequest(SendRequest sendRequest)
    {
        throw new NotImplementedException();
    }
}