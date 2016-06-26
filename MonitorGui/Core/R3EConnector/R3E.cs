using System;
using System.Runtime.InteropServices;

namespace SecondMonitor.Core.R3EConnector
{
    public class Constant
    {        

        enum Session
        {
            Unavailable = -1,
            Practice = 0,
            Qualify = 1,
            Race = 2,
        };

        enum SessionPhase
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

        enum Control
        {
            Unavailable = -1,

            // Controlled by the actual player
            Player = 0,

            // Controlled by AI
            AI = 1,

            // Controlled by a network entity of some sort
            Remote = 2,

            // Controlled by a replay or ghost
            Replay = 3,
        };

        enum PitWindow
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

        enum TireType
        {
            Unavailable = -1,
            Option = 0,
            Prime = 1,
        };
    }

    namespace Data
    {
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
        public struct UserInput
        {
            public Single _1, _2, _3, _4, _5, _6;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TireTemperature
        {
            public Single FrontLeft_Left;
            public Single FrontLeft_Center;
            public Single FrontLeft_Right;

            public Single FrontRight_Left;
            public Single FrontRight_Center;
            public Single FrontRight_Right;

            public Single RearLeft_Left;
            public Single RearLeft_Center;
            public Single RearLeft_Right;

            public Single RearRight_Left;
            public Single RearRight_Center;
            public Single RearRight_Right;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PlayerData
        {
            // Virtual physics time
            // Unit: Ticks (1 tick = 1/400th of a second)
            public Int32 GameSimulationTicks;

            // Padding to accomodate for legacy alignment
            [ObsoleteAttribute("Used for padding", true)]
            public Int32 Padding1;

            // Virtual physics time
            // Unit: Seconds
            public Double GameSimulationTime;

            // Car world-space position
            public Vector3<Double> Position;

            // Car world-space velocity
            // Unit: Meter per second (m/s)
            public Vector3<Double> Velocity;

            // Car world-space acceleration
            // Unit: Meter per second squared (m/s^2)
            public Vector3<Double> Acceleration;

            // Car local-space acceleration
            // Unit: Meter per second squared (m/s^2)
            public Vector3<Double> LocalAcceleration;

            // Car body orientation
            // Unit: Euler angles
            public Vector3<Double> Orientation;

            // Car body rotation
            public Vector3<Double> Rotation;

            // Car body angular acceleration (torque divided by inertia)
            public Vector3<Double> AngularAcceleration;

            // Reserved for future implementation of DriverBodyAcceleration
            [ObsoleteAttribute("Reserved for future use", false)]
            public Vector3<Double> DriverBodyAcceleration;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Flags
        {
            // Whether yellow flag is currently active
            // -1 = no data
            //  0 = not active
            //  1 = active
            // Note: Uses legacy code and isn't really supported at the moment. Use at your own risk.
            public Int32 Yellow;

            // Whether blue flag is currently active
            // -1 = no data
            //  0 = not active
            //  1 = active
            // Note: Uses legacy code and isn't really supported at the moment. Use at your own risk.
            public Int32 Blue;

            // Whether black flag is currently active
            // -1 = no data
            //  0 = not active
            //  1 = active
            public Int32 Black;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarDamage
        {
            // Range: 0.0 - 1.0
            // Note: -1.0 = N/A
            public Single Engine;

            // Range: 0.0 - 1.0
            // Note: -1.0 = N/A
            public Single Transmission;

            // Range: 0.0 - 1.0
            // Note: A bit arbitrary at the moment. 0.0 doesn't necessarily mean completely destroyed.
            // Note: -1.0 = N/A
            public Single Aerodynamics;

            // Tire wear
            // Range: 0.0 - 1.0
            // Note: -1.0 = N/A
            public Single TireFrontLeft;
            public Single TireFrontRight;
            public Single TireRearLeft;
            public Single TireRearRight;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TirePressure
        {
            public Single FrontLeft;
            public Single FrontRight;
            public Single RearLeft;
            public Single RearRight;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BrakeTemperatures
        {
            public Single FrontLeft;
            public Single FrontRight;
            public Single RearLeft;
            public Single RearRight;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CutTrackPenalties
        {
            public Int32 DriveThrough;
            public Int32 StopAndGo;
            public Int32 PitStop;
            public Int32 TimeDeduction;
            public Int32 SlowDown;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Sectors<T>
        {
            public T Sector1;
            public T Sector2;
            public T Sector3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct R3ESharedData
        {
            [ObsoleteAttribute("Not set anymore", false)]
            public UserInput UserInput;

            // Engine speed
            // Unit: Radians per second (rad/s)
            public Single EngineRps;

            // Maximum engine speed
            // Unit: Radians per second (rad/s)
            public Single MaxEngineRps;

            // Unit: Kilopascals (KPa)
            public Single FuelPressure;

            // Current amount of fuel in the tank(s)
            // Unit: Liters (l)
            public Single FuelLeft;

            // Maximum capacity of fuel tank(s)
            // Unit: Liters (l)
            public Single FuelCapacity;

            // Unit: Celsius (C)
            public Single EngineWaterTemp;

            // Unit: Celsius (C)
            public Single EngineOilTemp;

            // Unit: Kilopascals (KPa)
            public Single EngineOilPressure;

            // Unit: Meter per second (m/s)
            public Single CarSpeed;

            // Total number of laps in the race, or -1 if player is not in race mode (practice, test mode, etc.)
            public Int32 NumberOfLaps;

            // How many laps the player has completed. If this value is 6, the player is on his 7th lap. -1 = n/a
            public Int32 CompletedLaps;

            // Unit: Seconds (-1.0 = none)
            public Single LapTimeBestSelf;

            // Unit: Seconds (-1.0 = none)
            public Single LapTimePreviousSelf;

            // Unit: Seconds (-1.0 = none)
            public Single LapTimeCurrentSelf;

            // Current position (1 = first place)
            public Int32 Position;

            // Number of cars (including the player) in the race
            public Int32 NumCars;

            // -2 = no data
            // -1 = reverse,
            //  0 = neutral
            //  1 = first gear
            // (... up to 7th)
            public Int32 Gear;

            // Temperature of three points across the tread of each tire
            // Unit: Celsius (C)
            public TireTemperature TireTemp;

            // Number of penalties pending for the player
            // Note: See the 'Penalties' field
            public Int32 NumPenalties;

            // Physical location of car's center of gravity in world space (X, Y, Z) (Y = up)
            public Vector3<Single> CarCgLocation;

            // Pitch, yaw, roll
            // Unit: Radians (rad)
            public Orientation<Single> CarOrientation;

            // Acceleration in three axes (X, Y, Z) of car body in local-space.
            // From car center, +X=left, +Y=up, +Z=back.
            // Unit: Meter per second squared (m/s^2)
            public Vector3<Single> LocalAcceleration;

            // -1 = no data for DRS
            //  0 = not available
            //  1 = available
            public Int32 DrsAvailable;

            // -1 = no data for DRS
            //  0 = not engaged
            //  1 = engaged
            public Int32 DrsEngaged;

            // Padding to accomodate for legacy alignment
            [ObsoleteAttribute("Used for padding", true)]
            public Int32 Padding1;

            // High precision data for player's vehicle only
            public PlayerData Player;

            // The current race event index, for championships with multiple events
            // Note: 0-indexed, -1 = N/A
            public Int32 EventIndex;

            // Which session the player is in (practice, qualifying, race, etc.)
            // Note: See the R3E.Constant.Session enum
            public Int32 SessionType;

            // Which phase the current session is in (gridwalk, countdown, green flag, etc.)
            // Note: See the R3E.Constant.SessionPhase enum
            public Int32 SessionPhase;

            // The current iteration of the current type of session (second qualifying session, etc.)
            // Note: 0-indexed, -1 = N/A
            public Int32 SessionIteration;

            // Which controller is currently controlling the player's car (AI, player, remote, etc.)
            // Note: See the R3E.Constant.Control enum
            public Int32 ControlType;

            // How pressed the throttle pedal is
            // Range: 0.0 - 1.0
            public Single ThrottlePedal;

            // How pressed the brake pedal is (-1.0 = N/A)
            // Range: 0.0 - 1.0
            public Single BrakePedal;

            // How pressed the clutch pedal is (-1.0 = N/A)
            // Range: 0.0 - 1.0
            public Single ClutchPedal;

            // How much the player's brakes are biased towards the back wheels (0.3 = 30%, etc.)
            // Note: -1.0 = N/A
            public Single BrakeBias;

            // Unit: Kilopascals (KPa)
            public TirePressure TirePressure;

            // -1 = no data available
            //  0 = not active
            //  1 = active
            public Int32 TireWearActive;

            // Which type of tires the player's car has (option, prime, etc.)
            // Note: See the R3E.Constant.TireType enum
            public Int32 TireType;

            // Brake temperatures for all four wheels
            // Unit: Celsius (C)
            public BrakeTemperatures BrakeTemp;

            // -1 = no data
            //  0 = not active
            //  1 = active
            public Int32 FuelUseActive;

            // Amount of time remaining for the current session
            // Note: Only available in time-based sessions, -1.0 = N/A
            // Units: Seconds
            public Single SessionTimeRemaining;

            // The current best lap time for the leader of the session (-1.0 = N/A)
            public Single LapTimeBestLeader;

            // The current best lap time for the leader of the player's class in the current session (-1.0 = N/A)
            public Single LapTimeBestLeaderClass;

            // Reserved for future (proper) implementation
            [ObsoleteAttribute("Improper implementation, use with caution", false)]
            public Single LapTimeDeltaSelf;

            // The time delta between the player's time and the leader of the current session (-1.0 = N/A)
            public Single LapTimeDeltaLeader;

            // The time delta between the player's time and the leader of the player's class in the current session (-1.0 = N/A)
            public Single LapTimeDeltaLeaderClass;

            // Reserved for future (proper) implementation
            [ObsoleteAttribute("Improper implementation, use with caution", false)]
            public Sectors<Single> SectorTimeDeltaSelf;

            // Reserved for future (proper) implementation
            [ObsoleteAttribute("Improper implementation, use with caution", false)]
            public Sectors<Single> SectorTimeDeltaLeader;

            // Reserved for future (proper) implementation
            [ObsoleteAttribute("Improper implementation, use with caution", false)]
            public Sectors<Single> SectorTimeDeltaLeaderClass;

            // Time delta between the player and the car placed in front (-1.0 = N/A)
            // Units: Seconds
            public Single TimeDeltaFront;

            // Time delta between the player and the car placed behind (-1.0 = N/A)
            // Units: Seconds
            public Single TimeDeltaBehind;

            // Current status of the pit stop
            // Note: See the R3E.Constant.PitWindow enum
            public Int32 PitWindowStatus;

            // The minute/lap from which you're obligated to pit (-1 = N/A)
            // Unit: Minutes in time-based sessions, otherwise lap
            public Int32 PitWindowStart;

            // The minute/lap into which you need to have pitted (-1 = N/A)
            // Unit: Minutes in time-based sessions, otherwise lap
            public Int32 PitWindowEnd;

            // Total number of cut track warnings (-1 = N/A)
            public Int32 CutTrackWarnings;

            // The number of penalties the player currently has pending of each type (-1 = N/A)
            public CutTrackPenalties Penalties;

            // The current state of each type of flag
            public Flags Flags;

            // The current state of various parts of the player's car
            public CarDamage CarDamage;
        }
    }
}