using ImageCircle.Forms.Plugin.Abstractions;
using KooshDaroo.Data;
using KooshDaroo.Models;
using KooshDaroo.Services;
using Microsoft.AspNetCore.SignalR.Client;
using KooshDaroo.Controls;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KooshDaroo.Customer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageTabbed : TabbedPage
    {
        private List<Prescribe> PrescribeList;
        private CircleImage camera;
        private CircleImage gallery;
        private CircleImage send;
        private Image prescribeimage;

        private HubConnection hubConnection;
        public byte[] img;
        public Stream imgStream;
        public int _prescribeId;
        public int _prescribeResourceId;

        private double fontsize = Device.GetNamedSize(NamedSize.Large, typeof(Entry));
        private double pharmacy_X = 0;
        private double pharmacy_Y = 0;

        public SelectableObservableCollection<Response> responseS { get; }
        private ObservableCollection<Response> _responseList = new ObservableCollection<Response>();

        public ObservableCollection<Response> responseList
        {
            get
            {
                return _responseList ?? (_responseList = new ObservableCollection<Response>());
            }
        }
        public MainPageTabbed()
        {
            InitializeComponent();

            camera = (CircleImage)FindByName("Camera");
            camera.Source = ImageSource.FromResource("KooshDaroo.Customer.Images.camera.jpg");

            gallery = (CircleImage)FindByName("Gallery");
            gallery.Source = ImageSource.FromResource("KooshDaroo.Customer.Images.gallery.jpg");

            send = (CircleImage)FindByName("Send");
            send.Source = ImageSource.FromResource("KooshDaroo.Customer.Images.send.jpg");

            prescribeimage = (Image)FindByName("PrescribeImage");

            PrescribeList = new List<Prescribe>();
            responseS = new SelectableObservableCollection<Response>();
            //var _r = new Response
            //{
            //    NDI = "8:8",
            //    Tag = "154"
            //};
            //responseS.Add(_r);


            //RemoveSelectedCommand = new Command(OnRemoveSelected);
            RouteToPharmacyCommand = new Command(OnRouteToPharmacy);
            AcceptCommand = new Command(OnAccept);

            BindingContext = this;

            hubConnection = new HubConnectionBuilder()
            .WithUrl(App.hubAddress)
            .Build();
            //hubConnection.On<double, double, DateTime, string>("New Prescribe", (X, Y, dateOf, phoneNo) =>
            //{
            //    string _phoneNo = phoneNo;
            //    //    DisplayAlert("Ouch", prescribe.DateOf.ToString() + "\n" + prescribe.PhoneNo, "OK");
            //});

            hubConnection.On<int, int>("SendAcceptToMember", (prescribeResourceId, prescribeId) =>
              {
                  if (_prescribeId != prescribeId)
                      return;

                  GetPrescribeResourseData(prescribeResourceId);
              });

            //GetPrescribeResourseData(23);
        }

        private async void GetPrescribeResourseData(int Id)
        {
            PrescribeResourceService prescribeResourcesService = new PrescribeResourceService();
            var prescribeResources = await prescribeResourcesService.GetPrescribeResourceById(Id);

            PharmacyService pharmacysService = new PharmacyService();
            var pharmacys = await pharmacysService.GetPharmacyById(prescribeResources.PharmacyId);
            pharmacy_X = pharmacys.X;
            pharmacy_Y = pharmacys.Y;

            double x = 0.0;
            double y = 0.0;
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    x = location.Latitude;
                    y = location.Longitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Faild", ex.Message, "OK");
            }
            Location sourceCoordinates = new Location(x, y);
            Location destinationCoordinates = new Location(pharmacys.X, pharmacys.Y);
            Int32 distance = Convert.ToInt32(Location.CalculateDistance(sourceCoordinates, destinationCoordinates, DistanceUnits.Kilometers) * 1000);
            string str = pharmacys.Title + "   ==>  فاصله: " + distance.ToString() + " متر   ==>  ";
            if (prescribeResources.Item01 &
                prescribeResources.Item02 &
                prescribeResources.Item03 &
                prescribeResources.Item04 &
                prescribeResources.Item05 &
                prescribeResources.Item06 &
                prescribeResources.Item07 &
                prescribeResources.Item08 &
                prescribeResources.Item09 &
                prescribeResources.Item10)
                str += "همه اقلام موجودند";
            else
            {
                str += "اقلام موجود: ";
                string str1 = "";
                if (prescribeResources.Item01)
                    if (str1 == "")
                        str1 += "1";
                if (prescribeResources.Item02)
                {
                    if (str1 == "")
                        str1 += "2";
                    else
                        str1 += ",2";
                }
                if (prescribeResources.Item03)
                {
                    if (str1 == "")
                        str1 += "3";
                    else
                        str1 += ",3";
                }
                if (prescribeResources.Item04)
                {
                    if (str1 == "")
                        str1 += "4";
                    else
                        str1 += ",4";
                }
                if (prescribeResources.Item05)
                {
                    if (str1 == "")
                        str1 += "5";
                    else
                        str1 += ",5";
                }
                if (prescribeResources.Item06)
                {
                    if (str1 == "")
                        str1 += "6";
                    else
                        str1 += ",6";
                }
                if (prescribeResources.Item07)
                {
                    if (str1 == "")
                        str1 += "7";
                    else
                        str1 += ",7";
                }
                if (prescribeResources.Item08)
                {
                    if (str1 == "")
                        str1 += "8";
                    else
                        str1 += ",8";
                }
                if (prescribeResources.Item09)
                {
                    if (str1 == "")
                        str1 += "9";
                    else
                        str1 += ",9";
                }
                if (prescribeResources.Item10)
                {
                    if (str1 == "")
                        str1 += "10";
                    else
                        str1 += ",10";
                }
                str += str1;
            }

            UpdateList(str, prescribeResources.id.ToString());
            this.CurrentPage = this.Children[1];
        }

        private void UpdateList(string str, string tag, bool add = true)
        {

            var response = new Response()
            {
                NDI = str,
                Tag = tag
            };

            //add or remove the a prescription to the list
            if (add)
            {
                responseS.Add(response);
                //_responseList.Insert(0, response);
                send.IsVisible = true;
            }
            else
            {
                responseS.Remove(responseS.SelectedItems.FirstOrDefault());
                //for (int i = 0; i < _responseList.Count; i++)
                //    if (response.Tag != _responseList[i].Tag)
                //    {
                //        _responseList.RemoveAt(i);
                //        send.IsVisible = false;
                //    }
            }

            //lvResponses = (ListView)FindByName("lvResponses");
            //lvResponses.ItemsSource = responseList;
        }

        public void SetContent()
        {
            routeToPharmacy = (Button)FindByName("RouteToPharmacy");
        }


        public ICommand AddItemCommand { get; }

        public ICommand RemoveSelectedCommand { get; }

        public ICommand RouteToPharmacyCommand { get; }

        public ICommand AcceptCommand { get; }

        //private void OnRemoveSelected()
        //{
        //    var selectedItems = Items.SelectedItems.ToArray();
        //    foreach (var item in selectedItems)
        //    {
        //        Items.Remove(item);
        //    }
        //}
        private void OnToggleSelection()
        {
            foreach (var item in responseS)
            {
                item.IsSelected = !item.IsSelected;
            }
        }
        private async void OnRouteToPharmacy()
        {
            if (pharmacy_X != 0 & pharmacy_Y != 0)
                await Map.OpenAsync(pharmacy_X, pharmacy_Y, new MapLaunchOptions { NavigationMode = NavigationMode.Driving });
            //await Map.OpenAsync(37.406523, -122.011044, new MapLaunchOptions { NavigationMode = NavigationMode.Driving });
        }
        private async void OnAccept()
        {
            if (_prescribeResourceId != 0)
            {
                PrescribeResourceService prescribeResourcesService = new PrescribeResourceService();
                PrescribeResource prescribeResources = await prescribeResourcesService.GetPrescribeResourceById(_prescribeResourceId);
                prescribeResources.MemberAccepted = true;
                var result = await prescribeResourcesService.PutPrescribeResourceAsync(prescribeResources);

                if (result)
                {
                    StartConnectionToHub();
                    await hubConnection.SendAsync("SendAcceptToPharmacy", prescribeResources.id);
                }
                //_prescribeResourceId = 0;
            }
        }
        private async void StartConnectionToHub()
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
                await hubConnection.StartAsync();
        }

        private async void OnCameraTapped(object sender, EventArgs args)
        {
            Permission permission = Permission.Camera;
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

            if (storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                var status = results[Permission.Storage];
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("عدم دسترسی به دوربین", "لطفا دسترسی به دوربین را برای نرم افزار فعال کنید.", "OK");
                    return;
                }
            }

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            try
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Prescribe",
                    SaveToAlbum = true,
                    CompressionQuality = 75,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 2000,
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
                    AllowCropping = true
                });
                if (file == null)
                    return;

                //DisplayAlert("File Location", file.Path, "OK");

                prescribeimage.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    return stream;
                });

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Faild", ex.Message, "OK");
            }

        }

        private async void OnGalleryTapped(object sender, EventArgs args)
        {
            Permission permission = Permission.Storage;
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

            if (storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                var status = results[Permission.Storage];
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("عدم دسترسی به گالری", "لطفا دسترسی به فضای ذخیره سازی را برای برنامه فعال کنید.", "OK");
                    return;
                }
            }

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                DisplayAlert("Oops", "Pick photo is not supported!", "OK");
                return;
            }

            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync();
                if (file == null)
                    return;

                //await DisplayAlert("File Location", file.Path, "OK");

                prescribeimage.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Faild", ex.Message, "OK");
            }
        }

        private async void OnSendTapped(object sender, EventArgs args)
        {
            double x = 0.0;
            double y = 0.0;
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    x = location.Latitude;
                    y = location.Longitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Faild", ex.Message, "OK");
            }


            if (x == 0 & y == 0)
                return;
            //await DisplayAlert("Sent", "To Server", "OK");
            PrescribeService Prescribeservice = new PrescribeService();
            if (Prescribeservice.CanPrescribeAsync().Result || true)
            {
                if (prescribeimage.Source == null)
                {
                    DisplayAlert("توجه", "ابتدا باید از نسخه عکس بگیرید", "OK");
                    return;
                }
                KooshDarooDatabase odb = new KooshDarooDatabase();
                var oLoginItemS = odb.GetItemsAsync();
                var oLoginItem = oLoginItemS.Result[0];
                Prescribe Prescribe = new Prescribe
                {
                    isCancelled = false,
                    Prescription = ReadFully((StreamImageSource)prescribeimage.Source),
                    X = x,
                    Y = y,
                    PhoneNo = oLoginItem.PhoneNo
                };
                var result = await Prescribeservice.PostPrescribeAsync(Prescribe);
                if (result.id == 0)
                    DisplayAlert("خطا", "امکان ارسال نسخه وجود ندارد و یادر 12 ساعت گذشته نسخه ارسال کرده اید و تا 12 ساعت امکان ثبت نسخه جدید ندارید.", "OK");
                else
                {
                    prescribeimage.Source = null;
                    DisplayAlert("توجه", "نسخه ارسال شد. لطفاً تا رسیدن پاسخ از داروخانه ها صبرکنید.", "OK");
                    _prescribeId = result.id;

                    if (hubConnection.State == HubConnectionState.Disconnected)
                        StartConnectionToHub();

                    await hubConnection.SendAsync("SendPrescribeToPharmacy", Prescribe.X, Prescribe.Y, result.DateOf, result.id);
                    //await hubConnection.SendAsync("Send", "Testing the hub...");

                }
                //if (result == 0)
                //    DisplayAlert("خطا", "امکان ارسال نسخه وجود ندارد.", "OK");
                //else if (result == -1)
                //    DisplayAlert("خطا", "در 12 ساعت گذشته نسخه ارسال کرده اید و تا 12 ساعت امکان ثبت نسخه جدید ندارید.", "OK");
                //else
                //{
                //    prescribeimage.Source = null;
                //    DisplayAlert("توجه", "نسخه ارسال شد. لطفاً تا رسیدن پاسخ از داروخانه ها صبرکنید.", "OK");

                //    StartConnectionToHub();

                //    //await hubConnection.SendAsync("SendPrescribeToPharmacy", Prescribe.PhoneNo, Prescribe.DateOf);
                //    //await hubConnection.SendAsync("Send", "Testing the hub...");

                //}
            }

        }
        public static byte[] ReadFully(StreamImageSource input)
        {
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Task<Stream> task = input.Stream(cancellationToken);
            Stream stream = task.Result;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private void lvResponses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedItem = (SelectableItem<KooshDaroo.Customer.Views.Response>) e.Item;

            _prescribeResourceId = Convert.ToInt32(selectedItem.Data.Tag);
        }
    }
    public class Response
    {
        public string NDI { get; set; }
        public string Tag { get; set; }
    }
}