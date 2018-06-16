namespace SecondMonitor.RFactorConnector.SharedMemory
{
    using System.Runtime.InteropServices;


    /*
rfSharedinternal struct.hpp
by Dan Alinto (daniel.s.alinto@gmail.com)

This is the internal structure of the shared memory map
It's nearly identical to the original internal structures specified in InternalsPlugin.hpp,
but with pragma pack 1 specified to get the most compact representation.
This means that you need to watch your types very closely!
*/

    internal enum RfGamePhase
    {
        Garage = 0,
        WarmUp = 1,
        GridWalk = 2,
        Formation = 3,
        Countdown = 4,
        GreenFlag = 5,
        FullCourseYellow = 6,
        SessionStopped = 7,
        SessionOver = 8
    }


    internal enum RfYellowFlagState
    {
        Invalid = -1,
        NoFlag = 0,
        Pending = 1,
        PitClosed = 2,
        PitLeadLap = 3,
        PitOpen = 4,
        LastLap = 5,
        Resume = 6,
        RaceHalt = 7
    }


    internal enum RfSurfaceType
    {
        Dry = 0,
        Wet = 1,
        Grass = 2,
        Dirt = 3,
        Gravel = 4,
        Kerb = 5
    }


    internal enum RfSector
    {
        Sector3 = 0,
        Sector1 = 1,
        Sector2 = 2
    }


    internal enum RfFinishStatus
    {
        None = 0,
        Finished = 1,
        Dnf = 2,
        Dq = 3
    }


    internal enum RfControl
    {
        Nobody = -1,
        Player = 0,
        AI = 1,
        Remote = 2,
        Replay = 3
    }


    internal enum RfWheelIndex
    {
        FrontLeft = 0,
        FrontRight = 1,
        RearLeft = 2,
        RearRight = 3
    }

    internal enum RfSessionType
    {
        TestDay = 0,
        Practice1 = 1,
        Practice2 = 2,
        Practice3 = 3,
        Undefined = 4,
        Qualification = 5,
        WarmUp = 6,
        Race1 = 7,
        Race2 = 8
    }




    // Our world coordinate system is left-handed, with +y pointing up.
    // The local vehicle coordinate system is as follows:
    // +x points out the left side of the car (from the driver's perspective)
    // +y points out the roof
    // +z points out the back of the car
    // Rotations are as follows:
    // +x pitches up
    // +y yaws to the right
    // +z rolls to the right
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RfVec3
    {
        public float X;

        public float Y;

        public float Z;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 67, Pack = 1)]
    internal struct RfWheel
    {
        public float Rotation;               // radians/sec
        public float SuspensionDeflection;   // meters
        public float RideHeight;             // meters
        public float TireLoad;               // Newtons
        public float LateralForce;           // Newtons
        public float GripFract;              // an approximation of what fraction of the contact patch is sliding
        public float BrakeTemp;              // Celsius
        public float Pressure;               // kPa

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Temperature;         // Celsius, left/center/right (not to be confused with inside/center/outside!)
        public float Wear;                   // wear (0.0-1.0, fraction of maximum) ... this is not necessarily proportional with grip loss

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] TerrainName;        // the material prefixes from the TDF file
        public byte SurfaceType;    // 0=dry, 1=wet, 2=grass, 3=dirt, 4=gravel, 5=rumblestrip
        public byte Flat;                    // whether tire is flat
        public byte Detached;                // whether wheel is detached
    }

    // scoring info only updates twice per second (values interpolated when deltaTime > 0)!
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 304, Pack = 1)]
    internal struct RfVehicleInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] DriverName; // driver name

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] VehicleName; // vehicle name

        public short TotalLaps; // laps completed

        public sbyte Sector; // 0=sector3, 1=sector1, 2=sector2 (don't ask why)

        public sbyte FinishStatus; // 0=none, 1=finished, 2=dnf, 3=dq

        public float LapDist; // current distance around track

        public float PathLateral; // lateral position with respect to *very approximate* "center" path

        public float TrackEdge; // track edge (w.r.t. "center" path) on same side of track as vehicle

        public float BestSector1; // best sector 1

        public float BestSector2; // best sector 2 (plus sector 1)

        public float BestLapTime; // best lap time

        public float LastSector1; // last sector 1

        public float LastSector2; // last sector 2 (plus sector 1)

        public float LastLapTime; // last lap time

        public float CurSector1; // current sector 1 if valid

        public float CurSector2; // current sector 2 (plus sector 1) if valid

        // no current laptime because it instantly becomes "last"
        public short NumPitstops; // number of pitstops made

        public short NumPenalties; // number of outstanding penalties

        public byte IsPlayer; // is this the player's vehicle

        public sbyte Control; // who's in control: -1=nobody (shouldn't get this), 0=local player, 1=local AI, 2=remote, 3=replay (shouldn't get this)

        public byte InPits; // between pit entrance and pit exit (not always accurate for remote vehicles)

        public byte Place; // 1-based position

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] VehicleClass; // vehicle class

        // Dash Indicators
        public float TimeBehindNext; // time behind vehicle in next higher place

        public int LapsBehindNext; // laps behind vehicle in next higher place

        public float TimeBehindLeader; // time behind leader

        public int LapsBehindLeader; // laps behind leader

        public float LapStartET; // time this lap was started

        // Position and derivatives
        public RfVec3 Pos; // world position in meters

        public RfVec3 LocalVel; // velocity (meters/sec) in local vehicle coordinates

        public RfVec3 LocalAccel; // acceleration (meters/sec^2) in local vehicle coordinates

        // Orientation and derivatives
        public RfVec3 OriX
            ; // top row of orientation matrix (also converts local vehicle vectors into world X using dot product)

        public RfVec3 OriY
            ; // mid row of orientation matrix (also converts local vehicle vectors into world Y using dot product)

        public RfVec3 OriZ
            ; // bot row of orientation matrix (also converts local vehicle vectors into world Z using dot product)

        public RfVec3 LocalRot; // rotation (radians/sec) in local vehicle coordinates

        public RfVec3 LocalRotAccel; // rotational acceleration (radians/sec^2) in local vehicle coordinates

        public float Speed; // meters/sec
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 39647, Pack = 1)]
    internal struct RfShared
    {
        // Time
        public float DeltaTime; // time since last scoring update (seconds)

        public int LapNumber; // current lap number

        public float LapStartET; // time this lap was started

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] VehicleName; // current vehicle name

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] TrackName; // current track name

        // Position and derivatives
        public RfVec3 Pos; // world position in meters

        public RfVec3 LocalVel; // velocity (meters/sec) in local vehicle coordinates

        public RfVec3 LocalAccel; // acceleration (meters/sec^2) in local vehicle coordinates

        // Orientation and derivatives
        public RfVec3 OriX
            ; // top row of orientation matrix (also converts local vehicle vectors into world X using dot product)

        public RfVec3 OriY
            ; // mid row of orientation matrix (also converts local vehicle vectors into world Y using dot product)

        public RfVec3 OriZ
            ; // bot row of orientation matrix (also converts local vehicle vectors into world Z using dot product)

        public RfVec3 LocalRot; // rotation (radians/sec) in local vehicle coordinates

        public RfVec3 LocalRotAccel; // rotational acceleration (radians/sec^2) in local vehicle coordinates

        public float Speed; // meters/sec

        // Vehicle status
        public int Gear; // -1=reverse, 0=neutral, 1+=forward gears

        public float EngineRPM; // engine RPM

        public float EngineWaterTemp; // Celsius

        public float EngineOilTemp; // Celsius

        public float ClutchRPM; // clutch RPM

        // Driver input
        public float UnfilteredThrottle; // ranges  0.0-1.0

        public float UnfilteredBrake; // ranges  0.0-1.0

        public float UnfilteredSteering; // ranges -1.0-1.0 (left to right)

        public float UnfilteredClutch; // ranges  0.0-1.0

        // Misc
        public float SteeringArmForce; // force on steering arms

        // state/damage info
        public float Fuel; // amount of fuel (liters)

        public float EngineMaxRPM; // rev limit

        public byte ScheduledStops; // number of scheduled pitstops

        public byte Overheating; // whether overheating icon is shown

        public byte Detached; // whether any parts (besides wheels) have been detached

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] DentSeverity; // dent severity at 8 locations around the car (0=none, 1=some, 2=more)

        public float LastImpactET; // time of last impact

        public float LastImpactMagnitude; // magnitude of last impact

        public RfVec3 LastImpactPos; // location of last impact

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public RfWheel[] Wheel; // wheel info (front left, front right, rear left, rear right)

        // scoring info only updates twice per second (values interpolated when deltaTime > 0)!
        public int Session; // current session

        public float CurrentET; // current time

        public float EndET; // ending time

        public int MaxLaps; // maximum laps

        public float LapDist; // distance around track

        public int NumVehicles; // current number of vehicles

        public byte GamePhase;

        public sbyte YellowFlagState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] SectorFlag; // whether there are any local yellows at the moment in each sector (not sure if sector 0 is first or last, so test)

        public byte StartLight; // start light frame (number depends on track)

        public byte NumRedLights; // number of red lights in start sequence

        public byte InRealtime; // in realtime as opposed to at the monitor

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] PlayerName; // player name (including possible multiplayer override)

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] PlrFileName; // may be encoded to be a legal filename

        // weather
        public float AmbientTemp; // temperature (Celsius)

        public float TrackTemp; // temperature (Celsius)

        public RfVec3 Wind; // wind speed

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public RfVehicleInfo[] Vehicle; // array of vehicle scoring info's
    }
}