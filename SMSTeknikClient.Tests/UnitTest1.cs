using NUnit.Framework;
using SMSTeknikClient.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMSTeknikClient.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestSendSingleMessage()
    {
        var client = SmsTeknik.CreateClient(
            new Config.SmsTeknikConfiguration(username: "username", password: "password"));

        var msg = new OutgoingSmsMessage
        {
            To = "+4790000001",
            From = "Test",
            Body = "Hello, World!",
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var response = await client.SendMessage(msg);
        // You can check for status, delivery reports, failure details etc on the response

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.OutgoingSmsMessage.To == msg.To);

    }

    [Test]
    public async Task TestSendMultipleMessages()
    {
        var client = SmsTeknik.CreateClient(
            new Config.SmsTeknikConfiguration(username: "username", password: "password"));

        var msg = new OutgoingSmsMessage
        {
            Body = "Hey, Y'all!",
            From = "Test",
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var receivers = new string[] { "+4790000001", "+4790000002" };

        var response = await client.SendMessageToMultipleRecipients(msg, receivers);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.MessageResponses.Length == receivers.Length);
        Assert.IsTrue(response.MessageResponses[0].OutgoingSmsMessage.To == receivers[0]);
    }
}
