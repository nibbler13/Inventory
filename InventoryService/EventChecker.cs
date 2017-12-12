using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;

namespace InventoryService {
	class EventChecker {
		private FBClient fbClient;
		private UInt64 lastId; 
		private string misSqlQueryLastId;
		private string misSqlQueryNewRecords;
		private Base1CClient base1CClient;
		private string base1CSqlQueryTechnoMap;

		public EventChecker() {
			string misDbAddress = ConfigurationManager.AppSettings["MisDbAddress"];
			string misDbName = ConfigurationManager.AppSettings["MisDbName"];
			string misDbUser = ConfigurationManager.AppSettings["MisDbUser"];
			string misDbPassword = ConfigurationManager.AppSettings["MisDbPassword"];
			misSqlQueryLastId = ConfigurationManager.AppSettings["MisDbSqlQueryLastId"];
			misSqlQueryNewRecords = ConfigurationManager.AppSettings["MisDbSqlQueryNewRecords"];

			fbClient = new FBClient(
				misDbAddress,
				misDbName,
				misDbUser,
				misDbPassword);

			UpdateLastIdValue();
			LoggingSystem.LogMessageToFile("lastId: " + lastId);

			string base1CAddress = ConfigurationManager.AppSettings["Base1CAddress"];
			string base1CBaseName = ConfigurationManager.AppSettings["Base1CBaseName"];
			string base1CUserId = ConfigurationManager.AppSettings["Base1CUserId"];
			string base1CPassword = ConfigurationManager.AppSettings["Base1CPassword"];
			base1CSqlQueryTechnoMap = ConfigurationManager.AppSettings["Base1CSqlQueryTechnoMap"];

			base1CClient = new Base1CClient(
				base1CAddress,
				base1CBaseName,
				base1CUserId,
				base1CPassword);
		}

		private void UpdateLastIdValue() {
			DataTable lastIdTable = fbClient.GetDataTable(misSqlQueryLastId, new Dictionary<string, object>());

			lastId = 0;

			if (lastIdTable.Rows.Count == 0)
				return;

			try {
				lastId = Convert.ToUInt64(lastIdTable.Rows[0][0]);
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
			}
		}

		public List<EventDataType> GetNewEvents() {
			List<EventDataType> events = new List<EventDataType>();

			DataTable eventTable = fbClient.GetDataTable(misSqlQueryNewRecords, new Dictionary<string, object>() { { "@id", lastId } });
			if (eventTable.Rows.Count == 0) {
				//LoggingSystem.LogMessageToFile("GetNewEvents: returned empty table");
				return events;
			}

			if (eventTable.Rows.Count == 1) {
				try {
					if (eventTable.Rows[0][0].ToString().Equals("-1")) {
						//LoggingSystem.LogMessageToFile("GetNewEvents: no new events");
						return events;
					}
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			Dictionary<string, EventDataType> orders = new Dictionary<string, EventDataType>();
			for (int i = 0; i < eventTable.Rows.Count; i++) {
				try {
					if (i == 0) {
						lastId = Convert.ToUInt64(eventTable.Rows[i][0]);
						continue;
					}

					string orderNo = eventTable.Rows[i]["ORDERNO"].ToString();
					string ipAddress = eventTable.Rows[i]["IPADDRESS"].ToString();
					string pid = eventTable.Rows[i]["PID"].ToString();
					string schCode = eventTable.Rows[i]["SCHCODE"].ToString();
					double schCount = Convert.ToDouble(eventTable.Rows[i]["SCHCOUNT"]);
					string schName = eventTable.Rows[i]["SCHNAME"].ToString();
					string doctorName = eventTable.Rows[i]["DOCTORNAME"].ToString();
					string clientName = eventTable.Rows[i]["CLIENTNAME"].ToString();
					string orderCreateDate = eventTable.Rows[i]["OCREATEDATE"].ToString();

					DataTable serviceTechnoMap = base1CClient.GetDataTable(
						base1CSqlQueryTechnoMap,
						new Dictionary<string, object>() { { "@id", schCode } });

					if (serviceTechnoMap.Rows.Count == 0) {
						Console.WriteLine("GetTechnoMap: no data for: " + schCode + " | " + schName);
						//
						//continue;
						//
					}

					List<EventDataType.Service.Material> materials = new List<EventDataType.Service.Material>();
					foreach (DataRow row in serviceTechnoMap.Rows) {
						string idUslugi = row["IdUslugi"].ToString();
						string codUslugiV1C = row["CodUslugiV1C"].ToString();
						string naimenovanie = row["Naimenovanie"].ToString();
						double kol = Convert.ToDouble(row["Kol"]);
						string ed = row["Ed"].ToString();

						EventDataType.Service.Material material = new EventDataType.Service.Material() {
							IdMis = idUslugi,
							Id1C = codUslugiV1C,
							Name = naimenovanie,
							Count = kol,
							Unit = ed
						};

						materials.Add(material);
					}

					materials = materials.OrderBy(m => m.Name).ToList();

					EventDataType.Service service = new EventDataType.Service() {
						Id = schCode,
						Count = schCount,
						Name = schName,
						Materials = materials
					};

					if (orders.ContainsKey(orderNo)) {
						orders[orderNo].Services.Add(service);
						continue;
					}

					EventDataType eventData = new EventDataType() {
						IpAddress = ipAddress,
						MisProcessPid = pid,
						CreateDate = orderCreateDate,
						ClientName = clientName,
						DoctorName = doctorName,
						OrderId = orderNo
					};

					eventData.Services.Add(service);
					orders.Add(orderNo, eventData);
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			events = orders.Values.ToList();

			return events;
		}

	}
}
