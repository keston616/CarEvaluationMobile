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

namespace CarEvaluation.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarApprovalPage : ContentPage
    {
        Car carValue;
        public CarApprovalPage(Car car)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.carValue = car;
            if (car.StatusId == 2)
            {
                buyEntry.Text = car.BuyPrice.ToString();
                sellEntry.Text = car.SellPrice.ToString();
            }
        }

        private async void Save_Tapped(object sender, EventArgs e)
        {
            try
            {
                bool war = true;
                if (String.IsNullOrWhiteSpace(buyEntry.Text))
                {
                    buyWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(sellEntry.Text))
                {
                    sellWarning.IsVisible = true;
                    war = false;
                }
                if (war)
                {

                    carValue.BuyPrice = Math.Round(Convert.ToDecimal(buyEntry.Text), 2);
                    carValue.SellPrice = Math.Round(Convert.ToDecimal(sellEntry.Text));
                    carValue.AniliticId = MainInfoClass.user.idUser;
                    carValue.user1 = MainInfoClass.user;
                    carValue.StatusId = 2;
                    carValue.Status = MainInfoClass.statusList.Where(x=>x.idStatus == 2).FirstOrDefault();
                    int indexCar = carValue.idCar;
                    string z = string.Format("{0}/Cars/{1}", MainInfoClass.apiDBSource, indexCar);
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        string json = JsonConvert.SerializeObject(carValue);
                        wc.UploadString(z, WebRequestMethods.Http.Put, json);
                    }
                    (MainInfoClass.carInspector as IRefreshListCar).RefreshData();
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", $"Отсутствует подключение к интернету", "ОК");
            }
        }

        private void buyEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            buyWarning.IsVisible = false;
        }

        private void sellEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            sellWarning.IsVisible = false;
        }

        private async void GoBack_Tapped(object sender, EventArgs e)
        {
            if (await App.Current.MainPage.DisplayAlert("Внимание", "Вы уверены, что хотите отменить редактирование", "Нет", "Да") == false)
            {
                await Navigation.PopAsync(false);
            }
        }
    }
}