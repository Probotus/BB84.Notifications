// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
namespace BB84.Notifications.Tests;

public sealed partial class NotifiablePropertyTests
{
  [TestMethod]
  public void SetPropertyWithComparerDoesNotRaiseWhenEqual()
  {
    bool raised = false;
    NotifiableProperty<string> property = new("hello", StringComparer.OrdinalIgnoreCase);
    property.PropertyChanged += (sender, e) => raised = true;

    property.Value = "HELLO";

    Assert.IsFalse(raised);
  }

  [TestMethod]
  public void SetPropertyWithComparerRaisesWhenNotEqual()
  {
    bool raised = false;
    NotifiableProperty<string> property = new("hello", StringComparer.OrdinalIgnoreCase);
    property.PropertyChanged += (sender, e) => raised = true;

    property.Value = "world";

    Assert.IsTrue(raised);
    Assert.AreEqual("world", property.Value);
  }

  [TestMethod]
  public void SetPropertyWithComparerChangingRaisesWhenNotEqual()
  {
    bool raised = false;
    NotifiableProperty<string> property = new("hello", StringComparer.OrdinalIgnoreCase);
    property.PropertyChanging += (sender, e) => raised = true;

    property.Value = "world";

    Assert.IsTrue(raised);
  }
}
