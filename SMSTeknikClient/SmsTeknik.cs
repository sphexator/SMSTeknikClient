using SMSTeknikClient.ClientImplementation;
using SMSTeknikClient.Config;

namespace SMSTeknikClient;

public static class SmsTeknik
{
    public static ISmsTeknikClient CreateClient(ISmsTeknikConfiguration config) =>
        new SmsTeknikXmlClient(config);
}