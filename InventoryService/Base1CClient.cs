using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService {
	class Base1CClient {
		private SqlConnection sqlConnection;

		public Base1CClient(string ipAddress, string baseName, string userId, string pass) {
			sqlConnection = new SqlConnection(
				"Server = " + ipAddress + ";" +
				"Database = " + baseName + ";" +
				"User Id = " + userId + ";" +
				"Password = " + pass + ";");

			IsConnectionOpened();
		}

		private bool IsConnectionOpened() {
			if (sqlConnection.State != ConnectionState.Open) {
				try {
					sqlConnection.Open();
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			return sqlConnection.State == ConnectionState.Open;
		}

		public DataTable GetDataTable(string query, Dictionary<string, object> parameters) {
			DataTable dataTable = new DataTable();

			if (!IsConnectionOpened())
				return dataTable;

			try {
				SqlCommand command = new SqlCommand(query);
				command.CommandType = System.Data.CommandType.Text;
				command.Connection = sqlConnection;

				if (parameters.Count > 0)
					foreach (KeyValuePair<string, object> parameter in parameters)
						command.Parameters.AddWithValue(parameter.Key, parameter.Value);

				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
				sqlDataAdapter.Fill(dataTable);
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile("GetDataTable exception: " + query +
					Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
			}

			return dataTable;
		}
	}
}
