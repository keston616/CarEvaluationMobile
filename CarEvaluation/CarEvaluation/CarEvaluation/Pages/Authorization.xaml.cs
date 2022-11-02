
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarEvaluation
{
    public partial class Authorization : ContentPage
    {
        List<User> user = new List<User>();
        public Authorization()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ConnectToApi();
        }

        private void ConnectToApi()
        {

            try
            {
                using (var client = new WebClient())
                {
                    var responze = client.DownloadString(string.Format("{0}/Users",MainInfoClass.apiDBSource));
                    user = JsonConvert.DeserializeObject<List<User>>(responze);
                    var responzeSecond = client.DownloadString(string.Format("{0}/Status", MainInfoClass.apiDBSource));
                    MainInfoClass.statusList = JsonConvert.DeserializeObject<List<Status>>(responzeSecond);
                }
                int z = Preferences.Get("id", 0);
                MainInfoClass.userList = user;
                MainInfoClass.user = user.Where(x => x.idUser == z).FirstOrDefault();
                if (MainInfoClass.user != null)
                {
                    TransitionPage();
                }
            }
            catch (Exception ex)
            {
                ChechConnect();
            }
        }

        private async void ChechConnect()
        {
            if (await DisplayAlert("Внимание", $"Отсутствует подключение к интернету", "Выйти", "Обновить") == false)
            {
                ConnectToApi();
            }
            else
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }



        private void login_Focused(object sender, FocusEventArgs e)
        {
            frameLogin.BorderColor = Color.FromHex("2D4B7F");
            loginIcon.Fill = new SolidColorBrush(Color.FromHex("2D4B7F"));
        }

        private void login_Unfocused(object sender, FocusEventArgs e)
        {
            if (loginEntry.Text == null || loginEntry.Text.ToString().Length == 0)
            {
                frameLogin.BorderColor = Color.LightGray;
                loginIcon.Fill = Brush.LightGray;
            }
        }

        private void passwordEntry_Focused(object sender, FocusEventArgs e)
        {
            framePassword.BorderColor = Color.FromHex("2D4B7F");
            passwordIcon.Fill = new SolidColorBrush(Color.FromHex("2D4B7F"));
        }

        private void passwordEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (passwordEntry.Text == null || passwordEntry.Text.ToString().Length == 0)
            {
                framePassword.BorderColor = Color.LightGray;
                passwordIcon.Fill = Brush.LightGray;
            }
        }

        private async void GoAuthorization_Tapped(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(loginEntry.Text) == true)
            {
                loginWarning.Text = "Заполните поле";
                loginWarning.IsVisible = true;
            }
            else if (String.IsNullOrWhiteSpace(passwordEntry.Text) == true)
            {
                loginWarning.Text = "Заполните поле";
                passWarning.IsVisible = true;
            }
            else
            {
                MainInfoClass.user = user.Where(x => x.Phone == loginEntry.Text).FirstOrDefault();
                if (MainInfoClass.user == null)
                {
                    loginWarning.Text = "Пользователь не найден";
                    loginWarning.IsVisible = true;
                }
                else if (MainInfoClass.user.Password != passwordEntry.Text)
                {
                    passWarning.Text = "Пароль неверный";
                    passWarning.IsVisible = true;
                }
                else
                {
                    Preferences.Set("id", MainInfoClass.user.idUser);
                    TransitionPage();
                }
            }
        }

        private async void TransitionPage()
        {
            try
            {
                await Navigation.PushAsync(new InspectorPage(), false);
                Navigation.RemovePage(this);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", $"Отсутствует подключение к интернету", "ОК");
            }

        }
    }
}
