# SMSTeknikClient



Step 1: 

`nuget install SMSTeknikClient`


Step 2: 

```
var msg = new SMSMessage 
{
   From = "+234234", 
   To = "+2134", 
}; 

await SMSTeknickClient.SendMsg(msg); 

