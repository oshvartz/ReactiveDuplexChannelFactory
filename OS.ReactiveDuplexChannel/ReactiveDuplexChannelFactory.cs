using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Castle.DynamicProxy;

namespace OS.ReactiveDuplexChannel
{
  public class ReactiveDuplexChannelFactory<TContract, TCallbackContract> : DuplexChannelFactory<TContract>
where TContract : class
where TCallbackContract : class
  {
    private static readonly ProxyGenerator _generator = new ProxyGenerator();
    private ReactiveInteceptor _reactiveInteceptor;

    public ReactiveDuplexChannelFactory(Binding binding, string remoteAddress)
      : base(new InstanceContext(_generator.CreateInterfaceProxyWithoutTarget<TCallbackContract>(new ReactiveInteceptor())), binding, remoteAddress)
    {
      _reactiveInteceptor = ExtractTheInterceptor();
    }


    //kongfu
    private ReactiveInteceptor ExtractTheInterceptor()
    {
      var cbi = this.GetType().GetProperty("CallbackInstance", BindingFlags.Instance | BindingFlags.NonPublic);
      var ic = (InstanceContext) cbi.GetValue(this);

      var memb = ic.GetType().GetProperty("UserObject", BindingFlags.Instance | BindingFlags.NonPublic);

      var obk = memb.GetValue(ic);

      var interc = obk.GetType().GetField("__interceptors", BindingFlags.Instance | BindingFlags.NonPublic);
      var arr = interc.GetValue(obk) as IInterceptor[];

       return (ReactiveInteceptor) arr[0];
    }

    public IObservable<T> AsObservable<T>()
    {
      return _reactiveInteceptor.AddListener<T>();
    }
  }
}
