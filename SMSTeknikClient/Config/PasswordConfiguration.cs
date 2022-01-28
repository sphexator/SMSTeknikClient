namespace SMSTeknikClient.Config;


/// <summary>
/// Class used for configuration of SMS Teknik client.
///
/// Typically bound from a configuration file during startup. 
/// </summary>
public class PasswordConfiguration
{
    public string Username { get; set; }
    
    public string Password { get; set; }
}