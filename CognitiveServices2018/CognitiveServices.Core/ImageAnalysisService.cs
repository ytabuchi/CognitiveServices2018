using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CognitiveServices.Core
{
    /// <summary>
    /// Image Recognition Service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/Computer-vision/quickstarts-sdk/csharp-analyze-sdk</remarks>
    public class ImageAnalysisService
    {
        // 取得する属性を指定
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Tags
        };

        /// <summary>
        /// 画像の URL から画像のシーンを分析します。
        /// </summary>
        /// <returns>分析したキャプション</returns>
        /// <param name="imageUrl"></param>
        public async Task<string> AnalyzeRemoteImageAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return $"Invalid image URL: {imageUrl}";

            using (var computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            })
            {
                try
                {
                    // API 呼び出し、結果取得
                    var analysisResult = await computerVisionClient.AnalyzeImageAsync(imageUrl, features, null, "zh");

                    return GetCaption(analysisResult);
                }
                catch (ComputerVisionErrorException e)
                {
                    Debug.WriteLine($"imageUrl: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return "Could not analyze image.";
            }
        }

        /// <summary>
        /// ローカル画像のパスから画像のシーンを分析します。
        /// </summary>
        /// <returns>分析したキャプション</returns>
        /// <param name="imagePath"></param>
        public async Task<string> AnalyzeLocalImageAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
                return $"Unable to open or read ImagePath: {imagePath}";

            // ComputerVisionClient の準備
            using (var computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            })
            {
                try
                {
                    using (var imageStream = File.OpenRead(imagePath))
                    {
                        // API 呼び出し、結果取得
                        var analysisResult = await computerVisionClient.AnalyzeImageInStreamAsync(imageStream, features);

                        return GetCaption(analysisResult);
                    }
                }
                catch (ComputerVisionErrorException e)
                {
                    Debug.WriteLine($"imageUrl: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return "Could not analyze image.";
            }
        }

        /// <summary>
        /// ローカル画像のパスから画像のシーンを分析します。
        /// </summary>
        /// <returns>分析したキャプション</returns>
        /// <param name="imagePath"></param>
        public async Task<string> AnalyzeLocalImageAsync(Stream imageStream)
        {
            // ComputerVisionClient の準備
            using (var computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            })
            {
                try
                {
                    // API 呼び出し、結果取得
                    var analysisResult = await computerVisionClient.AnalyzeImageInStreamAsync(imageStream, features);

                    return GetCaption(analysisResult);
                }
                catch (ComputerVisionErrorException e)
                {
                    Debug.WriteLine($"imageUrl: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return "Could not analyze image.";
            }
        }

        private static string GetCaption(ImageAnalysis analysis)
        {
            return analysis.Description.Captions[0].Text ?? "Could not analyze image.";
        }
    }
}
