using System;
using System.Collections.Generic;
using System.Linq;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Provides helper methods to invoke multicast delegates and aggregate results.
    /// </summary>
    public static class DelegateExtensions
    {
        /// <summary>
        /// Invokes each handler in the delegate invocation list and returns each handler result.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type being invoked.</typeparam>
        /// <typeparam name="TResult">The result type returned by each invocation.</typeparam>
        /// <param name="delegate">The multicast delegate instance.</param>
        /// <param name="defaultWhenNoInvocable">The default value returned when the delegate is null.</param>
        /// <param name="delegateInvoke">The invocation function used for each handler.</param>
        /// <returns>A sequence containing all handler results, or the default value if no handler exists.</returns>
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

        /// <summary>
        /// Invokes each handler in the delegate invocation list using dynamic invocation and returns each result.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type being invoked.</typeparam>
        /// <typeparam name="TResult">The result type returned by each invocation.</typeparam>
        /// <param name="delegate">The multicast delegate instance.</param>
        /// <param name="defaultWhenNoInvocable">The default value returned when the delegate is null.</param>
        /// <param name="args">The arguments passed to each delegate invocation.</param>
        /// <returns>A sequence containing all handler results, or the default value if no handler exists.</returns>
        public static IEnumerable<TResult> InvokeMany<TDelegate, TResult>(
            this TDelegate @delegate,
            TResult defaultWhenNoInvocable,
            params object[] args)
            where TDelegate : Delegate
            => @delegate.InvokeMany(defaultWhenNoInvocable, d => (TResult)d.DynamicInvoke(args));
    }
}
