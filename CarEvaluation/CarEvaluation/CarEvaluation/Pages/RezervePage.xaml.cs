using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RezervePage : ContentPage
    {
        Car carValue;
        public RezervePage(Car car)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            carValue = car;
        }

        private async void Save_Tapped(object sender, EventArgs e)
        {
            try
            {
                Regex r = new Regex(@"^(\+7)(\d{10})$");
                bool war = true;
                if (String.IsNullOrWhiteSpace(surnameEntry.Text))
                {
                    surnameWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(nameEntry.Text))
                {
                    nameWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(phoneEntry.Text))
                {
                    phoneWarning.Text = "Номер телефона не указан";
                    phoneWarning.IsVisible = true;
                    war = false;
                }
                else if (!r.IsMatch(phoneEntry.Text))
                {
                    phoneWarning.Text = "Неверный формат. Пример: +78888888888";
                    phoneWarning.IsVisible = true;
                    war = false;
                }
                if (war)
                {
                    Booking book = new Booking();
                    book.Date = DateTime.Now;
                    book.Phone = phoneEntry.Text;
                    book.SurnameClient = surnameEntry.Text;
                    book.NameClient = nameEntry.Text;
                    book.CarId = carValue.idCar;
                    book.UserId = MainInfoClass.user.idUser;
                    string z = string.Format("{0}/Bookings", MainInfoClass.apiDBSource);
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        string html = wc.UploadString(z, JsonConvert.SerializeObject(book));
                        carValue.Booking.Add(JsonConvert.DeserializeObject<Booking>(html));
                    }
                     (MainInfoClass.carInspector as IRefreshListCar).RefreshData();
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }

        private void phoneEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            phoneWarning.IsVisible = false;
        }

        private void nameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameWarning.IsVisible = false;
        }

        private void surnameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            surnameWarning.IsVisible = false;
        }

        private async void GoBack_Tapped(object sender, EventArgs e)
        {
            if (await App.Current.MainPage.DisplayAlert("Внимание", "Вы уверены, что хотите отменить бронирование", "Нет", "Да") == false)
            {
                await Navigation.PopAsync(false);
            }
        }
    }
}