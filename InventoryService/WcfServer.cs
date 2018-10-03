using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService {
	[ServiceContract(CallbackContract = typeof(IWcfServiceCallback))]
	public interface IWcfService {
		[OperationContract(IsOneWay = true)]
		void RegisterClient(string userIdentifier);

		[OperationContract(IsOneWay = true)]
		void SendResult(EventDataType )
	}

	public interface IWcfServiceCallback {
		[OperationContract(IsOneWay = true)]
		void NotificateClient(EventDataType eventData);
	}

	[DataContract()]
	public class EventDataType {
		[DataMember]
		public string IpAddress { get; set; }

		[DataMember]
		public string MisProcessPid { get; set; }

		[DataMember]
		public string CreateDate { get; set; }

		[DataMember]
		public string ClientName { get; set; }

		[DataMember]
		public string DoctorName { get; set; }

		[DataMember]
		public string OrderId { get; set; }

		[DataMember]
		public List<Service> Services { get; set; }

		public EventDataType() {
			IpAddress = string.Empty;
			MisProcessPid = string.Empty;
			CreateDate = string.Empty;
			ClientName = string.Empty;
			DoctorName = string.Empty;
			OrderId = string.Empty;
			Services = new List<Service>();
		}

		[DataContract()]
		public class Service {
			[DataMember]
			public string Id { get; set; }

			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public double Count { get; set; }

			[DataMember]
			public List<Material> Materials { get; set; }

			public Service() {
				Id = string.Empty;
				Name = string.Empty;
				Count = 0;
				Materials = new List<Material>();
			}

			[DataContract()]
			public class Material {
				[DataMember]
				public string IdMis { get; set; }

				[DataMember]
				public string Id1C { get; set; }

				[DataMember]
				public string Name { get; set; }

				[DataMember]
				public double Count { get; set; }

				[DataMember]
				public string Unit { get; set; }

				public Material() {
					IdMis = string.Empty;
					Id1C = string.Empty;
					Name = string.Empty;
					Count = 0;
					Unit = string.Empty;
				}
			}
		}
	}

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	class WcfServer : IWcfService {
		private static Dictionary<string, IWcfServiceCallback> clients =
			new Dictionary<string, IWcfServiceCallback>();
		private static object locker = new object();
		private EventChecker eventCheck = new EventChecker();

		public WcfServer() {
			LoggingSystem.LogMessageToFile("WcfServer constructor");

			Task.Run(() => {
				while (true) {
					Thread.Sleep(3000);

					List<EventDataType> events = eventCheck.GetNewEvents();
					if (events.Count == 0)
						continue;
					
					foreach (EventDataType eventData in events) {
						LoggingSystem.LogMessageToFile("Notificate to: " + eventData.IpAddress);

						Task.Run(() => {
							bool isSended = false;
							List<string> inactiveClients = new List<string>();

							foreach (KeyValuePair<string, IWcfServiceCallback> client in clients) {
								if (!client.Key.Contains(eventData.IpAddress))
									continue;

								try {
									client.Value.NotificateClient(eventData);
									isSended = true;
								} catch (Exception e) {
									LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
									inactiveClients.Add(client.Key);
								}
							}

							LoggingSystem.LogMessageToFile("isSended: " + isSended);

							foreach (string clientKey in inactiveClients)
								clients.Remove(clientKey);
						});
					}
				}
			});
		}

		public void RegisterClient(string userIdentifier) {
			if (string.IsNullOrEmpty(userIdentifier))
				return;

			LoggingSystem.LogMessageToFile("RegisterClient: " + userIdentifier);

			try {
				IWcfServiceCallback callback = 
					OperationContext.Current.GetCallbackChannel<IWcfServiceCallback>();
				lock (locker) {
					if (clients.Keys.Contains(userIdentifier))
						clients.Remove(userIdentifier);
					clients.Add(userIdentifier, callback);
				}
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
			}
		}
	}
}