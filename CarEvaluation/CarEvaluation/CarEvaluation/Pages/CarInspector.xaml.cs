
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.ObjectModel;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarInspector : ContentPage, IRefreshListCar
    {
        internal List<Car> carValue = new List<Car>();
        internal List<Car> carAll = new List<Car>();
        internal ObservableRangeCollection<Car> carObc;
        List<Brand> brandValue;
        int loadValue = 0;
        string apiBDCars = string.Format("{0}/Cars", MainInfoClass.apiDBSource);
        string apiBDBrands = string.Format("{0}/Brands", MainInfoClass.apiDBSource);
        int ndx = 4;
        public CarInspector()
        {
            InitializeComponent();
            LoadCar();
            carValue = carValue.OrderByDescending(x => x.DateBuy).ToList();
            refreshView.Command = RefreshCommand;

            foreach (var item in carValue)
            {
                item.Model.Brand = brandValue.Where(x => x.idBrand == item.Model.BrandId).FirstOrDefault();
            }
        }

        private async void LoadCar()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var responze = client.DownloadString(apiBDCars);
                    carValue = JsonConvert.DeserializeObject<List<Car>>(responze);
                    var responzeSecond = client.DownloadString(apiBDBrands);
                    brandValue = JsonConvert.DeserializeObject<List<Brand>>(responzeSecond);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", $"Отсутствует подключение к интернету", "ОК");
            }
        }

        public CarInspector(bool z) : this()
        {
            GetCarAnalit();
            loadValue = 1;
            titleApp.Text = "Ожидают оценку";
        }
        public CarInspector(bool z, int r) : this()
        {
            GetCarActivInspect();
            loadValue = 2;
            titleApp.Text = "Активные осмотры";
            AddButton.IsVisible = true;
        }

        public CarInspector(int r, bool z) : this()
        {
            GetCarInspect();
            loadValue = 6;
            titleApp.Text = "История осмотров";
        }

        public CarInspector(int z) : this()
        {
            GetCar();
            loadValue = 3;
            titleApp.Text = "Автомобили";
        }
        public CarInspector(string z) : this()
        {
            GetCarManager();
            loadValue = 4;
            listCarReserv.IsVisible = true;
            listCar.IsVisible = false;
            titleApp.Text = "Забронированные авто";
        }

        public CarInspector(string z, bool r) : this()
        {
            GetCarHistory();
            loadValue = 5;
            titleApp.Text = "История оценок";
        }

        public CarInspector(string z, int r) : this()
        {
            GetCarOther();
            loadValue = 7;
            titleApp.Text = "Другие";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        void GetCarOther()
        {
            carAll = carValue.Where(x => x.StatusId == 3 || x.StatusId == 4).ToList();
            listCar.ItemsSource = carObc = new ObservableRangeCollection<Car>(carAll.Take(ndx));
        }

        void GetCarHistory()
        {
            carAll = carValue.Where(x => x.AniliticId == MainInfoClass.user.idUser).ToList();
            listCar.ItemsSource = carObc = new ObservableRangeCollection<Car>(carAll.Take(ndx));
        }

        void GetCar()
        {
            carAll = carValue.Where(x => x.StatusId == 2 && x.BookingDate < DateTime.Now).ToList();
            listCar.ItemsSource = carObc = new ObservableRangeCollection<Car>(carAll.Take(ndx));
        }

        void GetCarAnalit()
        {
            carAll = carValue.Where(x => x.StatusId == 1).ToList();
            listCar.ItemsSource = carObc = new ObservableRangeCollection<Car>(carAll.Take(ndx));
        }

        void GetCarActivInspect()
        {
            carAll = carValue.Where(x => x.InspectorId == MainInfoClass.user.idUser && x.StatusId == 1).ToList();
            listCar.ItemsSource = carObc = new ObservableRangeCollection<Car>(carAll.Take(ndx));
        }

        void GetCarInspect()
        {
            carAll = carValue.Where(x => x.InspectorId == MainInfoClass.user.idUser && x.StatusId != 1).ToList();
            listCar.ItemsSource = carObc = new ObservableRangeCollection<Car>(carAll.Take(ndx));
        }

        void GetCarManager()
        {
            if (MainInfoClass.user.RoleId == 3)
            {
                carAll = carValue.Where(x => x.StatusId == 2 && x.BookingDate > DateTime.Now && x.BookingLast.UserId == MainInfoClass.user.idUser).ToList();
                listCarReserv.ItemsSource = carValue.Where(x => x.StatusId == 2 && x.BookingDate > DateTime.Now && x.BookingLast.UserId == MainInfoClass.user.idUser);
            }
            else
            {
                listCarReserv.ItemsSource = carValue.Where(x => x.StatusId == 2 && x.BookingDate > DateTime.Now);
            }
        }

        public ICommand RefreshCommand => new Command(async () => await RefreshData());

        public async Task RefreshData()
        {
            try
            {
                refreshView.IsRefreshing = true;
                using (var client = new WebClient())
                {
                    string responze = "";
                    await Task.Run(() => responze = client.DownloadString(apiBDCars));
                    carValue = JsonConvert.DeserializeObject<List<Car>>(responze);
                    var responzeSecond = "";
                    await Task.Run(() => responzeSecond = client.DownloadString(apiBDBrands));
                    brandValue = JsonConvert.DeserializeObject<List<Brand>>(responzeSecond);
                }

                foreach (var item in carValue)
                {
                    item.Model.Brand = brandValue.Where(x => x.idBrand == item.Model.BrandId).FirstOrDefault();
                }
                carValue = carValue.OrderByDescending(x => x.DateBuy).ToList();

                if (loadValue == 1)
                {
                    GetCarAnalit();
                }
                else if (loadValue == 2)
                {
                    GetCarActivInspect();
                }
                else if (loadValue == 3)
                {
                    GetCar();
                }
                else if (loadValue == 4)
                {
                    GetCarManager();
                }
                else if (loadValue == 5)
                {
                    GetCarHistory();
                }
                else if (loadValue == 6)
                {
                    GetCarInspect();
                }
                else if (loadValue == 7)
                {
                    GetCarOther();
                }
                refreshView.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                refreshView.IsRefreshing = false;
                DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }
        }

        private void InsertCarInspector_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VehicleReportPage(this), false);
        }

        private async void GoInfoCar_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoCar((sender as Frame).BindingContext as Car, this), false);
        }

        private void search_Focused(object sender, FocusEventArgs e)
        {
            searchFrame.BorderColor = Color.FromHex("2D4B7F");
            searchIcon.Fill = new SolidColorBrush(Color.FromHex("2D4B7F"));
        }

        private void search_Unfocused(object sender, FocusEventArgs e)
        {
            if (searchEntry.Text == null || searchEntry.Text.ToString().Length == 0)
            {
                searchFrame.BorderColor = Color.LightGray;
                searchIcon.Fill = Brush.LightGray;
            }
        }

        private void searchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Car> cars = new List<Car>();
            carAll = carValue.Where(x => x.GosNumber.Contains(searchEntry.Text.ToUpper())).ToList();
            carObc = new ObservableRangeCollection<Car>(carAll.Take(ndx));
            if (loadValue == 4)
            {
                listCarReserv.ItemsSource = carObc;
            }
            else
            {
                listCar.ItemsSource = carObc;
            }
        }

        private void listCar_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            try
            {
                if (carAll.Count >= ndx)
                {
                    carAll.RemoveRange(0, ndx);
                    if (carAll.Count >= ndx)
                    {
                        carObc.AddRange(carAll.Take(4));
                    }
                    else carObc.AddRange(carAll.Take(carAll.Count));
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }
    }
}