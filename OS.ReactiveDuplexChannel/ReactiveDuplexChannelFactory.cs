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
    // ReSharper disable once StaticMemberInGenericType
    private static readonly ProxyGenerator Generator = new ProxyGenerator();
    private readonly ReactiveInteceptor _reactiveInteceptor;

    public ReactiveDuplexChannelFactory(Binding binding, string remoteAddress)
      : base(new InstanceContext(Generator.CreateInterfaceProxyWithoutTarget<TCallbackContract>(new ReactiveInteceptor())), binding, remoteAddress)
    {
      _reactiveInteceptor = ExtractTheInterceptor();
    }

    public IObservable<T> AsObservable<T>()
    {
      return _reactiveInteceptor.AddListener<T>();
    }


    //kongfu
    private ReactiveInteceptor ExtractTheInterceptor()
    {
      try
      {
        var callbackInstancePropInfo = GetType().GetProperty("CallbackInstance",
          BindingFlags.Instance | BindingFlags.NonPublic);
        // ReSharper disable once PossibleNullReferenceException
        var instanceContext = (InstanceContext) callbackInstancePropInfo.GetValue(this);

        var userObjectMemberInfo = instanceContext.GetType().GetProperty("UserObject", BindingFlags.Instance | BindingFlags.NonPublic);
        // ReSharper disable once PossibleNullReferenceException
        var userObject = userObjectMemberInfo.GetValue(instanceContext);

        var interceptorsFieldInfo = userObject.GetType().GetField("__interceptors", BindingFlags.Instance | BindingFlags.NonPublic);
        // ReSharper disable once PossibleNullReferenceException
        var interceptors = interceptorsFieldInfo.GetValue(userObject) as IInterceptor[];
        // ReSharper disable once PossibleNullReferenceException
        return (ReactiveInteceptor)interceptors[0];
      }
      catch (Exception ex)
      {
        throw new NotSupportedException("fail to Extract The Interceptor - something was change", ex);
      }
    }

  }
}
