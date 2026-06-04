// Copyright: 2023 Robert Peter Meyer
// License: MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using BB84.Notifications.Components;
using BB84.Notifications.Interfaces;

namespace BB84.Notifications;

/// <summary>
/// Represents a collection that provides notifications when its contents or properties change.
/// </summary>
[SuppressMessage("Naming", "CA1711", Justification = "Identifier is correct here")]
public abstract class NotifiableCollection : NotifiableObject, INotifiableCollection
{
  /// <inheritdoc/>
  public event CollectionChangedEventHandler? CollectionChanged;

  /// <inheritdoc/>
  public event CollectionChangingEventHandler? CollectionChanging;

  /// <summary>
  /// Raises the <see cref="CollectionChanged"/> event to notify subscribers of changes to the collection.
  /// </summary>
  /// <remarks>This method invokes the <see cref="CollectionChanged"/> event with the specified
  /// <paramref name="action"/>. Ensure that any subscribers to the event handle the change appropriately.
  /// </remarks>
  /// <param name="action">
  /// The type of change that occurred in the collection.
  /// This value indicates whether items were added, removed, or the collection was refreshed.
  /// </param>
  protected void RaiseCollectionChanged(CollectionChangeAction action)
    => CollectionChanged?.Invoke(this, new(action));

  /// <summary>
  /// Raises the <see cref="CollectionChanged"/> event to notify subscribers of changes to the collection. 
  /// </summary>
  /// <remarks>
  /// This method invokes the <see cref="CollectionChanged"/> event with the specified action and item.
  /// Ensure that any subscribers to the event handle the change appropriately.
  /// </remarks>
  /// <typeparam name="T">The type of the item affected by the collection change.</typeparam>
  /// <param name="action">
  /// The action that describes the type of change to the collection, such as Add or Remove.
  /// </param>
  /// <param name="item">
  /// The item that is affected by the collection change.
  /// </param>
  protected void RaiseCollectionChanged<T>(CollectionChangeAction action, T item)
    => CollectionChanged?.Invoke(this, new CollectionChangedEventArgs<T>(action, item));

  /// <summary>
  /// Raises the <see cref="CollectionChanging"/> event to notify subscribers that the collection
  /// is about to change.
  /// </summary>
  /// <param name="action">
  /// The type of change that is occurring in the collection.
  /// This value indicates whether items are being added, removed or the entire collection is being refreshed.
  /// </param>
  protected void RaiseCollectionChanging(CollectionChangeAction action)
    => CollectionChanging?.Invoke(this, new(action));

  /// <summary>
  /// Raises the <see cref="CollectionChanging"/> event to notify subscribers that the collection
  /// is about to change.
  /// </summary>
  /// <remarks>
  /// This method should be called before a change is made to the collection to allow subscribers
  /// to respond to the impending change.
  /// </remarks>
  /// <typeparam name="T">The type of the item involved in the collection change.</typeparam>
  /// <param name="action">
  /// The action indicating the type of change being performed on the collection.
  /// </param>
  /// <param name="item">
  /// The item that is affected by the upcoming collection change.
  /// </param>
  protected void RaiseCollectionChanging<T>(CollectionChangeAction action, T item)
    => CollectionChanging?.Invoke(this, new CollectionChangingEventArgs<T>(action, item));
}

/// <summary>
/// Represents a generic collection that provides notifications when its contents or properties change.
/// </summary>
/// <remarks>
/// The <see cref="NotifiableCollection{T}"/> class provides a ready-to-use observable list that
/// raises <see cref="NotifiableCollection.CollectionChanging"/> and
/// <see cref="NotifiableCollection.CollectionChanged"/> events for each mutating operation.
/// </remarks>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class NotifiableCollection<T> : NotifiableCollection, INotifiableCollection<T>
{
  private readonly List<T> _items;

  /// <summary>
  /// Initializes a new empty instance of the <see cref="NotifiableCollection{T}"/> class.
  /// </summary>
  public NotifiableCollection()
    => _items = [];

  /// <summary>
  /// Initializes a new instance of the <see cref="NotifiableCollection{T}"/> class that contains
  /// elements copied from the specified collection.
  /// </summary>
  /// <param name="collection">The collection whose elements are copied to the new list.</param>
  public NotifiableCollection(IEnumerable<T> collection)
    => _items = [.. collection];

  /// <inheritdoc/>
  public T this[int index]
  {
    get => _items[index];
    set
    {
      T old = _items[index];
      if (EqualityComparer<T>.Default.Equals(old, value))
        return;
      RaiseCollectionChanging(CollectionChangeAction.Refresh, old);
      _items[index] = value;
      RaiseCollectionChanged(CollectionChangeAction.Refresh, value);
    }
  }

  /// <inheritdoc/>
  public int Count => _items.Count;

  /// <inheritdoc/>
  public bool IsReadOnly => false;

  /// <inheritdoc/>
  public void Add(T item)
  {
    RaiseCollectionChanging(CollectionChangeAction.Add, item);
    _items.Add(item);
    RaiseCollectionChanged(CollectionChangeAction.Add, item);
  }

  /// <inheritdoc/>
  public void Clear()
  {
    RaiseCollectionChanging(CollectionChangeAction.Refresh);
    _items.Clear();
    RaiseCollectionChanged(CollectionChangeAction.Refresh);
  }

  /// <inheritdoc/>
  public bool Contains(T item)
    => _items.Contains(item);

  /// <inheritdoc/>
  public void CopyTo(T[] array, int arrayIndex)
    => _items.CopyTo(array, arrayIndex);

  /// <inheritdoc/>
  public IEnumerator<T> GetEnumerator()
    => _items.GetEnumerator();

  /// <inheritdoc/>
  public int IndexOf(T item)
    => _items.IndexOf(item);

  /// <inheritdoc/>
  public void Insert(int index, T item)
  {
    RaiseCollectionChanging(CollectionChangeAction.Add, item);
    _items.Insert(index, item);
    RaiseCollectionChanged(CollectionChangeAction.Add, item);
  }

  /// <inheritdoc/>
  public bool Remove(T item)
  {
    if (!_items.Contains(item))
      return false;
    RaiseCollectionChanging(CollectionChangeAction.Remove, item);
    _ = _items.Remove(item);
    RaiseCollectionChanged(CollectionChangeAction.Remove, item);
    return true;
  }

  /// <inheritdoc/>
  public void RemoveAt(int index)
  {
    T item = _items[index];
    RaiseCollectionChanging(CollectionChangeAction.Remove, item);
    _items.RemoveAt(index);
    RaiseCollectionChanged(CollectionChangeAction.Remove, item);
  }

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator()
    => _items.GetEnumerator();
}
