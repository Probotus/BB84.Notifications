// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using System.ComponentModel;

using BB84.Notifications.Components;

namespace BB84.Notifications.Tests;

public sealed partial class NotifiableCollectionOfTTests
{
  [TestMethod]
  public void Removing()
  {
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanging += (s, e) =>
    {
      if (e is CollectionChangingEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    _ = strings.Remove(UnitTestString);

    Assert.AreEqual(CollectionChangeAction.Remove, action);
    Assert.AreEqual(UnitTestString, item);
  }

  [TestMethod]
  public void Removed()
  {
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanged += (s, e) =>
    {
      if (e is CollectionChangedEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    _ = strings.Remove(UnitTestString);

    Assert.AreEqual(CollectionChangeAction.Remove, action);
    Assert.AreEqual(UnitTestString, item);
  }

  [TestMethod]
  public void RemovingAt()
  {
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanging += (s, e) =>
    {
      if (e is CollectionChangingEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    strings.RemoveAt(0);

    Assert.AreEqual(CollectionChangeAction.Remove, action);
    Assert.AreEqual(UnitTestString, item);
  }

  [TestMethod]
  public void RemovedAt()
  {
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanged += (s, e) =>
    {
      if (e is CollectionChangedEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    strings.RemoveAt(0);

    Assert.AreEqual(CollectionChangeAction.Remove, action);
    Assert.AreEqual(UnitTestString, item);
  }

  [TestMethod]
  public void RemoveReturnsFalseWhenNotFound()
  {
    NotifiableCollection<string> strings = [];

    bool result = strings.Remove(UnitTestString);

    Assert.IsFalse(result);
  }
}
