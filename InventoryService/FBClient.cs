using System;
using System.Data;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;

namespace InventoryService {
    class FBClient {
        private FbConnection connection;

		public FBClient(string ipAddress, string baseName, string user, string pass) {
			LoggingSystem.LogMessageToFile("Создание подключения к базе FB: " + 
				ipAddress + ":" + baseName);

			FbConnectionStringBuilder cs = new FbConnectionStringBuilder {
				DataSource = ipAddress,
				Database = baseName,
				UserID = user,
				Password = pass,
				Charset = "NONE",
				Pooling = false
			};

			connection = new FbConnection(cs.ToString());
			IsConnectionOpened();
		}

		private bool IsConnectionOpened() {
			if (connection.State != ConnectionState.Open) {
				try {
					connection.Open();
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			return connection.State == ConnectionState.Open;
		}

		public DataTable GetDataTable(string query, Dictionary<string, object> parameters) {
			DataTable dataTable = new DataTable();

			if (!IsConnectionOpened())
				return dataTable;

			try {
				FbCommand command = new FbCommand(query, connection);

				if (parameters.Count > 0) {
					foreach (KeyValuePair<string, object> parameter in parameters)
						command.Parameters.AddWithValue(parameter.Key, parameter.Value);
				}

				FbDataAdapter fbDataAdapter = new FbDataAdapter(command);
				fbDataAdapter.Fill(dataTable);
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile("GetDataTable exception: " + query + 
					Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
				connection.Close();
			}

			return dataTable;
		}

		public bool ExecuteUpdateQuery(string query, Dictionary<string, object> parameters) {
			bool updated = false;

			if (!IsConnectionOpened())
				return updated;

			try {
				FbCommand update = new FbCommand(query, connection);

				if (parameters.Count > 0) {
					foreach (KeyValuePair<string, object> parameter in parameters)
						update.Parameters.AddWithValue(parameter.Key, parameter.Value);
				}

				updated = update.ExecuteNonQuery() > 0 ? true : false;
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile("ExecuteUpdateQuery exception: " + query +
					Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
				connection.Close();
			}

			return updated;
		}
	}
}
