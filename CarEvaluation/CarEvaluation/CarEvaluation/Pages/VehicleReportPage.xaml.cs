using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using RestSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Print.PrintAttributes;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VehicleReportPage : ContentPage
    {
        List<Editor> repairInfo = new List<Editor>();
        List<Picker> detailInfo = new List<Picker>();
        internal ObservableCollection<Brand> brandCollection;
        internal ObservableCollection<Model> modelCollection = new ObservableCollection<Model>();
        internal ObservableCollection<ColorCar> colorCollection;
        internal ObservableCollection<Equipment> equipmentCollection;
        internal ObservableCollection<Generation> generationCollection;
        List<MediaFile> pathImage;
        ObservableCollection<PhotoCar> newPhoto = new ObservableCollection<PhotoCar>();
        List<PhotoCar> oldPhoto = new List<PhotoCar>();
        List<PhotoCar> delPhoto = new List<PhotoCar>();
        Car carNew;
        ObservableCollection<Detail> detailList;
        CarInspector ci;
        public VehicleReportPage(CarInspector ci)
        {

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Loaded();
            this.ci = ci;
            carImages.ItemsSource = newPhoto;
        }

        public VehicleReportPage(Car car, CarInspector ci) : this(ci)
        {
            carNew = car;
            EditLoad();
        }

        private async void EditLoad()
        {
            try
            {
                Car car = carNew;
                vinPicker.Text = carNew.VIN;
                gosNumberPicker.Text = carNew.GosNumber;
                bodyPicker.SelectedItem = (bodyPicker.ItemsSource as List<Body>).Where(x => x.idBody == car.BodyId).FirstOrDefault();
                drivePicker.SelectedItem = (drivePicker.ItemsSource as List<Drive>).Where(x => x.idDrive == car.DriverId).FirstOrDefault();
                colorPicker.SelectedItem = colorCollection.Where(x => x.idColor == car.ColorId).FirstOrDefault();
                equipmentPicker.SelectedItem = equipmentCollection.Where(x => x.idEquipment == car.EquipmentId).FirstOrDefault();
                enginePicker.SelectedItem = (enginePicker.ItemsSource as List<TypeEngine>).Where(x => x.idTypeEngine == car.TypeEngineId).FirstOrDefault();
                fuelPicker.SelectedItem = (fuelPicker.ItemsSource as List<FuelType>).Where(x => x.idFuelType == car.FuelTypeId).FirstOrDefault();
                generationPicker.SelectedItem = generationCollection.Where(x => x.idGeneration == car.GenerationId).FirstOrDefault();
                brandPicker.SelectedItem = brandCollection.Where(x => x.idBrand == car.Model.BrandId).FirstOrDefault();
                modelPicker.SelectedItem = (brandPicker.SelectedItem as Brand).Model.Where(x => x.idModel == car.ModelId).FirstOrDefault();
                engineCapacityPicker.Text = car.EngineCapacity.ToString();
                powerEntry.Text = car.PowerCapacity.ToString();
                mileageEntry.Text = car.Mileage.ToString();
                yearEntry.Text = car.YearOfRealease.ToString();
                countOwnerPicker.Text = car.CountOwner.ToString();
                noteEntry.Text = car.Description;
                compressionPicker.Text = car.CompressionMotor;
                transmissPicker.SelectedItem = (transmissPicker.ItemsSource as List<TypeTransmission>).Where(x => x.idTypeTransmission == car.TypeTransmissionId).FirstOrDefault(); ;
                countRepairPicker.Text = car.Repairedparts.Count().ToString();
                int indexRepair = 1;
                foreach (var item in car.Repairedparts)
                {
                    countRepair(indexRepair.ToString(), item);
                    indexRepair++;
                }
                oldPhoto = car.PhotoCar;
                juriidicCheck.IsChecked = car.Juridical;
                newPhoto = new ObservableCollection<PhotoCar>(car.PhotoCar);
                carImages.ItemsSource = newPhoto;
                if (newPhoto.Count != 0)
                {
                    carImages.HeightRequest = 100;
                }
                if (car.IsOriginalPTC)
                {
                    ptcPicker.SelectedIndex = 0;
                }
                else
                {
                    ptcPicker.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }

        public async void Loaded()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var responze = client.DownloadString(string.Format("{0}/Details", MainInfoClass.apiDBSource));
                    detailList = JsonConvert.DeserializeObject<ObservableCollection<Detail>>(responze);
                    var responzeBrand = client.DownloadString(string.Format("{0}/Brands", MainInfoClass.apiDBSource));
                    brandCollection = new ObservableCollection<Brand>((JsonConvert.DeserializeObject<List<Brand>>(responzeBrand) as List<Brand>).OrderBy(x => x.Name));
                    brandPicker.ItemsSource = brandCollection;
                    modelPicker.ItemsSource = modelCollection;
                    var responzeColor = client.DownloadString(string.Format("{0}/Colors", MainInfoClass.apiDBSource));
                    colorCollection = new ObservableCollection<ColorCar>((JsonConvert.DeserializeObject<List<ColorCar>>(responzeColor) as List<ColorCar>).OrderBy(x => x.Name));
                    colorPicker.ItemsSource = colorCollection;
                    var responzeEquipment = client.DownloadString(string.Format("{0}/Equipments", MainInfoClass.apiDBSource));
                    equipmentCollection = new ObservableCollection<Equipment>((JsonConvert.DeserializeObject<List<Equipment>>(responzeEquipment) as List<Equipment>).OrderBy(x => x.Name));
                    equipmentPicker.ItemsSource = equipmentCollection;
                    var responzeGeneration = client.DownloadString(string.Format("{0}/Generations", MainInfoClass.apiDBSource));
                    generationCollection = new ObservableCollection<Generation>((JsonConvert.DeserializeObject<List<Generation>>(responzeGeneration) as List<Generation>).OrderBy(x => x.Name));
                    generationPicker.ItemsSource = generationCollection;
                    var responzeEngine = client.DownloadString(string.Format("{0}/TypeEngines", MainInfoClass.apiDBSource));
                    enginePicker.ItemsSource = JsonConvert.DeserializeObject<List<TypeEngine>>(responzeEngine);
                    var responzeTransmission = client.DownloadString(string.Format("{0}/TypeTransmissions", MainInfoClass.apiDBSource));
                    transmissPicker.ItemsSource = JsonConvert.DeserializeObject<List<TypeTransmission>>(responzeTransmission);
                    var responzeBody = client.DownloadString(string.Format("{0}/Bodies", MainInfoClass.apiDBSource));
                    bodyPicker.ItemsSource = JsonConvert.DeserializeObject<List<Body>>(responzeBody);
                    var responzeFuel = client.DownloadString(string.Format("{0}/FuelTypes", MainInfoClass.apiDBSource));
                    fuelPicker.ItemsSource = JsonConvert.DeserializeObject<List<FuelType>>(responzeFuel);
                    var responzeDrive = client.DownloadString(string.Format("{0}/Drives", MainInfoClass.apiDBSource));
                    drivePicker.ItemsSource = JsonConvert.DeserializeObject<List<Drive>>(responzeDrive);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
                Navigation.PopAsync();
            }

        }

        private async void GoBack_Tapped(object sender, EventArgs e)
        {
            string z = "добавление";
            if (carNew != null)
            {
                z = "редактирование";
            }
            if (await App.Current.MainPage.DisplayAlert("Внимание", $"Вы уверены, что хотите отменить {z}", "Нет", "Да") == false)
            {
                await Navigation.PopAsync(false);
            }
        }

        private async void GoSave_Tapped(object sender, EventArgs e)
        {
            try
            {
                bool war = true;
                Regex remGosnumRegex = new Regex(@"^[АВЕКМНОРСТУХ]\d{3}(?<!000)[АВЕКМНОРСТУХ]{2}\d{2,3}$");
                Regex yearRegex = new Regex(@"^(19|20)\d{2}$");
                Regex vinRegex = new Regex(@"^[(A-H|J-N|P|R-Z|0-9)]{17}$");
                Regex countOwnerRegex = new Regex(@"^(((1)[0-9])|([1-9]))$");
                Regex mileageRegex = new Regex(@"^[1-9][0-9]{0,5}$");
                Regex compressionRegex = new Regex(@"^(((([0]{1})((,)([0-9]{0,1}))?)||([1-3]{1}([0-9]{0,1})((,)([0-9]{0,1}))?))((\s){0,1})){1,12}$");
                Regex powerRegex = new Regex(@"^[1-9]([0-9]{1,2})$");
                if (String.IsNullOrWhiteSpace(vinPicker.Text) == true)
                {
                    vinPickerWarning.Text = "VIN номер не указан";
                    vinPickerWarning.IsVisible = true;
                    war = false;
                }
                else if (!vinRegex.IsMatch(vinPicker.Text))
                {
                    vinPickerWarning.Text = "Неверная форма. Пример: XTAGFL110MY463549";
                    vinPickerWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(gosNumberPicker.Text) == true)
                {
                    gosnumWarning.Text = "Гос номер не указан";
                    gosnumWarning.IsVisible = true;
                    war = false;
                }
                else if (!remGosnumRegex.IsMatch(gosNumberPicker.Text) == true)
                {
                    gosnumWarning.Text = "Неверный форма. Пример: А777АА777";
                    gosnumWarning.IsVisible = true;
                    war = false;
                }
                else if (ci.carValue.Where(x => x.GosNumber == gosNumberPicker.Text && x.StatusId == 2).FirstOrDefault() != null)
                {
                    gosnumWarning.Text = "Данный гос номер уже существует";
                    gosnumWarning.IsVisible = true;
                    war = false;
                }
                if (brandPicker.SelectedItem == null)
                {
                    brandWarning.IsVisible = true;
                    war = false;
                }
                if (modelPicker.SelectedItem == null)
                {
                    modelWarning.IsVisible = true;
                    war = false;
                }
                if (generationPicker.SelectedItem == null)
                {
                    generationWarning.IsVisible = true;
                    war = false;
                }
                if (transmissPicker.SelectedItem == null)
                {
                    transmissWarning.IsVisible = true;
                    war = false;
                }
                if (enginePicker.SelectedItem == null)
                {
                    engineWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(engineCapacityPicker.Text) == true)
                {
                    engineCapacityWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(powerEntry.Text) == true)
                {
                    powerWarning.IsVisible = true;
                    war = false;
                }
                else if (!powerRegex.IsMatch(powerEntry.Text))
                {
                    powerWarning.Text = "Заполните поле корректно";
                    powerWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(mileageEntry.Text) == true)
                {
                    mileageWarning.IsVisible = true;
                    war = false;
                }
                else if (!mileageRegex.IsMatch(mileageEntry.Text))
                {
                    mileageWarning.Text = "Заполните поле корректно";
                    mileageWarning.IsVisible = true;
                    war = false;
                }
                if (colorPicker.SelectedItem == null)
                {
                    colorWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(countOwnerPicker.Text) == true)
                {
                    countOwnerWarning.Text = "Кол-во владельцев не указано";
                    countOwnerWarning.IsVisible = true;
                    war = false;
                }
                else if (!countOwnerRegex.IsMatch(countOwnerPicker.Text))
                {
                    countOwnerPicker.Text = "Заполните поле корректно";
                    countOwnerWarning.IsVisible = true;
                    war = false;
                }
                if (bodyPicker.SelectedItem == null)
                {
                    bodyWarning.IsVisible = true;
                    war = false;
                }
                if (equipmentPicker.SelectedItem == null)
                {
                    equipmentWarning.IsVisible = true;
                    war = false;
                }
                if (drivePicker.SelectedItem == null)
                {
                    driveWarning.IsVisible = true;
                    war = false;
                }
                if (fuelPicker.SelectedItem == null)
                {
                    fuelWarning.IsVisible = true;
                    war = false;
                }
                if (ptcPicker.SelectedItem == null)
                {
                    ptcWarning.IsVisible = true;
                    war = false;
                }

                if (String.IsNullOrWhiteSpace(yearEntry.Text) == true)
                {
                    yearWarning.Text = "Год выпуска не указан";
                    yearWarning.IsVisible = true;
                    war = false;
                }
                else if (!yearRegex.IsMatch(yearEntry.Text) || Convert.ToInt32(yearEntry.Text) > 2022)
                {
                    yearWarning.Text = "Заполните поле корректно";
                    yearWarning.IsVisible = true;
                    war = false;
                }
                if (String.IsNullOrWhiteSpace(compressionPicker.Text) == true)
                {
                    compressionWarning.IsVisible = true;
                    war = false;
                }
                else if (!compressionRegex.IsMatch(compressionPicker.Text))
                {
                    compressionWarning.Text = "Заполните поле корректно. Пример: 10 10 10 10";
                    compressionWarning.IsVisible = true;
                    war = false;
                }
                detailInfo.ForEach(x =>
                {
                    if (x.SelectedItem == null)
                    {
                        repairDetailWarning.IsVisible = true;
                        war = false;
                    }
                });

                if (repairDetailWarning.IsVisible == false) repairInfo.ForEach(x =>
                 {
                     if (String.IsNullOrWhiteSpace(x.Text) == true)
                     {
                         repairDetailWarning.IsVisible = true;
                         war = false;
                     }
                 });
                if (newPhoto.Count < 4 || newPhoto.Count > 10)
                {
                    photoWarning.IsVisible = true;
                    war = false;
                }
                if (war)
                {
                    if (carNew == null)
                    {
                        carNew = new Car();
                    }
                    Car carVal = (Car)carNew.Clone();
                    carNew.Juridical = juriidicCheck.IsChecked;
                    carNew.ModelId = (modelPicker.SelectedItem as Model).idModel;
                    carNew.YearOfRealease = Convert.ToInt32(yearEntry.Text);
                    carNew.EquipmentId = (equipmentPicker.SelectedItem as Equipment).idEquipment;
                    carNew.StatusId = 1;
                    carNew.VIN = vinPicker.Text;
                    carNew.GosNumber = gosNumberPicker.Text;
                    carNew.BodyId = (bodyPicker.SelectedItem as Body).idBody;
                    carNew.Mileage = Convert.ToInt32(mileageEntry.Text);
                    carNew.EngineCapacity = Math.Round(Convert.ToDecimal(engineCapacityPicker.Text), 1);
                    carNew.DriverId = (drivePicker.SelectedItem as Drive).idDrive;
                    carNew.ColorId = (colorPicker.SelectedItem as ColorCar).idColor;
                    carNew.CountOwner = Convert.ToInt32(countOwnerPicker.Text);
                    carNew.TypeTransmissionId = (transmissPicker.SelectedItem as TypeTransmission).idTypeTransmission;
                    carNew.GenerationId = (generationPicker.SelectedItem as Generation).idGeneration;
                    carNew.TypeEngineId = (enginePicker.SelectedItem as TypeEngine).idTypeEngine;
                    carNew.FuelTypeId = (fuelPicker.SelectedItem as FuelType).idFuelType;
                    carNew.CompressionMotor = compressionPicker.Text;
                    carNew.Description = noteEntry.Text;
                    carNew.PowerCapacity = Convert.ToInt32(powerEntry.Text);
                    List<RepairedParts> repParts = new List<RepairedParts>();
                    if (carNew.idCar != default(int) && carNew.Repairedparts.Count != 0)
                    {
                        using (WebClient client = new WebClient())
                        {

                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            foreach (var item in carNew.Repairedparts)
                            {
                                string z = string.Format("{0}/RepairedParts/{1}", MainInfoClass.apiDBSource, item.idRepairedParts);
                                client.UploadString(z, "DELETE", "");
                            }

                        }
                    }
                    if (detailInfo.Count() != 0)
                    {
                        carNew.Repairedparts = new List<RepairedParts>();
                        foreach (var item in detailInfo)
                        {
                            RepairedParts repValue = new RepairedParts { DetailId = ((Detail)item.SelectedItem).idDetail, Description = repairInfo.First().Text };
                            if (carNew.idCar != default(int))
                            {
                                repValue.CarId = carNew.idCar;
                                using (WebClient client = new WebClient())
                                {
                                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                    var json = JsonConvert.SerializeObject(repValue);
                                    string HtmlResult = client.UploadString(string.Format("{0}/RepairedParts", MainInfoClass.apiDBSource), json);
                                    repValue = JsonConvert.DeserializeObject<RepairedParts>(HtmlResult);
                                    repValue.Detail = ((Detail)item.SelectedItem);
                                }
                            }
                            carNew.Repairedparts.Add(repValue);
                            Editor newEditor = repairInfo.First();
                            repairInfo.Remove(newEditor);
                        }
                    }
                    if (ptcPicker.SelectedIndex == 0)
                    {
                        carNew.IsOriginalPTC = true;
                    }
                    else
                    {
                        carNew.IsOriginalPTC = false;
                    }
                    if (carNew.idCar == default(int))
                    {
                        carNew.InspectorId = MainInfoClass.user.idUser; ;
                        carNew.DateBuy = DateTime.Now;
                    }

                    if (delPhoto != null)
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            foreach (var item in (delPhoto.Intersect(oldPhoto)).ToList())
                            {
                                carNew.PhotoCar.Remove(item);
                                string z = string.Format("{0}/PhotoCars/{1}", MainInfoClass.apiDBSource, item.idPhotoCar);
                                client.UploadString(z, "DELETE", "");
                            }
                        }
                    }

                    List<PhotoCar> pcAdd = newPhoto.ToList();
                    pcAdd = pcAdd.Except(oldPhoto).ToList();
                    if (pcAdd != null)
                    {
                        foreach (var item in pcAdd)
                        {
                            string fullApiImage = MainInfoClass.apiImageSource + "/api/ImageUpload";
                            var restClient = new RestClient(fullApiImage);
                            restClient.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddFile("file", item.Picture);
                            IRestResponse response = restClient.Execute(request);
                            string imageAddress = MainInfoClass.apiImageSource + response.Content.Replace("\"", "");
                            PhotoCar valuePhoto = new PhotoCar { Picture = imageAddress };
                            if (carNew.idCar != default(int))
                            {
                                valuePhoto.CarId = carNew.idCar;
                                using (WebClient client = new WebClient())
                                {
                                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                    var json = JsonConvert.SerializeObject(valuePhoto);
                                    string HtmlResult = client.UploadString(string.Format("{0}/PhotoCars", MainInfoClass.apiDBSource), json);
                                    valuePhoto = JsonConvert.DeserializeObject<PhotoCar>(HtmlResult);
                                }
                            }
                            carNew.PhotoCar.Add(valuePhoto);
                        }
                    }

                    using (WebClient client = new WebClient())
                    {

                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                        string json = JsonConvert.SerializeObject(carNew, Formatting.None,
                         new JsonSerializerSettings()
                         {
                             ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                         });
                        if (carNew.idCar == default(int))
                        {
                            client.UploadString(string.Format("{0}/Cars", MainInfoClass.apiDBSource), json);
                            (ci as IRefreshListCar).RefreshData();
                        }
                        else
                        {
                            int indexCar = carNew.idCar;
                            string z = string.Format("{0}/Cars/{1}", MainInfoClass.apiDBSource, indexCar);
                            client.UploadString(z, WebRequestMethods.Http.Put, json);
                        }
                    }
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", ex.Message, "ОК");
            }

        }

        private void DeletedImage_Tapped(object sender, EventArgs e)
        {
            newPhoto.Remove((sender as Frame).BindingContext as PhotoCar);
            delPhoto.Add((sender as Frame).BindingContext as PhotoCar);
            if (newPhoto.Count == 0)
            {
                carImages.HeightRequest = 0;
            }
        }



        private async void ImageInsert_Tapped(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                pathImage = await CrossMedia.Current.PickPhotosAsync();
                if (pathImage != null)
                {
                    foreach (var item in pathImage)
                    {
                        PhotoCar newPC = new PhotoCar { Picture = item.Path };
                        newPhoto.Add(newPC);
                    }
                    carImages.HeightRequest = 100;
                    photoWarning.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }

        private void countRepair(string value, RepairedParts rpValue)
        {
            try
            {
                stacKH.Children.Add(new Label
                {
                    Text = $"Ремонтированная деталь №{value}",
                    TextColor = Color.Gray,
                    FontSize = 16,
                    Margin = new Thickness(0, 10, 0, 0),
                    FontFamily = "NunitoRegular"
                });
                Editor entry = new Editor
                {
                    Placeholder = "Описание ремонта",
                    AutoSize = EditorAutoSizeOption.TextChanges,
                    TextColor = Color.Black,
                    Text = rpValue.Description,
                    Margin = new Thickness(-4, 4, 0, 5),
                    FontSize = 18,
                    FontFamily = "NunitoRegular"
                };
                Picker detail = new Picker
                {
                    Title = $"Кузовная деталь №{value}",
                    TextColor = Color.Black,
                    ItemsSource = detailList,
                    ItemDisplayBinding = new Binding("Name"),
                    Margin = new Thickness(-4, 4, 0, 0)
                };

                detail.SelectedIndexChanged += detailSelect;
                detail.Focused += detailFocus;
                detail.Unfocused += detailUnfocus;
                if (!string.IsNullOrWhiteSpace(rpValue.Description))
                {
                    Detail dt = detailList.Where(x => x.idDetail == rpValue.Detail.idDetail).FirstOrDefault();
                    detail.SelectedItem = dt;
                }
                detailInfo.Add(detail);
                repairInfo.Add(entry);
                stacKH.Children.Add(detail);
                stacKH.Children.Add(entry);
            }
            catch (Exception ex)
            {
                DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }
        Detail detInfoNew;

        private void detailFocus(object sender, EventArgs e)
        {
            List<Detail> det = detailList.ToList();
            if ((sender as Picker).BindingContext as Detail != null)
            {
                det.Add((sender as Picker).BindingContext as Detail);
            }
            (sender as Picker).ItemsSource = det.OrderBy(x => x.idDetail).ToList();
        }

        private void detailSelect(object sender, EventArgs e)
        {
            if ((sender as Picker).SelectedItem != null)
            {
                detInfoNew = (sender as Picker).SelectedItem as Detail;
                if ((sender as Picker).BindingContext != null)
                {
                    detailList.Add((sender as Picker).BindingContext as Detail);
                }
                (sender as Picker).BindingContext = detInfoNew;
                detailList.Remove(detInfoNew);
            }
        }

        private void detailUnfocus(object sender, EventArgs e)
        {
            if ((sender as Picker).SelectedItem == null)
            {
                (sender as Picker).SelectedItem = (sender as Picker).BindingContext as Detail;
            }
        }

        private void vinPicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            vinPickerWarning.IsVisible = false;
        }

        private void gosNumberPicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            gosnumWarning.IsVisible = false;
        }

        private async void brandPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                brandWarning.IsVisible = false;
                modelCollection.Clear();
                if ((brandPicker.SelectedItem as Brand).Model != null)
                {
                    (brandPicker.SelectedItem as Brand).Model.ToList().ForEach(x =>
                    {
                        modelCollection.Add(x);
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }

        private void modelPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelWarning.IsVisible = false;
        }

        private void transmissPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            transmissWarning.IsVisible = false;
        }

        private void enginePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            engineWarning.IsVisible = false;
        }

        private void engineCapacityPicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            engineCapacityWarning.IsVisible = false;
        }

        private void colorPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            colorWarning.IsVisible = false;
        }

        private void countOwnerPicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            countOwnerWarning.IsVisible = false;
        }

        private void bodyPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            bodyWarning.IsVisible = false;
        }

        private void equipmentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            equipmentWarning.IsVisible = false;
        }

        private void fuelPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            fuelWarning.IsVisible = false;
        }

        private void drivePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            driveWarning.IsVisible = false;
        }

        private void ptcPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ptcWarning.IsVisible = false;
        }

        private void compressionPicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            compressionWarning.IsVisible = false;
        }

        private void countRepairPicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            countOwnerWarning.IsVisible = false;
        }

        private async void GoAddBrand_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddNewItemPage(1, this), false);
        }

        private async void GoAddModel_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddNewItemPage(2, this), false);
        }

        private async void GoAddColor_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddNewItemPage(3, this), false);
        }

        private async void GoAddEquipment_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddNewItemPage(4, this), false);
        }

        private void powerEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            powerWarning.IsVisible = false;
        }

        private async void GoAddGeneration_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddNewItemPage(5, this), false);
        }

        private void generationPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            generationWarning.IsVisible = false;
        }

        private void mileageEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            mileageWarning.IsVisible = false;
        }

        private void yearEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            yearWarning.IsVisible = false;
        }

        private void MinusCountRepairDetail_Tapped(object sender, EventArgs e)
        {
            if (Convert.ToInt32(countRepairPicker.Text) != 0)
            {
                countRepairPicker.Text = (Convert.ToInt32(countRepairPicker.Text) - 1).ToString();
                List<View> st = stacKH.Children.ToList();
                stacKH.Children.Remove(st.Last());
                View vi1 = st.Last();
                st.Remove(vi1);
                stacKH.Children.Remove(st.Last());
                View vi2 = st.Last();
                st.Remove(vi2);
                stacKH.Children.Remove(st.Last());
                repairInfo.Remove(repairInfo.Last());
                Detail det = (detailInfo.Last() as Picker).BindingContext as Detail;
                detailInfo.Remove(detailInfo.Last());
                if (det != null)
                {
                    detailList.Add(det);
                }
            }

        }

        private void AddCountRepairDetail_Tapped(object sender, EventArgs e)
        {
            countRepairPicker.Text = (Convert.ToInt32(countRepairPicker.Text) + 1).ToString();
            countRepair(countRepairPicker.Text, new RepairedParts());

        }
    }
}