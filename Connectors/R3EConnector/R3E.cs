namespace SecondMonitor.R3EConnector
{
    using System;
    using System.Runtime.InteropServices;

    internal class Constant
    {
        public const string SharedMemoryName = "$R3E";

        internal enum VersionMajor
        {
            // Major version number to test against
            R3EVersionMajor = 2
        };

        internal enum VersionMinor
        {
            // Minor version number to test against
            R3EVersionMinor = 6
        };

        internal enum Session
        {
            Unavailable = -1,
            Practice = 0,
            Qualify = 1,
            Race = 2,
            Warmup = 3,
        };

        internal enum SessionPhase
        {
            Unavailable = -1,

            // Currently in garage
            Garage = 1,

            // Gridwalk or track walkthrough
            Gridwalk = 2,

            // Formation lap, rolling start etc.
            Formation = 3,

            // Countdown to race is ongoing
            Countdown = 4,

            // Race is ongoing
            Green = 5,

            // End of session
            Checkered = 6,
        };

        internal enum Control
        {
            Unavailable = -1,

            // Controlled by the actual player
            Player = 0,

            // Controlled by AI
            Ai = 1,

            // Controlled by a network entity of some sort
            Remote = 2,

            // Controlled by a replay or ghost
            Replay = 3,
        };

        internal enum PitWindow
        {
            Unavailable = -1,

            // Pit stops are not enabled for this session
            Disabled = 0,

            // Pit stops are enabled, but you're not allowed to perform one right now
            Closed = 1,

            // Allowed to perform a pit stop now
            Open = 2,

            // Currently performing the pit stop changes (changing driver, etc.)
            Stopped = 3,

            // After the current mandatory pitstop have been completed
            Completed = 4,
        };

        internal enum PitStopStatus
        {
            // No mandatory pitstops
            Unavailable = -1,

            // Mandatory pitstop not served yet
            Unserved = 0,

            // Mandatory pitstop served
            Served = 1,
        };

        internal enum FinishStatus
        {
            // N/A
            Unavailable = -1,

            // Still on track, not finished
            None = 0,

            // Finished session normally
            Finished = 1,

            // Did not finish
            Dnf = 2,

            // Did not qualify
            Dnq = 3,

            // Did not start
            Dns = 4,

            // Disqualified
            Dq = 5,
        };

        internal enum SessionLengthFormat
        {
            // N/A
            Unavailable = -1,

            TimeBased = 0,

            LapBased = 1,

            // Time and lap based session means there will be an extra lap after the time has run out
            TimeAndLapBased = 2
        };

        internal enum PitMenuSelection
        {
            // Pit menu unavailable
            Unavailable = -1,

            // Pit menu preset
            Preset = 0,

            // Pit menu actions
            Penalty = 1,
            Driverchange = 2,
            Fuel = 3,
            Fronttires = 4,
            Reartires = 5,
            Frontwing = 6,
            Rearwing = 7,
            Suspension = 8,

            // Pit menu buttons
            ButtonTop = 9,
            ButtonBottom = 10,

            // Pit menu nothing selected
            Max = 11
        };

        internal enum TireType
        {
            Unavailable = -1,
            Option = 0,
            Prime = 1,
        };

        internal enum TireSubtype
        {
            Unavailable = -1,
            Primary = 0,
            Alternate = 1,
            Soft = 2,
            Medium = 3,
            Hard = 4
        };

        internal enum EngineType
        {
            Combustion = 0,
            Electric = 1,
            Hybrid = 2,
        };
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RaceDuration<T>
    {
        public T Race1;
        public T Race2;
        public T Race3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Vector3<T>
    {
        public T X;
        public T Y;
        public T Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Orientation<T>
    {
        public T Pitch;
        public T Yaw;
        public T Roll;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SectorStarts<T>
    {
        public T Sector1;
        public T Sector2;
        public T Sector3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PlayerData
    {
        // Virtual physics time
        // Unit: Ticks (1 tick = 1/400th of a second)
        public int GameSimulationTicks;

        // Virtual physics time
        // Unit: Seconds
        public double GameSimulationTime;

        // Car world-space position
        public Vector3<double> Position;

        // Car world-space velocity
        // Unit: Meter per second (m/s)
        public Vector3<double> Velocity;

        // Car local-space velocity
        // Unit: Meter per second (m/s)
        public Vector3<double> LocalVelocity;

        // Car world-space acceleration
        // Unit: Meter per second squared (m/s^2)
        public Vector3<double> Acceleration;

        // Car local-space acceleration
        // Unit: Meter per second squared (m/s^2)
        public Vector3<double> LocalAcceleration;

        // Car body orientation
        // Unit: Euler angles
        public Vector3<double> Orientation;

        // Car body rotation
        public Vector3<double> Rotation;

        // Car body angular acceleration (torque divided by inertia)
        public Vector3<double> AngularAcceleration;

        // Car world-space angular velocity
        // Unit: Radians per second
        public Vector3<double> AngularVelocity;

        // Car local-space angular velocity
        // Unit: Radians per second
        public Vector3<double> LocalAngularVelocity;

        // Driver g-force local to car
        public Vector3<double> LocalGforce;

        // Total steering force coming through steering bars
        public double SteeringForce;
        public double SteeringForcePercentage;

        // Current engine torque
        public double EngineTorque;

        // Current downforce
        // Unit: Newtons (N)
        public double CurrentDownforce;

        // Currently unused
        public double Voltage;
        public double ErsLevel;
        public double PowerMguH;
        public double PowerMguK;
        public double TorqueMguK;

        // Car setup (radians, meters, meters per second)
        public TireData<double> SuspensionDeflection;
        public TireData<double> SuspensionVelocity;
        public TireData<double> Camber;
        public TireData<double> RideHeight;
        public double FrontWingHeight;
        public double FrontRollAngle;
        public double RearRollAngle;

        // Reserved data
        public double Unused1;
        public double Unused2;
        public Vector3<double> Unused3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Flags
    {
        // Whether yellow flag is currently active
        // -1 = no data
        //  0 = not active
        //  1 = active
        public int Yellow;

        // Whether yellow flag was caused by current slot
        // -1 = no data
        //  0 = didn't cause it
        //  1 = caused it
        public int YellowCausedIt;

        // Whether overtake of car in front by current slot is allowed under yellow flag
        // -1 = no data
        //  0 = not allowed
        //  1 = allowed
        public int YellowOvertake;

        // Whether you have gained positions illegaly under yellow flag to give back
        // -1 = no data
        //  0 = no positions gained
        //  n = number of positions gained
        public int YellowPositionsGained;

        // Yellow flag for each sector; -1 = no data, 0 = not active, 1 = active
        public Sectors<int> SectorYellow;

        // Distance into track for closest yellow, -1.0 if no yellow flag exists
        // Unit: Meters (m)
        public float ClosestYellowDistanceIntoTrack;

        // Whether blue flag is currently active
        // -1 = no data
        //  0 = not active
        //  1 = active
        public int Blue;

        // Whether black flag is currently active
        // -1 = no data
        //  0 = not active
        //  1 = active
        public int Black;

        // Whether green flag is currently active
        // -1 = no data
        //  0 = not active
        //  1 = active
        public int Green;

        // Whether checkered flag is currently active
        // -1 = no data
        //  0 = not active
        //  1 = active
        public int Checkered;

        // Whether white flag is currently active
        // -1 = no data
        //  0 = not active
        //  1 = active
        public int White;

        // Whether black and white flag is currently active and reason
        // -1 = no data
        //  0 = not active
        //  1 = blue flag 1st warnings
        //  2 = blue flag 2nd warnings
        //  3 = wrong way
        //  4 = cutting track
        public int BlackAndWhite;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CarDamage
    {
        // Range: 0.0 - 1.0
        // Note: -1.0 = N/A
        public float Engine;

        // Range: 0.0 - 1.0
        // Note: -1.0 = N/A
        public float Transmission;

        // Range: 0.0 - 1.0
        // Note: A bit arbitrary at the moment. 0.0 doesn't necessarily mean completely destroyed.
        // Note: -1.0 = N/A
        public float Aerodynamics;

        // Range: 0.0 - 1.0
        // Note: -1.0 = N/A
        public float Suspension;

        // Reserved data
        public float Unused1;
        public float Unused2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TireData<T>
    {
        public T FrontLeft;
        public T FrontRight;
        public T RearLeft;
        public T RearRight;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PitMenuState
    {
        // Pit menu preset
        public int Preset;

        // Pit menu actions
        public int Penalty;
        public int Driverchange;
        public int Fuel;
        public int FrontTires;
        public int RearTires;
        public int FrontWing;
        public int RearWing;
        public int Suspension;

        // Pit menu buttons
        public int ButtonTop;
        public int ButtonBottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CutTrackPenalties
    {
        public int DriveThrough;
        public int StopAndGo;
        public int PitStop;
        public int TimeDeduction;
        public int SlowDown;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Drs
    {
        // If DRS is equipped and allowed
        // 0 = No, 1 = Yes, -1 = N/A
        public int Equipped;

        // Got DRS activation left
        // 0 = No, 1 = Yes, -1 = N/A
        public int Available;

        // Number of DRS activations left this lap
        // Note: In sessions with 'endless' amount of drs activations per lap this value starts at int32::max
        // -1 = N/A
        public int NumActivationsLeft;

        // DRS engaged
        // 0 = No, 1 = Yes, -1 = N/A
        public int Engaged;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PushToPass
    {
        public int Available;
        public int Engaged;
        public int AmountLeft;
        public float EngagedTimeLeft;
        public float WaitTimeLeft;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TireTempInformation
    {
        public TireTemperature<float> CurrentTemp;
        public float OptimalTemp;
        public float ColdTemp;
        public float HotTemp;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BrakeTemp
    {
        public float CurrentTemp;
        public float OptimalTemp;
        public float ColdTemp;
        public float HotTemp;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TireTemperature<T>
    {
        public T Left;
        public T Center;
        public T Right;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AidSettings
    {
        // ABS; -1 = N/A, 0 = off, 1 = on, 5 = currently active
        public int Abs;

        // TC; -1 = N/A, 0 = off, 1 = on, 5 = currently active
        public int Tc;

        // ESP; -1 = N/A, 0 = off, 1 = on low, 2 = on medium, 3 = on high, 5 = currently active
        public int Esp;

        // Countersteer; -1 = N/A, 0 = off, 1 = on, 5 = currently active
        public int Countersteer;

        // Cornering; -1 = N/A, 0 = off, 1 = on, 5 = currently active
        public int Cornering;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Sectors<T>
    {
        public T Sector1;
        public T Sector2;
        public T Sector3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct R3EDriverInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Name; // UTF-8

        public int CarNumber;
        public int ClassId;
        public int ModelId;
        public int TeamId;
        public int LiveryId;
        public int ManufacturerId;
        public int UserId;
        public int SlotId;

        public int ClassPerformanceIndex;

        // Note: See the EngineType enum
        public int EngineType;

        public int Unused1;
        public int Unused2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DriverData
    {
        public R3EDriverInfo R3EDriverInfo;

        // Note: See the R3E.Constant.FinishStatus enum
        public int FinishStatus;

        public int Place;

        // Based on performance index
        public int PlaceClass;
        public float LapDistance;
        public Vector3<float> Position;
        public int TrackSector;
        public int CompletedLaps;
        public int CurrentLapValid;
        public float LapTimeCurrentSelf;
        public Sectors<float> SectorTimeCurrentSelf;
        public Sectors<float> SectorTimePreviousSelf;
        public Sectors<float> SectorTimeBestSelf;
        public float TimeDeltaFront;

        public float TimeDeltaBehind;

        // Note: See the R3E.Constant.PitStopStatus enum
        public int PitStopStatus;
        public int InPitlane;

        public int NumPitstops;

        public CutTrackPenalties Penalties;

        public float CarSpeed;

        // Note: See the R3E.Constant.TireType enum
        public int TireTypeFront;

        public int TireTypeRear;

        // Note: See the R3E.Constant.TireSubtype enum
        public int TireSubtypeFront;
        public int TireSubtypeRear;

        public float BasePenaltyWeight;
        public float AidPenaltyWeight;

        // -1 unavailable, 0 = not engaged, 1 = engaged
        public int DrsState;
        public int PtpState;

        // -1 unavailable, DriveThrough = 0, StopAndGo = 1, Pitstop = 2, Time = 3, Slowdown = 4, Disqualify = 5,
        public int PenaltyType;

        // Based on the PenaltyType you can assume the reason is:

        // DriveThroughPenaltyInvalid = 0,
        // DriveThroughPenaltyCutTrack = 1,
        // DriveThroughPenaltyPitSpeeding = 2,
        // DriveThroughPenaltyFalseStart = 3,
        // DriveThroughPenaltyIgnoredBlue = 4,
        // DriveThroughPenaltyDrivingTooSlow = 5,
        // DriveThroughPenaltyIllegallyPassedBeforeGreen = 6,
        // DriveThroughPenaltyIllegallyPassedBeforeFinish = 7,
        // DriveThroughPenaltyIllegallyPassedBeforePitEntrance = 8,
        // DriveThroughPenaltyIgnoredSlowDown = 9,
        // DriveThroughPenaltyMax = 10

        // StopAndGoPenaltyInvalid = 0,
        // StopAndGoPenaltyCutTrack1st = 1,
        // StopAndGoPenaltyCutTrackMult = 2,
        // StopAndGoPenaltyYellowFlagOvertake = 3,
        // StopAndGoPenaltyMax = 4

        // PitstopPenaltyInvalid = 0,
        // PitstopPenaltyIgnoredPitstopWindow = 1,
        // PitstopPenaltyMax = 2

        // ServableTimePenaltyInvalid = 0,
        // ServableTimePenaltyServedMandatoryPitstopLate = 1,
        // ServableTimePenaltyIgnoredMinimumPitstopDuration = 2,
        // ServableTimePenaltyMax = 3

        // SlowDownPenaltyInvalid = 0,
        // SlowDownPenaltyCutTrack1st = 1,
        // SlowDownPenaltyCutTrackMult = 2,
        // SlowDownPenaltyMax = 3

        // DisqualifyPenaltyInvalid = -1,
        // DisqualifyPenaltyFalseStart = 0,
        // DisqualifyPenaltyPitlaneSpeeding = 1,
        // DisqualifyPenaltyWrongWay = 2,
        // DisqualifyPenaltyEnteringPitsUnderRed = 3,
        // DisqualifyPenaltyExitingPitsUnderRed = 4,
        // DisqualifyPenaltyFailedDriverChange = 5,
        // DisqualifyPenaltyThreeDriveThroughsInLap = 6,
        // DisqualifyPenaltyLappedFieldMultipleTimes = 7,
        // DisqualifyPenaltyIgnoredDriveThroughPenalty = 8,
        // DisqualifyPenaltyIgnoredStopAndGoPenalty = 9,
        // DisqualifyPenaltyIgnoredPitStopPenalty = 10,
        // DisqualifyPenaltyIgnoredTimePenalty = 11,
        // DisqualifyPenaltyExcessiveCutting = 12,
        // DisqualifyPenaltyIgnoredBlueFlag = 13,
        // DisqualifyPenaltyMax = 14
        public int PenaltyReason;

        // Reserved data
        public int Unused1;
        public int Unused2;
        public float Unused3;
        public float Unused4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct R3ESharedData
    {
        //////////////////////////////////////////////////////////////////////////
        // Version
        //////////////////////////////////////////////////////////////////////////
        public int VersionMajor;
        public int VersionMinor;
        public int AllDriversOffset; // Offset to NumCars variable
        public int DriverDataSize; // Size of DriverData

        //////////////////////////////////////////////////////////////////////////
        // Game State
        //////////////////////////////////////////////////////////////////////////

        public int GamePaused;
        public int GameInMenus;
        public int GameInReplay;
        public int GameUsingVr;

        public int GameUnused1;

        //////////////////////////////////////////////////////////////////////////
        // High Detail
        //////////////////////////////////////////////////////////////////////////

        // High precision data for player's vehicle only
        public PlayerData Player;

        //////////////////////////////////////////////////////////////////////////
        // Event And Session
        //////////////////////////////////////////////////////////////////////////

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] TrackName; // UTF-8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] LayoutName; // UTF-8

        public int TrackId;
        public int LayoutId;

        // Layout length in meters
        public float LayoutLength;
        public SectorStarts<float> SectorStartFactors;

        // Race session durations
        // Note: Index 0-2 = race 1-3
        // Note: Value -1 = N/A
        // Note: If both laps and minutes are more than 0, race session starts with minutes then adds laps
        public RaceDuration<int> RaceSessionLaps;
        public RaceDuration<int> RaceSessionMinutes;

        // The current race event index, for championships with multiple events
        // Note: 0-indexed, -1 = N/A
        public int EventIndex;

        // Which session the player is in (practice, qualifying, race, etc.)
        // Note: See the R3E.Constant.Session enum
        public int SessionType;

        // The current iteration of the current type of session (second qualifying session, etc.)
        // Note: 1 = first, 2 = second etc, -1 = N/A
        public int SessionIteration;

        // If the session is time based, lap based or time based with an extra lap at the end
        public int SessionLengthFormat;

        // Unit: Meter per second (m/s)
        public float SessionPitSpeedLimit;

        // Which phase the current session is in (gridwalk, countdown, green flag, etc.)
        // Note: See the R3E.Constant.SessionPhase enum
        public int SessionPhase;

        // Which phase start lights are in; -1 = unavailable, 0 = off, 1-5 = redlight on and counting down, 6 = greenlight on
        // Note: See the r3e_session_phase enum
        public int StartLights;

        // -1 = no data available
        //  0 = not active
        //  1 = active
        //  2 = 2x
        //  3 = 3x
        //  4 = 4x
        public int TireWearActive;

        // -1 = no data
        //  0 = not active
        //  1 = active
        //  2 = 2x
        //  3 = 3x
        //  4 = 4x
        public int FuelUseActive;

        // Total number of laps in the race, or -1 if player is not in race mode (practice, test mode, etc.)
        public int NumberOfLaps;

        // Amount of time and time remaining for the current session
        // Note: Only available in time-based sessions, -1.0 = N/A
        // Units: Seconds
        public float SessionTimeDuration;
        public float SessionTimeRemaining;

        // Reserved data
        public int EventUnused1;
        public float EventUnused2;

        //////////////////////////////////////////////////////////////////////////
        // Pit
        //////////////////////////////////////////////////////////////////////////

        // Current status of the pit stop
        // Note: See the R3E.Constant.PitWindow enum
        public int PitWindowStatus;

        // The minute/lap from which you're obligated to pit (-1 = N/A)
        // Unit: Minutes in time-based sessions, otherwise lap
        public int PitWindowStart;

        // The minute/lap into which you need to have pitted (-1 = N/A)
        // Unit: Minutes in time-based sessions, otherwise lap
        public int PitWindowEnd;

        // If current vehicle is in pitline (-1 = N/A)
        public int InPitlane;

        // What is currently selected in pit menu, and array of states (preset/buttons: -1 = not selectable, 1 = selectable) (actions: -1 = N/A, 0 = unmarked for fix, 1 = marked for fix)
        public int PitMenuSelection;
        public PitMenuState PitMenuState;

        // Current vehicle pit state (-1 = N/A, 0 = None, 1 = Requested stop, 2 = Entered pitlane heading for pitspot, 3 = Stopped at pitspot, 4 = Exiting pitspot heading for pit exit)
        public int PitState;

        // Current vehicle pitstop actions duration
        public float PitTotalDuration;
        public float PitElapsedTime;

        // Current vehicle pit action (-1 = N/A, 0 = None, 1 = Preparing, (combination of 2 = Penalty serve, 4 = Driver change, 8 = Refueling, 16 = Front tires, 32 = Rear tires, 64 = Front wing, 128 = Rear wing, 256 = Suspension))
        public int PitAction;

        // Number of pitstops the current vehicle has performed (-1 = N/A)
        public int NumPitstopsPerformed;

        // Reserved data
        public int PitUnused1;
        public float PitUnused2;

        //////////////////////////////////////////////////////////////////////////
        // Scoring & Timings
        //////////////////////////////////////////////////////////////////////////

        // The current state of each type of flag
        public Flags Flags;

        // Current position (1 = first place)
        public int Position;

        // Based on performance index
        public int PositionClass;

        // Note: See the R3E.Constant.FinishStatus enum
        public int FinishStatus;

        // Total number of cut track warnings (-1 = N/A)
        public int CutTrackWarnings;

        // The number of penalties the car currently has pending of each type (-1 = N/A)
        public CutTrackPenalties Penalties;

        // Total number of penalties pending for the car
        // Note: See the 'penalties' field
        public int NumPenalties;

        // How many laps the player has completed. If this value is 6, the player is on his 7th lap. -1 = n/a
        public int CompletedLaps;
        public int CurrentLapValid;
        public int TrackSector;

        public float LapDistance;

        // fraction of lap completed, 0.0-1.0, -1.0 = N/A
        public float LapDistanceFraction;

        // The current best lap time for the leader of the session (-1.0 = N/A)
        public float LapTimeBestLeader;

        // The current best lap time for the leader of the player's class in the current session (-1.0 = N/A)
        public float LapTimeBestLeaderClass;

        // Sector times of fastest lap by anyone in session
        // Unit: Seconds (-1.0 = N/A)
        public Sectors<float> SectorTimesSessionBestLap;

        // Unit: Seconds (-1.0 = none)
        public float LapTimeBestSelf;

        public Sectors<float> SectorTimesBestSelf;

        // Unit: Seconds (-1.0 = none)
        public float LapTimePreviousSelf;

        public Sectors<float> SectorTimesPreviousSelf;

        // Unit: Seconds (-1.0 = none)
        public float LapTimeCurrentSelf;

        public Sectors<float> SectorTimesCurrentSelf;

        // The time delta between the player's time and the leader of the current session (-1.0 = N/A)
        public float LapTimeDeltaLeader;

        // The time delta between the player's time and the leader of the player's class in the current session (-1.0 = N/A)
        public float LapTimeDeltaLeaderClass;

        // Time delta between the player and the car placed in front (-1.0 = N/A)
        // Units: Seconds
        public float TimeDeltaFront;

        // Time delta between the player and the car placed behind (-1.0 = N/A)
        // Units: Seconds
        public float TimeDeltaBehind;

        // Time delta between this car's current laptime and this car's best laptime
        // Unit: Seconds (-1000.0 = N/A)
        public float TimeDeltaBestSelf;

        // Best time for each individual sector no matter lap
        // Unit: Seconds (-1.0 = N/A)
        public Sectors<float> BestIndividualSectorTimeSelf;
        public Sectors<float> BestIndividualSectorTimeLeader;
        public Sectors<float> BestIndividualSectorTimeLeaderClass;

        // Reserved data
        public int ScoreUnused1;
        public int ScoreUnused2;
        public float ScoreUnused3;
        public float ScoreUnused4;

        //////////////////////////////////////////////////////////////////////////
        // Vehicle information
        //////////////////////////////////////////////////////////////////////////

        public R3EDriverInfo VehicleInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] PlayerName; // UTF-8

        //////////////////////////////////////////////////////////////////////////
        // Vehicle State
        //////////////////////////////////////////////////////////////////////////

        // Which controller is currently controlling the player's car (AI, player, remote, etc.)
        // Note: See the R3E.Constant.Control enum
        public int ControlType;

        // Unit: Meter per second (m/s)
        public float CarSpeed;

        // Unit: Radians per second (rad/s)
        public float EngineRps;
        public float MaxEngineRps;
        public float UpshiftRps;

        // -2 = N/A, -1 = reverse, 0 = neutral, 1 = first gear, ...
        public int Gear;

        // -1 = N/A
        public int NumGears;

        // Physical location of car's center of gravity in world space (X, Y, Z) (Y = up)
        public Vector3<float> CarCgLocation;

        // Pitch, yaw, roll
        // Unit: Radians (rad)
        public Orientation<float> CarOrientation;

        // Acceleration in three axes (X, Y, Z) of car body in local-space.
        // From car center, +X=left, +Y=up, +Z=back.
        // Unit: Meter per second squared (m/s^2)
        public Vector3<float> LocalAcceleration;

        // Unit: Kilograms (kg)
        // Note: Car + penalty weight + fuel
        public float TotalMass;

        // Unit: Liters (l)
        // Note: Fuel per lap show estimation when not enough data, then max recorded fuel per lap
        // Note: Not valid for remote players
        public float FuelLeft;
        public float FuelCapacity;

        public float FuelPerLap;

        // Unit: Celsius (C)
        // Note: Not valid for AI or remote players
        public float EngineWaterTemp;

        public float EngineOilTemp;

        // Unit: Kilopascals (KPa)
        // Note: Not valid for AI or remote players
        public float FuelPressure;

        // Unit: Kilopascals (KPa)
        // Note: Not valid for AI or remote players
        public float EngineOilPressure;

        // Unit: (Bar)
        // Note: Not valid for AI or remote players (-1.0 = N/A)
        public float TurboPressure;

        // How pressed the throttle pedal is
        // Range: 0.0 - 1.0 (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float Throttle;

        public float ThrottleRaw;

        // How pressed the brake pedal is
        // Range: 0.0 - 1.0 (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float Brake;

        public float BrakeRaw;

        // How pressed the clutch pedal is
        // Range: 0.0 - 1.0 (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float Clutch;

        public float ClutchRaw;

        // How much the steering wheel is turned
        // Range: -1.0 - 1.0
        // Note: Not valid for AI or remote players
        public float SteerInputRaw;

        // How many degrees in steer lock (center to full lock)
        // Note: Not valid for AI or remote players
        public int SteerLockDegrees;

        // How many degrees in wheel range (degrees full left to rull right)
        // Note: Not valid for AI or remote players
        public int SteerWheelRangeDegrees;

        // Aid settings
        public AidSettings AidSettings;

        // DRS data
        public Drs Drs;

        // Pit limiter (-1 = N/A, 0 = inactive, 1 = active)
        public int PitLimiter;

        // Push to pass data
        public PushToPass PushToPass;

        // How much the vehicle's brakes are biased towards the back wheels (0.3 = 30%, etc.) (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float BrakeBias;

        // Reserved data
        public int VehicleUnused1;
        public int VehicleUnused2;
        public float VehicleUnused3;
        public float VehicleUnused4;
        Orientation<float> VehicleUnused5;

        //////////////////////////////////////////////////////////////////////////
        // Tires
        //////////////////////////////////////////////////////////////////////////

        // Which type of tires the player's car has (option, prime, etc.)
        // Note: See the R3E.Constant.TireType enum, deprecated - use the values further down instead
        public int TireType;

        // Rotation speed
        // Uint: Radians per second
        public TireData<float> TireRps;

        // Wheel speed
        // Uint: Meters per second
        public TireData<float> TireSpeed;

        // Range: 0.0 - 1.0 (-1.0 = N/A)
        public TireData<float> TireGrip;

        // Range: 0.0 - 1.0 (-1.0 = N/A)
        public TireData<float> TireWear;

        // (-1 = N/A, 0 = false, 1 = true)
        public TireData<int> TireFlatspot;

        // Unit: Kilopascals (KPa) (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public TireData<float> TirePressure;

        // Percentage of dirt on tire (-1.0 = N/A)
        // Range: 0.0 - 1.0
        public TireData<float> TireDirt;

        // Current temperature of three points across the tread of the tire (-1.0 = N/A)
        // Optimum temperature
        // Cold temperature
        // Hot temperature
        // Unit: Celsius (C)
        // Note: Not valid for AI or remote players
        public TireData<TireTempInformation> TireTemp;

        // Which type of tires the car has (option, prime, etc.)
        // Note: See the R3E.Constant.TireType enum
        public int TireTypeFront;

        public int TireTypeRear;

        // Which subtype of tires the car has
        // Note: See the R3E.Constant.TireSubtype enum
        public int TireSubtypeFront;
        public int TireSubtypeRear;

        // Current brake temperature (-1.0 = N/A)
        // Optimum temperature
        // Cold temperature
        // Hot temperature
        // Unit: Celsius (C)
        // Note: Not valid for AI or remote players
        public TireData<BrakeTemp> BrakeTemp;

        // Brake pressure (-1.0 = N/A)
        // Unit: Kilo Newtons (kN)
        // Note: Not valid for AI or remote players
        public TireData<float> BrakePressure;

        // Reserved data
        public int TireUnused1;
        public int TireUnused2;
        public float TireUnused3;
        public float TireUnused4;
        public TireData<float> TireUnused5;
        public TireData<float> TireUnused6;

        //////////////////////////////////////////////////////////////////////////
        // Damage
        //////////////////////////////////////////////////////////////////////////

        // The current state of various parts of the car
        // Note: Not valid for AI or remote players
        public CarDamage CarDamage;

        //////////////////////////////////////////////////////////////////////////
        // Driver Info
        //////////////////////////////////////////////////////////////////////////

        // Number of cars (including the player) in the race
        public int NumCars;

        // Contains name and basic vehicle info for all drivers in place order
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public DriverData[] DriverData;
    }
}