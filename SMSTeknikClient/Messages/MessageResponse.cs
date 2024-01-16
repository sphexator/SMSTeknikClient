namespace SMSTeknikClient.Messages;

public class MessageResponse
{
    public MessageResponse(OutgoingSmsMessage outgoingSmsMessage, bool success, string? errorMessage, long? smsId)
    {
        OutgoingSmsMessage = outgoingSmsMessage;
        Success = success;
        ErrorMessage = errorMessage;
        SmsId = smsId;
    }
    
    public OutgoingSmsMessage OutgoingSmsMessage { get; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public long? SmsId { get; set; }
    
    /// <summary>
    /// This method throws an exception if not all of the messages
    /// were sent successfully. Useful as simple check to ensure
    /// exceptions are not ignored, however it is preferred to
    /// check the actual statuses and handle the response properly. 
    /// </summary>
    public void EnsureSuccess()
    {
        if (!Success) throw new AggregateException($"Send the sms failed. Error code = {ErrorMessage}");
    }
}