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
  public void Adding()
  {
    CollectionChangeAction action = default!;
    NotifiableCollection<string> strings = [];
    strings.CollectionChanging += (s, e) => action = e.Action;

    strings.Add(UnitTestString);

    Assert.AreEqual(CollectionChangeAction.Add, action);
  }

  [TestMethod]
  public void Added()
  {
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [];
    strings.CollectionChanged += (s, e) =>
    {
      if (e is CollectionChangedEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    strings.Add(UnitTestString);

    Assert.AreEqual(CollectionChangeAction.Add, action);
    Assert.AreEqual(UnitTestString, item);
  }

  [TestMethod]
  public void Inserting()
  {
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [];
    strings.CollectionChanging += (s, e) =>
    {
      if (e is CollectionChangingEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    strings.Insert(0, UnitTestString);

    Assert.AreEqual(CollectionChangeAction.Add, action);
    Assert.AreEqual(UnitTestString, item);
  }

  [TestMethod]
  public void Inserted()
  {
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [];
    strings.CollectionChanged += (s, e) =>
    {
      if (e is CollectionChangedEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    strings.Insert(0, UnitTestString);

    Assert.AreEqual(CollectionChangeAction.Add, action);
    Assert.AreEqual(UnitTestString, item);
  }
}
