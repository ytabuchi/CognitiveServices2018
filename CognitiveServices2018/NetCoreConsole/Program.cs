using System;
using System.Collections.Generic;
using CognitiveServices.Core;
using Newtonsoft.Json;

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

            //Console.WriteLine("Cognitive Services - Vision - Face - DetectFace");

            //var faceClient = new FaceService();
            //var faces = faceClient.GetRemoteEmotionsAsync(faceImage).Result;

            //Console.WriteLine($"Detected: {faces.Count} Person.");
            //foreach (var face in faces)
            //{
            //    Console.WriteLine($"Emotion Result:\n" +
            //        $"Age:{face.Age} Gender:{face.Gender} Happiness:{face.Happiness}%");
            //}
            //Console.WriteLine("");



            //Console.WriteLine("Cognitive Services - Vision - ComputerVision - Image Analysis");

            //var analysisClient = new ImageAnalysisService();
            //var caption = analysisClient.AnalyzeRemoteImageAsync(meetupImage).Result;

            //Console.WriteLine($"Analysis Result:\n" +
            //    $"{caption}");
            //Console.WriteLine("");



            //Console.WriteLine("Cognitive Services - Vision - ComputerVision - OCR");

            //var computerVisionClient = new OcrService();
            //var regions = computerVisionClient.ExtractRemoteTextAsync(ocrImage).Result;

            //Console.WriteLine($"Detedted: {regions.Count} Regions");
            //foreach (var region in regions)
            //{
            //    Console.WriteLine($"OCR Result:\n" +
            //        $"{region}");
            //}
            //Console.WriteLine("");



            Console.WriteLine("Cognitive Services - Language - Translator Text");

            var ja = "今日は人生で一番良い日です。";
            var en = "I had a wonderful trip to Seattle and enjoyed seeing the Space Needle!";

            var translationClient = new TranslatorTextService();
            var ja2en = translationClient.TranslateTextAsync(ja, TranslatorTextService.ToLanguage.en).Result;
            var en2ja = translationClient.TranslateTextAsync(en, TranslatorTextService.ToLanguage.ja).Result;

            Console.WriteLine($"Source: {ja}\n" +
            	$"Translated: {ja2en}");
            Console.WriteLine($"Source: {en}\n" +
            	$"Translated: {en2ja}");
            Console.WriteLine("");



            Console.WriteLine("Cognitive Services - Language - Text Analytics");

            var textAnalysisClient = new TextAnalyticsService();
            var sentiment1 = textAnalysisClient.AnalyzeSentimentAsync(en).Result;
            var sentiment2 = textAnalysisClient.AnalyzeSentimentAsync(ja2en).Result;

            Console.WriteLine($"Source: {en}\n" +
            	$"Sentiment(Happiness): {sentiment1*100:00}%");
            Console.WriteLine($"Source: {ja2en}\n" +
            	$"Sentiment(Happiness): {sentiment2*100:00}%");
            Console.WriteLine("");


            Console.ReadLine();
        }
    }
}
