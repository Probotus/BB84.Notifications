// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
namespace BB84.Notifications.Tests;

public sealed partial class ReversiblePropertyTests
{
  [TestMethod]
  [TestCategory("Method")]
  public void SnapshotReturnsAllValues()
  {
    ReversibleProperty<int> property = new(0);
    property.Value = 1;
    property.Value = 2;

    IReadOnlyList<int> snapshot = property.Snapshot();

    Assert.AreEqual(3, snapshot.Count);
    Assert.AreEqual(0, snapshot[0]);
    Assert.AreEqual(1, snapshot[1]);
    Assert.AreEqual(2, snapshot[2]);
  }

  [TestMethod]
  [TestCategory("Method")]
  public void SnapshotAfterClearContainsOnlyCurrentValue()
  {
    ReversibleProperty<int> property = new(0);
    property.Value = 1;
    property.Value = 2;
    property.Clear();

    IReadOnlyList<int> snapshot = property.Snapshot();

    Assert.AreEqual(1, snapshot.Count);
    Assert.AreEqual(2, snapshot[0]);
  }
}
