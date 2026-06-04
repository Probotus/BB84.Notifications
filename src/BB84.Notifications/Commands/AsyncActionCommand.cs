// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using BB84.Notifications.Extensions;
using BB84.Notifications.Interfaces.Commands;

namespace BB84.Notifications.Commands;

/// <summary>
/// Represents an asynchronous command that can be executed and queried for its ability to execute.
/// </summary>
/// <remarks>
/// When a <c>Func&lt;CancellationToken, Task&gt;</c> delegate is supplied the command manages its own
/// <see cref="CancellationTokenSource"/> internally. Call <see cref="Cancel"/> (or execute
/// <see cref="CancelCommand"/>) to cancel a running operation.
/// </remarks>
/// <param name="execute">The task to execute (without cancellation token).</param>
/// <param name="canExecute">The condition to execute.</param>
/// <param name="action">The action to invoke if an exception occurs.</param>
public sealed class AsyncActionCommand(Func<Task> execute, Func<bool>? canExecute = null, Action<Exception>? action = null) : IAsyncActionCommand
{
  private readonly Func<CancellationToken, Task> _execute = _ => execute();
  private readonly Func<bool>? _canExecute = canExecute;
  private readonly Action<Exception>? _action = action;

  private bool _isExecuting;
  private CancellationTokenSource? _cts;

  /// <summary>
  /// Initializes a new instance of <see cref="AsyncActionCommand"/> with a cancellable execute delegate.
  /// </summary>
  /// <param name="execute">The task to execute, accepting a <see cref="CancellationToken"/>.</param>
  /// <param name="canExecute">The condition to execute.</param>
  /// <param name="action">The action to invoke if an exception occurs.</param>
  public AsyncActionCommand(Func<CancellationToken, Task> execute, Func<bool>? canExecute = null, Action<Exception>? action = null)
    : this(() => execute(CancellationToken.None), canExecute, action)
  {
    _execute = execute;
  }

  /// <inheritdoc/>
  public bool IsCancellationRequested => _cts?.IsCancellationRequested ?? false;

  /// <inheritdoc/>
  public IActionCommand CancelCommand => new ActionCommand(Cancel, () => _isExecuting);

  /// <inheritdoc/>
  public event EventHandler? CanExecuteChanged;

  /// <inheritdoc/>
  public bool CanExecute()
    => CanExecute(null);

  /// <inheritdoc/>
  public void Cancel()
  {
    _cts?.Cancel();
    RaiseCanExecuteChanged();
  }

  /// <inheritdoc/>
  public Task ExecuteAsync()
    => ExecuteAsync(CancellationToken.None);

  /// <inheritdoc/>
  public async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    if (!CanExecute())
    {
      RaiseCanExecuteChanged();
      return;
    }

    using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    _cts = cts;

    try
    {
      _isExecuting = true;
      RaiseCanExecuteChanged();
      await _execute(cts.Token);
    }
    finally
    {
      _isExecuting = false;
      _cts = null;
      RaiseCanExecuteChanged();
    }
  }

  /// <inheritdoc/>
  public bool CanExecute(object? parameter)
    => !_isExecuting && (_canExecute?.Invoke() ?? true);

  /// <inheritdoc/>
  public void Execute(object? parameter)
    => ExecuteAsync().FireAndForgetSafeAsync(_action);

  /// <inheritdoc/>
  public void RaiseCanExecuteChanged()
    => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Represents an asynchronous command that can be executed with a parameter of type <typeparamref name="T"/>
/// and queried for its ability to execute.
/// </summary>
/// <remarks>
/// When a <c>Func&lt;T, CancellationToken, Task&gt;</c> delegate is supplied the command manages its own
/// <see cref="CancellationTokenSource"/> internally. Call <see cref="Cancel"/> (or execute
/// <see cref="CancelCommand"/>) to cancel a running operation.
/// </remarks>
/// <typeparam name="T">The generic type to work with.</typeparam>
/// <param name="execute">The task to execute (without cancellation token).</param>
/// <param name="canExecute">The condition to execute.</param>
/// <param name="action">The action to invoke if an exception occurs.</param>
public sealed class AsyncActionCommand<T>(Func<T, Task> execute, Func<T, bool>? canExecute = null, Action<Exception>? action = null) : IAsyncActionCommand<T>
{
  private readonly Func<T, CancellationToken, Task> _execute = (p, _) => execute(p);
  private readonly Func<T, bool>? _canExecute = canExecute;
  private readonly Action<Exception>? _action = action;

  private bool _isExecuting;
  private CancellationTokenSource? _cts;

  /// <summary>
  /// Initializes a new instance of <see cref="AsyncActionCommand{T}"/> with a cancellable execute delegate.
  /// </summary>
  /// <param name="execute">The task to execute, accepting a parameter and a <see cref="CancellationToken"/>.</param>
  /// <param name="canExecute">The condition to execute.</param>
  /// <param name="action">The action to invoke if an exception occurs.</param>
  public AsyncActionCommand(Func<T, CancellationToken, Task> execute, Func<T, bool>? canExecute = null, Action<Exception>? action = null)
    : this((p) => execute(p, CancellationToken.None), canExecute, action)
  {
    _execute = execute;
  }

  /// <inheritdoc/>
  public bool IsCancellationRequested => _cts?.IsCancellationRequested ?? false;

  /// <inheritdoc/>
  public IActionCommand CancelCommand => new ActionCommand(Cancel, () => _isExecuting);

  /// <inheritdoc/>
  public event EventHandler? CanExecuteChanged;

  /// <inheritdoc/>
  public bool CanExecute(T parameter)
    => !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

  /// <inheritdoc/>
  public void Cancel()
  {
    _cts?.Cancel();
    RaiseCanExecuteChanged();
  }

  /// <inheritdoc/>
  public bool CanExecute(object? parameter)
    => CanExecute((T)parameter!);

  /// <inheritdoc/>
  public void Execute(object? parameter)
    => ExecuteAsync((T)parameter!).FireAndForgetSafeAsync(_action);

  /// <inheritdoc/>
  public Task ExecuteAsync(T parameter)
    => ExecuteAsync(parameter, CancellationToken.None);

  /// <inheritdoc/>
  public async Task ExecuteAsync(T parameter, CancellationToken cancellationToken)
  {
    if (!CanExecute(parameter))
    {
      RaiseCanExecuteChanged();
      return;
    }

    using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    _cts = cts;

    try
    {
      _isExecuting = true;
      RaiseCanExecuteChanged();
      await _execute(parameter, cts.Token);
    }
    finally
    {
      _isExecuting = false;
      _cts = null;
      RaiseCanExecuteChanged();
    }
  }

  /// <inheritdoc/>
  public void RaiseCanExecuteChanged()
    => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
