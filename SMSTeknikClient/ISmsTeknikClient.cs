using SMSTeknikClient.Messages;

namespace SMSTeknikClient;

/// <summary>
/// This interface is the main entity that clients
/// will use to send messages. 
/// </summary>
public interface ISmsTeknikClient : IDisposable
{
    public MessageResponse SendMessage(OutgoingSmsMessage message);

    public SendResponse SendMessages(params OutgoingSmsMessage[] messages);

    public SendResponse SendRequest(SendRequest sendRequest);
}