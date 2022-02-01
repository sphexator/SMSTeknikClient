using System.Text;

namespace SMSTeknikClient.ClientImplementation;

public class Utils
{
    public static string Base64Encode(string textToEncode) => 
        Convert.ToBase64String(Encoding.UTF8.GetBytes(textToEncode));
}