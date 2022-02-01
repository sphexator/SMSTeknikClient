namespace SMSTeknikClient.Messages;

/// <summary>
/// This class represent a single SMS message, with
/// a single recipient. 
/// </summary>
public record OutgoingSmsMessage
{
    public string? From { get; init; }

    public string? To { get; init; }

    public string? Body { get; init; }

    public DateTimeOffset? SendAt { get; set; }

    public string? StatusCallBackUrl { get; set; }

    public OutgoingSmsMessage WithTo(string to) => this with { To = to };
}