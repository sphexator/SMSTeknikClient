namespace SMSTeknikClient.Messages;

/// <summary>
/// This class represent a single SMS message, with
/// a single recipient. 
/// </summary>
public struct OutgoingSmsMessage
{
    public string? From { get; set; }

    public string? To { get; set; }

    public string? Body { get; set; }

    public DateTimeOffset? SendAt { get; set; }

    public string? StatusCallBackUrl { get; set; }

    public OutgoingSmsMessage WithTo(string to) => this with { To = to };
}