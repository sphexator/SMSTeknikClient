using SMSTeknikClient.Messages;

namespace SMSTeknikClient;

/// <summary>
/// This interface is the main entity that clients
/// will use to send messages. 
/// </summary>
public interface ISmsTeknikClient : IDisposable
{
    /// <summary>
    /// This is the main method, used to send one or multiple SMS messages
    /// </summary>
    public Task<SendResponse> Send(SendRequest sendRequest, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// This is a helper method to send a single SMS message
    /// </summary>
    public Task<MessageResponse> Send(OutgoingSmsMessage sendRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns remaining credits on your account. 
    /// </summary>
    /// <returns></returns>
    public Task<int> CheckCredits(CancellationToken cancellationToken = default);
}