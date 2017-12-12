using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InventoryClient {
	/// <summary>
	/// Логика взаимодействия для WindowAbout.xaml
	/// </summary>
	public partial class WindowInventory : Window {
		public string DoctorName { get; set; }
		public string ClientName { get; set; }
		public string TitleText { get; set; }
		public string CreateDateTime { get; set; }
		public ObservableCollection<ServiceItem> ServiceItems { get; set; }

		public WindowInventory(InventoryService.EventDataType eventData) {
			InitializeComponent();

			DoctorName = eventData.DoctorName;
			ClientName = eventData.ClientName;
			TitleText = "Лечение от " + eventData.CreateDate;
			CreateDateTime = eventData.CreateDate;

			ServiceItems = new ObservableCollection<ServiceItem>();
			foreach (InventoryService.EventDataType.Service service in eventData.Services) {
				if (service.Materials.Length == 0) {
					ServiceItems.Add(new ServiceItem() {
						Name = "Сведения о материалах из технологических карт отсутствуют",
						ServiceName = service.Name + " (" + service.Count + " шт.)"
					});
				}

				foreach (InventoryService.EventDataType.Service.Material material in service.Materials) {
					ServiceItems.Add(new ServiceItem() {
						Id1C = material.Id1C,
						IdMis = material.IdMis,
						ServiceCount = service.Count,
						CountStandard = material.Count,
						CountFact = material.Count * service.Count,
						Name = material.Name,
						ServiceName = service.Name + " (" + service.Count + " шт.)",
						Unit = material.Unit
					});
				}

			}

			DataContext = this;
			listViewServices.DataContext = this;
			listViewServices.ItemsSource = ServiceItems;

			CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewServices.ItemsSource);
			PropertyGroupDescription groupDescription = new PropertyGroupDescription("ServiceName");
			view.GroupDescriptions.Add(groupDescription);
		}

		private void ButtonSubmit_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void ButtonAddOrRemove_Click(object sender, RoutedEventArgs e) {
			try {
				ServiceItem item = (sender as Button).DataContext as ServiceItem;
				string tag = (sender as Button).Tag.ToString();
				double changeValue = item.CountStandard;
				
				if (tag.Equals("Add")) {

				} else if (tag.Equals("Remove")) {
					changeValue *= -1;
				} else {
					Console.WriteLine("ButtonAddOrRemove_Click: Unknown tag");
					return;
				}

				item.CountFact += changeValue;
			} catch (Exception exception) {
				Console.WriteLine(exception.Message);
			}
		}
	}
}
