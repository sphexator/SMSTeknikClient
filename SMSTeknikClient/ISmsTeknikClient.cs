using SMSTeknikClient.Messages;

namespace SMSTeknikClient;

/// <summary>
/// This interface is the main entity that clients
/// will use to send messages. 
/// </summary>
public interface ISmsTeknikClient : IDisposable
{
    public Task<MessageResponse> SendMessage(OutgoingSmsMessage message);
    public Task<SendResponse> SendMessageToMultipleRecipients(OutgoingSmsMessage message, string[] toMultipleRecipients);

    public Task<SendResponse> SendMessages(params OutgoingSmsMessage[] messages);

    public Task<SendResponse> SendRequest(SendRequest sendRequest);
}