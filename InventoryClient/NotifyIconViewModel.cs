using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InventoryClient {
	public class NotifyIconViewModel {
		/// <summary>
		/// Shows a window, if none is already open.
		/// </summary>
		public ICommand AboutWindowCommand {
			get {
				return new DelegateCommand {
					CanExecuteFunc = () => Application.Current.MainWindow == null,
					CommandAction = () => {
						Application.Current.MainWindow = new WindowAbout();
						Application.Current.MainWindow.Show();
					}
				};
			}
		}
	}


	/// <summary>
	/// Simplistic delegate command for the demo.
	/// </summary>
	public class DelegateCommand : ICommand {
		public Action CommandAction { get; set; }
		public Func<bool> CanExecuteFunc { get; set; }

		public void Execute(object parameter) {
			CommandAction();
		}

		public bool CanExecute(object parameter) {
			return CanExecuteFunc == null || CanExecuteFunc();
		}

		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}