using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SMSTeknikClient.Config;
using SMSTeknikClient.Messages;
using static NUnit.Framework.Assert;

namespace SMSTeknikClient.Tests;

public class Tests
{
    private IConfiguration _configuration = null!;

    public static IConfiguration InitConfiguration() =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.local.json", false)
            .Build();

    [SetUp]
    public void Setup() => _configuration = InitConfiguration();

    [Test]
    public async Task TestSendSingleMessage()
    {
        var config = new SmsTeknikConfiguration(Username: _configuration["username"], Password: _configuration["password"]);
        var client = SmsTeknik.CreateClient(config);

        // TODO: Load actual test configuration from a non-git-committed file. 
        var msg = new OutgoingSmsMessage
        {
            To = _configuration["To"],
            From = _configuration["from"],
            Body = "Hello, World!",
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var response = await client.SendMessage(msg);
        // You can check for status, delivery reports, failure details etc on the response

        IsTrue(response.Success, "response.Success" + "; Errors: " + response.ErrorMessage);
        IsTrue(response.SmsId > 0);
        IsTrue(response.OutgoingSmsMessage.To == msg.To);
    }

    [Test]
    public async Task TestSendMultipleMessages()
    {
        var client = SmsTeknik.CreateClient(
            new SmsTeknikConfiguration(Username: _configuration["username"], Password: _configuration["password"]));

        var msg = new OutgoingSmsMessage
        {
            Body = "Hey, Y'all!",
            From = _configuration["from"],
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var receivers = new[] { "+4790000001", "+4790000002" };

        var response = await client.SendMessageToMultipleRecipients(msg, receivers);

        IsTrue(response.Success);
        IsTrue(response.MessageResponses.Length == receivers.Length);
        IsTrue(response.MessageResponses[0].OutgoingSmsMessage.To == receivers[0]);
    }

    [Test]
    public void TestCalculateMessageParts()
    {
        var messageParts = ClientImplementation.Utils.CalculateMessageParts("".PadRight(170));

        IsTrue(messageParts == 2);
    }
}