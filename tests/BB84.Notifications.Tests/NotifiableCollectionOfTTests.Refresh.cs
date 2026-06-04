// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using System.ComponentModel;

namespace BB84.Notifications.Tests;

public sealed partial class NotifiableCollectionOfTTests
{
  [TestMethod]
  public void Refreshing()
  {
    CollectionChangeAction action = default!;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanging += (s, e) => action = e.Action;

    strings.Clear();

    Assert.AreEqual(CollectionChangeAction.Refresh, action);
  }

  [TestMethod]
  public void Refreshed()
  {
    CollectionChangeAction action = default!;
    NotifiableCollection<string> strings = [UnitTestString];
    strings.CollectionChanged += (s, e) => action = e.Action;

    strings.Clear();

    Assert.AreEqual(CollectionChangeAction.Refresh, action);
  }
}
