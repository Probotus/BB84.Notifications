﻿namespace BB84.Notifications.Enumerators;

/// <summary>
/// The collection change action enumerator.
/// </summary>
public enum CollectionChangeAction
{
  /// <summary>
  /// An item was added to the collection.
  /// </summary>
  Add,
  /// <summary>
  /// The content of the collection was cleared.
  /// </summary>
  Clear,
  /// <summary>
  /// An item was moved within the collection.
  /// </summary>
  Move,
  /// <summary>
  /// An item was removed from the collection.
  /// </summary>
  Remove,
  /// <summary>
  /// An item was replaced in the collection.
  /// </summary>
  Replace
}
