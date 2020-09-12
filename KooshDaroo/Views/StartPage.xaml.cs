using KooshDaroo.Data;
using KooshDaroo.Services;
using KooshDaroo.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KooshDaroo.Customer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage : ContentPage
    {
        public StartPage()
        {
            InitializeComponent();
        }
        private void btnGo_Clicked(object sender,EventArgs e)
        {
            OpenPage();
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
                    App.Current.MainPage = new SignUpPage();
                }
                else
                    App.Current.MainPage = new MainPageTabbed();
            }
            else
                App.Current.MainPage = new SignUpPage();
            //MainPage = new NavigationPage(new SignUpPage());

        }

    }
}