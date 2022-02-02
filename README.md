# SMSTeknikClient

This is a client library for SMSTeknik SMS Gateway. 


Step 1:

`nuget Install-Package SMSTeknikClient`

Step 2:

```csharp
var client = SmsTeknik.CreateClient(new SmsTeknikConfiguration(myUserName, myPassword));
        
var msg = new OutgoingSmsMessage
{
    To = "+4790000001",
    From = "Test",
    Body = "Hello, World!",
    // You can specify other things here, like callback urls, scheduling etc! 
    // See documentation for details. 
};

var response = await client.Send(msg);

// You can check for status, delivery reports, failure details etc on the response
if(response.Success)
    Console.WriteLine($"Your message was sent successfully! Message id: {response.SmsId}");
else
    Console.WriteLine($"Failed with reason: {response.ErrorMessage}");
```

See the unit tests for more complete examples. 