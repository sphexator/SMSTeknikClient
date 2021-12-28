namespace SMSTeknikClient.Messages;

/// <summary>
/// This class represent a single SMS message, with
/// a single recipient. 
/// </summary>
public class OutgoingSmsMessage
{
    public OutgoingSmsMessage(string recipient, string body)
    {
        Recipient = recipient;
        Body = body;
    }

    public string Recipient { get; }
    public string Body { get; }
}