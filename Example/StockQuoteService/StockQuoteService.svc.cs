// StockQuoteService.svc.cs  
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace StockQuoteService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,InstanceContextMode = InstanceContextMode.PerSession)]
    public class StockQuoteService : IStockQuoteService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        public StockQuoteService()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartSendingQuotes()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IStockQuoteCallback>();
            var random = new Random();
            double price = 29.00;
            price += random.NextDouble();

            while (((IChannel)callback).State == CommunicationState.Opened && !_cancellationTokenSource.IsCancellationRequested)
            {
                await callback.SendQuote(new Quote { Code = "MSFT", Price = price });
                price += random.NextDouble();
                await Task.Delay(1);
            }
        }

        public Task StopSendingQuotes()
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}