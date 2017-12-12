using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryClient {
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application {
		private TaskbarIcon notifyIcon;
		private WcfClient wcfClient;

		protected override void OnStartup(StartupEventArgs e) {
			LoggingSystem.LogMessageToFile("App OnStartup");
			base.OnStartup(e);
			notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
			wcfClient = new WcfClient();
		}

		protected override void OnExit(ExitEventArgs e) {
			LoggingSystem.LogMessageToFile("App OnExit");
			notifyIcon.Dispose();
			base.OnExit(e);
		}
	}
}
