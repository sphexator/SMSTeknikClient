namespace SMSTeknikClient.Config;


/// <summary>
/// Class used for configuration of SMS Teknik client.
///
/// Typically bound from a configuration file during startup. 
/// </summary>
public class SmsTeknikConfiguration
{
    public string Password { get; set; }
    public string Username { get; set; }
}