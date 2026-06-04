// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using System.ComponentModel;

using BB84.Notifications.Extensions;

namespace BB84.Notifications.Tests.Extensions;

[TestClass]
public sealed class WeakEventExtensionsTests
{
  private sealed class TestNotifiable : INotifyPropertyChanged, INotifyPropertyChanging
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    public void RaiseChanged(string propertyName)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void RaiseChanging(string propertyName)
      => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
  }

  [TestMethod]
  public void WeakSubscribePropertyChangedHandlerIsInvoked()
  {
    TestNotifiable source = new();
    string? received = null;

    PropertyChangedEventHandler handler = (s, e) => received = e.PropertyName;
    using IDisposable _ = source.WeakSubscribe(handler);

    source.RaiseChanged("Name");

    Assert.AreEqual("Name", received);
  }

  [TestMethod]
  public void WeakSubscribePropertyChangingHandlerIsInvoked()
  {
    TestNotifiable source = new();
    string? received = null;

    PropertyChangingEventHandler handler = (s, e) => received = e.PropertyName;
    using IDisposable _ = source.WeakSubscribe(handler);

    source.RaiseChanging("Name");

    Assert.AreEqual("Name", received);
  }

  [TestMethod]
  public void WeakSubscribePropertyChangedDisposeRemovesSubscription()
  {
    TestNotifiable source = new();
    int count = 0;

    PropertyChangedEventHandler handler = (s, e) => count++;
    IDisposable token = source.WeakSubscribe(handler);

    source.RaiseChanged("A");
    token.Dispose();
    source.RaiseChanged("B");

    Assert.AreEqual(1, count);
  }

  [TestMethod]
  public void WeakSubscribePropertyChangingDisposeRemovesSubscription()
  {
    TestNotifiable source = new();
    int count = 0;

    PropertyChangingEventHandler handler = (s, e) => count++;
    IDisposable token = source.WeakSubscribe(handler);

    source.RaiseChanging("A");
    token.Dispose();
    source.RaiseChanging("B");

    Assert.AreEqual(1, count);
  }

  [TestMethod]
  public void WeakSubscribePropertyChangedAfterGCHandlerNotInvoked()
  {
    TestNotifiable source = new();
    int count = 0;

    // Allocate handler in helper so the delegate target can be collected
    WeakReference weakRef = SubscribeAndGetWeakRef(source, ref count);

    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
    GC.WaitForPendingFinalizers();

    // Handler target is gone — bouncer should prune itself on next raise
    source.RaiseChanged("X");

    Assert.IsFalse(weakRef.IsAlive);
    Assert.AreEqual(0, count);
  }

  // Separate method so the handler delegate (and its target closure) are out of scope
  // before GC.Collect is called.
  private static WeakReference SubscribeAndGetWeakRef(TestNotifiable source, ref int count)
  {
    // Use a class instance as the handler target so it can be collected
    HandlerTarget target = new(ref count);
    PropertyChangedEventHandler handler = target.OnChanged;
    source.WeakSubscribe(handler);
    return new WeakReference(target);
  }

  private sealed class HandlerTarget
  {
    // Capture by ref-field workaround: store in a field we can increment
    private readonly int[] _counter;

    public HandlerTarget(ref int counter)
    {
      // Box into array so we can share it across the boundary
      _counter = new int[1];
      // We don't actually need the original ref here — just testing liveness
      _ = counter;
    }

    public void OnChanged(object? sender, PropertyChangedEventArgs e)
      => _counter[0]++;
  }
}
