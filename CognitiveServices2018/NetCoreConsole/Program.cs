using System;
using CognitiveServices.Core;

namespace NetCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var faceImage = "https://pbs.twimg.com/profile_images/747601253266395136/2HeCGdiG_400x400.jpg";
            var ocrImage = "https://pbs.twimg.com/media/DtdfaSeVsAAeRis.jpg";
            var colluptUrl = "xxxxxxxx";

            Console.WriteLine("Cognitive Services - Face - DetectFace\n");

            var faceClient = new FaceService();
            var faces = faceClient.GetRemoteEmotionsAsync(colluptUrl).Result;

            Console.WriteLine($"Detected: {faces.Count} Person.");
            foreach (var face in faces)
            {
                Console.WriteLine($"Emotion Result: \nAge:{face.Age} Gender:{face.Gender} Happiness:{face.Happiness}%\n\n");
            }


            Console.WriteLine("Cognitive Services - ComputerVision - OCR\n");

            var computerVisionClient = new ComputerVisionService();
            var regions = computerVisionClient.ExtractRemoteTextAsync(ocrImage).Result;

            Console.WriteLine($"Detedted: {regions.Count} Regions");
            foreach (var region in regions)
            {
                Console.WriteLine($"OCR Result:\n{region}\n\n");
            }


            Console.ReadLine();
        }
    }
}
