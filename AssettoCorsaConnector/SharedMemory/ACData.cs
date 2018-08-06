namespace SecondMonitor.AssettoCorsaConnector.SharedMemory
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;


    /**
     * Based on ACDate reader from CrewChief
     * https://github.com/mrbelowski/CrewChiefV4
     */
    public enum AcStatus
    {
        AC_OFF = 0,
        AC_REPLAY = 1,
        AC_LIVE = 2,
        AC_PAUSE = 3
    }

    public enum AcSessionType
    {
        AC_UNKNOWN = -1,
        AC_PRACTICE = 0,
        AC_QUALIFY = 1,
        AC_RACE = 2,
        AC_HOT_LAP = 3,
        AC_TIME_ATTACK = 4,
        AC_DRIFT = 5,
        AC_DRAG = 6,
    }

    public enum AcFlagType
    {
        AC_NO_FLAG = 0,
        AC_BLUE_FLAG = 1,
        AC_YELLOW_FLAG = 2,
        AC_BLACK_FLAG = 3,
        AC_WHITE_FLAG = 4,
        AC_CHECKERED_FLAG = 5,
        AC_PENALTY_FLAG = 6,
    }
    public enum AcWheels
    {
        FL = 0,
        FR = 1,
        RL = 2,
        RR = 3,
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    public struct SPageFilePhysics
    {
        public int packetId;
        public float gas;
        public float brake;
        public float fuel;
        public int gear;
        public int rpms;
        public float steerAngle;
        public float speedKmh;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] velocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] accG;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelSlip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelLoad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelsPressure;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelAngularSpeed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreWear;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreDirtyLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreCoreTemperature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] camberRAD;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionTravel;

        public float drs;
        public float tc;
        public float heading;
        public float pitch;
        public float roll;
        public float cgHeight;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public float[] carDamage;
        public int numberOfTyresOut;
        public int pitLimiterOn;
        public float abs;
        public float kersCharge;
        public float kersInput;
        public int autoShifterOn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] rideHeight;
        public float turboBoost;
        public float ballast;
        public float airDensity;
        public float airTemp;
        public float roadTemp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] localAngularVel;
        public float finalFF;
        public float performanceMeter;

        public int engineBrake;
        public int ersRecoveryLevel;
        public int ersPowerLevel;
        public int ersHeatCharging;
        public int ersIsCharging;
        public float kersCurrentKJ;

        public int drsAvailable;
        public int drsEnabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] brakeTemp;
        public float clutch;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTempI;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTempM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTempO;
        public int isAIControlled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public AcsVec3[] tyreContactPoint;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public AcsVec3[] tyreContactNormal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public AcsVec3[] tyreContactHeading;
        public float brakeBias;
        public AcsVec3 localVelocity;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    public struct SPageFileGraphic
    {

        public int packetId;
        public AcStatus status;
        public AcSessionType session;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string currentTime;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string lastTime;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string bestTime;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string split;
        public int completedLaps;
        public int position;
        public int iCurrentTime;
        public int iLastTime;
        public int iBestTime;
        public float sessionTimeLeft;
        public float distanceTraveled;
        public int isInPit;
        public int currentSectorIndex;
        public int lastSectorTime;
        public int numberOfLaps;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string tyreCompound;

        public float replayTimeMultiplier;
        public float normalizedCarPosition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] carCoordinates;
        public float penaltyTime;
        public AcFlagType flag;
        public int idealLineOn;
        public int isInPitLane;

        public float surfaceGrip;
        public int MandatoryPitDone;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    public struct SPageFileStatic
    {
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string smVersion;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string acVersion;

        // session static info
        public int numberOfSessions;
        public int numCars;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string carModel;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string track;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string playerName;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string playerSurname;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string playerNick;
        public int sectorCount;

        // car static info
        public float maxTorque;
        public float maxPower;
        public int maxRpm;
        public float maxFuel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionMaxTravel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreRadius;
        public float maxTurboBoost;

        public float deprecated_1;
        public float deprecated_2;

        public int penaltiesEnabled;

        public float aidFuelRate;
        public float aidTireRate;
        public float aidMechanicalDamage;
        public int aidAllowTyreBlankets;
        public float aidStability;
        public int aidAutoClutch;
        public int aidAutoBlip;

        public int hasDRS;
        public int hasERS;
        public int hasKERS;
        public float kersMaxJ;
        public int engineBrakeSettingsCount;
        public int ersPowerControllerCount;

        public float trackSPlineLength;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string trackConfiguration;
        public float ersMaxJ;
        public int isTimedRace;
        public int hasExtraLap;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string carSkin;
        public int reversedGridPositions;
        public int PitWindowStart;
        public int PitWindowEnd;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    public struct AcsVec3
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    public struct AcsVehicleInfo
    {
        public int carId;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] driverName;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] carModel;
        public float speedMS;
        public int bestLapMS;
        public int lapCount;
        public int currentLapInvalid;
        public int currentLapTimeMS;
        public int lastLapTimeMS;
        public AcsVec3 worldPosition;
        public int isCarInPitlane;
        public int isCarInPit;
        public int carLeaderboardPosition;
        public int carRealTimeLeaderboardPosition;
        public float spLineLength;
        public int isConnected;
        public int finishedStatus;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    public struct SPageFileSecondMonitor
    {
        public int numVehicles;
        public int focusVehicle;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] serverName;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
        public AcsVehicleInfo[] vehicle;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] acInstallPath;
        public int isInternalMemoryModuleLoaded;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] pluginVersion;
    }
}