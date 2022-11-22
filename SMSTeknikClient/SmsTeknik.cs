using SMSTeknikClient.ClientImplementation;
using SMSTeknikClient.Config;

namespace SMSTeknikClient;

public static class SmsTeknik
{
    /// <summary>
    /// Main entry point for creating SmsTeknik clients. 
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static ISmsTeknikClient CreateClient(SmsTeknikConfiguration config) =>
        new SmsTeknikXmlClient(config);
}