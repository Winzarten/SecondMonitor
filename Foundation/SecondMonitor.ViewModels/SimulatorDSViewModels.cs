namespace SecondMonitor.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using DataModel.Snapshot;

    public class SimulatorDSViewModels : IList<ISimulatorDataSetViewModel>, ISimulatorDataSetViewModel
    {
        private readonly List<ISimulatorDataSetViewModel> _list;

        public SimulatorDSViewModels()
        {
            _list = new List<ISimulatorDataSetViewModel>();
        }

        public IEnumerator<ISimulatorDataSetViewModel> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(ISimulatorDataSetViewModel item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(ISimulatorDataSetViewModel item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(ISimulatorDataSetViewModel[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISimulatorDataSetViewModel item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public int IndexOf(ISimulatorDataSetViewModel item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, ISimulatorDataSetViewModel item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public ISimulatorDataSetViewModel this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public T GetFirst<T>() where T : class, ISimulatorDataSetViewModel
        {
            return (T)_list.FirstOrDefault(x => x is T);
        }

        public IEnumerable<T> GetAll<T>() where T : class, ISimulatorDataSetViewModel
        {
            return _list.OfType<T>();
        }

        public IEnumerable<T> GetAll<T>(Func<T, bool> condition) where T : class, ISimulatorDataSetViewModel
        {
            return _list.OfType<T>().Where(condition);
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            _list.ForEach(x => x.ApplyDateSet(dataSet));
        }

        public void Reset()
        {
            _list.ForEach(x => x.Reset());
        }
    }
}