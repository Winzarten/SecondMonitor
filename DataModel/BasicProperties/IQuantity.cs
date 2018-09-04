namespace SecondMonitor.DataModel.BasicProperties
{
    public interface IQuantity
    {
        IQuantity ZeroQuantity { get; }

        bool IsZero { get; }

        double RawValue { get; }
    }
}