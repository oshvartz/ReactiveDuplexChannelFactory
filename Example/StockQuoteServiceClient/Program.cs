using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using OS.ReactiveDuplexChannel;
using StockQuoteServiceClient.QuotesServiceReference;


namespace StockQuoteServiceClient
{
  class Program
  {
    static void Main(string[] args)
    {
      var binding = new NetHttpBinding();
      binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
      ReactiveDuplexChannelFactory<IStockQuoteService, IStockQuoteServiceCallback> duplexChannel =
        new ReactiveDuplexChannelFactory<IStockQuoteService, IStockQuoteServiceCallback>(binding, "ws://localhost:3201/StockQuoteService.svc");


      duplexChannel.AsObservable<Quote>().Subscribe(quote => Console.WriteLine($"{quote.Code}-{quote.Price}"));
      var channel = duplexChannel.CreateChannel();

      channel.StartSendingQuotes();


      Console.ReadLine();
    }
  }
}
