using System;
using System.Collections.Generic;
using System.Linq;

public static class DelegateExtensions
{
    public static IEnumerable<TResult> InvokeMany<TDelegate, TResult>(
        this TDelegate @delegate,
        TResult defaultWhenNoInvocable,
        Func<TDelegate, TResult> delegateInvoke)
        where TDelegate : Delegate
    {
        List<TResult> results = new();
        if (@delegate != null)
        {
            foreach (TDelegate handler in @delegate.GetInvocationList().OfType<TDelegate>())
            {
                results.Add(delegateInvoke(handler));
            }
        }
        else
        {
            results.Add(defaultWhenNoInvocable);
        }
        return results;
    }

    public static IEnumerable<TResult> InvokeMany<TDelegate, TResult>(
        this TDelegate @delegate,
        TResult defaultWhenNoInvocable,
        params object[] args)
        where TDelegate : Delegate
        => @delegate.InvokeMany(defaultWhenNoInvocable, d => (TResult)d.DynamicInvoke(args));
}
