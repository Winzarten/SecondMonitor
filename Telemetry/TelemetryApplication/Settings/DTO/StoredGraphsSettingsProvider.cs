namespace SecondMonitor.Telemetry.TelemetryApplication.Settings.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels.GraphPanel;

    public class StoredGraphsSettingsProvider : IGraphsSettingsProvider
    {
        private readonly IGraphsSettingsProvider _backupProvider;
        private readonly ITelemetrySettingsRepository _telemetrySettingsRepository;
        private Lazy<TelemetrySettingsDto> _graphSettingsLazy;

        public StoredGraphsSettingsProvider(IGraphsSettingsProvider backupProvider, ITelemetrySettingsRepository telemetrySettingsRepository)
        {
            _backupProvider = backupProvider;
            _telemetrySettingsRepository = telemetrySettingsRepository;
            RefreshGraphSettings();
        }

        public TelemetrySettingsDto TelemetrySettings => _graphSettingsLazy.Value;

        public void ForceSettingsReload()
        {
            RefreshGraphSettings();
        }

        public bool IsGraphViewModelLeft(IGraphViewModel graphViewModel)
        {
            return GetGraphSettings(graphViewModel).GraphLocation == GraphLocationKind.LeftPanel;
        }

        public bool IsGraphViewModelRight(IGraphViewModel graphViewModel)
        {
            return GetGraphSettings(graphViewModel).GraphLocation == GraphLocationKind.RightPanel;
        }

        public int GetGraphPriority(IGraphViewModel graphViewModel)
        {
            return GetGraphSettings(graphViewModel).GraphPriority;
        }

        private GraphSettingsDto GetGraphSettings(IGraphViewModel graphViewModel)
        {
            GraphSettingsDto graphSettings = TelemetrySettings.GraphSettings.FirstOrDefault(x => x.Title == graphViewModel.Title);
            if (graphSettings == null)
            {
                graphSettings = new GraphSettingsDto()
                {
                    Title = graphViewModel.Title,
                    GraphLocation = _backupProvider.IsGraphViewModelLeft(graphViewModel) ? GraphLocationKind.LeftPanel : GraphLocationKind.RightPanel,
                    GraphPriority = _backupProvider.GetGraphPriority(graphViewModel)
                };
                TelemetrySettings.GraphSettings.Add(graphSettings);
                _telemetrySettingsRepository.SaveTelemetrySettings(TelemetrySettings);
            }

            return graphSettings;
        }

        private void RefreshGraphSettings()
        {
            _graphSettingsLazy = new Lazy<TelemetrySettingsDto>(LoadTelemetrySettings);
        }

        private TelemetrySettingsDto LoadTelemetrySettings()
        {
            return _telemetrySettingsRepository.TryLoadTelemetrySettings(out TelemetrySettingsDto telemetrySettings) ? telemetrySettings : new TelemetrySettingsDto()
            {
                GraphSettings = new List<GraphSettingsDto>()
            };
        }
    }
}