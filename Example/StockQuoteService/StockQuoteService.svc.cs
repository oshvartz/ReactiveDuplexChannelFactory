// StockQuoteService.svc.cs  
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace StockQuoteService
{
  public class StockQuoteService : IStockQuoteService
  {
    public async Task StartSendingQuotes()
    {
      var callback = OperationContext.Current.GetCallbackChannel<IStockQuoteCallback>();
      var random = new Random();
      double price = 29.00;
      price += random.NextDouble();

      while (((IChannel)callback).State == CommunicationState.Opened)
      {
        await callback.SendQuote(new Quote {Code = "MSFT",Price  = price});
        price += random.NextDouble();
        await Task.Delay(50);
      }
    }
  }
}