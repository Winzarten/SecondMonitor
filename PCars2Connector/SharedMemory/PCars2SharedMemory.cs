// ReSharper disable InconsistentNaming
namespace SecondMonitor.PCars2Connector.SharedMemory
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;




    // WheelIndex
    public enum WheelIndex
    {
        TyreFrontLeft = 0,
        TyreFrontRight,
        TyreRearLeft,
        TyreRearRight,
        TyreMax
    };

    // Vector
    public enum VectorAxis

    {
    VecX = 0,
    VecY,
    VecZ,
    VecMax
    };

    // (Type#1) GameState (to be used with 'mSessionType')
    public enum GameState

    {
    GameExited = 0,

    GameFrontEnd,

    GameInGamePlaying,

    GameInGamePaused,

    GameInGameInMenuTimeTicking,

    GameInGameRestarting,

    GameInGameReplay,

    GameFrontEndReplay,

    //-------------
    GameMax
    };

    // (Type#2) Session state (to be used with 'mSessionState')
    public enum PCars2SessionType

    {
    SessionInvalid = 0,

    SessionPractice,

    SessionTest,

    SessionQualify,

    SessionFormationLap,

    SessionRace,

    SessionTimeAttack,

    //-------------
    SessionMax
    };

    // (Type#3) RaceState (to be used with 'mRaceState' and 'mRaceStates')
    public enum RaceState

    {
    RaceStateInvalid,

    RaceStateNotStarted,

    RaceStateRacing,

    RaceStateFinished,

    RaceStateDisqualified,

    RaceStateRetired,

    RaceStateDnf,

    //-------------
    RaceStateMax
    };

    // (Type#5) Flag Colours (to be used with 'mHighestFlagColour')
    public enum HighestFlagColor

    {
    FlagColourNone = 0, // Not used for actual flags, only for some query functions

    FlagColourGreen, // End of danger zone, or race started

    FlagColourBlue, // Faster car wants to overtake the participant

    FlagColourWhiteSlowCar, // Slow car in area

    FlagColourWhiteFinalLap, // Final Lap

    FlagColourRed, // Huge collisions where one or more cars become wrecked and block the track

    FlagColourYellow, // Danger on the racing surface itself

    FlagColourDoubleYellow, // Danger that wholly or partly blocks the racing surface

    FlagColourBlackAndWhite, // Unsportsmanlike conduct

    FlagColourBlackOrangeCircle, // Mechanical Failure

    FlagColourBlack, // Participant disqualified

    FlagColourChequered, // Chequered flag

    //-------------
    FlagColourMax
    };

    // (Type#6) Flag Reason (to be used with 'mHighestFlagReason')
    public enum Reason

    {
    FlagReasonNone = 0,

    FlagReasonSoloCrash,

    FlagReasonVehicleCrash,

    FlagReasonVehicleObstruction,

    //-------------
    FlagReasonMax
    };

    // (Type#7) Pit Mode (to be used with 'mPitMode')
    public enum PitMode

    {
    PitModeNone = 0,

    PitModeDrivingIntoPits,

    PitModeInPit,

    PitModeDrivingOutOfPits,

    PitModeInGarage,

    PitModeDrivingOutOfGarage,

    //-------------
    PitModeMax
    };

    // (Type#8) Pit Stop Schedule (to be used with 'mPitSchedule')
    public enum PitStopSchedule

    {
    PitScheduleNone = 0, // Nothing scheduled

    PitSchedulePlayerRequested, // Used for standard pit sequence - requested by player

    PitScheduleEngineerRequested, // Used for standard pit sequence - requested by engineer

    PitScheduleDamageRequested, // Used for standard pit sequence - requested by engineer for damage

    PitScheduleMandatory, // Used for standard pit sequence - requested by engineer from career enforced lap number

    PitScheduleDriveThrough, // Used for drive-through penalty

    PitScheduleStopGo, // Used for stop-go penalty

    PitSchedulePitspotOccupied, // Used for drive-through when pitspot is occupied

    //-------------
    PitScheduleMax
    };

    // (Type#9) Car Flags (to be used with 'mCarFlags')
    public enum CarFlags

    {
    CarHeadlight = (1 << 0),

    CarEngineActive = (1 << 1),

    CarEngineWarning = (1 << 2),

    CarSpeedLimiter = (1 << 3),

    CarAbs = (1 << 4),

    CarHandbrake = (1 << 5),
    };

    // (Type#10) Tyre Flags (to be used with 'mTyreFlags')
    public enum TyreFlags

    {
    TyreAttached = (1 << 0),

    TyreInflated = (1 << 1),

    TyreIsOnGround = (1 << 2),
    };

    // (Type#11) Terrain Materials (to be used with 'mTerrain')
    public enum TerrainType

    {
    TerrainRoad = 0,

    TerrainLowGripRoad,

    TerrainBumpyRoad1,

    TerrainBumpyRoad2,

    TerrainBumpyRoad3,

    TerrainMarbles,

    TerrainGrassyBerms,

    TerrainGrass,

    TerrainGravel,

    TerrainBumpyGravel,

    TerrainRumbleStrips,

    TerrainDrains,

    TerrainTyrewalls,

    TerrainCementwalls,

    TerrainGuardrails,

    TerrainSand,

    TerrainBumpySand,

    TerrainDirt,

    TerrainBumpyDirt,

    TerrainDirtRoad,

    TerrainBumpyDirtRoad,

    TerrainPavement,

    TerrainDirtBank,

    TerrainWood,

    TerrainDryVerge,

    TerrainExitRumbleStrips,

    TerrainGrasscrete,

    TerrainLongGrass,

    TerrainSlopeGrass,

    TerrainCobbles,

    TerrainSandRoad,

    TerrainBakedClay,

    TerrainAstroturf,

    TerrainSnowhalf,

    TerrainSnowfull,

    TerrainDamagedRoad1,

    TerrainTrainTrackRoad,

    TerrainBumpycobbles,

    TerrainAriesOnly,

    TerrainOrionOnly,

    TerrainB1Rumbles,

    TerrainB2Rumbles,

    TerrainRoughSandMedium,

    TerrainRoughSandHeavy,

    TerrainSnowwalls,

    TerrainIceRoad,

    TerrainRunoffRoad,

    TerrainIllegalStrip,

    //-------------
    TerrainMax
    };

    // (Type#12) Crash Damage State  (to be used with 'mCrashState')
    public enum DamageState

    {
    CrashDamageNone = 0,

    CrashDamageOfftrack,

    CrashDamageLargeProp,

    CrashDamageSpinning,

    CrashDamageRolling,

    //-------------
    CrashMax
    }

    [Serializable]
    public struct CarClassNameString
    {
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] classNameByteArray;
    }

    [Serializable]
    public struct ParticipantInfo
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool mIsActive;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mName;                                    // [ string ]

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mWorldPosition;                          // [ UNITS = World Space  X  Y  Z ]

        public float mCurrentLapDistance;                       // [ UNITS = Metres ]   [ RANGE = 0.0f->... ]    [ UNSET = 0.0f ]
        public uint mRacePosition;                              // [ RANGE = 1->... ]   [ UNSET = 0 ]
        public uint mLapsCompleted;                             // [ RANGE = 0->... ]   [ UNSET = 0 ]
        public uint mCurrentLap;                                // [ RANGE = 0->... ]   [ UNSET = 0 ]
        public int mCurrentSector;                             // [ enum (Type#4) Current Sector ]

    }

    [Serializable]
    public struct PCars2SharedMemory
    {

        // Version Number
        public uint mVersion;                           // [ RANGE = 0->... ]
        public uint mBuildVersionNumber;                // [ RANGE = 0->... ]   [ UNSET = 0 ]

        // Game States
        public uint mGameState;                         // [ enum (Type#1) Game state ]
        public uint mSessionState;                      // [ enum (Type#2) Session state ]
        public uint mRaceState;                         // [ enum (Type#3) Race State ]

        // Participant Info
        public int mViewedParticipantIndex;                                  // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        public int mNumParticipants;                                         // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public ParticipantInfo[] mParticipantData;    // [ struct (Type#13) ParticipantInfo struct ]

        // Unfiltered Input
        public float mUnfilteredThrottle;                        // [ RANGE = 0.0f->1.0f ]
        public float mUnfilteredBrake;                           // [ RANGE = 0.0f->1.0f ]
        public float mUnfilteredSteering;                        // [ RANGE = -1.0f->1.0f ]
        public float mUnfilteredClutch;                          // [ RANGE = 0.0f->1.0f ]

        // Vehicle information
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mCarName;                 // [ string ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mCarClassName;            // [ string ]

        // Event information
        public uint mLapsInEvent;                        // [ RANGE = 0->... ]   [ UNSET = 0 ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mTrackLocation;           // [ string ] - untranslated shortened English name
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mTrackVariation;          // [ string ]- untranslated shortened English variation description
        public float mTrackLength;                               // [ UNITS = Metres ]   [ RANGE = 0.0f->... ]    [ UNSET = 0.0f ]

        public int mmfOnly_mNumSectors;                                  // [ RANGE = 0->... ]   [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.I1)]
        public bool mLapInvalidated;                             // [ UNITS = boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        public float mmfOnly_mBestLapTime;                               // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mLastLapTime;                               // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mmfOnly_mCurrentTime;                               // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mSplitTimeAhead;                            // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mSplitTimeBehind;                           // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mSplitTime;                                 // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mEventTimeRemaining;                        // [ UNITS = milli-seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mPersonalFastestLapTime;                    // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mWorldFastestLapTime;                       // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mmfOnly_mCurrentSector1Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mmfOnly_mCurrentSector2Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mmfOnly_mCurrentSector3Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mmfOnly_mFastestSector1Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mmfOnly_mFastestSector2Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mmfOnly_mFastestSector3Time;                        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mPersonalFastestSector1Time;                // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mPersonalFastestSector2Time;                // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mPersonalFastestSector3Time;                // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mWorldFastestSector1Time;                   // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mWorldFastestSector2Time;                   // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        public float mWorldFastestSector3Time;                   // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]

        // Flags
        public uint mHighestFlagColour;                 // [ enum (Type#5) Flag Colour ]
        public uint mHighestFlagReason;                 // [ enum (Type#6) Flag Reason ]

        // Pit Info
        public uint mPitMode;                           // [ enum (Type#7) Pit Mode ]
        public uint mPitSchedule;                       // [ enum (Type#8) Pit Stop Schedule ]

        // Car State
        public uint mCarFlags;                          // [ enum (Type#9) Car Flags ]
        public float mOilTempCelsius;                           // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        public float mOilPressureKPa;                           // [ UNITS = Kilopascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mWaterTempCelsius;                         // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        public float mWaterPressureKPa;                         // [ UNITS = Kilopascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mFuelPressureKPa;                          // [ UNITS = Kilopascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mFuelLevel;                                // [ RANGE = 0.0f->1.0f ]
        public float mFuelCapacity;                             // [ UNITS = Liters ]   [ RANGE = 0.0f->1.0f ]   [ UNSET = 0.0f ]
        public float mSpeed;                                    // [ UNITS = Metres per-second ]   [ RANGE = 0.0f->... ]
        public float mRpm;                                      // [ UNITS = Revolutions per minute ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mMaxRPM;                                   // [ UNITS = Revolutions per minute ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        public float mBrake;                                    // [ RANGE = 0.0f->1.0f ]
        public float mThrottle;                                 // [ RANGE = 0.0f->1.0f ]
        public float mClutch;                                   // [ RANGE = 0.0f->1.0f ]
        public float mSteering;                                 // [ RANGE = -1.0f->1.0f ]
        public int mGear;                                       // [ RANGE = -1 (Reverse)  0 (Neutral)  1 (Gear 1)  2 (Gear 2)  etc... ]   [ UNSET = 0 (Neutral) ]
        public int mNumGears;                                   // [ RANGE = 0->... ]   [ UNSET = -1 ]
        public float mOdometerKM;                               // [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.I1)]
        public bool mAntiLockActive;                            // [ UNITS = boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        public int mLastOpponentCollisionIndex;                 // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        public float mLastOpponentCollisionMagnitude;           // [ RANGE = 0.0f->... ]
        [MarshalAs(UnmanagedType.I1)]
        public bool mBoostActive;                               // [ UNITS = boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        public float mBoostAmount;                              // [ RANGE = 0.0f->100.0f ]

        // Motion & Device Related
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mOrientation;                     // [ UNITS = Euler Angles ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mLocalVelocity;                   // [ UNITS = Metres per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mWorldVelocity;                   // [ UNITS = Metres per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mAngularVelocity;                 // [ UNITS = Radians per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mLocalAcceleration;               // [ UNITS = Metres per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mWorldAcceleration;               // [ UNITS = Metres per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mExtentsCentre;                   // [ UNITS = Local Space  X  Y  Z ]

        // Wheels / Tyres
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] mTyreFlags;               // [ enum (Type#10) Tyre Flags ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] mTerrain;                 // [ enum (Type#11) Terrain Materials ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreY;                          // [ UNITS = Local Space  Y ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreRPS;                        // [ UNITS = Revolutions per second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreSlipSpeed;                  // OBSOLETE, kept for backward compatibility only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreTemp;                       // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreGrip;                       // OBSOLETE, kept for backward compatibility only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreHeightAboveGround;          // [ UNITS = Local Space  Y ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreLateralStiffness;           // OBSOLETE, kept for backward compatibility only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreWear;                       // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mBrakeDamage;                    // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mSuspensionDamage;               // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mBrakeTempCelsius;               // [ UNITS = Celsius ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreTreadTemp;                  // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreLayerTemp;                  // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreCarcassTemp;                // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreRimTemp;                    // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreInternalAirTemp;            // [ UNITS = Kelvin ]

        // Car Damage
        public uint mCrashState;                        // [ enum (Type#12) Crash Damage State ]
        public float mAeroDamage;                               // [ RANGE = 0.0f->1.0f ]
        public float mEngineDamage;                             // [ RANGE = 0.0f->1.0f ]

        // Weather
        public float mAmbientTemperature;                       // [ UNITS = Celsius ]   [ UNSET = 25.0f ]
        public float mTrackTemperature;                         // [ UNITS = Celsius ]   [ UNSET = 30.0f ]
        public float mRainDensity;                              // [ UNITS = How much rain will fall ]   [ RANGE = 0.0f->1.0f ]
        public float mWindSpeed;                                // [ RANGE = 0.0f->100.0f ]   [ UNSET = 2.0f ]
        public float mWindDirectionX;                           // [ UNITS = Normalised Vector X ]
        public float mWindDirectionY;                           // [ UNITS = Normalised Vector Y ]
        // from sunny all the way to light rain = 2 (at start of session - not transition), rain to thunder storm, blizzard, fog = 1.5.
        // Transitions from 2 down to 1.9ish as rain starts.
        // NOTE: this is not in the UDP data for pcars or pcars2 (why?)
        public float mCloudBrightness;                          // [ RANGE = 0.0f->... ]

        //PCars2 additions start, version 8
        // Sequence Number to help slightly with data integrity reads
        public uint mSequenceNumber;          // 0 at the start, incremented at start and end of writing, so odd when Shared Memory is being filled, even when the memory is not being touched

        //Additional car variables
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mWheelLocalPositionY;           // [ UNITS = Local Space  Y ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mSuspensionTravel;              // [ UNITS = meters ] [ RANGE 0.f =>... ]  [ UNSET =  0.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mSuspensionVelocity;            // [ UNITS = Rate of change of pushrod deflection ] [ RANGE 0.f =>... ]  [ UNSET =  0.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mAirPressure;                   // [ UNITS = PSI ]  [ RANGE 0.f =>... ]  [ UNSET =  0.0f ]
        public float mEngineSpeed;                             // [ UNITS = Rad/s ] [UNSET = 0.f ]
        public float mEngineTorque;                            // [ UNITS = Newton Meters] [UNSET = 0.f ] [ RANGE = 0.0f->... ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] mWings;                                // [ RANGE = 0.0f->1.0f ] [UNSET = 0.f ]
        public float mHandBrake;                               // [ RANGE = 0.0f->1.0f ] [UNSET = 0.f ]

        // additional race variables for each participant
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mCurrentSector1Times;        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mCurrentSector2Times;        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mCurrentSector3Times;        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestSector1Times;        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestSector2Times;        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestSector3Times;        // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestLapTimes;            // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mLastLapTimes;               // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mLapsInvalidated;            // [ UNITS = boolean for all participants ]   [ RANGE = false->true ]   [ UNSET = false ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public uint[] mRaceStates;         // [ enum (Type#3) Race State ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public uint[] mPitModes;           // [ enum (Type#7)  Pit Mode ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 3)]
        public float[] mOrientations;      // [ UNITS = Euler Angles ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mSpeeds;                     // [ UNITS = Metres per-second ]   [ RANGE = 0.0f->... ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 64)]
        public byte[] mCarNames; // [ string ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 64)]
        public byte[] mCarClassNames; // [ string ]
                                      /*[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
                                      public pCars2APIParticipantAdditionalDataStruct[] mAdditionalParticipantData;   */   //
                                                                                                                           // additional race variables
        public int mEnforcedPitStopLap;                          // [ UNITS = in which lap there will be a mandatory pitstop] [ RANGE = 0.0f->... ] [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mTranslatedTrackLocation;  // [ string ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] mTranslatedTrackVariation; // [ string ]]

        public float mBrakeBias;                                                                        // [ RANGE = 0.0f->1.0f... ]   [ UNSET = -1.0f ]
        public float mTurboBoostPressure;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] mLFTyreCompoundName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] mRFTyreCompoundName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] mLRTyreCompoundName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] mRRTyreCompoundName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public uint[] mPitSchedules;  // [ enum (Type#7)  Pit Mode ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public uint[] mHighestFlagColours;                 // [ enum (Type#5) Flag Colour ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public uint[] mHighestFlagReasons;                 // [ enum (Type#6) Flag Reason ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public uint[] mNationalities;                      // [ nationality table , SP AND UNSET = 0 ] See nationalities.txt file for details
        public float mSnowDensity;                         // [ UNITS = How much snow will fall ]   [ RANGE = 0.0f->1.0f ], this will be non zero only in Snow season, in other seasons whatever is falling from the sky is reported as rain


        // extra from the UDP data
        public uint mSessionLengthTimeFromGame;  // seconds, 0 => not a timed session
        public byte mJoyPad1;
        public byte mJoyPad2; public byte mDPad;


        // and other stuff that's missing from the MMF:
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mSuspensionRideHeight;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mRideHeight;

        // more per-participant data items not in the shared memory. Or documented.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] participantHighestFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public float[] lastSectorTimes;

        // this is a big byte array of all the car class names being sent via UDP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public CarClassNameString[] carClassNames;

        /*
        // extras from the UDP data - pcars1



        public float[] mWheelLocalPosition;

        public float[] mRideHeight;

        public float[] mSuspensionTravel;

        public float[] mSuspensionVelocity;

        public float[] mAirPressure;

        public float mEngineSpeed;

        public float mEngineTorque;

        public int mEnforcedPitStopLap;

        public Boolean hasNewPositionData;

        public Boolean[] isSameClassAsPlayer;

        public Boolean hasOpponentClassData;

        public float[] mLastSectorData;

        public Boolean[] mLapInvalidatedData;*/
    }

}