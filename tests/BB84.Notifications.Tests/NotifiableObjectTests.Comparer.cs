// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using BB84.Notifications.Components;

namespace BB84.Notifications.Tests;

public sealed partial class NotifiableObjectTests
{
  private sealed class TestClassWithComparer : NotifiableObject
  {
    private string _property = string.Empty;

    public string Property
    {
      get => _property;
      set => SetProperty(ref _property, value, StringComparer.OrdinalIgnoreCase);
    }
  }

  [TestMethod]
  public void SetPropertyWithComparerDoesNotRaiseWhenEqual()
  {
    bool raised = false;
    TestClassWithComparer testClass = new();
    testClass.Property = "hello";
    testClass.PropertyChanged += (sender, e) => raised = true;

    testClass.Property = "HELLO";

    Assert.IsFalse(raised);
  }

  [TestMethod]
  public void SetPropertyWithComparerRaisesWhenNotEqual()
  {
    string? propertyName = string.Empty;
    TestClassWithComparer testClass = new();
    testClass.PropertyChanged += (sender, e) => propertyName = e.PropertyName;

    testClass.Property = "world";

    Assert.AreEqual(nameof(testClass.Property), propertyName);
  }

  [TestMethod]
  public void SetPropertyWithComparerChangingRaisesWhenNotEqual()
  {
    string? propertyName = string.Empty;
    string oldValue = string.Empty;
    TestClassWithComparer testClass = new();
    testClass.Property = "hello";
    testClass.PropertyChanging += (sender, e) =>
    {
      if (e is PropertyChangingEventArgs<string> sArgs)
        oldValue = sArgs.Value;
      propertyName = e.PropertyName;
    };

    testClass.Property = "world";

    Assert.AreEqual(nameof(testClass.Property), propertyName);
    Assert.AreEqual("hello", oldValue);
  }
}
