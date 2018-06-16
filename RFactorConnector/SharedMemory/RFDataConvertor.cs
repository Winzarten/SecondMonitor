﻿namespace SecondMonitor.RFactorConnector.SharedMemory
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.Extensions;

    internal class RFDataConvertor
    {

        public SimulatorDataSet CreateSimulatorDataSet(RfShared rfData)
        {
            SimulatorDataSet simData = new SimulatorDataSet("RFactor");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.NONE;

            FillSessionInfo(rfData, simData);

            return simData;
        }

        internal void FillSessionInfo(RfShared data, SimulatorDataSet simData)
        {
            // Timing
            simData.SessionInfo.SessionTime = TimeSpan.FromSeconds(data.CurrentET);
            simData.SessionInfo.TrackInfo.LayoutLength = data.LapDist;
            simData.SessionInfo.IsActive = data.InRealtime == 1;
            switch ((RfSessionType)data.Session)
            {
                case RfSessionType.Practice1:
                case RfSessionType.Practice2:
                case RfSessionType.Practice3:
                case RfSessionType.TestDay:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case RfSessionType.Qualification:
                    simData.SessionInfo.SessionType = SessionType.Qualification;
                    break;
                case RfSessionType.WarmUp:
                    simData.SessionInfo.SessionType = SessionType.WarmUp;
                    break;
                case RfSessionType.Race1:
                case RfSessionType.Race2:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                default:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
            }

            switch ((RfGamePhase)data.GamePhase)
            {
                case RfGamePhase.Garage:
                case RfGamePhase.WarmUp:
                case RfGamePhase.GridWalk:
                case RfGamePhase.Formation:
                case RfGamePhase.Countdown:
                    simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                    break;
                case RfGamePhase.SessionStopped:
                case RfGamePhase.GreenFlag:
                case RfGamePhase.FullCourseYellow:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case RfGamePhase.SessionOver:
                    simData.SessionInfo.SessionPhase = SessionPhase.Checkered;
                    break;
            }

            simData.SessionInfo.TrackInfo.TrackName = StringExtensions.FromArray(data.TrackName);
            simData.SessionInfo.TrackInfo.TrackLayoutName = string.Empty;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            /*if (data.SessionTimeRemaining != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining = data.SessionTimeRemaining;
            }
            else if (data.NumberOfLaps != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                simData.SessionInfo.TotalNumberOfLaps = data.NumberOfLaps;
            }*/
        }
    }
}