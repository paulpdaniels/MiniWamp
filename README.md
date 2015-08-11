MiniWamp
========

A (relatively) small implementation of WAMP for .NET 4.5 and Windows (Phone) 8.1.

Currently only supports WAMP v1 but who knows v2 might be in the cards

Only requires JSON.net.  Ideally would have no dependencies but lets face it the built in JSON libraries are not really up to snuff.



#### Quickstart

Get It: [Nuget Package](https://www.nuget.org/packages/MiniWamp/)


Start by using the WampClient to start up a connection
```csharp

//At the top
using DapperWare;

//Will use the default Transport which is a MessageWebSocket

Task<WampSession> session = WampClient.ConnectAsync("ws://localhost:3000");

//You can also implement your own transport as long as it implements IWampTransport
//We can use the handy await keyword to wait for everything to start up
var session2 = await WampClient.ConnectAsync("ws://localhost:3000", new MyTransportFactory());
```

Super simple right?

But wait there's more!
Say you want to actually use Wamp to do something!

MiniWamp implements all the of the [Version 1 spec](http://wamp.ws/spec/wamp1/) (if you find something it doesn't, kindly submit a bug report).

##### Subscribing and Unsubscribing

```csharp
//Subscription
//Don't worry subscriptions will be shared if there are multiple ones
IWampSubject<string> mySubject = session.Subscribe("mytopic/something");

//Start listening for messages from the subject
mySubject.Message += mySubject_MessageHandler;

//There are two ways to unsubscribe

//Unsubscribes only this subject
mySubject.Dispose();

//Deprecated - Use .Dispose() instead
mySubject.Unsubscribe();

//Unsubscribes all subjects on this topic
session.Unsubscribe("http://mytopic/something");

//Unsubscribes all subjects everywhere
session.Unsubscribe();

```

##### Publishing messages

```csharp

//Publication
session.Publish("http://mytopic/somethingelse", "Hello, world!");

//Don't receive your own publication
session.Publish("http://mytopic/somethingelse", 42, excludeMe: true);

//Add fine grained control to who can listen and who can't
session.Publish("http://mytopic/somethingelse", new MyCustomObject(), 
                 ["someid1", "someid2"], /*Exclude*/
                 ["someid3", "someid4"] /*Eligible*/);
                 
                 
```

##### Calling methods
```csharp
var sum = await session.Call<int>("rpc#add", 3, 4); //sum = 7

//Exceptions in the call should be handled with try/catch

try {
  var sum = await session.Call<int>("rpc#add", 3, 4); //sum = 7
} catch (WampCallException ex) {
  //Handle the exception
}

```

##### Using Prefixes

```csharp

//Note: you will still use the full uri when calling methods, but under the covers
//MiniWamp will swap it out with the prefix before sending.
session.Prefixes.Add("calc", "http://mytopic/methods#");


i.e. session.Call<int>("http://mytopic/methods#random") -> [2, callid, "calc:random"]

```
