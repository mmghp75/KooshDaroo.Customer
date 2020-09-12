using KooshDaroo.Data;
using KooshDaroo.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using KooshDaroo.Services;
using KooshDaroo.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using SQLite;
using KooshDaroo.Customer.Views;
using System.Net.Http;

namespace KooshDaroo
{
    public partial class App : Application
    {
        public static string apiAddress = "http://kooshdaroo.nezarat.irimctest.ir/api/";
        public static string hubAddress = "http://kooshdaroo.nezarat.irimctest.ir/Hubs/PrescriptionHub";
        //public static string apiAddress = "http://10.0.2.2:55011/api/";
        //public static string hubAddress = "http://10.0.2.2:55011/Hubs/PrescriptionHub";
    
        public App()
        {
            InitializeComponent();
            //testHttpClient();
            MainPage = new StartPage();
            //OpenPage();
        }
        private async void testHttpClient()
        {
            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(apiAddress + "Member");
            var json1 = await httpClient.GetStringAsync(apiAddress + "Member/PhoneNo/09352226589");
            var a = 100;
        }

        private async void OpenPage()
        {
            KooshDarooDatabase odb = new KooshDarooDatabase();
            var oLoginItemS = odb.GetItemsAsync();
            if (oLoginItemS.Result.Count > 0)
            {
                MemberService memberservices = new MemberService();
                //var member = await memberservices.GetMemberByPhoneNoAsync(oLoginItemS.Result[0].PhoneNo);
                var member = Task.Run(() => memberservices.GetMemberByPhoneNoAsync(oLoginItemS.Result[0].PhoneNo));
                if (member.Result.Count == 0)
                //if (member.Count == 0)
                {
                    oLoginItemS.Result.ForEach(f => odb.DeleteItemAsync(f));
                    MainPage = new SignUpPage();
                }
                else
                    MainPage = new MainPageTabbed();
            }
            else
                MainPage = new SignUpPage();
            //MainPage = new NavigationPage(new SignUpPage());


        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
