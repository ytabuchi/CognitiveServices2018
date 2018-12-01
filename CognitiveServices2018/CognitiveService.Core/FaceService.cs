using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitiveService.Core
{
    public class FaceService
    {
        public async Task<List<FaceEmotion>> GetRemoteEmotionsAsync(string imageUrl)
        {
            // FaceClient の準備
            var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(Secrets.FaceApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.FaceEndpoint,
            };

            // 取得する属性を指定
            FaceAttributeType[] faceAttributes = { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Emotion };

            // API 呼び出し、結果取得
            var faceList = await faceClient.Face.DetectWithUrlAsync(imageUrl, true, false, faceAttributes);

            var emotions = new List<FaceEmotion>();
            foreach (var face in faceList)
            {
                emotions.Add(new FaceEmotion
                {
                    Age = face.FaceAttributes.Age,
                    Gender = ((Gender)face.FaceAttributes.Gender).ToString(),
                    Happiness = face.FaceAttributes.Emotion.Happiness * 100d
                });
            }

            return emotions;
        }
    }

    public class FaceEmotion
    {
        public double? Age { get; set; }
        public string Gender { get; set; }
        public double? Happiness { get; set; }
    }
}
