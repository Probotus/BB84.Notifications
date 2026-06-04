// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using BB84.Notifications.Commands;

namespace BB84.Notifications.Tests.Commands;

public sealed partial class AsyncActionCommandTests
{
  [TestMethod]
  public async Task ExecuteAsyncWithCancellableDelegatePassesToken()
  {
    CancellationToken captured = CancellationToken.None;
    AsyncActionCommand command = new(async ct =>
    {
      captured = ct;
      await Task.CompletedTask;
    });

    await command.ExecuteAsync();

    Assert.IsFalse(captured.IsCancellationRequested);
  }

  [TestMethod]
  public async Task ExecuteAsyncWithExternalTokenPassesLinkedToken()
  {
    using CancellationTokenSource cts = new();
    bool wasCancelled = false;

    AsyncActionCommand command = new(async ct =>
    {
      await Task.Delay(500, ct);
      wasCancelled = ct.IsCancellationRequested;
    });

    cts.CancelAfter(50);

    try
    {
      await command.ExecuteAsync(cts.Token);
    }
    catch (OperationCanceledException) { }

    Assert.IsFalse(wasCancelled); // OperationCanceledException thrown before assignment
  }

  [TestMethod]
  public async Task CancelStopsExecution()
  {
    TaskCompletionSource<bool> started = new();
    bool wasCancelled = false;
    AsyncActionCommand command = new(async ct =>
    {
      started.SetResult(true);
      try { await Task.Delay(5000, ct); }
      catch (OperationCanceledException) { wasCancelled = true; throw; }
    });

    Task execution = command.ExecuteAsync();
    await started.Task;

    command.Cancel();

    try { await execution; } catch (OperationCanceledException) { }

    Assert.IsTrue(wasCancelled);
  }

  [TestMethod]
  public async Task CancelCommandExecuteCancelsRunningOperation()
  {
    TaskCompletionSource<bool> started = new();
    bool wasCancelled = false;
    AsyncActionCommand command = new(async ct =>
    {
      started.SetResult(true);
      try { await Task.Delay(5000, ct); }
      catch (OperationCanceledException) { wasCancelled = true; throw; }
    });

    Task execution = command.ExecuteAsync();
    await started.Task;

    command.CancelCommand.Execute();

    try { await execution; } catch (OperationCanceledException) { }

    Assert.IsTrue(wasCancelled);
  }

  [TestMethod]
  public async Task CanExecuteIsFalseWhileExecuting()
  {
    TaskCompletionSource<bool> started = new();
    TaskCompletionSource<bool> release = new();
    AsyncActionCommand command = new(async ct =>
    {
      started.SetResult(true);
      await release.Task;
    });

    Task execution = command.ExecuteAsync();
    await started.Task;

    Assert.IsFalse(command.CanExecute());

    release.SetResult(true);
    await execution;
  }

  [TestMethod]
  public async Task ExecuteAsyncWithCancellableDelegateGenericPassesToken()
  {
    CancellationToken captured = CancellationToken.None;
    AsyncActionCommand<int> command = new(async (p, ct) =>
    {
      captured = ct;
      await Task.CompletedTask;
    });

    await command.ExecuteAsync(42);

    Assert.IsFalse(captured.IsCancellationRequested);
  }

  [TestMethod]
  public async Task CancelGenericStopsExecution()
  {
    TaskCompletionSource<bool> started = new();
    bool wasCancelled = false;
    AsyncActionCommand<int> command = new(async (p, ct) =>
    {
      started.SetResult(true);
      try { await Task.Delay(5000, ct); }
      catch (OperationCanceledException) { wasCancelled = true; throw; }
    });

    Task execution = command.ExecuteAsync(1);
    await started.Task;

    command.Cancel();

    try { await execution; } catch (OperationCanceledException) { }

    Assert.IsTrue(wasCancelled);
  }

  [TestMethod]
  public async Task CanExecuteIsFalseWhileExecutingGeneric()
  {
    TaskCompletionSource<bool> started = new();
    TaskCompletionSource<bool> release = new();
    AsyncActionCommand<int> command = new(async (p, ct) =>
    {
      started.SetResult(true);
      await release.Task;
    });

    Task execution = command.ExecuteAsync(1);
    await started.Task;

    Assert.IsFalse(command.CanExecute(1));

    release.SetResult(true);
    await execution;
  }
}
