namespace SMSTeknikClient.Messages;

/// <summary>
/// This class represent a single SMS message, with
/// a single recipient. 
/// </summary>
public class OutgoingSmsMessage
{
    public OutgoingSmsMessage(string @from, string to, string body, DateTimeOffset? sendAt = null, string? statusCallBackUrl = null)
    {
        To = to;
        Body = body;
        From = @from;
        SendAt = sendAt;
        StatusCallBackUrl = statusCallBackUrl;
    }

    public string From { get; set; }

    public string To { get; set; }

    public string Body { get; set; }

    public DateTimeOffset? SendAt { get; set; }

    public string? StatusCallBackUrl { get; set; }

    public OutgoingSmsMessage Clone() =>
        (OutgoingSmsMessage)MemberwiseClone();
}