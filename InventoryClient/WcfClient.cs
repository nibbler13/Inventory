using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InventoryClient {
	class WcfClient {
		private delegate void HandleInventoryCallback(object sender, EventArgs e);
		private InventoryService.WcfServiceClient client;

		public WcfClient() {
			LoggingSystem.LogMessageToFile("WcfClient constructor");
			RegisterClient();
		}

		private void RegisterClient() {
			if (client != null) {
				client.Abort();
				client = null;
			}

			InventoryCallback cb = new InventoryCallback();
			cb.SetHandler(HandleNotification);

			InstanceContext context = new InstanceContext(cb);
			client = new InventoryService.WcfServiceClient(context);

			string userName = Environment.UserName;
			IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			foreach (IPAddress address in hostAddresses)
				try {
					client.RegisterClient(userName + "@" + address.ToString());
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
		}

		public void HandleNotification(object sender, EventArgs e) {
			try {
				InventoryService.EventDataType eventData = (InventoryService.EventDataType)sender;
				
				WindowInventory windowInventory = new WindowInventory(eventData);
				windowInventory.Show();
			} catch (Exception ex) {
				LoggingSystem.LogMessageToFile(ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
	}
}
