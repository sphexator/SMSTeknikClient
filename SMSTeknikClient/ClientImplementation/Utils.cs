using System.Text;

namespace SMSTeknikClient.ClientImplementation;

public class Utils
{
    public static string Base64Encode(string textToEncode)
    {
        var textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
        return Convert.ToBase64String(textAsBytes);
    }
}