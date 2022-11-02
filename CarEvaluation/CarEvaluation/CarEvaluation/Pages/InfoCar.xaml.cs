using CarEvaluation.Pages;
using Newtonsoft.Json;
using Plugin.Messaging;
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
    public partial class InfoCar : ContentPage
    {
        Car carValue;
        CarInspector ci;
        public InfoCar(Car car, CarInspector ci)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            carValue = car;
            this.ci = ci;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                BindingContext = null;
                BindingContext = carValue;
                if (carValue.BookingDate > DateTime.Now)
                {
                    reservLabel.IsVisible = true;
                    infoBooking.IsVisible = true;
                }
                if (MainInfoClass.user.RoleId == 1 && carValue.StatusId == 1)
                {
                    addPrice.IsVisible = true;
                }
                else if (MainInfoClass.user.RoleId == 1 && carValue.StatusId == 2 && carValue.BookingDate < DateTime.Now)
                {
                    addPrice.IsVisible = true;
                    doneButton.Text = "Редактировать";
                }
                else if (MainInfoClass.user.RoleId == 2 && carValue.StatusId == 1)
                {
                    edit.IsVisible = true;
                }
                else if (MainInfoClass.user.RoleId == 3 && carValue.BookingDate < DateTime.Now)
                {
                    managStack.IsVisible = true;
                    reserve.IsVisible = true;
                }
                else if (MainInfoClass.user.RoleId == 3 && carValue.BookingDate > DateTime.Now)
                {
                    managStack.IsVisible = true;
                    callPhone.IsVisible = true;
                    reserveClose.IsVisible = true;
                }
                if (!carValue.Juridical)
                {
                    boxJuridic.BackgroundColor = Color.Red;
                    labelJuridic.Text = "Автомобиль юр. не чист";
                }
                stackRepair.Children.Clear();
                if (carValue.Repairedparts.Count == 0)
                {

                    stackRepair.Children.Add(new StackLayout
                    {
                        VerticalOptions = LayoutOptions.Start,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                    {
                         new BoxView
                        {
                            BackgroundColor = Color.Green,
                             VerticalOptions = LayoutOptions.Start,
                             HorizontalOptions = LayoutOptions.Start,
                             HeightRequest = 8,
                             Margin = new Thickness(0,7,0,0),
                             WidthRequest = 8,
                             CornerRadius = 50
                        },

                        new Label
                        {
                                    FontSize = 15,
                                    VerticalOptions = LayoutOptions.Start,
                                    HorizontalOptions = LayoutOptions.Start,
                                    TextColor = Color.Black,
                                    FontFamily = "NunitoRegular",
                                    Text = "Отсутствуют"
                        }

                    }

                    });

                }
                else
                {
                    foreach (var item in carValue.Repairedparts)
                    {
                        stackRepair.Children.Add(new StackLayout
                        {
                            VerticalOptions = LayoutOptions.Start,
                            Orientation = StackOrientation.Horizontal,
                            Children =
                    {
                        new BoxView
                        {
                            BackgroundColor = Color.Red,
                             VerticalOptions = LayoutOptions.Start,
                             HorizontalOptions = LayoutOptions.Start,
                             HeightRequest = 8,
                             Margin = new Thickness(0,7,0,0),
                             WidthRequest = 8,
                             CornerRadius = 50
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                new Label
                                {
                                    FontSize = 15,
                                    VerticalOptions = LayoutOptions.Start,
                                    TextColor = Color.Black,
                                    HorizontalOptions = LayoutOptions.Start,
                                    FontFamily = "NunitoRegular",
                                    Text = string.Format("Деталь: {0}",item.Detail.Name)
                                },
                                   new Label
                                {
                                    FontSize = 15,
                                    VerticalOptions = LayoutOptions.Start,
                                     TextColor = Color.Black,
                                    HorizontalOptions = LayoutOptions.Start,
                                    FontFamily = "NunitoRegular",
                                    Text = string.Format("Описание: {0}",item.Description)
                                }
                            }
                        }
                    }
                        });
                    }
                }

                if (carValue.AniliticId == null)
                {
                    analitLabelFirst.IsVisible = false;
                    analitLabelSecond.IsVisible = false;
                }
                else
                {
                    analitLabelFirst.IsVisible = true;
                    analitLabelSecond.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }

        private async void GoBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        private async void GoEdit_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new VehicleReportPage(carValue, ci), false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }

        private async void Reserve_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RezervePage(carValue), false);
        }

        private async void Cancle_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (await App.Current.MainPage.DisplayAlert("Внимание", $"Вы уверены, что хотите отклонить автомобиле", "Нет", "Да") == false)
                {
                    carValue.StatusId = 3;
                    carValue.Status = null;
                    int indexCar = carValue.idCar;
                    string z = string.Format("{0}/Cars/{1}", MainInfoClass.apiDBSource, indexCar);
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        string json = JsonConvert.SerializeObject(carValue);
                        wc.UploadString(z, WebRequestMethods.Http.Put, json);
                    }
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }

        private async void Done_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CarApprovalPage(BindingContext as Car), false);
        }

        private async void CloseRezer_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (await App.Current.MainPage.DisplayAlert("Внимание", $"Вы уверены, что хотите отменить бронь", "Нет", "Да") == false)
                {
                    int indx = carValue.Booking.Last().IdBooking;
                    string z = string.Format("{0}/Bookings/{1}", MainInfoClass.apiDBSource, indx);
                    using (WebClient client = new WebClient())
                    {

                        client.UploadString(z, "DELETE", "");
                    }
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }

        private async void Sell_Tapped(object sender, EventArgs e)
        {
            try
            {
                carValue.StatusId = 4;
                carValue.Status = null;
                int indexCar = carValue.idCar;
                string z = string.Format("{0}/Cars/{1}", MainInfoClass.apiDBSource, indexCar);
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string json = JsonConvert.SerializeObject(carValue);
                    wc.UploadString(z, WebRequestMethods.Http.Put, json);
                }

                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            mainImage.Source = (sender as Image).Source;
        }

        private async void CallPhone_Tapped(object sender, EventArgs e)
        {
            try
            {
                var phoneDialer = CrossMessaging.Current.PhoneDialer;
                if (phoneDialer.CanMakePhoneCall)
                    phoneDialer.MakePhoneCall(carValue.BookingLast.Phone);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }

        private async void Deleted_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (await App.Current.MainPage.DisplayAlert("Внимание", $"Вы уверены, что хотите удалить информацию об автомобиле", "Нет", "Да") == false)
                {
                    carValue.StatusId = 3;
                    carValue.Status = null;
                    using (WebClient client = new WebClient())
                    {
                        string json = JsonConvert.SerializeObject(carValue);
                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        string z = string.Format("{0}/Cars/{1}", MainInfoClass.apiDBSource, carValue.idCar);
                        client.UploadString(z, WebRequestMethods.Http.Put, json);
                    }
                     (ci as IRefreshListCar).RefreshData();
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }
    }
}