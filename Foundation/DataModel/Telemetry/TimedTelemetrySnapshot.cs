using System;
using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.DataModel.Telemetry
{
    using System.Xml.Serialization;

    public class TimedTelemetrySnapshot : TelemetrySnapshot
    {
        public TimedTelemetrySnapshot()
        {

        }

        public TimedTelemetrySnapshot(TimeSpan lapTime, DriverInfo playerInfo, WeatherInfo weatherInfo) : base(playerInfo, weatherInfo)
        {
            LapTime = lapTime;
        }

        [XmlIgnore]
        public TimeSpan LapTime { get; set; }

        [XmlAttribute]
        public double LapTimeSeconds
        {
            get => LapTime.TotalSeconds;
            set => LapTime = TimeSpan.FromSeconds(value);
        }
    }
}