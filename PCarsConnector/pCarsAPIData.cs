using System.Runtime.InteropServices;
using SecondMonitor.PCarsConnector.enums;

namespace SecondMonitor.PCarsConnector
{   
    
    public struct PCarsApiParticipantStruct
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool MIsActive;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = (int)EApiStructLengths.StringLengthMax)]
        public string MName;                                    // [ string ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MWorldPosition;                          // [ UNITS = World Space  X  Y  Z ]
        
        public float MCurrentLapDistance;                       // [ UNITS = Metres ]   [ RANGE = 0.0f->... ]    [ UNSET = 0.0f ]
        public uint MRacePosition;                              // [ RANGE = 1->... ]   [ UNSET = 0 ]
        public uint MLapsCompleted;                             // [ RANGE = 0->... ]   [ UNSET = 0 ]
        public uint MCurrentLap;                                // [ RANGE = 0->... ]   [ UNSET = 0 ]
        public uint MCurrentSector;                             // [ enum (Type#4) Current Sector ]
    }

    public struct PCarsApiStruct
    {
        //SMS supplied data structure
        // Version Number
        public uint MVersion;                           // [ RANGE = 0->... ]
        public uint MBuildVersion;                      // [ RANGE = 0->... ]   [ UNSET = 0 ]

        // Session type
        public uint MGameState;                         // [ enum (Type#1) Game state ]
        public uint MSessionState;                      // [ enum (Type#2) Session state ]
        public uint MRaceState;                         // [ enum (Type#3) Race State ]

        // Participant Info
        public int MViewedParticipantIndex;                      // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        public int MNumParticipants;                             // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)EApiStructLengths.NumParticipants)]
        public PCarsApiParticipantStruct[] MParticipantData;

        // Unfiltered Input
        public float MUnfilteredThrottle;                       // [ RANGE = 0.0f->1.0f ]
        public float MUnfilteredBrake;                          // [ RANGE = 0.0f->1.0f ]
        public float MUnfilteredSteering;                       // [ RANGE = -1.0f->1.0f ]
        public float MUnfilteredClutch;                         // [ RANGE = 0.0f->1.0f ]

        // Vehicle & Track information
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = (int)EApiStructLengths.StringLengthMax)]
        public string MCarName;                                 // [ string ]

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = (int)EApiStructLengths.StringLengthMax)]
        public string MCarClassName;                            // [ string ]

        public uint MLapsInEvent;                               // [ RANGE = 0->... ]   [ UNSET = 0 ]

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = (int)EApiStructLengths.StringLengthMax)]
        public string MTrackLocation;                           // [ string ]

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = (int)EApiStructLengths.StringLengthMax)]
        public string MTrackVariation;                          // [ string ]

        public float MTrackLength;                              // [ UNITS = Metres ]   [ RANGE = 0.0f->... ]    [ UNSET = 0.0f ]

        // Timing & Scoring
        public bool MLapInvalidated;                            // [ UNITS = boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        public float MSessionFastestLapTime;                              // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MLastLapTime;                              // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MCurrentTime;                              // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MSplitTimeAhead;                            // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MSplitTimeBehind;                           // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MSplitTime;                                // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MEventTimeRemaining;                       // [ UNITS = milli-seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MPersonalFastestLapTime;                    // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MWorldFastestLapTime;                       // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MCurrentSector1Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MCurrentSector2Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MCurrentSector3Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MSessionFastestSector1Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MSessionFastestSector2Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MSessionFastestSector3Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MPersonalFastestSector1Time;                // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MPersonalFastestSector2Time;                // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MPersonalFastestSector3Time;                // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MWorldFastestSector1Time;                   // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MWorldFastestSector2Time;                   // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float MWorldFastestSector3Time;                   // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]

        // Flags
        public uint MHighestFlagColour;                 // [ enum (Type#5) Flag Colour ]
        public uint MHighestFlagReason;                 // [ enum (Type#6) Flag Reason ]

        // Pit Info
        public uint MPitMode;                           // [ enum (Type#7) Pit Mode ]
        public uint MPitSchedule;                       // [ enum (Type#8) Pit Stop Schedule ]

        // Car State
        public uint MCarFlags;                          // [ enum (Type#6) Car Flags ]
        public float MOilTempCelsius;                           // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        public float MOilPressureKPa;                           // [ UNITS = Kilopascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MWaterTempCelsius;                         // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        public float MWaterPressureKPa;                         // [ UNITS = Kilopascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MFuelPressureKPa;                          // [ UNITS = Kilopascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MFuelLevel;                                // [ RANGE = 0.0f->1.0f ]
        public float MFuelCapacity;                             // [ UNITS = Liters ]   [ RANGE = 0.0f->1.0f ]   [ UNSET = 0.0f ]
        public float MSpeed;                                    // [ UNITS = Metres per-second ]   [ RANGE = 0.0f->... ]
        public float MRpm;                                      // [ UNITS = Revolutions per minute ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MMaxRpm;                                   // [ UNITS = Revolutions per minute ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float MBrake;                                    // [ RANGE = 0.0f->1.0f ]
        public float MThrottle;                                 // [ RANGE = 0.0f->1.0f ]
        public float MClutch;                                   // [ RANGE = 0.0f->1.0f ]
        public float MSteering;                                 // [ RANGE = -1.0f->1.0f ]
        public int MGear;                                       // [ RANGE = -1 (Reverse)  0 (Neutral)  1 (Gear 1)  2 (Gear 2)  etc... ]   [ UNSET = 0 (Neutral) ]
        public int MNumGears;                                   // [ RANGE = 0->... ]   [ UNSET = -1 ]
        public float MOdometerKm;                               // [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public bool MAntiLockActive;                            // [ UNITS = boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        public int MLastOpponentCollisionIndex;                 // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        public float MLastOpponentCollisionMagnitude;           // [ RANGE = 0.0f->... ]
        public bool MBoostActive;                               // [ UNITS = boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        public float MBoostAmount;                              // [ RANGE = 0.0f->100.0f ] 
 
        // Motion & Device Related
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MOrientation;                     // [ UNITS = Euler Angles ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MLocalVelocity;                   // [ UNITS = Metres per-second ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MWorldVelocity;                   // [ UNITS = Metres per-second ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MAngularVelocity;                 // [ UNITS = Radians per-second ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MLocalAcceleration;               // [ UNITS = Metres per-second ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MWorldAcceleration;               // [ UNITS = Metres per-second ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)EVector.VecMax)]
        public float[] MExtentsCentre;                   // [ UNITS = Local Space  X  Y  Z ]

        // Wheels / Tyres
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public uint[] MTyreFlags;               // [ enum (Type#7) Tyre Flags ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public uint[] MTerrain;                 // [ enum (Type#3) Terrain Materials ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreY;                          // [ UNITS = Local Space  Y ]
        
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreRps;                        // [ UNITS = Revolutions per second ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreSlipSpeed;                  // [ UNITS = Metres per-second ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreTemp;                       // [ UNITS = Celsius ]   [ UNSET = 0.0f ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreGrip;                       // [ RANGE = 0.0f->1.0f ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreHeightAboveGround;          // [ UNITS = Local Space  Y ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreLateralStiffness;           // [ UNITS = Lateral stiffness coefficient used in tyre deformation ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreWear;                       // [ RANGE = 0.0f->1.0f ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MBrakeDamage;                    // [ RANGE = 0.0f->1.0f ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MSuspensionDamage;               // [ RANGE = 0.0f->1.0f ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MBrakeTempCelsius;               // [ RANGE = 0.0f->1.0f ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreTreadTemp;                  // [ UNITS = Kelvin ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreLayerTemp;                  // [ UNITS = Kelvin ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreCarcassTemp;                // [ UNITS = Kelvin ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreRimTemp;                    // [ UNITS = Kelvin ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = (int)ETyres.TyreMax)]
        public float[] MTyreInternalAirTemp;            // [ UNITS = Kelvin ]

        // Car Damage
        public uint MCrashState;                        // [ enum (Type#4) Crash Damage State ]
        public float MAeroDamage;                               // [ RANGE = 0.0f->1.0f ]
        public float MEngineDamage;                             // [ RANGE = 0.0f->1.0f ]

        // Weather
        public float MAmbientTemperature;                       // [ UNITS = Celsius ]   [ UNSET = 25.0f ]
        public float MTrackTemperature;                         // [ UNITS = Celsius ]   [ UNSET = 30.0f ]
        public float MRainDensity;                              // [ UNITS = How much rain will fall ]   [ RANGE = 0.0f->1.0f ]
        public float MWindSpeed;                                // [ RANGE = 0.0f->100.0f ]   [ UNSET = 2.0f ]
        public float MWindDirectionX;                           // [ UNITS = Normalised Vector X ]
        public float MWindDirectionY;                           // [ UNITS = Normalised Vector Y ]
        public float MCloudBrightness;                          // [ RANGE = 0.0f->... ]
    }
}