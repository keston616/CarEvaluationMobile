using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPasswordPage : ContentPage
    {
        public EditPasswordPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void GoSave_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (MainInfoClass.user.Password != oldPassEntry.Text)
                {
                    oldPassWarning.IsVisible = true;
                }
                else if (newPassEntryFirst.Text != newPassEntrySecond.Text)
                {
                    newPassWarning.IsVisible = true;
                }
                else
                {
                    MainInfoClass.user.Password = newPassEntryFirst.Text;
                    string z = string.Format("http://crazy-ramanujan.37-140-192-136.plesk.page/api/Users/{0}", MainInfoClass.user.idUser);
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        string json = JsonConvert.SerializeObject(MainInfoClass.user);
                        wc.UploadString(z, "PUT", json);
                    }
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }

        private async void GoBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        private void oldPassEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            oldPassWarning.IsVisible = false;
        }

        private void newPassEntryFirst_TextChanged(object sender, TextChangedEventArgs e)
        {
            newPassWarning.IsVisible = false;
        }
    }
}