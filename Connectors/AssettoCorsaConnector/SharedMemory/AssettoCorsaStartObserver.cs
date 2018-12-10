﻿namespace SecondMonitor.AssettoCorsaConnector.SharedMemory
{
    using System;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    public class AssettoCorsaStartObserver
    {
        private enum StartState
        {
            Countdown, StartSequence, Started, StartCompleted, StartRestartTimeout
        }

        private StartState _startState;
        private SimulatorDataSet _lastDataSet;

        private TimeSpan _restartTimeoutEnd;

        public AssettoCorsaStartObserver()
        {
            ResetStartState();
        }

        public void Observe(SimulatorDataSet dataSet)
        {
            if (!ShouldObserve(dataSet))
            {
                _lastDataSet = dataSet;
                return;
            }

            CheckAndAdvanceState(dataSet);


            _lastDataSet = dataSet;
        }

        private void CheckAndAdvanceState(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionType != SessionType.Race && _startState != StartState.Countdown)
            {
                _startState = StartState.Countdown;
                return;
            }

            switch (_startState)
            {
                case StartState.Countdown:
                    CheckAndAdvanceCountdown(dataSet);
                    return;
                case StartState.StartSequence:
                    CheckAndAdvanceStartSequence(dataSet);
                    return;
                case StartState.Started:
                    CheckAndAdvanceStarted(dataSet);
                    return;
                case StartState.StartCompleted:
                    CheckAndAdvanceStartCompleted(dataSet);
                    return;
                case StartState.StartRestartTimeout:
                    CheckAndAdvanceStartRestartTimeout(dataSet);
                    return;
            }

        }

        private void CheckAndAdvanceCountdown(SimulatorDataSet dataSet)
        {
            if ((dataSet.PlayerInfo != null && !dataSet.PlayerInfo.InPits) || dataSet.LeaderInfo.Speed > Velocity.Zero)
            {
                _startState = StartState.StartSequence;
            }
        }

        private void CheckAndAdvanceStartSequence(SimulatorDataSet dataSet)
        {
            if (dataSet.PlayerInfo != null && dataSet.PlayerInfo.InPits)
            {
                _startState = StartState.Countdown;
                dataSet.SessionInfo.SessionType = SessionType.Na;
            }

            if (dataSet.LeaderInfo.Speed > Velocity.FromKph(2))
            {
                _startState = StartState.Started;
                dataSet.SessionInfo.SessionType = SessionType.Na;
            }
        }

        private void CheckAndAdvanceStarted(SimulatorDataSet dataSet)
        {
            if (dataSet.PlayerInfo != null && dataSet.PlayerInfo.InPits)
            {
                _startState = StartState.Countdown;
                dataSet.SessionInfo.SessionType = SessionType.Na;
            }

            if (dataSet.LeaderInfo.TotalDistance > 500)
            {
                _startState = StartState.StartCompleted;
            }

        }

        private void CheckAndAdvanceStartCompleted(SimulatorDataSet dataSet)
        {
            if (dataSet.LeaderInfo.TotalDistance < 400)
            {
                _startState = StartState.StartRestartTimeout;
                _restartTimeoutEnd = dataSet.SessionInfo.SessionTime.Add(TimeSpan.FromSeconds(2));
            }

        }

        private void CheckAndAdvanceStartRestartTimeout(SimulatorDataSet dataSet)
        {
            if (dataSet.LeaderInfo.TotalDistance < 400 && dataSet.SessionInfo.SessionTime > _restartTimeoutEnd)
            {
                _startState = StartState.Countdown;
                dataSet.SessionInfo.SessionType = SessionType.Na;
            }

            if (dataSet.LeaderInfo.CompletedLaps > 1)
            {
                _startState = StartState.StartCompleted;
            }

        }

        private bool ShouldObserve(SimulatorDataSet dataSet)
        {
            return !(_lastDataSet == null || (dataSet.SessionInfo.SessionType != SessionType.Race && _startState == StartState.Countdown));
        }

        private void ResetStartState()
        {
            _startState = StartState.Countdown;
        }
    }
}