// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using System.ComponentModel;

namespace BB84.Notifications.Extensions;

/// <summary>
/// Provides weak-reference subscription extensions for <see cref="INotifyPropertyChanged"/>
/// and <see cref="INotifyPropertyChanging"/>.
/// </summary>
/// <remarks>
/// Standard event subscriptions hold a strong reference to the handler delegate and, through it,
/// to the handler's target object. This prevents short-lived subscribers from being garbage-collected
/// as long as the event source is alive. The methods in this class store the handler as a
/// <see cref="WeakReference{T}"/> so that the subscriber can be collected normally. A lightweight
/// "bouncer" delegate is subscribed to the source event; it forwards calls when the original handler
/// is still alive and removes itself automatically once the handler has been collected.
/// </remarks>
public static class WeakEventExtensions
{
  /// <summary>
  /// Subscribes <paramref name="eventHandler"/> to <paramref name="source"/>'s
  /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event via a weak reference.
  /// </summary>
  /// <remarks>
  /// The subscription is automatically removed the first time the event fires after
  /// <paramref name="eventHandler"/>'s target has been garbage-collected. Call
  /// <see cref="IDisposable.Dispose"/> on the returned token to remove the subscription eagerly.
  /// </remarks>
  /// <param name="source">The object whose <see cref="INotifyPropertyChanged.PropertyChanged"/> event to subscribe to.</param>
  /// <param name="eventHandler">The handler to invoke on each property-changed notification.</param>
  /// <returns>
  /// An <see cref="IDisposable"/> token that, when disposed, removes the subscription immediately.
  /// </returns>
  public static IDisposable WeakSubscribe(this INotifyPropertyChanged source, PropertyChangedEventHandler eventHandler)
  {
    WeakReference<PropertyChangedEventHandler> weakHandler = new(eventHandler);
    PropertyChangedEventHandler? bouncer = null;

    bouncer = (sender, args) =>
    {
      if (weakHandler.TryGetTarget(out PropertyChangedEventHandler? targetHandler))
        targetHandler(sender, args);
      else
        source.PropertyChanged -= bouncer;
    };

    source.PropertyChanged += bouncer;
    return new WeakSubscriptionToken(() => source.PropertyChanged -= bouncer);
  }

  /// <summary>
  /// Subscribes <paramref name="eventHandler"/> to <paramref name="source"/>'s
  /// <see cref="INotifyPropertyChanging.PropertyChanging"/> event via a weak reference.
  /// </summary>
  /// <remarks>
  /// The subscription is automatically removed the first time the event fires after
  /// <paramref name="eventHandler"/>'s target has been garbage-collected. Call
  /// <see cref="IDisposable.Dispose"/> on the returned token to remove the subscription eagerly.
  /// </remarks>
  /// <param name="source">The object whose <see cref="INotifyPropertyChanging.PropertyChanging"/> event to subscribe to.</param>
  /// <param name="eventHandler">The handler to invoke on each property-changing notification.</param>
  /// <returns>
  /// An <see cref="IDisposable"/> token that, when disposed, removes the subscription immediately.
  /// </returns>
  public static IDisposable WeakSubscribe(this INotifyPropertyChanging source, PropertyChangingEventHandler eventHandler)
  {
    WeakReference<PropertyChangingEventHandler> weakHandler = new(eventHandler);
    PropertyChangingEventHandler? bouncer = null;

    bouncer = (sender, args) =>
    {
      if (weakHandler.TryGetTarget(out PropertyChangingEventHandler? targetHandler))
        targetHandler(sender, args);
      else
        source.PropertyChanging -= bouncer;
    };

    source.PropertyChanging += bouncer;
    return new WeakSubscriptionToken(() => source.PropertyChanging -= bouncer);
  }

  /// <summary>
  /// A disposable token that removes a weak-event subscription when disposed.
  /// </summary>
  private sealed class WeakSubscriptionToken : IDisposable
  {
    private Action? _unsubscribe;

    internal WeakSubscriptionToken(Action unsubscribe)
      => _unsubscribe = unsubscribe;

    /// <inheritdoc/>
    public void Dispose()
    {
      _unsubscribe?.Invoke();
      _unsubscribe = null;
    }
  }
}
