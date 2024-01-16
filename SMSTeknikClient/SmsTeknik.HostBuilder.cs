using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMSTeknikClient.ClientImplementation;
using SMSTeknikClient.Config;

namespace SMSTeknikClient;

public static class SmsTeknikHostBuilder
{
	/// <summary>
	/// Add scoped SMS Teknik client to dependency injection with a configured http client.
	/// </summary>
	/// <param name="hostBuilder"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	public static IHostBuilder AddSmsTeknik(this IHostBuilder hostBuilder, 
			Action<SmsTeknikConfiguration> configure)
	{
		hostBuilder.ConfigureServices((_, services) =>
		{
			var config = new SmsTeknikConfiguration();
			configure(config);
			services.Configure(configure);
			services.AddHttpClient<ISmsTeknikClient, SmsTeknikXmlClient>(client =>
			{
				client.BaseAddress = new("https://api.smsteknik.se/");
				client.DefaultRequestHeaders.Add("Authorization",
						$"Basic {Utils.Base64Encode($"{config.Username}:{config.Password}")}");
			});
			services.AddScoped<ISmsTeknikClient, SmsTeknikXmlClient>();
		});
		return hostBuilder;
	}
}