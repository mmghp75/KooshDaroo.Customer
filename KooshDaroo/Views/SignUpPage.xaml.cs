using KooshDaroo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using KooshDaroo.Data;
using KooshDaroo.Services;
using KooshDaroo.Customer.Views;

namespace KooshDaroo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        private Entry phoneNo;
        private Button signUpButton;
        public SignUpPage()
        {
            StackLayout stackLayout = new StackLayout();

            phoneNo = new Entry();
            phoneNo.Placeholder = "شماره تلفن : 09123456789";
            stackLayout.Children.Add(phoneNo);

            signUpButton = new Button();
            signUpButton.Text = "ثبت نام";
            signUpButton.Clicked += SignUpButton_Clicked;
            stackLayout.Children.Add(signUpButton);

            Content = stackLayout;
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            MemberService memberservices = new MemberService();
            //var xo = await memberservices.GetMemberAsync();
            var member = await memberservices.GetMemberByPhoneNoAsync(phoneNo.Text);

            if (member.Count == 0)
            {
                Member _member = new Member { PhoneNo = phoneNo.Text, isInactive = false };
                var result = memberservices.PostMemberAsync(_member);
            }
            KooshDarooDatabase odb = new KooshDarooDatabase();
            tblMember newmember = new tblMember { PhoneNo = phoneNo.Text };
            int r = await odb.SaveItemAsync(newmember);

            odb = new KooshDarooDatabase();
            var oLoginItemS = odb.GetItemsAsync();
            var o = oLoginItemS.Result.Count;

            App.Current.MainPage= new MainPageTabbed();
            //this.Content = (new MainPage()).Content;
        }
    }
}