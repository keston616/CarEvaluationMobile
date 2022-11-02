
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using Newtonsoft.Json;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewItemPage : ContentPage
    {
        private int valueType;
        VehicleReportPage vhp = new VehicleReportPage(null);
        List<Model> models;
        public AddNewItemPage(int z, VehicleReportPage vhp)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            valueType = z;
            this.vhp = vhp;

            OnLoad();
        }


        private async void OnLoad()
        {
            try
            {
                if (valueType == 1)
                {
                    AppBar.Text = "Добавление новой марки";
                    fieldEntry.Placeholder = "Введите название марки";
                }
                else if (valueType == 2)
                {
                    AppBar.Text = "Добавление новой модели";
                    fieldEntry.Placeholder = "Введите название модели";
                    brandEdit.IsVisible = true;
                    using (var client = new WebClient())
                    {
                        string responze = "";
                        responze = client.DownloadString(string.Format("{0}/Models", MainInfoClass.apiDBSource));
                        models = JsonConvert.DeserializeObject<List<Model>>(responze);
                    }
                    brandPicker.ItemsSource = vhp.brandCollection.ToList();
                }
                else if (valueType == 3)
                {
                    AppBar.Text = "Добавление нового цвета";
                    fieldEntry.Placeholder = "Введите название цвета";
                }
                else if (valueType == 4)
                {
                    AppBar.Text = "Добавление новой комплектации";
                    fieldEntry.Placeholder = "Введите название комплектации";
                }
                else if (valueType == 5)
                {
                    AppBar.Text = "Добавление нового поколения";
                    fieldEntry.Placeholder = "Введите название поколения";
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

        private async void GoSave_Tapped(object sender, EventArgs e)
        {
            try
            {
                using (var client = new WebClient())
                {
                    if (valueType == 1)
                    {
                        string apiBD = string.Format("{0}/Brands", MainInfoClass.apiDBSource);
                        var responze = client.DownloadString(apiBD);
                        List<Brand> brands = JsonConvert.DeserializeObject<List<Brand>>(responze);

                        if (brands.ToList().Where(x => x.Name.Trim().ToUpper() == fieldEntry.Text.Trim().ToUpper()).FirstOrDefault() != null)
                        {
                            labelWarning.Text = "Данная марка уже существует";
                            labelWarning.IsVisible = true;
                            return;
                        }
                        else
                        {
                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            string html = client.UploadString(apiBD, JsonConvert.SerializeObject(new Brand { Name = fieldEntry.Text }));
                            var newBrand = JsonConvert.DeserializeObject<Brand>(html);
                            vhp.brandCollection.Add(newBrand);
                        }
                    }
                    else if (valueType == 2)
                    {
                        if (brandPicker.SelectedItem == null)
                        {
                            brandWarning.IsVisible = true;
                            return;
                        }

                        if (models.Where(x => x.Name.Trim().ToUpper() == fieldEntry.Text.Trim().ToUpper()).FirstOrDefault() != null)
                        {
                            labelWarning.Text = "Данная модель уже существует";
                            labelWarning.IsVisible = true;
                            return;
                        }
                        else
                        {
                            Brand brandValue = brandPicker.SelectedItem as Brand;

                            Model newModel = new Model { Name = fieldEntry.Text, BrandId = brandValue.idBrand };
                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            string html = client.UploadString(string.Format("{0}/Models", MainInfoClass.apiDBSource), JsonConvert.SerializeObject(newModel));
                            Model eq = JsonConvert.DeserializeObject<Model>(html);
                            if (brandValue.Model == null)
                            {
                                brandValue.Model = new List<Model>();
                            }
                            brandValue.Model.Add(eq);

                            if (vhp.modelCollection.Count != 0 && (vhp.modelCollection.FirstOrDefault() as Model).BrandId == (brandPicker.SelectedItem as Brand).idBrand)
                            {
                                vhp.modelCollection.Add(eq);
                            }
                        }
                    }
                    else if (valueType == 3)
                    {
                        string apiBD = string.Format("{0}/Colors", MainInfoClass.apiDBSource);
                        var responze = client.DownloadString(apiBD);
                        List<ColorCar> colors = JsonConvert.DeserializeObject<List<ColorCar>>(responze);
                        if (colors.Where(x => x.Name.Trim().ToUpper() == fieldEntry.Text.Trim().ToUpper()).FirstOrDefault() != null)
                        {
                            labelWarning.Text = "Данный цвет уже существует";
                            labelWarning.IsVisible = true;
                            return;
                        }
                        else
                        {
                            ColorCar сColor = new ColorCar { Name = fieldEntry.Text.ToString() };
                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            string html = client.UploadString(apiBD, JsonConvert.SerializeObject(сColor));
                            vhp.colorCollection.Add(JsonConvert.DeserializeObject<ColorCar>(html));
                        }
                    }
                    else if (valueType == 4)
                    {
                         string apiBD = string.Format("{0}/Equipments", MainInfoClass.apiDBSource);
                        var responze = client.DownloadString(apiBD);
                        List<Equipment> equipments = JsonConvert.DeserializeObject<List<Equipment>>(responze);
                        if (equipments.Where(x => x.Name.Trim().ToUpper() == fieldEntry.Text.Trim().ToUpper()).FirstOrDefault() != null)
                        {
                            labelWarning.Text = "Данная комплектация уже существует";
                            labelWarning.IsVisible = true;
                            return;
                        }
                        else
                        {

                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            string html = client.UploadString(apiBD, JsonConvert.SerializeObject(new Equipment { Name = fieldEntry.Text }));
                            vhp.equipmentCollection.Add(JsonConvert.DeserializeObject<Equipment>(html));
                        }
                    }
                    else if (valueType == 5)
                    {
                        string apiBD = string.Format("{0}/Generations", MainInfoClass.apiDBSource);
                        var responze = client.DownloadString(apiBD);
                        List<Generation> generations = JsonConvert.DeserializeObject<List<Generation>>(responze);
                        if (generations.ToList().Where(x => x.Name.Trim().ToUpper() == fieldEntry.Text.Trim().ToUpper()).FirstOrDefault() != null)
                        {
                            labelWarning.Text = "Данное поколение уже существует";
                            labelWarning.IsVisible = true;
                            return;
                        }
                        else
                        {
                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            string html = client.UploadString(apiBD, JsonConvert.SerializeObject(new Generation { Name = fieldEntry.Text }));
                            vhp.generationCollection.Add(JsonConvert.DeserializeObject<Generation>(html));
                        }
                    }
                }
                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }

        }

        private void fieldEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            labelWarning.IsVisible = false;
        }
    }
}