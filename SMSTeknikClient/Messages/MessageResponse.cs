namespace SMSTeknikClient.Messages;

public class MessageResponse
{
    public OutgoingSmsMessage OutgoingSmsMessage { get; init; }

    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public long? SmsId { get; init; }

    public MessageResponse(OutgoingSmsMessage message, bool success, string? errorMessage, long? smsId)
    {
        OutgoingSmsMessage = message;
        Success = success;
        ErrorMessage = errorMessage;
        SmsId = smsId;
    }

    /// <summary>
    /// This method throws an exception if not all of the messages
    /// were sent successfully. Useful as simple check to ensure
    /// exceptions are not ignored, however it is preferred to
    /// check the actual statuses and handle the response properly. 
    /// </summary>
    public void EnsureSuccess()
    {
        if (!Success) throw new AggregateException("One or more of the messages failed. Check the error message for details. ");
    }
}