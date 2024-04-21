﻿namespace BB84.NotificationsTests.Commands;

public sealed partial class AsyncActionCommandTests
{
  [TestMethod]
  public void CanExecute()
  {
    TestClass? test;

    test = new();

    Assert.IsTrue(test.Command.CanExecute());
    Assert.IsTrue(test.Command.CanExecute(null));
    Assert.IsFalse(test.CondCommand.CanExecute());
    Assert.IsFalse(test.CondCommand.CanExecute(null));
  }

  [TestMethod]
  public void CanExecuteWithParams()
  {
    TestClass? test;

    test = new();

    Assert.IsTrue(test.IntCommand.CanExecute(1));
    Assert.IsTrue(test.IntCommand.CanExecute((object)1));
    Assert.IsFalse(test.CondIntCommand.CanExecute(1));
    Assert.IsFalse(test.CondIntCommand.CanExecute((object)1));
  }
}
