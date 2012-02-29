using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace TKLib
{
    /// <summary>
    /// Manages weak references to <see cref="INotifyPropertyChanged"/> 
    /// subscribers and provides a friendly interface for <see cref="INotifyPropertyChanged"/> 
    /// implementors to expose typed event subscription without relying on 
    /// property string names.
    /// </summary>
    /// <remarks>
    /// This class never leaks references to subscribers. 
    /// </remarks>
    /// <typeparam name="TSource">The type of the property changed source.</typeparam>
    public class PropertyChangeManager<TSource>
        where TSource : INotifyPropertyChanged
    {
        private List<Subscription> subscriptions = new List<Subscription>();
        private TSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeManager&lt;TSource&gt;"/> class.
        /// </summary>
        /// <param name="source">The property changed source.</param>
        public PropertyChangeManager(TSource source)
        {
            this.source = source;
        }

        /// <summary>
        /// Subscribes to changes in the property referenced in the given 
        /// <paramref name="propertyExpression"/> with the given 
        /// <paramref name="callbackAction"/> delegate.
        /// </summary>
        /// <param name="propertyExpression">A lambda expression that accesses a property, such as <c>x => x.Name</c> 
        /// (where the type of x is <typeparamref name="TSource"/>).</param>
        /// <param name="callbackAction">The callback action to invoke when the given property changes.</param>
        public IDisposable SubscribeChanged(Expression<Func<TSource, object>> propertyExpression, Action<TSource> callbackAction)
        {
            return AddSubscription(new Subscription
            {
                IsStatic = callbackAction.Target == null,
                PropertyName = Reflect<TSource>.GetProperty(propertyExpression).Name,
                SubscriberReference = new WeakReference(callbackAction.Target),
                MethodCallback = callbackAction.Method
            });
        }

        /// <summary>
        /// Registers a regular event handler for change notification.
        /// </summary>
        public IDisposable AddHandler(PropertyChangedEventHandler handler)
        {
            return AddSubscription(new Subscription
            {
                IsStatic = handler.Target == null,
                SubscriberReference = new WeakReference(handler.Target),
                MethodCallback = handler.Method
            });
        }

        /// <summary>
        /// Unregisters the given event handler from change notification.
        /// </summary>
        /// <param name="handler">The value.</param>
        public void RemoveHandler(PropertyChangedEventHandler handler)
        {
            CleanupSubscribers();

            subscriptions.RemoveAll(s => s.SubscriberReference.Target == handler.Target && s.MethodCallback == handler.Method);
        }

        /// <summary>
        /// Notifies subscribers that the given property has changed.
        /// </summary>
        /// <param name="propertyExpression">A lambda expression that accesses a property, such as <c>x => x.Name</c> 
        /// (where the type of x is <typeparamref name="TSource"/>).</param>
        public void NotifyChanged(Expression<Func<TSource, object>> propertyExpression)
        {
            CleanupSubscribers();

            var propertyName = Reflect<TSource>.GetProperty(propertyExpression).Name;

            foreach (var subscription in subscriptions.Where(s => s.PropertyName == propertyName))
            {
                try
                {
                    subscription.MethodCallback.Invoke(subscription.SubscriberReference.Target, new object[] { this.source });
                }
                catch (TargetInvocationException tie)
                {
                    tie.InnerException.RethrowWithNoStackTraceLoss();
                }
            }

            // Call "old-style" handlers with the right signature.
                foreach (var subscription in subscriptions.Where(s => s.PropertyName == null))
                {
                    try
                    {
                        subscription.MethodCallback.Invoke(subscription.SubscriberReference.Target, new object[] { this.source, new PropertyChangedEventArgs(propertyName) });
                    }
                    catch (TargetInvocationException tie)
                    {
                        tie.InnerException.RethrowWithNoStackTraceLoss();
                    }
                }
            
        }

        private IDisposable AddSubscription(Subscription subscription)
        {
            CleanupSubscribers();

            subscriptions.Add(subscription);

            return new SubscriptionReference(this.subscriptions, subscription);
        }

        private void CleanupSubscribers()
        {
            subscriptions.RemoveAll(s => !s.IsStatic && !s.SubscriberReference.IsAlive);
        }

        /// <summary>
        /// Provides deterministic removal of a subscription without having to 
        /// create a separate class to hold the delegate reference. 
        /// Callers can simply keep the returned disposable from Subscribe 
        /// and use it to unsubscribe.
        /// </summary>
        private sealed class SubscriptionReference : IDisposable
        {
            private List<Subscription> subscriptions;
            private Subscription entry;

            public SubscriptionReference(List<Subscription> subscriptions, Subscription entry)
            {
                this.subscriptions = subscriptions;
                this.entry = entry;
            }

            public void Dispose()
            {
                this.subscriptions.Remove(this.entry);
            }
        }

        private class Subscription
        {
            public bool IsStatic { get; set; }
            public string PropertyName { get; set; }
            public WeakReference SubscriberReference { get; set; }
            public MethodInfo MethodCallback { get; set; }
        }
    }

    #region Helpers

    /// <summary>
    /// Provides strong-typed reflection of the <typeparamref name="TTarget"/> 
    /// type.
    /// </summary>
    /// <typeparam name="TTarget">Type to reflect.</typeparam>
    internal static class Reflect<TTarget>
    {
        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <param name="method">An expression that invokes a method.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
        /// <returns>The method info.</returns>
        public static MethodInfo GetMethod(Expression<Action<TTarget>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <param name="method">An expression that invokes a method.</param>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
        /// <returns>The method info.</returns>
        public static MethodInfo GetMethod<T1>(Expression<Action<TTarget, T1>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <param name="method">An expression that invokes a method.</param>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument.</typeparam>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
        /// <returns>The method info.</returns>
        public static MethodInfo GetMethod<T1, T2>(Expression<Action<TTarget, T1, T2>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <param name="method">An expression that invokes a method.</param>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument.</typeparam>
        /// <typeparam name="T3">Type of the third argument.</typeparam>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
        /// <returns>The method info.</returns>
        public static MethodInfo GetMethod<T1, T2, T3>(Expression<Action<TTarget, T1, T2, T3>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the property represented by the lambda expression.
        /// </summary>
        /// <param name="property">An expression that accesses a property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a property access.</exception>
        /// <returns>The property info.</returns>
        public static PropertyInfo GetProperty(Expression<Func<TTarget, object>> property)
        {
            PropertyInfo info = GetMemberInfo(property) as PropertyInfo;
            if (info == null)
            {
                throw new ArgumentException("Member is not a property");
            }

            return info;
        }

        /// <summary>
        /// Gets the field represented by the lambda expression.
        /// </summary>
        /// <param name="field">An expression that accesses a field.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a field access.</exception>
        /// <returns>The field info.</returns>
        public static FieldInfo GetField(Expression<Func<TTarget, object>> field)
        {
            FieldInfo info = GetMemberInfo(field) as FieldInfo;
            if (info == null)
            {
                throw new ArgumentException("Member is not a field");
            }

            return info;
        }

        private static MethodInfo GetMethodInfo(Expression method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentException("Not a lambda expression", "method");
            }

            if (lambda.Body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException("Not a method call", "method");
            }

            return ((MethodCallExpression)lambda.Body).Method;
        }

        private static MemberInfo GetMemberInfo(Expression member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            LambdaExpression lambda = member as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentException("Not a lambda expression", "member");
            }

            MemberExpression memberExpr = null;

            // The Func<TTarget, object> we use returns an object, so first statement can be either 
            // a cast (if the field/property does not return an object) or the direct member access.
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                // The cast is an unary expression, where the operand is the 
                // actual member access expression.
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
            {
                throw new ArgumentException("Not a member access", "member");
            }

            return memberExpr.Member;
        }
    }

    /// <summary>
    /// Utility methods for exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        private static readonly FieldInfo remoteStackTraceString =
            typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic) ??
            typeof(Exception).GetField("remote_stack_trace", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Rethrows an exception object without losing the existing stack trace information.
        /// </summary>
        /// <param name="ex">The exception to re-throw.</param>
        /// <remarks>
        /// For more information on this technique, see
        /// http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2004/03/03/8353.aspx
        /// </remarks>
        public static void RethrowWithNoStackTraceLoss(this Exception ex)
        {
            remoteStackTraceString.SetValue(ex, ex.StackTrace + Environment.NewLine);

            throw ex;
        }
    }

    #endregion
}
