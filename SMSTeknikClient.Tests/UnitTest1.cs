using NUnit.Framework;
using SMSTeknikClient.Messages;
using System.Collections.Generic;

namespace SMSTeknikClient.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async System.Threading.Tasks.Task TestSendSingleMessage()
    {
        var client = SmsTeknik.CreateClient(
            new Config.SmsTeknikConfiguration(username: "username", password: "password"));

        var msg = new OutgoingSmsMessage
        {
            To = "+47123",
            From = "Test",
            Body = "Hello, World!",
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var response = await client.SendMessage(msg);
        // You can check for status, delivery reports, failure details etc on the response

    }

    [Test]
    public async void TestSendMultipleMessages()
    {
        var client = SmsTeknik.CreateClient(
            new Config.SmsTeknikConfiguration(username: "username", password: "password"));

        var msg = new OutgoingSmsMessage
        {
            Body = "Hey, Y'all!",
            From = "Test",
            // You can specify lots of other stuff here! See documentation for details. 
        };

        var receivers = new string[] { "+47123", "+47123" };

        var response = await client.SendMessageToMultipleRecipients(msg, receivers);

    }
}
