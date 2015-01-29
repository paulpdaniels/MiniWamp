MiniWamp
========

A (relatively) small implementation of WAMP for .NET 4.5.1 and Windows 8.1.

Currently only supports WAMP v1 but who knows v2 might be in the cards

Only requires JSON.net.  Ideally would have no dependencies but lets face it the built in JSON libraries are not really up to snuff.



#### Quickstart

Get It: [Nuget Package](https://www.nuget.org/packages/MiniWamp/)


Start by using the WampClient to start up a connection
```csharp

//At the top
using DapperWare;

//Will use the default Transport which is a MessageWebSocket

Task<WampSession> session = WampClient.Connect("ws://localhost:3000");

//You can also implement your own transport as long as it implements IWampTransport
//Notice that we can await the connection to complete it asynchronously
var session2 = await WampClient.Connect("ws://localhost:3000", new MyTransportFactory());
```

Super simple right?

But wait there's more!
Say you want to actually use Wamp to do something!

```csharp
//Subscription
IWampSubject<string> mySubject = session.Subscribe("mytopic/something");

mySubject.Message += mySubject_MessageHandler;

//There are two ways to unsubscribe

//Unsubscribes only this subject
mySubject.Unsubscribe();

//Unsubscribes all subjects
session.Unsubscribe("mytopic/something");

//Publication
session.Publish("mytopic/somethingelse", new JObject(), excludeMe: true);

//RPC
var sum = await session.Call<int>("rpc#add", 3, 4); //7

```
