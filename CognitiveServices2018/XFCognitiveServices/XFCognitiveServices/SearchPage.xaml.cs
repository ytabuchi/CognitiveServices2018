using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CognitiveServices.Core;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace XFCognitiveServices
{
    public partial class SearchPage : ContentPage
    {
        MediaFile file;
        bool isPermissionGranted;
        ObservableCollection<FoundImage> _foundImages = new ObservableCollection<FoundImage>();

        public SearchPage()
        {
            InitializeComponent();
        }

        private void EnableCognitiveButtons()
        {
            SearchButton.IsEnabled = true;
        }

        private void DisableCognitiveButtons()
        {
            SearchButton.IsEnabled = false;
        }

        private async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            // 機能のチェック
            if (!PermissionService.CheckFeaturesAvalable())
            {
                await DisplayAlert("Features", "This phone does not support this app.", "OK");
                return;
            }

            // パーミッションをチェックし、許可されていなければ許可してもらう。
            if (!isPermissionGranted)
                isPermissionGranted = await PermissionService.CheckPermissionGranted();

            if (isPermissionGranted)
            {
                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    PhotoSize = PhotoSize.Medium
                });

                if (file == null)
                    return;

                image.Source = ImageSource.FromFile(file.Path);

                EnableCognitiveButtons();
            }
            else
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }
        }

        private async void OpenPictureButton_Clicked(object sender, EventArgs e)
        {
            // 機能のチェック
            if (!PermissionService.CheckFeaturesAvalable())
            {
                await DisplayAlert("Features", "This phone does not support this app.", "OK");
                return;
            }

            // パーミッションをチェックし、許可されていなければ許可してもらう。
            if (!isPermissionGranted)
                isPermissionGranted = await PermissionService.CheckPermissionGranted();

            if (isPermissionGranted)
            {
                file = await CrossMedia.Current.PickPhotoAsync();

                if (file == null)
                    return;

                image.Source = ImageSource.FromFile(file.Path);

                EnableCognitiveButtons();
            }
            else
            {
                await DisplayAlert("Permissions Denied", "Unable to open photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }
        }

        private async void SearchButton_Clicked(object sender, EventArgs e)
        {
            DisableCognitiveButtons();

            _foundImages.Clear();

            var visualSearchClient = new BingVisualSearchService();
            var imageResponse = await visualSearchClient.SearchLocalImageAsync(file.Path);

            if (imageResponse == null)
            {
                await DisplayAlert("Not found", "Could not found any similer images.", "OK");
                return;
            }

            foreach (var img in imageResponse)
            {
                _foundImages.Add(new FoundImage
                {
                    ImageUrl = img.ImageUrl,
                    Name = img.Name
                });
            }

            this.BindingContext = _foundImages;

            EnableCognitiveButtons();
        }
    }
}
