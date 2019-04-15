using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace StockQuoteService
{
    [ServiceContract(CallbackContract = typeof(IStockQuoteCallback))]
    public interface IStockQuoteService
    {
        [OperationContract(IsOneWay = true)]
        Task StartSendingQuotes();

        [OperationContract(IsOneWay = true)]
        Task StopSendingQuotes();
    }

    [ServiceContract]
    public interface IStockQuoteCallback
    {
        [OperationContract(IsOneWay = true)]
        Task SendQuote(Quote quote);
    }

    [DataContract]
    public class Quote
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public double Price { get; set; }
    }
}