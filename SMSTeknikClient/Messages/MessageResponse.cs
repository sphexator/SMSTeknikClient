namespace SMSTeknikClient.Messages;

public record MessageResponse(OutgoingSmsMessage OutgoingSmsMessage, bool Success, string? ErrorMessage, long? SmsId)
{
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