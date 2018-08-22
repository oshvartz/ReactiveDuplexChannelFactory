# ReactiveDuplexChannelFactory
Extend DuplexChannelFactory to support IObservalbe

This is a .NET Standard project - part of learning the process of creating muliti-platform Nuget

Usage is very simple just replace DuplexChannelFactory to ReactiveDuplexChannelFactory

# Nuget
[from MyGet](https://www.myget.org/feed/oshvartz/package/nuget/OS.ReactiveDuplexChannel)

example:

 ```C#
 ReactiveDuplexChannelFactory<IStockQuoteService, IStockQuoteServiceCallback> duplexChannel = 
 new ReactiveDuplexChannelFactory<IStockQuoteService, IStockQuoteServiceCallback>(binding,"ws://localhost:3201/StockQuoteService.svc");

 duplexChannel.AsObservable<Quote>().Subscribe(quote => Console.WriteLine($"{quote.Code}-{quote.Price}"));
 ```
    
