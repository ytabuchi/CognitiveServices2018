using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace CognitiveServices.Core
{
    /// <summary>
    /// OCR service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/computer-vision/quickstarts-sdk/csharp-hand-text-sdk</remarks>
    public class OcrService
    {
        /// <summary>
        /// URL先の画像からテキストをOCRします。
        /// </summary>
        /// <returns>OCRした結果の各地域のテキストを返します。</returns>
        /// <param name="imageUrl">Image URL.</param>
        public async Task<List<string>> ExtractRemoteTextAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return new List<string> { $"Invalid image URL: {imageUrl}" };

            // ComputerVisionClient の準備
            using (var computerVisionClient = new ComputerVisionClient(new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            })
            {
                try
                {
                    // API 呼び出し、結果取得
                    var ocrResult = await computerVisionClient.RecognizePrintedTextAsync(true, imageUrl, OcrLanguages.Ja);

                    return GetRegionTextAsync(ocrResult);
                }
                catch (ComputerVisionErrorException e)
                {
                    Debug.WriteLine($"imageUrl: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return new List<string> { "Could not recognize text." };
            }
        }

        /// <summary>
        /// ローカルの画像のファイルパスからテキストをOCRします。
        /// </summary>
        /// <returns>OCRした結果の各地域のテキストを返します。</returns>
        /// <param name="imagePath">Image path.</param>
        public async Task<List<string>> ExtractLocalTextAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
                return new List<string> { $"Unable to open or read ImagePath: {imagePath}" };

            // ComputerVisionClient の準備
            using (var computerVisionClient = new ComputerVisionClient(new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
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
                        var ocrResult = await computerVisionClient.RecognizePrintedTextInStreamAsync(true, imageStream, OcrLanguages.Ja);

                        return GetRegionTextAsync(ocrResult);
                    }
                }
                catch (ComputerVisionErrorException e)
                {
                    Debug.WriteLine($"imagePath: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return new List<string> { "Could not recognize text." };
            }
        }

        /// <summary>
        /// ローカルの画像のストリームからテキストをOCRして抽出します。
        /// </summary>
        /// <returns>OCRした結果の各地域のテキストを返します。</returns>
        /// <param name="imageStream">Image stream.</param>
        public async Task<List<string>> ExtractLocalTextAsync(Stream imageStream)
        {
            // ComputerVisionClient の準備
            using (var computerVisionClient = new ComputerVisionClient(new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            })
            {
                try
                {
                    // API 呼び出し、結果取得
                    var ocrResult = await computerVisionClient.RecognizePrintedTextInStreamAsync(true, imageStream, OcrLanguages.Ja);

                    return GetRegionTextAsync(ocrResult);
                }
                catch (ComputerVisionErrorException e)
                {
                    Debug.WriteLine($"imageStream: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return new List<string> { "Could not recognize text." };
            }
        }

        private static List<string> GetRegionTextAsync(OcrResult result)
        {
            if (result.Regions.Count == 0)
                return new List<string> { "Could not recognized." };

            var regionTexts = new List<string>();

            // よしなに処理
            foreach (var region in result.Regions)
            {
                var sb = new StringBuilder();
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        sb.Append(word.Text);
                    }
                    sb.Append("\n");
                }
                regionTexts.Add(sb.ToString());
            }

            return regionTexts;
        }
    }
}
