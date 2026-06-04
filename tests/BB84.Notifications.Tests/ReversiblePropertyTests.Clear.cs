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
  public void ClearResetsHistoryToCurrentValue()
  {
    ReversibleProperty<int> property = new(0);
    property.Value = 1;
    property.Value = 2;
    property.Value = 3;

    property.Clear();

    Assert.AreEqual(3, property.Value);
    Assert.AreEqual(1, property.Count);
    Assert.AreEqual(0, property.Index);
    Assert.IsFalse(property.HasNextValue);
    Assert.IsFalse(property.HasPreviousValue);
  }

  [TestMethod]
  [TestCategory("Method")]
  public void ClearAfterNavigateBackResetsToCurrentValue()
  {
    ReversibleProperty<int> property = new(0);
    property.Value = 1;
    property.Value = 2;
    property.PreviousValue();

    property.Clear();

    Assert.AreEqual(1, property.Value);
    Assert.AreEqual(1, property.Count);
    Assert.AreEqual(0, property.Index);
  }
}
