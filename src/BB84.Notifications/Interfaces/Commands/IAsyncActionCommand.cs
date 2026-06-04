// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using System.Windows.Input;

namespace BB84.Notifications.Interfaces.Commands;

/// <summary>
/// Represents an asynchronous command that can be executed and queried for its ability to execute.
/// </summary>
/// <remarks>
/// This interface extends <see cref="ICommand"/> to support asynchronous operations.
/// It provides methods for executing the command asynchronously, determining whether the command can execute,
/// and raising notifications when the ability to execute changes.
/// </remarks>
public interface IAsyncActionCommand : ICommand
{
  /// <summary>
  /// Gets a value indicating whether cancellation has been requested for the currently executing operation.
  /// </summary>
  bool IsCancellationRequested { get; }

  /// <summary>
  /// Gets an <see cref="IActionCommand"/> that, when executed, cancels the currently running operation.
  /// </summary>
  IActionCommand CancelCommand { get; }

  /// <summary>
  /// Defines the method to be called when the command is invoked.
  /// </summary>
  /// <returns><see cref="Task"/></returns>
  Task ExecuteAsync();

  /// <summary>
  /// Defines the method to be called when the command is invoked with the specified cancellation token.
  /// </summary>
  /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
  /// <returns><see cref="Task"/></returns>
  Task ExecuteAsync(CancellationToken cancellationToken);

  /// <summary>
  /// Defines the method that determines whether the command can execute in its current state.
  /// </summary>
  /// <returns>True if this command can be executed, otherwise false.</returns>
  bool CanExecute();

  /// <summary>
  /// Requests cancellation of the currently executing operation.
  /// </summary>
  void Cancel();

  /// <summary>
  /// Notifies that the <see cref="ICommand.CanExecuteChanged"/> property has changed.
  /// </summary>
  void RaiseCanExecuteChanged();
}

/// <summary>
/// Represents an asynchronous command that can be executed with a parameter of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// This interface extends <see cref="ICommand"/> to support asynchronous operations.
/// It provides methods to execute the command asynchronously, determine whether the command can be executed,
/// and notify changes to the command's ability to execute.
/// </remarks>
/// <typeparam name="T">The type of the parameter used by the command.</typeparam>
public interface IAsyncActionCommand<T> : ICommand
{
  /// <summary>
  /// Gets a value indicating whether cancellation has been requested for the currently executing operation.
  /// </summary>
  bool IsCancellationRequested { get; }

  /// <summary>
  /// Gets an <see cref="IActionCommand"/> that, when executed, cancels the currently running operation.
  /// </summary>
  IActionCommand CancelCommand { get; }

  /// <summary>
  /// Executes an asynchronous operation using the specified parameter.
  /// </summary>
  /// <param name="parameter">The input parameter required to perform the operation.</param>
  /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
  Task ExecuteAsync(T parameter);

  /// <summary>
  /// Executes an asynchronous operation using the specified parameter and cancellation token.
  /// </summary>
  /// <param name="parameter">The input parameter required to perform the operation.</param>
  /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
  /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
  Task ExecuteAsync(T parameter, CancellationToken cancellationToken);

  /// <summary>
  /// Determines whether the command can execute with the specified parameter.
  /// </summary>
  /// <param name="parameter">The parameter to evaluate.</param>
  /// <returns>
  /// <see langword="true"/> if the command can execute with the specified parameter; otherwise, <see langword="false"/>.
  /// </returns>
  bool CanExecute(T parameter);

  /// <summary>
  /// Requests cancellation of the currently executing operation.
  /// </summary>
  void Cancel();

  /// <summary>
  /// Notifies subscribers that the ability to execute the command may have changed.
  /// </summary>
  void RaiseCanExecuteChanged();
}
