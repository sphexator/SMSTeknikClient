using SMSTeknikClient.ClientImplementation;
using SMSTeknikClient.Config;

namespace SMSTeknikClient;

public static class SmsTeknik
{
    public static ISmsTeknikClient CreateClient(SmsTeknikConfiguration config) =>
        new SmsTeknikXmlClient(config);
}