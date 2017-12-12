using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryClient.InventoryService;
using System.Threading;
using System.ComponentModel;

namespace InventoryClient {
	class InventoryCallback : InventoryService.IWcfServiceCallback {
		private SynchronizationContext syncContext = AsyncOperationManager.SynchronizationContext;
		private EventHandler inventoryCallbackHandler;

		public void SetHandler (EventHandler handler) {
			inventoryCallbackHandler = handler;
		}

		public void NotificateClient(EventDataType eventData) {
			syncContext.Post(new SendOrPostCallback(OnNotificate), eventData);
		}

		private void OnNotificate(object eventData) {
			inventoryCallbackHandler.Invoke(eventData, null);
		}
	}
}
