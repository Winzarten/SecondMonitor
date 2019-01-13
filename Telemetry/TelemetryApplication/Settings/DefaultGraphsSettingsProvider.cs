﻿namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using System.Linq;
    using ViewModels.GraphPanel;

    public class DefaultGraphsSettingsProvider : IGraphsSettingsProvider
    {
        private static readonly string[] LeftGraphs = { "Speed", "Engine RPM", "Gear", "Lateral Acceleration", "Horizontal Acceleration", "Brake Temperature", "Tyre Pressures", "Left Front Tyre Temperatures", "Right Front Tyre Temperatures", "Left Rear Tyre Temperatures", "Right Rear Tyre Temperatures", "Tyre Rps", "Suspension Travel", "Ride Height - Wheels" };
        private static readonly string[] RightGraphs = { "Lap Time", "Steering Angle", "Throttle", "Brake", "Clutch", "Ride Height - Chassis" };

        public void ForceSettingsReload()
        {
            //Nothing to do
        }

        public bool IsGraphViewModelLeft(IGraphViewModel graphViewModel)
        {
            return LeftGraphs.Contains(graphViewModel.Title);
        }

        public bool IsGraphViewModelRight(IGraphViewModel graphViewModel)
        {
            return RightGraphs.Contains(graphViewModel.Title);
        }

        public int GetGraphPriority(IGraphViewModel graphViewModel) => 1;
    }
}