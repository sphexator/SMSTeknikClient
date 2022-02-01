using SMSTeknikClient.Messages;

namespace SMSTeknikClient.ClientImplementation;

public static class Validation
{
    public static string[] GetValidationErrors(OutgoingSmsMessage i)
    {
        var errors = new List<string>();
        // TODO: Validate content, length etc? 
        if (string.IsNullOrEmpty(i.Body)) errors.Add(nameof(OutgoingSmsMessage.Body));
        if (string.IsNullOrEmpty(i.From)) errors.Add(nameof(OutgoingSmsMessage.From));
        if (string.IsNullOrEmpty(i.To)) errors.Add(nameof(OutgoingSmsMessage.To));
        return errors.ToArray();
    }

    public static string[] GetValidationErrors(SendRequest sendRequest)
    {
        var errors = new List<string>();
        if (!sendRequest.OutgoingSmsMessages.Any())
            errors.Add(nameof(SendRequest.OutgoingSmsMessages));
        errors.AddRange(sendRequest.OutgoingSmsMessages.SelectMany(GetValidationErrors));

        return errors.ToArray();
    }
}