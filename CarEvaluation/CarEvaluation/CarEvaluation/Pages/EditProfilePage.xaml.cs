using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePage : ContentPage
    {
        FileResult photo;
        bool isImageDel = false;
        public EditProfilePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            surnameEntry.Text = MainInfoClass.user.Surname;
            nameEntry.Text = MainInfoClass.user.Name;
            middleNameEntry.Text = MainInfoClass.user.MiddleName;
            phoneEntry.Text = MainInfoClass.user.Phone;
            personIcon.BindingContext = MainInfoClass.user;
            if (string.IsNullOrEmpty( MainInfoClass.user.Photo) && photo == null)
            {
                delFrame.IsVisible = false;
            }
        }

        private async void GoSave_Tapped(object sender, EventArgs e)
        {
            try
            {
                Regex r = new Regex(@"^(\+7)(\d{10})$");
                if (String.IsNullOrWhiteSpace(surnameEntry.Text))
                {
                    surnameWarning.IsVisible = true;
                }
                else if (String.IsNullOrWhiteSpace(nameEntry.Text))
                {
                    nameWarning.IsVisible = true;

                }
                else if (String.IsNullOrWhiteSpace(middleNameEntry.Text))
                {
                    middleNameWarning.IsVisible = true;
                }
                else if (String.IsNullOrWhiteSpace(phoneEntry.Text))
                {
                    surnameWarning.IsVisible = true;
                }
                else if (String.IsNullOrWhiteSpace(phoneEntry.Text))
                {
                    phoneWarning.Text = "Заполните поле";
                    phoneWarning.IsVisible = true;
                }
                else if (!r.IsMatch(phoneEntry.Text))
                {
                    phoneWarning.Text = "Неверный формат. Пример: +78888888888";
                    phoneWarning.IsVisible = true;
                }
                else if (MainInfoClass.userList.Where(x => x.Phone == phoneEntry.Text).FirstOrDefault() != null)
                {
                    phoneWarning.Text = "Данный номер телефона уже занят";
                    phoneWarning.IsVisible = true;
                }
                else
                {
                    MainInfoClass.user.Surname = surnameEntry.Text;
                    MainInfoClass.user.Name = nameEntry.Text;
                    MainInfoClass.user.MiddleName = middleNameEntry.Text;
                    MainInfoClass.user.Phone = phoneEntry.Text;

                    if (isImageDel)
                    {
                        MainInfoClass.user.Photo = null;
                    }
                    if (photo != null)
                    {
                        string fullApiImage = MainInfoClass.apiImageSource + "/api/ImageUpload";
                        var restClient = new RestClient(fullApiImage);
                        restClient.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddFile("file", photo.FullPath);
                        IRestResponse response = restClient.Execute(request);
                        MainInfoClass.user.Photo = MainInfoClass.apiImageSource + response.Content.Replace("\"", "");
                    }
                    int indexUser = MainInfoClass.user.idUser;
                    string z = string.Format("{0}/Users/{1}", MainInfoClass.apiDBSource, indexUser);
                    using (WebClient wc = new WebClient())
                    {
                        User mainUser = (User)MainInfoClass.user.Clone();
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        wc.UploadString(z, WebRequestMethods.Http.Put, JsonConvert.SerializeObject(mainUser));
                    }

                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Внимание", "Отсутствует подключение к интернету", "ОК");
            }

        }

        private async void GoBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        private async void GoInsertPhoto_Tapped(object sender, EventArgs e)
        {
            try
            {
                photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    personIcon.Source = photo.FullPath;
                    delFrame.IsVisible = true;
                }
               
            }
            catch (Exception ex)
            {
                DisplayAlert("Внимание", ex.Message, "ОК");
            }
        }

        private void phoneEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            phoneWarning.IsVisible = false;
        }

        private void middleNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            middleNameWarning.IsVisible = false;
        }

        private void nameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameWarning.IsVisible = false;
        }

        private void surnameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            surnameWarning.IsVisible = false;
        }

        private void DeletedImage_Tapped(object sender, EventArgs e)
        {
            personIcon.Source = "personDefault.png";
            photo = null;
            isImageDel = true;
            delFrame.IsVisible = false;
        }
    }
}