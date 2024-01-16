namespace SMSTeknikClient.Messages;

/// <summary>
/// This class represents the response received
/// from the SMS teknik server when sending one or more SMS messages. 
/// </summary>
public class SendResponse
{
    public SendResponse(MessageResponse[] messageResponses)
    {
        MessageResponses = messageResponses;
    }

    public MessageResponse[] MessageResponses { get; }
    public bool Success => MessageResponses.All(r => r.Success);

    /// <summary>
    /// This method throws an exception if not all of the messages
    /// were sent successfully. Useful as simple check to ensure
    /// exceptions are not ignored, however it is preferred to
    /// check the actual statuses and handle the response properly. 
    /// </summary>
    public void EnsureSuccess()
    {
        if (!Success) throw new AggregateException("One or more messages failed. Check individual messages for details. ");
    }
}