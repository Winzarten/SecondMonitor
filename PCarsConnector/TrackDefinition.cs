using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.PCarsConnector
{
    public class TrackDetails
    {
        private static TrackDetails lastDetails;
        private const int PitPointDetectionDistance = 2;

        private static List<TrackDetails> pcCarsTracks = new List<TrackDetails>()
        {
            new TrackDetails("Autodromo Nazionale Monza:Grand Prix", new float[] {22.20312f, -437.1672f},
                new float[] {63.60915f, -1.117797f}),
            new TrackDetails("Autodromo Nazionale Monza:Short", new float[] {22.20312f, -437.1672f},
                new float[] {63.60915f, -1.117797f}),
            new TrackDetails("Azure Circuit:Grand Prix", new float[] {-203.8109f, 613.3162f},
                new float[] {105.2057f, 525.9147f}),
            new TrackDetails("Bathurst:", new float[] {80.84997f, 7.21405f},
                new float[] {-368.7227f, 12.93535f}),
            new TrackDetails("Brands Hatch:Indy", new float[] {-329.1295f, 165.8752f},
                new float[] {-36.68332f, 355.611f}),
            new TrackDetails("Brands Hatch:Grand Prix", new float[] {-329.1295f, 165.8752f},
                new float[] {-36.68332f, 355.611f}),
            new TrackDetails("Brno:", new float[] {-194.1228f, -11.41852f},
                new float[] {139.6739f, 0.06169825f}),
            new TrackDetails("Cadwell:Woodland", new float[] {45.92422f, 72.04858f},
                new float[] {-10.31487f, -40.43255f}),
            new TrackDetails("Cadwell:Grand Prix", new float[] {45.92422f, 72.04858f},
                new float[] {-10.31487f, -40.43255f}),
            new TrackDetails("Circuit de Barcelona-Catalunya:Grand Prix",
                new float[] {622.7108f, -137.3975f}, new float[] {26.52858f, -167.9301f}),
            new TrackDetails("Circuit de Barcelona-Catalunya:National",
                new float[] {622.7108f, -137.3975f}, new float[] {26.52858f, -167.9301f}),
            new TrackDetails("Circuit de Spa-Francorchamps:", new float[] {-685.1871f, 1238.607f},
                new float[] {-952.3125f, 1656.81f}),
            new TrackDetails("Le Mans:Circuit des 24 Heures du Mans",
                new float[] {-737.9395f, 1107.367f}, new float[] {-721.3452f, 1582.873f}),
            new TrackDetails("Dubai Autodrome:Club", new float[] {971.8023f, 199.1564f},
                new float[] {452.5084f, 126.7626f}),
            new TrackDetails("Dubai Autodrome:Grand Prix", new float[] {971.8023f, 199.1564f},
                new float[] {452.5084f, 126.7626f}),
            new TrackDetails("Dubai Autodrome:International", new float[] {971.8023f, 199.1564f},
                new float[] {452.5084f, 126.7626f}),
            new TrackDetails("Dubai Autodrome:National", new float[] {971.8023f, 199.1564f},
                new float[] {452.5084f, 126.7626f}),
            new TrackDetails("Donington Park:Grand Prix", new float[] {200.8843f, 144.8465f},
                new float[] {486.4654f, 119.9713f}),
            new TrackDetails("Donington Park:National", new float[] {200.8843f, 144.8465f},
                new float[] {486.4654f, 119.9713f}),
            new TrackDetails("Hockenheim:Grand Prix", new float[] {-483.1076f, -428.47f},
                new float[] {-704.3397f, 11.15407f}),
            new TrackDetails("Hockenheim:Short", new float[] {-483.1076f, -428.47f},
                new float[] {-704.3397f, 11.15407f}),
            new TrackDetails("Hockenheim:National", new float[] {-483.1076f, -428.47f},
                new float[] {-704.3397f, 11.15407f}),
            new TrackDetails("Imola:Grand Prix", new float[] {311.259f, 420.3269f},
                new float[] {-272.6198f, 418.3795f}),
            new TrackDetails("Le Mans:Le Circuit Bugatti", new float[] {-737.9395f, 1107.367f},
                new float[] {-721.3452f, 1582.873f}),
            new TrackDetails("Mazda Raceway:Laguna Seca", new float[] {-70.22401f, 432.3777f},
                new float[] {-279.2681f, 228.165f}),
            new TrackDetails("Nordschleife:Full", new float[] {599.293f, 606.7135f},
                new float[] {391.6694f, 694.4844f}),
            new TrackDetails("Nürburgring:Grand Prix", new float[] {443.6332f, 527.8024f},
                new float[] {66.84711f, 96.7378f}),
            new TrackDetails("Nürburgring:Sprint", new float[] {443.6332f, 527.8024f},
                new float[] {66.84711f, 96.7378f}),
            new TrackDetails("Nürburgring:Sprint Short", new float[] {443.6332f, 527.8024f},
                new float[] {66.84711f, 96.7378f}),
            new TrackDetails("Oschersleben:Grand Prix", new float[] {-350.7033f, 31.39084f},
                new float[] {239.3137f, 91.73861f}),
            new TrackDetails("Oschersleben:National", new float[] {-350.7033f, 31.39084f},
                new float[] {239.3137f, 91.73861f}),
            new TrackDetails("Oulton Park:Fosters", new float[] {46.9972f, 80.40176f},
                new float[] {114.8132f, -165.5994f}),
            new TrackDetails("Oulton Park:International", new float[] {46.9972f, 80.40176f},
                new float[] {114.8132f, -165.5994f}),
            new TrackDetails("Oulton Park:Island", new float[] {46.9972f, 80.40176f},
                new float[] {114.8132f, -165.5994f}),
            new TrackDetails("Road America:", new float[] {430.8689f, 245.7329f},
                new float[] {451.5659f, -330.7411f}),
            new TrackDetails("Sakitto:Grand Prix", new float[] {576.6671f, -142.1608f},
                new float[] {607.291f, -646.9218f}),
            new TrackDetails("Sakitto:International", new float[] {-265.1671f, 472.4344f},
                new float[] {-154.9505f, 278.1627f}),
            new TrackDetails("Sakitto:National", new float[] {-265.1671f, 472.4344f},
                new float[] {-154.9505f, 278.1627f}),
            new TrackDetails("Sakitto:Sprint", new float[] {576.6671f - 142.1608f},
                new float[] {607.291f, -646.9218f}),
            new TrackDetails("Silverstone:Grand Prix", new float[] {-504.739f, -1274.686f},
                new float[] {-273.1427f, -861.1436f}),
            new TrackDetails("Silverstone:International", new float[] {-504.739f, -1274.686f},
                new float[] {-273.1427f, -861.1436f}),
            new TrackDetails("Silverstone:National", new float[] {-323.1119f, -115.6939f},
                new float[] {157.4515f, 0.4208831f}),
            new TrackDetails("Silverstone:Stowe", new float[] {-75.90499f, -1396.183f},
                new float[] {-0.5095776f, -1096.397f}),
            new TrackDetails("Snetterton:200 Circuit", new float[] {228.4838f, -25.23679f},
                new float[] {-44.5122f, -55.82156f}),
            new TrackDetails("Snetterton:300 Circuit", new float[] {228.4838f, -25.23679f},
                new float[] {-44.5122f, -55.82156f}),
            new TrackDetails("Sonoma Raceway:Grand Prix", new float[] {-592.7792f, 87.43731f},
                new float[] {-152.6224f, -30.71386f}),
            new TrackDetails("Sonoma Raceway:National", new float[] {-592.7792f, 87.43731f},
                new float[] {-152.6224f, -30.71386f}),
            new TrackDetails("Sonoma Raceway:Short", new float[] {-592.7792f, 87.43731f},
                new float[] {-152.6224f, -30.71386f}),
            new TrackDetails("Watkins Glen International:Grand Prix",
                new float[] {589.6273f, -928.2814f}, new float[] {542.0042f, -1410.464f}),
            new TrackDetails("Watkins Glen International:Short Circuit",
                new float[] {589.6273f, -928.2814f}, new float[] {542.0042f, -1410.464f}),
            new TrackDetails("Willow Springs:Short Circuit", new float[] {-386.1919f, 818.131f},
                new float[] {-317.1366f, 641.947f}),
            new TrackDetails("Willow Springs:International Raceway",
                new float[] {319.4275f, -21.51243f}, new float[] {-44.84023f - 23.41344f}),
            new TrackDetails("Zhuhai:International Circuit", new float[] {-193.7068f, 123.679f},
                new float[] {64.56277f, -71.51254f}),
            new TrackDetails("Zolder:Grand Prix", new float[] {138.3811f, 132.7747f},
                new float[] {682.2009f, 179.8147f}),

            new TrackDetails("Bannochbrae:Road Circuit", new float[] {-175f, 16f},
                new float[] {131f, 14.5f}),
            new TrackDetails("Rouen Les Essarts:", new float[] {117.25f, 25.5f},
                new float[] {-84.75f, -13.5f}),
            new TrackDetails("Rouen Les Essarts:Short", new float[] {117.25f, 25.5f},
                new float[] {-84.75f, -13.5f}),
            // this is the classic version. Which has *exactly* the same name as the non-classic version. Not cool.
            new TrackDetails("Silverstone:Grand Prix", new float[] {-347f, -165f},
                new float[] {152.75f, -1.25f}),
            new TrackDetails("Hockenheim:Classic", new float[] {-533f, -318.25f},
                new float[] {-705.5f, -2f})
        };

        public TrackDetails(string name, float[] pitEntryLocation, float[] pitExitLocation)
        {
            this.Name = name;
            this.PitEntryLocation = pitEntryLocation;
            this.PitExitLocation = pitExitLocation;
        }

        public bool AtPitEntry(DriverInfo driver)
        {
            double distance = Math.Sqrt(Math.Pow(driver.WorldPostion.X - PitEntryLocation[0], 2) +
                                        Math.Pow(driver.WorldPostion.Z - PitEntryLocation[1], 2));
            driver.DriverDebugInfo.DistanceToPits = distance;
            return distance < PitPointDetectionDistance;
        }

        public bool AtPitExit(DriverInfo driver)
        {
            double distance = Math.Sqrt(Math.Pow(driver.WorldPostion.X - PitExitLocation[0], 2) +
                                        Math.Pow(driver.WorldPostion.Z - PitExitLocation[1], 2));
            driver.DriverDebugInfo.DistanceToPits = distance;
            return distance < PitPointDetectionDistance;
        }



        public string Name { get; private set; }

        public float[] PitEntryLocation { get; private set; }

        public float[] PitExitLocation { get; private set; }

        public static TrackDetails GetTrackDetails(string trackName, string trackLayout)
        {
            var trackID =String.IsNullOrEmpty(trackLayout) ? trackName +":" : trackName +":"+ trackLayout;
            if (trackID == lastDetails?.Name)
                return lastDetails;
            TrackDetails trackDetails = pcCarsTracks.Find(p => p.Name == trackID);
            lastDetails = trackDetails;
            return trackDetails;
        }
    }
}
