﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService {
	[RunInstaller(true)]
	public class ProjectInstaller : Installer {
		private ServiceProcessInstaller process;
		private ServiceInstaller service;

		public ProjectInstaller() {
			process = new ServiceProcessInstaller();
			process.Account = ServiceAccount.LocalSystem;
			service = new ServiceInstaller();
			service.ServiceName = "InventoryService";
			Installers.Add(process);
			Installers.Add(service);
		}
	}
}
