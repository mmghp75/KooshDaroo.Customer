using KooshDaroo.Services;
using MultiSelectDemo.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KooshDaroo.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;

namespace KooshDaroo.Pharmacy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MultiSelectPage : ContentPage
    {
        public byte[] img;
        public Stream imgStream;
        public int prescribeID;

        private bool enableMultiSelect;
        private double fontsize = Device.GetNamedSize(NamedSize.Large, typeof(Entry));
        private int idx = 1;
        private HubConnection hubConnection;

        public MultiSelectPage()
        {
            InitializeComponent();
  
            var initialItems = new[] {
                "1st",
                "2nd",
                "3rd",
                "4th",
                "5th",
                "6th",
                "7th",
                "8th",
                "9th",
                "10th"
            };

            enableMultiSelect = false;
            Items = new SelectableObservableCollection<string>(initialItems);
            AddItemCommand = new Command(OnAddItem);
            RemoveSelectedCommand = new Command(OnRemoveSelected);
            ToggleSelectionCommand = new Command(OnToggleSelection);
            SelectAllCommand = new Command(OnSelectAll);
            SendCommand = new Command(OnSend);

            BindingContext = this;

            hubConnection = new HubConnectionBuilder()
            .WithUrl("http://10.0.2.2:55011/Hubs/PrescriptionHub")
            .Build();
            hubConnection.On<double, double, double, string>("New Prescribe", (X, Y, dateOf, phoneNo) =>
            {
                string _phoneNo = phoneNo;
                //    DisplayAlert("Ouch", prescribe.DateOf.ToString() + "\n" + prescribe.PhoneNo, "OK");
            });

        }
        public void SetContent()
        {
            image = (Image)FindByName("image");
            selectAll = (Button)FindByName("selectAll");
            send = (Button)FindByName("send");

            image.Source = ImageSource.FromStream(() =>
            {
                return imgStream;
            });
        }

        public bool EnableMultiSelect
        {
            get { return enableMultiSelect; }
            set
            {
                enableMultiSelect = value;
                OnPropertyChanged();
            }
        }

        public SelectableObservableCollection<string> Items { get; }

        public ICommand AddItemCommand { get; }

        public ICommand RemoveSelectedCommand { get; }

        public ICommand ToggleSelectionCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand SendCommand { get; }

        private void OnAddItem()
        {
            Items.Add(idx.ToString());
            idx += 1;
        }

        private void OnRemoveSelected()
        {
            var selectedItems = Items.SelectedItems.ToArray();
            foreach (var item in selectedItems)
            {
                Items.Remove(item);
            }
        }

        private void OnToggleSelection()
        {
            foreach (var item in Items)
            {
                item.IsSelected = !item.IsSelected;
            }
        }
        private void OnSelectAll()
        {
            var selectedItems = Items.SelectedItems.ToArray();
            if (selectAll.Text == "Select All")
            {
                foreach (var item in Items)
                    item.IsSelected = true;
                selectAll.Text = "DeSelect All";
            }
            else
            {
                foreach (var item in Items)
                    item.IsSelected = false;
                selectAll.Text = "Select All";
            }
        }
        private async void OnSend()
        {
            PrescribeResourcesService prescribeResourcesService = new PrescribeResourcesService();
            PrescribeResources prescribeResources = new PrescribeResources();
            prescribeResources.Accepted = false;
            prescribeResources.DrugstoresId = GetDrugStoreID();
            prescribeResources.Item01 = Items[0].IsSelected;
            prescribeResources.Item02 = Items[1].IsSelected;
            prescribeResources.Item03 = Items[2].IsSelected;
            prescribeResources.Item04 = Items[3].IsSelected;
            prescribeResources.Item05 = Items[4].IsSelected;
            prescribeResources.Item06 = Items[5].IsSelected;
            prescribeResources.Item07 = Items[6].IsSelected;
            prescribeResources.Item08 = Items[7].IsSelected;
            prescribeResources.Item09 = Items[8].IsSelected;
            prescribeResources.Item10 = Items[9].IsSelected;
            prescribeResources.PrescribesId = prescribeID;
            
            var result = await prescribeResourcesService.PostPrescribeResourcesAsync(prescribeResources);
            // if (result.id == 0)
                                 
            Navigation.PopAsync();

            if (hubConnection.State == HubConnectionState.Disconnected)
                StartConnectionToHub();
            await hubConnection.SendAsync("IHaveThisPrescribes", result.PrescribesId);
        }
        private async void StartConnectionToHub()
        {
            await hubConnection.StartAsync();
        }

        private int GetDrugStoreID()
        {

            return 0;
        }
    }
}