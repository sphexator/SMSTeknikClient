namespace SMSTeknikClient.Messages;

public class MessageResponse
{
    public MessageResponse(OutgoingSmsMessage message, bool success, string? errorMessage)
    {
        OutgoingSmsMessage = message;
        Success = success;
        ErrorMessage = errorMessage;
    }

    public OutgoingSmsMessage OutgoingSmsMessage { get; }

    public bool Success { get; }

    
    public string? ErrorMessage { get; }
    

    /// <summary>
    /// This method throws an exception if not all of the messages
    /// were sent successfully. Useful as simple check to ensure
    /// exceptions are not ignored, however it is preferred to
    /// check the actual statuses and handle the response properly. 
    /// </summary>
    public void EnsureSuccess()
    {
        if (!Success) throw new AggregateException("Sending the message failed. Check the error message for details. ");
    }
    
}