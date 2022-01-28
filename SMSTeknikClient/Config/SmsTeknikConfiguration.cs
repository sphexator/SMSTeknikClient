namespace SMSTeknikClient.Config;


/// <summary>
/// Class used for configuration of SMS Teknik client.
///
/// Typically bound from a configuration file during startup. 
/// </summary>
public class SmsTeknikConfiguration
{
    public SmsTeknikConfiguration(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; set; }
    
    public string Password { get; set; }
}