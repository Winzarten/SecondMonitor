namespace SecondMonitor.DataModel.BasicProperties
{
    using System;

    [Serializable]
    public sealed class OptimalQuantity<T> where T : class, IQuantity, new()
    {

        public T ActualQuantity { get; set; } = (T)new T().ZeroQuantity;

        public T IdealQuantity { get; set; } = (T)new T().ZeroQuantity;

        public T IdealQuantityWindow { get; set; } = (T)new T().ZeroQuantity;

    }
}