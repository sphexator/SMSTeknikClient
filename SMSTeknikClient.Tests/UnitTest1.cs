using System.Collections.Generic;
using System.Linq;
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

    private ISmsTeknikClient CreateClient() => SmsTeknik.CreateClient(new SmsTeknikConfiguration(
        _configuration["username"], _configuration["password"]));

    [SetUp]
    public void Setup() => _configuration = InitConfiguration();

    [Test]
    public async Task TestSendSingleMessage()
    {
        var client = CreateClient();
        var recipients = _configuration.GetSection("to").Get<IList<string>>();

        // TODO: Load actual test configuration from a non-git-committed file. 
        var msg = new OutgoingSmsMessage
        {
            To = recipients.First(),
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
        var client = CreateClient();
        var recipients = _configuration.GetSection("to").Get<string[]>();

        var msg = new OutgoingSmsMessage
        {
            From = _configuration["from"],
            Body = "Hi there! Please ignore this message. ",
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var response = await client.SendMessageToMultipleRecipients(msg, recipients);

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
}