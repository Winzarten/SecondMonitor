﻿namespace SecondMonitor.Contracts.Collection
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class ObservableCollection<T> : IList<T>, INotifyCollectionChanged
    {
        private readonly List<T> _internalList;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableCollection()
        {
            _internalList = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _internalList.Add(item);
            OnItemAdded(item);
        }

        public void Clear()
        {
            _internalList.Clear();
            OnCollectionReset();
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            int index = _internalList.IndexOf(item);
            bool removeResult = _internalList.Remove(item);
            if (removeResult)
            {
                OnItemRemoved(item, index);
            }

            return removeResult;
        }

        public int Count => _internalList.Count;


        public bool IsReadOnly => false;


        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
            OnItemAdded(item);
        }

        public void RemoveAt(int index)
        {
            T item = _internalList[index];
            _internalList.RemoveAt(index);
            OnItemRemoved(item, index);
        }

        public T this[int index]
        {
            get => _internalList[index];
            set => _internalList[index] = value;
        }

        public void AddRange(IEnumerable<T> items)
        {
            List<T> itemsList = items.ToList();
            _internalList.AddRange(itemsList);
            OnItemsAdded(itemsList);
        }

        private void OnCollectionReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnItemAdded(T addedItem)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItem));
        }

        private void OnItemsAdded(IList addedItems)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItems));
        }

        private void OnItemRemoved(T removedItem, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removedItem, index));
        }
    }
}