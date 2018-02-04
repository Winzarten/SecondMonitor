namespace SecondMonitor.Timing.Settings.Model
{
    using System;

    [Serializable]
    public class ColumnsSettings
    {
        public ColumnSettings Position { get; set; } = new ColumnSettings { Visible = true, Width = 60 };
        public ColumnSettings Name { get; set; } = new ColumnSettings { Visible = true, Width = 180 };
        public ColumnSettings CarName { get; set; } = new ColumnSettings { Visible = true, Width = 250 };
        public ColumnSettings CompletedLaps { get; set; } = new ColumnSettings { Visible = true, Width = 60 };
        public ColumnSettings LastLapTime { get; set; } = new ColumnSettings { Visible = true, Width = 150 };
        public ColumnSettings Pace { get; set; } = new ColumnSettings { Visible = true, Width = 150 };
        public ColumnSettings BestLap { get; set; } = new ColumnSettings { Visible = true, Width = 180 };
        public ColumnSettings CurrentLapProgressTime { get; set; } = new ColumnSettings { Visible = true, Width = 150 };
        public ColumnSettings LastPitInfo { get; set; } = new ColumnSettings { Visible = true, Width = 180 };
        public ColumnSettings TimeToPlayer { get; set; } = new ColumnSettings { Visible = true, Width = 150 };
        public ColumnSettings TopSpeed { get; set; } = new ColumnSettings { Visible = true, Width = 100 };
        public ColumnSettings Sector1 { get; set; } = new ColumnSettings { Visible = true, Width = 80 };
        public ColumnSettings Sector2 { get; set; } = new ColumnSettings { Visible = true, Width = 80 };
        public ColumnSettings Sector3 { get; set; } = new ColumnSettings { Visible = true, Width = 80 };


    }

    /* <GridViewColumn DisplayMemberBinding="{Binding Position, Mode=OneWay}" Header="#" Width="60"/>
                    <GridViewColumn DisplayMemberBinding="{Binding Name, Mode=OneTime}" Header="Name" Width="180"/>
                    <GridViewColumn DisplayMemberBinding="{Binding CarName, Mode=OneTime}" Header="Car" Width="250"/>
                    <GridViewColumn DisplayMemberBinding="{Binding CompletedLaps, Mode=OneWay}" Header="Laps" Width="60"/>
                    <GridViewColumn DisplayMemberBinding="{Binding LastLapTime, Mode=OneWay}" Header="Last Lap" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding Pace, Mode=OneWay}" Header="Pace" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding BestLap, Mode=OneWay}" Header="Best Lap" Width="180"/>
                    <GridViewColumn DisplayMemberBinding="{Binding CurrentLapProgressTime, Mode=OneWay}" Header="Current Lap" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding LastPitInfo, Mode=OneWay}" Header="Pits" Width="180"/>
                    <GridViewColumn DisplayMemberBinding="{Binding TimeToPlayer, Mode=OneWay}" Header="Gap" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding TopSpeed, Mode=OneWay}" Header="Top Speed" Width="100"/>*/
}
