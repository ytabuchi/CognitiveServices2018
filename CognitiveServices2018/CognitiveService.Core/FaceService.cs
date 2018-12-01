using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitiveService.Core
{
    /// <summary>
    /// Face service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/face/quickstarts/csharp-detect-sdk</remarks>
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

            try
            {
                // API 呼び出し、結果取得
                var faceList = await faceClient.Face.DetectWithUrlAsync(imageUrl, true, false, faceAttributes);

                // よしなに処理
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
            catch (APIErrorException e)
            {
                Debug.WriteLine($"imageUrl: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Unknown error: {e.Message}");
            }

            return null;

        }

        public async Task<List<FaceEmotion>> GetLocalEmotionAsync(string imagePath)
        {
            // FaceClient の準備
            var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(Secrets.FaceApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.FaceEndpoint,
            };
            // 取得する属性を指定
            FaceAttributeType[] faceAttributes = { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Emotion };

            try
            {
                using (var imageStream = File.OpenRead(imagePath))
                {
                    // API 呼び出し、結果取得
                    var faceList = await faceClient.Face.DetectWithStreamAsync(imageStream, true, false, faceAttributes);

                    // よしなに処理
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
            catch (APIErrorException e)
            {
                Debug.WriteLine($"imageUrl: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Unknown error: {e.Message}");
            }

            return null;
        }
    }

    public class FaceEmotion
    {
        public double? Age { get; set; }
        public string Gender { get; set; }
        public double? Happiness { get; set; }
    }
}
