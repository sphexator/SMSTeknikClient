namespace SMSTeknikClient.Messages;

/// <summary>
/// This class is used for sending more complex messages, including
/// multiple recipients, specifying other parameters etc.
///
/// Currently no other parameters are defined. 
/// </summary>
public record SendRequest(params OutgoingSmsMessage[] OutgoingSmsMessages);