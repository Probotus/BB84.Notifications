// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
namespace BB84.Notifications.Tests;

public sealed partial class ReversiblePropertyTests
{
  [TestMethod]
  [TestCategory("OverflowStrategy")]
  public void EvictOldestDropsOldestWhenFull()
  {
    ReversibleProperty<int> property = new(0, size: 3, overflow: OverflowStrategy.EvictOldest);
    property.Value = 1;
    property.Value = 2;

    // Buffer is now full: [0, 1, 2]
    property.Value = 3;

    // Oldest (0) should be evicted: [1, 2, 3]
    Assert.AreEqual(3, property.Count);
    Assert.AreEqual(3, property.Value);
    Assert.AreEqual(2, property.Index);
    Assert.AreEqual(1, property.Snapshot()[0]);
  }

  [TestMethod]
  [TestCategory("OverflowStrategy")]
  public void EvictNewestIgnoresNewValueWhenFull()
  {
    ReversibleProperty<int> property = new(0, size: 3, overflow: OverflowStrategy.EvictNewest);
    property.Value = 1;
    property.Value = 2;

    // Buffer is now full: [0, 1, 2]
    bool changedRaised = false;
    property.PropertyChanged += (s, e) => changedRaised = true;
    property.Value = 3;

    Assert.AreEqual(3, property.Count);
    Assert.AreEqual(2, property.Value);
    Assert.AreEqual(2, property.Index);
    Assert.IsFalse(changedRaised);
  }

  [TestMethod]
  [TestCategory("OverflowStrategy")]
  public void ThrowThrowsWhenFull()
  {
    ReversibleProperty<int> property = new(0, size: 3, overflow: OverflowStrategy.Throw);
    property.Value = 1;
    property.Value = 2;

    // Buffer is now full: [0, 1, 2]
    Assert.ThrowsExactly<InvalidOperationException>(() => property.Value = 3);
  }

  [TestMethod]
  [TestCategory("OverflowStrategy")]
  public void ThrowDoesNotRaiseEventsWhenFull()
  {
    ReversibleProperty<int> property = new(0, size: 3, overflow: OverflowStrategy.Throw);
    property.Value = 1;
    property.Value = 2;

    bool changedRaised = false;
    property.PropertyChanged += (s, e) => changedRaised = true;

    try { property.Value = 3; } catch (InvalidOperationException) { }

    Assert.IsFalse(changedRaised);
  }
}
