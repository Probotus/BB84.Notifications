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
  public void IndexerGet()
  {
    NotifiableCollection<string> strings = [UnitTestString];

    string result = strings[0];

    Assert.AreEqual(UnitTestString, result);
  }

  [TestMethod]
  public void IndexerSetting()
  {
    const string newValue = "NewValue";
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanging += (s, e) =>
    {
      if (e is CollectionChangingEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    strings[0] = newValue;

    Assert.AreEqual(CollectionChangeAction.Refresh, action);
    Assert.AreEqual(UnitTestString, item);
  }

  [TestMethod]
  public void IndexerSet()
  {
    const string newValue = "NewValue";
    CollectionChangeAction action = default!;
    string item = string.Empty;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanged += (s, e) =>
    {
      if (e is CollectionChangedEventArgs<string> stringEvent)
        item = stringEvent.Item;
      action = e.Action;
    };

    strings[0] = newValue;

    Assert.AreEqual(CollectionChangeAction.Refresh, action);
    Assert.AreEqual(newValue, item);
  }

  [TestMethod]
  public void IndexOfReturnsCorrectIndex()
  {
    NotifiableCollection<string> strings = [UnitTestString];

    int index = strings.IndexOf(UnitTestString);

    Assert.AreEqual(0, index);
  }

  [TestMethod]
  public void ContainsReturnsTrueWhenItemExists()
  {
    NotifiableCollection<string> strings = [UnitTestString];

    bool result = strings.Contains(UnitTestString);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public void CopyToCopiesItems()
  {
    NotifiableCollection<string> strings = [UnitTestString];
    string[] array = new string[1];

    strings.CopyTo(array, 0);

    Assert.AreEqual(UnitTestString, array[0]);
  }

  [TestMethod]
  public void ConstructorWithCollectionContainsItems()
  {
    NotifiableCollection<string> strings = new([UnitTestString]);

    Assert.AreEqual(1, strings.Count);
    Assert.AreEqual(UnitTestString, strings[0]);
  }
}
