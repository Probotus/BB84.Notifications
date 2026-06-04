// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
namespace BB84.Notifications;

/// <summary>
/// Defines the behaviour of <see cref="ReversibleProperty{T}"/> when a new value is added and
/// the history buffer has reached its configured maximum size.
/// </summary>
public enum OverflowStrategy
{
  /// <summary>
  /// The oldest entry in the history is silently removed to make room for the new value.
  /// This is the default strategy.
  /// </summary>
  EvictOldest,

  /// <summary>
  /// The new value is silently discarded; the history and current value remain unchanged.
  /// </summary>
  EvictNewest,

  /// <summary>
  /// An <see cref="InvalidOperationException"/> is thrown when the history is full and a
  /// new value is set.
  /// </summary>
  Throw
}
