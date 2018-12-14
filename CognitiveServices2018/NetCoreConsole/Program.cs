using System;
using CognitiveServices.Core;

namespace NetCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var faceImage = "https://pbs.twimg.com/profile_images/747601253266395136/2HeCGdiG_400x400.jpg";
            var meetupImage = "https://pbs.twimg.com/media/DsmYWMFU8AEn0D1.jpg";
            var ocrImage = "https://pbs.twimg.com/media/DtdfaSeVsAAeRis.jpg";
            var colluptUrl = "xxxxxxxx";


            //Console.WriteLine("Cognitive Services - Face - DetectFace");

            //var faceClient = new FaceService();
            //var faces = faceClient.GetRemoteEmotionsAsync(faceImage).Result;

            //Console.WriteLine($"Detected: {faces.Count} Person.");
            //foreach (var face in faces)
            //{
            //    Console.WriteLine($"Emotion Result:\n" +
            //        $"Age:{face.Age} Gender:{face.Gender} Happiness:{face.Happiness}%");
            //}
            //Console.WriteLine("");


            Console.WriteLine("Cognitive Services - ComputerVision - Image Analysis");

            var analysisClient = new ImageAnalysisService();
            var caption = analysisClient.AnalyzeRemoteImageAsync(faceImage).Result;

            Console.WriteLine($"Analysis Result:\n" +
                $"{caption}");
            Console.WriteLine("");


            //Console.WriteLine("Cognitive Services - ComputerVision - OCR");

            //var computerVisionClient = new OcrService();
            //var regions = computerVisionClient.ExtractRemoteTextAsync(ocrImage).Result;

            //Console.WriteLine($"Detedted: {regions.Count} Regions");
            //foreach (var region in regions)
            //{
            //    Console.WriteLine($"OCR Result:\n" +
            //        $"{region}");
            //}


            Console.ReadLine();
        }
    }
}
