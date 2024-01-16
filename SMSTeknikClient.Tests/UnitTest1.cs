using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SMSTeknikClient.Config;
using SMSTeknikClient.Messages;
using SMSTeknikClient.Tests.Config;
using static NUnit.Framework.Assert;

namespace SMSTeknikClient.Tests;

public class Tests
{
    private IConfiguration _configuration = null!;

    private SmsTeknikConfiguration? _clientConfiguration;
    private TestConfiguration? _testConfiguration;

    public static IConfiguration InitConfiguration() =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.local.json", true)
            .Build();

    private ISmsTeknikClient CreateClient() => SmsTeknik.CreateClient(_clientConfiguration);

    [SetUp]
    public void Setup()
    {
        _configuration = InitConfiguration();

        _clientConfiguration = new SmsTeknikConfiguration();
        _configuration.Bind(nameof(SmsTeknik), _clientConfiguration);

        _testConfiguration = new TestConfiguration();
        _configuration.Bind(nameof(TestConfiguration), _testConfiguration);
    }

    [Test]
    [Ignore("pls dont run this unless you know what you are doing :)")]
    public async Task TestSendSingleMessage()
    {
        var client = CreateClient();

        // TODO: Load actual test configuration from a non-git-committed file. 
        var msg = new OutgoingSmsMessage
        {
            To = _testConfiguration.Recipients.First(),
            From = _testConfiguration.From,
            Body = "Hello, World!",
            // You can other stuff here! See documentation for details. 
        };

        var response = await client.Send(msg);
        // You can check for status, delivery reports, failure details etc on the response

        IsTrue(response.Success, "response.Success" + "; Errors: " + response.ErrorMessage);
        IsTrue(response.SmsId > 0);
        IsTrue(response.OutgoingSmsMessage.To == msg.To);
    }

    [Test]
    [Ignore("pls dont run this unless you know what you are doing :)")]
    public async Task TestSendMultipleMessages()
    {
        var client = CreateClient();
        var recipients = _configuration.GetSection("to").Get<string[]>();

        var msg = new OutgoingSmsMessage
        {
            From = _configuration["from"],
            Body = "Hi there! Please ignore this message. ",
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var response = await client.Send(SendRequest.ToMultipleRecipients(msg, recipients));

        IsTrue(response.Success);
        IsTrue(response.MessageResponses.Length == recipients.Length);
        IsTrue(response.MessageResponses[0].OutgoingSmsMessage.To == recipients[0]);
    }

    [Test]
    public void TestCalculateMessageParts()
    {
        var messageParts = ClientImplementation.Utils.CalculateMessageParts("".PadRight(170));

        IsTrue(messageParts == 2);
    }

    [Test]
    [Ignore("pls dont run this unless you know what you are doing :)")]
    public async Task TestCheckCredits()
    {
        var client = CreateClient();

        var credits = await client.CheckCredits();

        IsTrue(credits >= 0);
    }
}