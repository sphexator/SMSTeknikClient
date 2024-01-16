using System.Text;

namespace SMSTeknikClient.ClientImplementation;

public static class Utils
{
    public static string Base64Encode(string textToEncode) => 
        Convert.ToBase64String(Encoding.UTF8.GetBytes(textToEncode));

    /// <summary>
    /// Calculates the character count of your text message. Assuming GSM7 (7-bit encoding).
    /// </summary>
    /// <param name="body"></param>
    /// <returns>Number of chars</returns>
    public static int CalculateMessageLength(string body)
    {
        var doubleChars = new char[] { '^', '\\', '{', '}', '[', ']', '~', '|', '�', '\n' };

        int count = 0;

        foreach (char c in body.ToCharArray())
        {
            if (doubleChars.Contains(c))
                count++;

            count++;
        }

        return count;
    }

    /// <summary>
    /// Calculates the number of message parts (segments) for your text message. Assuming GSM7 (7-bit encoding). The number of message parts influences your messaging cost.
    /// </summary>
    /// <param name="body"></param>
    /// <returns>Number of message parts (segments)</returns>
    public static int CalculateMessageParts(string body)
    {
        var len = CalculateMessageLength(body);

        return (len <= 160 ? 1 : (int)Math.Ceiling(len / 153m));
    }

}