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
    public partial class VisionPage : ContentPage
    {
        MediaFile file;
        bool isPermissionGranted;

        public VisionPage()
        {
            InitializeComponent();
        }

        private void EnableCognitiveButtons()
        {
            AnalyzeButton.IsEnabled = true;
            OcrButton.IsEnabled = true;
            FaceButton.IsEnabled = true;
        }

        private void DisableCognitiveButtons()
        {
            AnalyzeButton.IsEnabled = false;
            OcrButton.IsEnabled = false;
            FaceButton.IsEnabled = false;
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

        private async void OpenPicureButton_Clicked(object sender, System.EventArgs e)
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

        private async void AnalyzeButton_Clicked(object sender, EventArgs e)
        {
            DisableCognitiveButtons();

            var analysisClient = new ImageAnalysisService();
            var caption = await analysisClient.AnalyzeLocalImageAsync(file.Path);

            await DisplayAlert("Image Analysis", caption, "OK");

            EnableCognitiveButtons();
        }

        private async void OcrButton_Clicked(object sender, EventArgs e)
        {
            DisableCognitiveButtons();

            var ocrClient = new OcrService();
            var regions = await ocrClient.ExtractLocalTextAsync(file.Path);

            var sb = new StringBuilder();
            sb.Append($"Extracted Regions: {regions.Count}\n\n");
            foreach (var region in regions)
            {
                sb.Append($"OCR Result:\n{region}\n");
            }

            await DisplayAlert("OCR", sb.ToString(), "OK");

            EnableCognitiveButtons();
        }

        private async void FaceButton_Clicked(object sender, EventArgs e)
        {
            DisableCognitiveButtons();

            var faceClient = new FaceService();
            var faces = await faceClient.GetLocalEmotionAsync(file.Path);

            if (faces == null)
                await DisplayAlert("Error", "Can not detect", "OK");

            var sb = new StringBuilder();
            sb.Append($"Detected: {faces.Count}\n\n");
            foreach (var face in faces)
            {
                sb.Append($"Emotion Rsult:\nAge:{face.Age}\nGender:{face.Gender}\nHappiness:{face.Happiness}%\n");
            }

            await DisplayAlert("Face", sb.ToString(), "OK");

            EnableCognitiveButtons();
        }
    }
}
