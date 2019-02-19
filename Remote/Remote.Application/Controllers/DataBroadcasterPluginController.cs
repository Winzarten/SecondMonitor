namespace SecondMonitor.Remote.Application.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Contracts.NInject;
    using PluginManager.Core;
    using PluginManager.GameConnector;
    using PluginsConfiguration.Common.Controller;
    using View;
    using ViewModels;

    public class DataBroadcasterPluginController : ISecondMonitorPlugin
    {
        private IPluginSettingsProvider _pluginSettingsProvider;
        private IBroadCastServerController _broadCastServerController;
        private readonly IServerOverviewViewModel _serverOverviewViewModel;
        private ServerOverviewWindow _serverOverviewWindow;
        private readonly KernelWrapper _kernel;

        public DataBroadcasterPluginController()
        {
            _kernel = new KernelWrapper();
            _serverOverviewViewModel = _kernel.Get <IServerOverviewViewModel>();
            _pluginSettingsProvider = _kernel.Get <IPluginSettingsProvider>();;
        }

        public PluginsManager PluginManager { get; set; }

        public string PluginName => "Data Broadcast Server";

        public bool IsEnabledByDefault => false;

        public bool IsDaemon => false;

        public async void RunPlugin()
        {
            InitializeInjectedProperties();
            InitializeUi();
            await StartChildControllers();
            Subscribe();
        }

        private void InitializeUi()
        {
            _serverOverviewWindow = new ServerOverviewWindow {DataContext = _serverOverviewViewModel};
            _serverOverviewWindow.Closed += ServerOverviewWindowOnClosed;
            _serverOverviewWindow.Show();
            InitializeAvailableIpList();

        }

        private void InitializeAvailableIpList()
        {
            int portNumber = _pluginSettingsProvider.RemoteConfiguration.Port;
            string strHostName = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);
            _serverOverviewViewModel.AvailableIps = ipHostEntry.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(y => $"{y.ToString()}:{portNumber}").ToList();
        }

        private async void ServerOverviewWindowOnClosed(object sender, EventArgs e)
        {
            await StopChildControllers();
            await PluginManager.DeletePlugin(this, new List<Exception>());
        }

        private async Task StartChildControllers()
        {
            await _broadCastServerController.StartControllerAsync();
        }

        private async Task StopChildControllers()
        {
            await _broadCastServerController.StopControllerAsync();
        }

        private void InitializeInjectedProperties()
        {
            _pluginSettingsProvider = _kernel.Get<IPluginSettingsProvider>();
            _broadCastServerController = _kernel.Get<IBroadCastServerController>();
        }

        private void Subscribe()
        {
            PluginManager.DataLoaded += PluginManagerOnDataLoaded;
            PluginManager.SessionStarted += PluginManagerOnSessionStarted;
        }

        private void PluginManagerOnSessionStarted(object sender, DataEventArgs e)
        {

        }

        private void PluginManagerOnDataLoaded(object sender, DataEventArgs e)
        {

        }
    }
}