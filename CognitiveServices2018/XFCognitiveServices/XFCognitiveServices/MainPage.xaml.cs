using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using CognitiveServices.Core;

namespace XFCognitiveServices
{
    public partial class MainPage : ContentPage
    {
        MediaFile file;
        bool isPermissionGranted;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (CheckFeaturesAvalable())
                isPermissionGranted = await CheckPermissionGranted();
            else
                await DisplayAlert("Features", "This phone does not support this app.", "OK");
        }

        private bool CheckFeaturesAvalable()
        {
            return CrossMedia.Current.IsTakePhotoSupported &&
                CrossMedia.Current.IsPickPhotoSupported;
        }

        private async Task<bool> CheckPermissionGranted()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
                return true;
            else
                return false;
        }

        private async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            if (isPermissionGranted)
            {
                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    PhotoSize = PhotoSize.Medium
                });

                image.Source = ImageSource.FromFile(file.Path);
            }
            else
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }
        }

        private async void OpenPicureButton_Clicked(object sender, System.EventArgs e)
        {
            if (isPermissionGranted)
            {
                file = await CrossMedia.Current.PickPhotoAsync();

                image.Source = ImageSource.FromFile(file.Path);
            }
            else
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }
        }

        private async void FaceButton_Clicked(object sender, EventArgs e)
        {
            FaceButtno.IsEnabled = false;

            var client = new FaceService();
            var faces = await client.GetLocalEmotionAsync(file.Path);

            if (faces == null)
                await DisplayAlert("Error", "Can not detect", "OK");

            var sb = new StringBuilder();
            sb.Append($"Detected: {faces.Count}\n\n");
            foreach (var face in faces)
            {
                sb.Append($"Emotion Rsult:\nAge:{face.Age}\nGender:{face.Gender}\nHappiness:{face.Happiness}%\n");
            }

            await DisplayAlert("Face", sb.ToString(), "OK");

            FaceButtno.IsEnabled = true;
        }

        private async void AnalyzeButton_Clicked(object sender, EventArgs e)
        {
            AnalyzeButton.IsEnabled = false;

            var client = new ImageAnalysisService();
            var caption = await client.AnalyzeLocalImageAsync(file.Path);

            await DisplayAlert("Image Analysis", caption, "OK");

            AnalyzeButton.IsEnabled = true;
        }

        private async void OcrButton_Clicked(object sender, EventArgs e)
        {
            OcrButton.IsEnabled = false;

            var client = new OcrService();
            var regions = await client.ExtractLocalTextAsync(file.Path);

            var sb = new StringBuilder();
            sb.Append($"Extracted Regions: {regions.Count}\n\n");
            foreach (var region in regions)
            {
                sb.Append($"OCR Result:\n{region}\n");
            }

            await DisplayAlert("OCR", sb.ToString(), "OK");

            OcrButton.IsEnabled = true;
        }
    }
}
