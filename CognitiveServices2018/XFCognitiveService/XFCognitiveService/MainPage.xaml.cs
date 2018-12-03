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
using CognitiveService.Core;

namespace XFCognitiveService
{
    public partial class MainPage : ContentPage
    {
        MediaFile file;

        public MainPage()
        {
            InitializeComponent();
        }

        async void PictureButton_Clicked(object sender, EventArgs e)
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
            {
                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
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

        async void FaceButton_Clicked(object sender, EventArgs e)
        {
            var client = new FaceService();
            var faces = await client.GetLocalEmotionAsync(file.Path);

            if (faces == null)
                await DisplayAlert("Error", "Can not detect", "OK");

            Console.WriteLine($"Detected: {faces.Count} Person.");

            var sb = new StringBuilder();
            sb.Append($"Detected: {faces.Count}\n\n");
            foreach (var face in faces)
            {
                sb.Append($"Emotion Rsult: \nAge:{face.Age}\nGender:{face.Gender}\nHappiness:{face.Happiness}%\n\n");
            }

            await DisplayAlert("Face", sb.ToString(), "OK");
        }

        async void OcrButton_Clicked(object sender, EventArgs e)
        {
            var client = new ComputerVisionService();

            var mode = ocrSwitch.IsToggled ? ComputerVisionService.OcrMode.HandWriting : ComputerVisionService.OcrMode.Print;
            var text = await client.ExtractLocalTextAsync(file.Path, mode);

            await DisplayAlert("OCR", text, "OK");
        }
    }
}
