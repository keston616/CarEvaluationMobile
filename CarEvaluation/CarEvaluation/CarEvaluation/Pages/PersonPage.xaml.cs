using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonPage : ContentPage
    {
        bool exit = true;
        public PersonPage()
        {
            InitializeComponent();
            BindingContext = MainInfoClass.user;
        }

        private async void GoExit_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (exit)
                {
                    exit = false;
                    Authorization a = new Authorization();
                    Navigation.PushAsync(a, false);
                    Navigation.RemovePage(MainInfoClass.iPage);
                    foreach (var item in Navigation.NavigationStack)
                    {
                        Navigation.RemovePage(item);
                    }
                    await Task.Run(() => Preferences.Set("id", 0));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }

        private async void GoEditProfile_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfilePage(), false);
        }

        private async void GoEditPassword_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditPasswordPage(), false);
        }
    }
}