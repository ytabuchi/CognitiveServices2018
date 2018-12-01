using System;
using CognitiveService.Core;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace NetCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cognitive Services - Face - DetectFace\n");

            var faceClient = new FaceService();
            var faces = faceClient.GetRemoteEmotionsAsync("https://pbs.twimg.com/profile_images/747601253266395136/2HeCGdiG_400x400.jpg").Result;

            Console.WriteLine($"Detected: {faces.Count} Person.");
            foreach (var face in faces)
            {
                Console.WriteLine($"Emotion Rsult: \nAge:{face.Age} Gender:{face.Gender} Happiness:{face.Happiness}%");
            }

            Console.WriteLine("Cognitive Services - ComputerVision - OCR\n");

            var cvRemoteImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Cursive_Writing_on_Notebook_paper.jpg/800px-Cursive_Writing_on_Notebook_paper.jpg";

            var cvClient = new ComputerVisionService();
            var text = cvClient.ExtractRemoteTextAsync(cvRemoteImageUrl, TextRecognitionMode.Handwritten).Result;

            Console.WriteLine($"OCR Result:\n{text}");


            Console.ReadLine();
        }
    }
}
