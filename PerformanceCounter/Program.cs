using System;
using System.Diagnostics;
using SecondMonitor.DataModel.Snapshot;

namespace PerformanceCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataSets = GetDataSets(10000);
            IterateThrough(dataSets);
        }

        private static SimulatorDataSet[] GetDataSets(int cont)
        {
            SimulatorDataSet[] dataSets = new SimulatorDataSet[cont];
            for (int i = 0; i < dataSets.Length; i++)
            {
                dataSets[i] = new SimulatorDataSet("Foo");
            }
            
            return dataSets;
        }

        private static void IterateThrough(SimulatorDataSet[] dataSets)
        {
            int sum = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var dataSet in dataSets)
            {
                foreach (var driver in dataSet.DriversInfo)
                {
                    sum += driver.Position;
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Count: {sum}");
            Console.WriteLine($"Ticks :{stopwatch.ElapsedTicks}");
            Console.WriteLine($"MS :{stopwatch.ElapsedMilliseconds}");
            Console.ReadLine();
        }
    }
}
