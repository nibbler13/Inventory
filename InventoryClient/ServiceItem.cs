using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace InventoryClient {
	public class ServiceItem : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public string Name { get; set; }
		public double ServiceCount { get; set; }
		public double CountStandard { get; set; }

		private double _countFact;
		public double CountFact {
			get {
				return _countFact;
			} set {
				if (value != _countFact) {
					WriteOff = value > 0;

					_countFact = value;
					if (_countFact < 0)
						_countFact = 0;

					if (_countFact > ServiceCount * CountStandard) {
						CountFactBackground = Brushes.OrangeRed;
					} else if (_countFact < ServiceCount * CountStandard) {
						CountFactBackground = Brushes.LightGreen;
					} else {
						CountFactBackground = Brushes.Transparent;
					}
					
					NotifyPropertyChanged();
				}
			}
		}

		public string IdMis { get; set; }
		public string Id1C { get; set; }
		public string ServiceName { get; set; }
		public string Unit { get; set; }

		private bool _writeOff;
		public bool WriteOff {
			get {
				return _writeOff;
			} set {
				if (value != _writeOff) {
					_writeOff = value;

					if (_writeOff)
						CountFact = ServiceCount * CountStandard;
					else
						CountFact = 0;

					NotifyPropertyChanged();
				}
			}
		}

		private Brush _countFackBackground;
		public Brush CountFactBackground {
			get {
				return _countFackBackground;
			} set {
				if (value != _countFackBackground) {
					_countFackBackground = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ServiceItem() {
			Id1C = string.Empty;
			IdMis = string.Empty;
			ServiceCount = 0;
			CountStandard = 0;
			WriteOff = true;
			CountFactBackground = Brushes.Transparent;
		}
	}
}
