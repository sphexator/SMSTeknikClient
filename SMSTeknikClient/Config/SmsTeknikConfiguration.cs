namespace SMSTeknikClient.Config;


/// <summary>
/// Class used for configuration of SMS Teknik client.
///
/// Typically bound from a configuration file during startup. 
/// </summary>
public record SmsTeknikConfiguration(string Username, string Password)
{
}