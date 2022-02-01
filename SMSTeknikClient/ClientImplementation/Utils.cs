namespace SMSTeknikClient.ClientImplementation;

public class Utils
{
    public static string Base64Encode(string textToEncode)
    {
        var textAsBytes = System.Text.Encoding.UTF8.GetBytes(textToEncode);
        return Convert.ToBase64String(textAsBytes);
    }
}