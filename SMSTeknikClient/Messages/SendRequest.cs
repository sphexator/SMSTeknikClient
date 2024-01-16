namespace SMSTeknikClient.Messages;

/// <summary>
/// This class is used for sending more complex messages, including
/// multiple recipients, specifying other parameters etc.
///
/// Currently no other parameters are defined. 
/// </summary>
public class SendRequest
{
    public SendRequest(params OutgoingSmsMessage[] outgoingSmsMessages)
    {
        OutgoingSmsMessages = outgoingSmsMessages;
    }
    
    public OutgoingSmsMessage[] OutgoingSmsMessages { get; }

    public static SendRequest FromMessage(OutgoingSmsMessage message) => 
        new(message);

    public static SendRequest ToMultipleRecipients(OutgoingSmsMessage message, string[] toMultipleRecipients) =>
        new(toMultipleRecipients.Select(message.WithTo).ToArray());

    // public Task<SendResponse> Send(params OutgoingSmsMessage[] messages);
}