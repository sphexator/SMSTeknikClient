# SMSTeknikClient



Step 1: 

`nuget Install-Package SMSTeknikClient`


Step 2: 

```
var client = SmsTeknik.CreateClient(new Config.SmsTeknikConfiguration(myUserName, myPassword));
        
var msg = new OutgoingSmsMessage
{
    To = "+4790000001",
    From = "Test",
    Body = "Hello, World!",
    // You can specify lots of other stuff here! See documentation for details. 
};

var response = await client.SendMessage(msg);

// You can check for status, delivery reports, failure details etc on the response
if(response.Success)
    Console.WriteLine($"Your message id {response.SmsId}");
else
    Console.WriteLine($"Failed with reason: {response.ErrorMessage}");
