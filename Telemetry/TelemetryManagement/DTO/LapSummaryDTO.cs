namespace SecondMonitor.Telemetry.TelemetryManagement.DTO
{
    using System;
    using System.Xml.Serialization;
    [Serializable]
    public class LapSummaryDto
    {

        [XmlIgnore]
        public string Id => $"{SessionIdentifier}-L{LapNumber}";

        [XmlAttribute]
        public int LapNumber { get; set; }

        [XmlIgnore]
        public TimeSpan LapTime { get; set; }

        [XmlAttribute]
        public string SessionIdentifier { get; set; }

        [XmlAttribute]
        public double LapTimeSeconds
        {
            get => LapTime.TotalSeconds;
            set => LapTime = TimeSpan.FromSeconds(value);
        }

        public static bool operator ==(LapSummaryDto lap1, LapSummaryDto lap2)
        {
            if (lap1 is null && lap2 is null)
            {
                return true;
            }

            if (lap1 is null || lap2 is null)
            {
                return false;
            }

            return lap1.LapNumber == lap2.LapNumber && lap1.SessionIdentifier == lap2.SessionIdentifier;

        }

        public static bool operator !=(LapSummaryDto lap1, LapSummaryDto lap2)
        {
            return !(lap1 == lap2);
        }

        protected bool Equals(LapSummaryDto other)
        {
            return LapNumber == other.LapNumber && LapTime.Equals(other.LapTime) && string.Equals(SessionIdentifier, other.SessionIdentifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((LapSummaryDto)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = LapNumber;
                hashCode = (hashCode * 397) ^ LapTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (SessionIdentifier != null ? SessionIdentifier.GetHashCode() : 0);
                return hashCode;
            }
        }

    }
}