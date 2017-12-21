using System;
using System.Collections.Generic;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.PCarsConnector
{
    public class TrackDetails
    {
        private static TrackDetails _lastDetails;
        private const int PitPointDetectionDistance = 2;

        private static List<TrackDetails> _pcCarsTracks = new List<TrackDetails>()
                                                              {
                                                                  new TrackDetails(
                                                                      "Autodromo Nazionale Monza:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              22.20312f,
                                                                              -437.1672f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              63.60915f,
                                                                              -1.117797f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Autodromo Nazionale Monza:Short",
                                                                      new[]
                                                                          {
                                                                              22.20312f,
                                                                              -437.1672f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              63.60915f,
                                                                              -1.117797f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Azure Circuit:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              -203.8109f,
                                                                              613.3162f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              105.2057f,
                                                                              525.9147f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Bathurst:",
                                                                      new[]
                                                                          {
                                                                              80.84997f, 7.21405f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -368.7227f,
                                                                              12.93535f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Brands Hatch:Indy",
                                                                      new[]
                                                                          {
                                                                              -329.1295f,
                                                                              165.8752f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -36.68332f,
                                                                              355.611f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Brands Hatch:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              -329.1295f,
                                                                              165.8752f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -36.68332f,
                                                                              355.611f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Brno:",
                                                                      new[]
                                                                          {
                                                                              -194.1228f,
                                                                              -11.41852f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              139.6739f,
                                                                              0.06169825f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Cadwell:Woodland",
                                                                      new[]
                                                                          {
                                                                              45.92422f,
                                                                              72.04858f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -10.31487f,
                                                                              -40.43255f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Cadwell:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              45.92422f,
                                                                              72.04858f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -10.31487f,
                                                                              -40.43255f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Circuit de Barcelona-Catalunya:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              622.7108f,
                                                                              -137.3975f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              26.52858f,
                                                                              -167.9301f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Circuit de Barcelona-Catalunya:National",
                                                                      new[]
                                                                          {
                                                                              622.7108f,
                                                                              -137.3975f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              26.52858f,
                                                                              -167.9301f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Circuit de Spa-Francorchamps:",
                                                                      new[]
                                                                          {
                                                                              -685.1871f,
                                                                              1238.607f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -952.3125f,
                                                                              1656.81f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Le Mans:Circuit des 24 Heures du Mans",
                                                                      new[]
                                                                          {
                                                                              -737.9395f,
                                                                              1107.367f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -721.3452f,
                                                                              1582.873f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Dubai Autodrome:Club",
                                                                      new[]
                                                                          {
                                                                              971.8023f,
                                                                              199.1564f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              452.5084f,
                                                                              126.7626f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Dubai Autodrome:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              971.8023f,
                                                                              199.1564f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              452.5084f,
                                                                              126.7626f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Dubai Autodrome:International",
                                                                      new[]
                                                                          {
                                                                              971.8023f,
                                                                              199.1564f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              452.5084f,
                                                                              126.7626f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Dubai Autodrome:National",
                                                                      new[]
                                                                          {
                                                                              971.8023f,
                                                                              199.1564f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              452.5084f,
                                                                              126.7626f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Donington Park:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              200.8843f,
                                                                              144.8465f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              486.4654f,
                                                                              119.9713f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Donington Park:National",
                                                                      new[]
                                                                          {
                                                                              200.8843f,
                                                                              144.8465f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              486.4654f,
                                                                              119.9713f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Hockenheim:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              -483.1076f,
                                                                              -428.47f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -704.3397f,
                                                                              11.15407f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Hockenheim:Short",
                                                                      new[]
                                                                          {
                                                                              -483.1076f,
                                                                              -428.47f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -704.3397f,
                                                                              11.15407f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Hockenheim:National",
                                                                      new[]
                                                                          {
                                                                              -483.1076f,
                                                                              -428.47f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -704.3397f,
                                                                              11.15407f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Imola:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              311.259f, 420.3269f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -272.6198f,
                                                                              418.3795f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Le Mans:Le Circuit Bugatti",
                                                                      new[]
                                                                          {
                                                                              -737.9395f,
                                                                              1107.367f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -721.3452f,
                                                                              1582.873f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Mazda Raceway:Laguna Seca",
                                                                      new[]
                                                                          {
                                                                              -70.22401f,
                                                                              432.3777f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -279.2681f,
                                                                              228.165f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Nordschleife:Full",
                                                                      new[]
                                                                          {
                                                                              599.293f, 606.7135f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              391.6694f,
                                                                              694.4844f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Nürburgring:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              443.6332f,
                                                                              527.8024f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              66.84711f, 96.7378f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Nürburgring:Sprint",
                                                                      new[]
                                                                          {
                                                                              443.6332f,
                                                                              527.8024f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              66.84711f, 96.7378f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Nürburgring:Sprint Short",
                                                                      new[]
                                                                          {
                                                                              443.6332f,
                                                                              527.8024f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              66.84711f, 96.7378f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Oschersleben:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              -350.7033f,
                                                                              31.39084f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              239.3137f,
                                                                              91.73861f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Oschersleben:National",
                                                                      new[]
                                                                          {
                                                                              -350.7033f,
                                                                              31.39084f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              239.3137f,
                                                                              91.73861f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Oulton Park:Fosters",
                                                                      new[]
                                                                          {
                                                                              46.9972f, 80.40176f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              114.8132f,
                                                                              -165.5994f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Oulton Park:International",
                                                                      new[]
                                                                          {
                                                                              46.9972f, 80.40176f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              114.8132f,
                                                                              -165.5994f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Oulton Park:Island",
                                                                      new[]
                                                                          {
                                                                              46.9972f, 80.40176f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              114.8132f,
                                                                              -165.5994f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Road America:",
                                                                      new[]
                                                                          {
                                                                              430.8689f,
                                                                              245.7329f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              451.5659f,
                                                                              -330.7411f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Sakitto:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              576.6671f,
                                                                              -142.1608f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              607.291f,
                                                                              -646.9218f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Sakitto:International",
                                                                      new[]
                                                                          {
                                                                              -265.1671f,
                                                                              472.4344f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -154.9505f,
                                                                              278.1627f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Sakitto:National",
                                                                      new[]
                                                                          {
                                                                              -265.1671f,
                                                                              472.4344f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -154.9505f,
                                                                              278.1627f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Sakitto:Sprint",
                                                                      new[]
                                                                          {
                                                                              576.6671f,
                                                                              -142.1608f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              607.291f,
                                                                              -646.9218f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Silverstone:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              -504.739f,
                                                                              -1274.686f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -273.1427f,
                                                                              -861.1436f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Silverstone:International",
                                                                      new[]
                                                                          {
                                                                              -504.739f,
                                                                              -1274.686f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -273.1427f,
                                                                              -861.1436f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Silverstone:National",
                                                                      new[]
                                                                          {
                                                                              -323.1119f,
                                                                              -115.6939f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              157.4515f,
                                                                              0.4208831f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Silverstone:Stowe",
                                                                      new[]
                                                                          {
                                                                              -75.90499f,
                                                                              -1396.183f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -0.5095776f,
                                                                              -1096.397f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Snetterton:200 Circuit",
                                                                      new[]
                                                                          {
                                                                              228.4838f,
                                                                              -25.23679f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -44.5122f,
                                                                              -55.82156f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Snetterton:300 Circuit",
                                                                      new[]
                                                                          {
                                                                              228.4838f,
                                                                              -25.23679f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -44.5122f,
                                                                              -55.82156f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Sonoma Raceway:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              -592.7792f,
                                                                              87.43731f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -152.6224f,
                                                                              -30.71386f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Sonoma Raceway:National",
                                                                      new[]
                                                                          {
                                                                              -592.7792f,
                                                                              87.43731f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -152.6224f,
                                                                              -30.71386f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Sonoma Raceway:Short",
                                                                      new[]
                                                                          {
                                                                              -592.7792f,
                                                                              87.43731f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -152.6224f,
                                                                              -30.71386f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Watkins Glen International:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              589.6273f,
                                                                              -928.2814f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              542.0042f,
                                                                              -1410.464f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Watkins Glen International:Short Circuit",
                                                                      new[]
                                                                          {
                                                                              589.6273f,
                                                                              -928.2814f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              542.0042f,
                                                                              -1410.464f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Willow Springs:Short Circuit",
                                                                      new[]
                                                                          {
                                                                              -386.1919f,
                                                                              818.131f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -317.1366f,
                                                                              641.947f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Willow Springs:International Raceway",
                                                                      new[]
                                                                          {
                                                                              319.4275f,
                                                                              -21.51243f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              -44.84023f
                                                                              - 23.41344f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Zhuhai:International Circuit",
                                                                      new[]
                                                                          {
                                                                              -193.7068f,
                                                                              123.679f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              64.56277f,
                                                                              -71.51254f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Zolder:Grand Prix",
                                                                      new[]
                                                                          {
                                                                              138.3811f,
                                                                              132.7747f
                                                                          },
                                                                      new[]
                                                                          {
                                                                              682.2009f,
                                                                              179.8147f
                                                                          }),
                                                                  new TrackDetails(
                                                                      "Bannochbrae:Road Circuit",
                                                                      new[] { -175f, 16f },
                                                                      new[] { 131f, 14.5f }),
                                                                  new TrackDetails(
                                                                      "Rouen Les Essarts:",
                                                                      new[] { 117.25f, 25.5f },
                                                                      new[] { -84.75f, -13.5f }),
                                                                  new TrackDetails(
                                                                      "Rouen Les Essarts:Short",
                                                                      new[] { 117.25f, 25.5f },
                                                                      new[] { -84.75f, -13.5f }),

                                                                  // this is the classic version. Which has *exactly* the same name as the non-classic version. Not cool.
                                                                  new TrackDetails(
                                                                      "Silverstone:Grand Prix",
                                                                      new[] { -347f, -165f },
                                                                      new[] { 152.75f, -1.25f }),
                                                                  new TrackDetails(
                                                                      "Hockenheim:Classic",
                                                                      new[] { -533f, -318.25f },
                                                                      new[] { -705.5f, -2f })
                                                              };

        public TrackDetails(string name, float[] pitEntryLocation, float[] pitExitLocation)
        {
            Name = name;
            PitEntryLocation = pitEntryLocation;
            PitExitLocation = pitExitLocation;
        }

        public bool AtPitEntry(DriverInfo driver)
        {
            double distance = Math.Sqrt(Math.Pow(driver.WorldPosition.X - PitEntryLocation[0], 2) +
                                        Math.Pow(driver.WorldPosition.Z - PitEntryLocation[1], 2));
            driver.DriverDebugInfo.DistanceToPits = distance;
            return distance < PitPointDetectionDistance;
        }

        public bool AtPitExit(DriverInfo driver)
        {
            double distance = Math.Sqrt(Math.Pow(driver.WorldPosition.X - PitExitLocation[0], 2) +
                                        Math.Pow(driver.WorldPosition.Z - PitExitLocation[1], 2));
            driver.DriverDebugInfo.DistanceToPits = distance;
            return distance < PitPointDetectionDistance;
        }



        public string Name { get; private set; }

        public float[] PitEntryLocation { get; private set; }

        public float[] PitExitLocation { get; private set; }

        public static TrackDetails GetTrackDetails(string trackName, string trackLayout)
        {
            var trackId =string.IsNullOrEmpty(trackLayout) ? trackName +":" : trackName +":"+ trackLayout;
            if (trackId == _lastDetails?.Name)
                return _lastDetails;
            TrackDetails trackDetails = _pcCarsTracks.Find(p => p.Name == trackId);
            _lastDetails = trackDetails;
            return trackDetails;
        }
    }
}
