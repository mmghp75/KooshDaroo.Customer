using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ImageCircle.Forms.Plugin.Abstractions;
using Plugin.Media;
using KooshDaroo.Services;
using KooshDaroo.Models;
using System.IO;
using System.Threading.Tasks;
using KooshDaroo.Data;
using Plugin.Media.Abstractions;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace KooshDaroo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private List<Prescribes> prescribesList;
        private CircleImage camera;
        private CircleImage gallery;
        private CircleImage send;
        private Image prescribeimage;

        private HubConnection hubConnection;

        public MainPage()
        {
            InitializeComponent();

            camera = (CircleImage)FindByName("Camera");
            camera.Source = ImageSource.FromResource("KooshDaroo.Customer.Images.camera.jpg");

            gallery = (CircleImage)FindByName("Gallery");
            gallery.Source = ImageSource.FromResource("KooshDaroo.Customer.Images.gallery.jpg");

            send = (CircleImage)FindByName("Send");
            send.Source = ImageSource.FromResource("KooshDaroo.Customer.Images.send.jpg");

            prescribeimage = (Image)FindByName("PrescribeImage");

            prescribesList = new List<Prescribes>();

            hubConnection = new HubConnectionBuilder()
                        .WithUrl("http://10.0.2.2:55011/Hubs/PrescriptionHub")
                        .Build();

            hubConnection.On<int>("DrugstoreResponse", (id) =>
            {
                
            });

            //    hubConnection.On<string>("New Message", (message) =>
            //{
            //    string mytext = message;
            //    //DisplayAlert("Ouch", message,"OK");
            //});
        }
        private async void StartConnectionToHub()
        {
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
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 2000,
                    DefaultCamera = CameraDevice.Front,
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


            //await DisplayAlert("Sent", "To Server", "OK");
            PrescribesService prescribesservice = new PrescribesService();
            if (prescribesservice.CanPrescribeAsync().Result ||  true)
            {
                if (prescribeimage.Source == null)
                {
                    DisplayAlert("توجه", "ابتدا باید از نسخه عکس بگیرید", "OK");
                    return;
                }
                KooshDarooDatabase odb = new KooshDarooDatabase();
                var oLoginItemS = odb.GetItemsAsync();
                var oLoginItem = oLoginItemS.Result[0];
                Prescribes prescribes = new Prescribes { DateOf = DateTime.Now.ToOADate(), isCancelled = false, Prescribe = ReadFully((StreamImageSource)prescribeimage.Source), X = x, Y = y, PhoneNo = oLoginItem.PhoneNo };
                var result = await prescribesservice.PostPrescribesAsync(prescribes);
                if (result.id == 0)
                    DisplayAlert("خطا", "امکان ارسال نسخه وجود ندارد و یادر 12 ساعت گذشته نسخه ارسال کرده اید و تا 12 ساعت امکان ثبت نسخه جدید ندارید.", "OK");
                else
                {
                    prescribeimage.Source = null;
                    DisplayAlert("توجه", "نسخه ارسال شد. لطفاً تا رسیدن پاسخ از داروخانه ها صبرکنید.", "OK");

                    if (hubConnection.State == HubConnectionState.Disconnected)
                        StartConnectionToHub();

                    await hubConnection.SendAsync("SendPrescribeToPharmacy", prescribes.X, prescribes.Y, prescribes.DateOf, result.id);
                    await hubConnection.SendAsync("Send", "Testing the hub...");

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

                //    //await hubConnection.SendAsync("SendPrescribeToPharmacy", prescribes.PhoneNo, prescribes.DateOf);
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
    }
}