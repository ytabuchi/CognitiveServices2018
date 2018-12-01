using System;
using CognitiveService.Core;

namespace NetCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var cvRemoteImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/" + 
            "Cursive_Writing_on_Notebook_paper.jpg/" +
            "800px-Cursive_Writing_on_Notebook_paper.jpg";

            var cvClient = new ComputerVisionService();
            var text = cvClient.ExtractRemoteTextAsync(cvRemoteImageUrl).Result;

            var faceClient = new FaceService();
            var face = faceClient.GetRemoteEmotionsAsync("https://pbs.twimg.com/profile_images/747601253266395136/2HeCGdiG_400x400.jpg").Result;



            Console.ReadLine();
        }
    }
}
