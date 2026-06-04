// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using System.ComponentModel.DataAnnotations;

namespace BB84.Notifications.Tests;

public sealed partial class ValidatableObjectTests
{
  private sealed class TestClassWithComparer : ValidatableObject
  {
    private string _name = string.Empty;

    [Required, MaxLength(50)]
    public string Name
    {
      get => _name;
      set => SetPropertyAndValidate(ref _name, value, StringComparer.OrdinalIgnoreCase);
    }
  }

  [TestMethod]
  public void SetPropertyAndValidateWithComparerDoesNotRaiseWhenEqual()
  {
    bool raised = false;
    TestClassWithComparer testClass = new();
    testClass.Name = "hello";
    testClass.PropertyChanged += (sender, e) => raised = true;

    testClass.Name = "HELLO";

    Assert.IsFalse(raised);
  }

  [TestMethod]
  public void SetPropertyAndValidateWithComparerRaisesWhenNotEqual()
  {
    string? propertyName = string.Empty;
    TestClassWithComparer testClass = new();
    testClass.PropertyChanged += (sender, e) => propertyName = e.PropertyName;

    testClass.Name = "world";

    Assert.AreEqual(nameof(testClass.Name), propertyName);
    Assert.AreEqual("world", testClass.Name);
  }
}
