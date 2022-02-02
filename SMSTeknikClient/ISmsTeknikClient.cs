using SMSTeknikClient.Messages;

namespace SMSTeknikClient;

/// <summary>
/// This interface is the main entity that clients
/// will use to send messages. 
/// </summary>
public interface ISmsTeknikClient : IDisposable
{
    public Task<SendResponse> Send(SendRequest sendRequest);
    public Task<MessageResponse> Send(OutgoingSmsMessage sendRequest);
    public Task<int> CheckCredits();
}