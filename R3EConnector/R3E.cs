namespace SecondMonitor.R3EConnector
{
    using System.Runtime.InteropServices;

    public class Constant
    {
        public const string SharedMemoryName = "$R3E";

        public enum VersionMajor
        {
            // Major version number to test against
            R3EVersionMajor = 1
        }

        public enum VersionMinor
        {
            // Minor version number to test against
            R3EVersionMinor = 7
        }

        public enum Session
        {
            Unavailable = -1,
            Practice = 0,
            Qualify = 1,
            Race = 2,
            Warmup = 3,
        }

        public enum SessionPhase
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
        }

        public enum Control
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
        }

        public enum PitWindow
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
        }

        public enum PitStopStatus
        {
            // No mandatory pitstops
            Unavailable = -1,

            // Mandatory pitstop not served yet
            Unserved = 0,

            // Mandatory pitstop served
            Served = 1,
        }

        public enum FinishStatus
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
        }

        public enum SessionLengthFormat
        {
            // N/A
            Unavailable = -1,

            TimeBased = 0,

            LapBased = 1,

            // Time and lap based session means there will be an extra lap after the time has run out
            TimeAndLapBased = 2
        }

        public enum TireType
        {
            Unavailable = -1,
            Option = 0,
            Prime = 1,
        }

        public enum TireSubtype
        {
            Unavailable = -1,
            Primary = 0,
            Alternate = 1,
            Soft = 2,
            Medium = 3,
            Hard = 4
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3<T>
    {
        public T X;
        public T Y;
        public T Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Orientation<T>
    {
        public T Pitch;
        public T Yaw;
        public T Roll;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TireTemperature
    {
        public float FrontLeft_Left;
        public float FrontLeft_Center;
        public float FrontLeft_Right;

        public float FrontRight_Left;
        public float FrontRight_Center;
        public float FrontRight_Right;

        public float RearLeft_Left;
        public float RearLeft_Center;
        public float RearLeft_Right;

        public float RearRight_Left;
        public float RearRight_Center;
        public float RearRight_Right;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PlayerData
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
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Flags
    {
        // Whether yellow flag is currently active
        // -1 = no data
        // 0 = not active
        // 1 = active
        public int Yellow;

        // Whether blue flag is currently active
        // -1 = no data
        // 0 = not active
        // 1 = active
        public int Blue;

        // Whether black flag is currently active
        // -1 = no data
        // 0 = not active
        // 1 = active
        public int Black;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ExtendedFlags
    {
        // Whether green flag is currently active
        // -1 = no data
        // 0 = not active
        // 1 = active
        public int Green;

        // Whether checkered flag is currently active
        // -1 = no data
        // 0 = not active
        // 1 = active
        public int Checkered;

        // Whether black and white flag is currently active and reason
        // -1 = no data
        // 0 = not active
        // 1 = blue flag 1st warnings
        // 2 = blue flag 2nd warnings
        // 3 = wrong way
        // 4 = cutting track
        public int BlackAndWhite;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ExtendedFlags2
    {
        // Whether white flag is currently active
        // -1 = no data
        // 0 = not active
        // 1 = active
        public int White;

        // Whether yellow flag was caused by current slot
        // -1 = no data
        // 0 = didn't cause it
        // 1 = caused it
        public int YellowCausedIt;

        // Whether overtake of car in front by current slot is allowed under yellow flag
        // -1 = no data
        // 0 = not allowed
        // 1 = allowed
        public int YellowOvertake;

        // Whether you have gained positions illegaly under yellow flag to give back
        // -1 = no data
        // 0 = no positions gained
        // n = number of positions gained
        public int YellowPositionsGained;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarDamage
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
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TireData
    {
        public float FrontLeft;
        public float FrontRight;
        public float RearLeft;
        public float RearRight;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CutTrackPenalties
    {
        public int DriveThrough;
        public int StopAndGo;
        public int PitStop;
        public int TimeDeduction;
        public int SlowDown;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Drs
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
    public struct PushToPass
    {
        public int Available;
        public int Engaged;
        public int AmountLeft;
        public float EngagedTimeLeft;
        public float WaitTimeLeft;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Sectors<T>
    {
        public T Sector1;
        public T Sector2;
        public T Sector3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R3EDriverInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Name; // UTF-8
        public int CarNumber;
        public int ClassId;
        public int ModelId;
        public int TeamId;
        public int LiveryId;
        public int ManufacturerId;
        public int SlotId;
        public int ClassPerformanceIndex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DriverData
    {
        public R3EDriverInfo DriverInfo;

        // Note: See the R3E.Constant.FinishStatus enum
        public int FinishStatus;

        public int Place;

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
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R3ESharedData
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

        // The current race event index, for championships with multiple events
        // Note: 0-indexed, -1 = N/A
        public int EventIndex;

        // Which session the player is in (practice, qualifying, race, etc.)
        // Note: See the R3E.Constant.Session enum
        public int SessionType;

        // The current iteration of the current type of session (second qualifying session, etc.)
        // Note: 0-indexed, -1 = N/A
        public int SessionIteration;

        // Which phase the current session is in (gridwalk, countdown, green flag, etc.)
        // Note: See the R3E.Constant.SessionPhase enum
        public int SessionPhase;

        // -1 = no data available
        // 0 = not active
        // 1 = active
        public int TireWearActive;

        // -1 = no data
        // 0 = not active
        // 1 = active
        public int FuelUseActive;

        // Total number of laps in the race, or -1 if player is not in race mode (practice, test mode, etc.)
        public int NumberOfLaps;

        // Amount of time remaining for the current session
        // Note: Only available in time-based sessions, -1.0 = N/A
        // Units: Seconds
        public float SessionTimeRemaining;

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

        // Number of pitstops the current vehicle has performed (-1 = N/A)
        public int NumPitstopsPerformed;

        //////////////////////////////////////////////////////////////////////////
        // Scoring & Timings
        //////////////////////////////////////////////////////////////////////////

        // The current state of each type of flag
        public Flags Flags;

        // Current position (1 = first place)
        public int Position;

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

        // Unit: Liters (l)
        // Note: Not valid for AI or remote players
        public float FuelLeft;

        public float FuelCapacity;

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

        // How pressed the throttle pedal is 
        // Range: 0.0 - 1.0 (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float ThrottlePedal;

        // How pressed the brake pedal is
        // Range: 0.0 - 1.0 (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float BrakePedal;

        // How pressed the clutch pedal is 
        // Range: 0.0 - 1.0 (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float ClutchPedal;

        // DRS data
        public Drs Drs;

        // Pit limiter (-1 = N/A, 0 = inactive, 1 = active)
        public int PitLimiter;

        // Push to pass data
        public PushToPass PushToPass;

        // How much the vehicle's brakes are biased towards the back wheels (0.3 = 30%, etc.) (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public float BrakeBias;

        //////////////////////////////////////////////////////////////////////////
        // Tires
        //////////////////////////////////////////////////////////////////////////

        // Which type of tires the player's car has (option, prime, etc.)
        // Note: See the R3E.Constant.TireType enum, deprecated - use the values further down instead
        public int TireType;

        // Rotation speed
        // Uint: Radians per second
        public TireData TireRps;

        // Range: 0.0 - 1.0 (-1.0 = N/A)
        public TireData TireGrip;

        // Range: 0.0 - 1.0 (-1.0 = N/A)
        public TireData TireWear;

        // Unit: Kilopascals (KPa) (-1.0 = N/A)
        // Note: Not valid for AI or remote players
        public TireData TirePressure;

        // Percentage of dirt on tire (-1.0 = N/A)
        // Range: 0.0 - 1.0
        public TireData TireDirt;

        // Brake temperature (-1.0 = N/A)
        // Unit: Celsius (C)
        // Note: Not valid for AI or remote players
        public TireData BrakeTemp;

        // Temperature of three points across the tread of the tire (-1.0 = N/A)
        // Unit: Celsius (C)
        // Note: Not valid for AI or remote players
        public TireTemperature TireTemp;

        // Which type of tires the car has (option, prime, etc.)
        // Note: See the R3E.Constant.TireType enum
        public int TireTypeFront;

        public int TireTypeRear;

        // Which subtype of tires the car has
        // Note: See the R3E.Constant.TireSubtype enum
        public int TireSubtypeFront;

        public int TireSubtypeRear;

        //////////////////////////////////////////////////////////////////////////
        // Damage
        //////////////////////////////////////////////////////////////////////////

        // The current state of various parts of the car
        // Note: Not valid for AI or remote players
        public CarDamage CarDamage;

        //////////////////////////////////////////////////////////////////////////
        // Additional Info
        //////////////////////////////////////////////////////////////////////////

        // The current state of each type of extended flag
        public ExtendedFlags ExtendedFlags;

        // Yellow flag for each sector
        // -1 = no data
        // 0 = not active
        // 1 = active
        public Sectors<int> SectorYellow;

        // Distance into track for closest yellow, -1.0 if no yellow flag exists
        // Unit: Meters (m)
        public float ClosestYellowDistanceIntoTrack;

        // Additional flag info
        public ExtendedFlags2 ExtendedFlags2;

        // If the session is time based, lap based or time based with an extra lap at the end
        // Note: See the R3E.Constant.SessionLengthFormat enum
        public int SessionLengthFormat;

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
