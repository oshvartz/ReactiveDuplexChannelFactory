using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Reflection;
using Castle.DynamicProxy;

namespace OS.ReactiveDuplexChannel
{
  internal class ReactiveInteceptor : IInterceptor
  {
    private readonly ConcurrentDictionary<Type, OnNextInvoker> _subjects = new ConcurrentDictionary<Type, OnNextInvoker>();

    public void Intercept(IInvocation invocation)
    {
      if (invocation.Arguments.Length == 1)
      {
        if (_subjects.TryGetValue(invocation.Arguments[0].GetType(), out OnNextInvoker onNextInvoker))
        {
          onNextInvoker?.Invoke(invocation.Arguments[0]);
        }
      }
      else
      {
        //log
        invocation.Proceed();
      }
    }


    internal IObservable<T> AddListener<T>()
    {
      ValidateTypeInCallback<T>();
      var subject = new Subject<T>();
      return (Subject<T>)_subjects.GetOrAdd(typeof(T), OnNextInvoker.Create(subject)).Traget;
    }

    private void ValidateTypeInCallback<T>()
    {
      // throw new NotImplementedException();
    }

    private class OnNextInvoker
    {
      MethodInfo MethodInfo { get; set; }
      internal object Traget { get; private set; }

      internal static OnNextInvoker Create<T>(Subject<T> subject)
      {
        return new OnNextInvoker
        {
          Traget = subject,
          MethodInfo = typeof(Subject<T>).GetMethod(nameof(subject.OnNext))
        };
      }

      internal void Invoke(object input)
      {
        MethodInfo.Invoke(Traget, new[] { input });
      }
    }
  }
}
