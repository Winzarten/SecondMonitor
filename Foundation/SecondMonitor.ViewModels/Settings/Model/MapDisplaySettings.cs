﻿namespace SecondMonitor.ViewModels.Settings.Model
{
    using System;

    [Serializable]
    public class MapDisplaySettings
    {
        public bool AutosScaleDrivers { get; set; } = true;
        public bool KeepMapRatio { get; set; } = true;
        public int MapPointsInterval { get; set; } = 500;
        public bool AlwaysUseCirce { get; set; } = false;
    }
}