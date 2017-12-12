using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService {
	static class Program {
		public class InventoryService : ServiceBase {
			public InventoryService() {
				ServiceName = "InventoryService";
			}

			protected override void OnStart(string[] args) {
				Program.Start(args);
			}

			protected override void OnStop() {
				Program.Stop();
			}
		}

		public static ServiceHost serviceHost = null;

		static void Main(string[] args) {
			if (!Environment.UserInteractive)
				using (InventoryService inventoryService = new InventoryService())
					ServiceBase.Run(inventoryService);
			else {
				Start(args);

				Console.WriteLine("Press any key to stop...");
				Console.ReadKey(true);

				Stop();
			}
		}

		private static void Start(string[] args) {
			LoggingSystem.LogMessageToFile("------------------Start------------------");

			if (serviceHost != null)
				serviceHost.Close();

			serviceHost = new ServiceHost(typeof(WcfServer));
			try {
				serviceHost.Open();
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
			}
		}

		private static void Stop() {
			LoggingSystem.LogMessageToFile("------------------Stop------------------");

			if (serviceHost != null) {
				serviceHost.Close();
				serviceHost = null;
			}
		}
	}
}
