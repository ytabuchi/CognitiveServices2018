using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitiveServices.Core
{
    /// <summary>
    /// Face service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/face/quickstarts/csharp-detect-sdk</remarks>
    public class FaceService
    {
        // 取得する属性を指定
        private static readonly FaceAttributeType[] faceAttributes =
        {
            FaceAttributeType.Age,
            FaceAttributeType.Gender,
            FaceAttributeType.Emotion
        };

        /// <summary>
        /// 画像の URL から顔を認識して年齢、性別、幸福度を得ます。
        /// </summary>
        /// <returns>取得したそれぞれの顔の情報を返します。</returns>
        /// <param name="imageUrl">Image URL.</param>
        public async Task<List<FaceEmotion>> GetRemoteEmotionsAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return new List<FaceEmotion>();

            // FaceClient の準備
            using (var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(Secrets.FaceApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.CognitiveApiEndpoint,
            })
            {
                try
                {
                    // API 呼び出し、結果取得
                    var faces = await faceClient.Face.DetectWithUrlAsync(imageUrl, true, false, faceAttributes);

                    return GetFaceEmotions(faces);
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

        /// <summary>
        /// ローカルの画像のパスから顔を認識して年齢、性別、幸福度を得ます。
        /// </summary>
        /// <returns>取得したそれぞれの顔の情報を返します。</returns>
        /// <param name="imagePath">Image path.</param>
        public async Task<List<FaceEmotion>> GetLocalEmotionAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
                return null;

            // FaceClient の準備
            using (var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(Secrets.FaceApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.CognitiveApiEndpoint,
            })
            {
                try
                {
                    using (var imageStream = File.OpenRead(imagePath))
                    {
                        // API 呼び出し、結果取得
                        var faces = await faceClient.Face.DetectWithStreamAsync(imageStream, true, false, faceAttributes);

                        return GetFaceEmotions(faces);
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

        /// <summary>
        /// ローカルの画像のストリームから顔を認識して年齢、性別、幸福度を得ます。
        /// </summary>
        /// <returns>取得したそれぞれの顔の情報を返します。</returns>
        /// <param name="imageStream">Image path.</param>
        public async Task<List<FaceEmotion>> GetLocalEmotionAsync(Stream imageStream)
        {
            // FaceClient の準備
            using (var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(Secrets.FaceApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.CognitiveApiEndpoint,
            })
            {
                try
                {
                    // API 呼び出し、結果取得
                    var faces = await faceClient.Face.DetectWithStreamAsync(imageStream, true, false, faceAttributes);

                    return GetFaceEmotions(faces);
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

        private List<FaceEmotion> GetFaceEmotions(IList<DetectedFace> faces)
        {
            // よしなに処理
            var emotions = new List<FaceEmotion>();
            foreach (var face in faces)
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
