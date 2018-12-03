using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.IO;

namespace CognitiveService.Core
{
    /// <summary>
    /// Computer vision service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/computer-vision/quickstarts-sdk/csharp-hand-text-sdk</remarks>
    public class ComputerVisionService
    {
        private const int numberOfCharsInOperationId = 36;
        public enum OcrMode
        {
            Print,
            HandWriting
        }

        /// <summary>
        /// URL先の画像からテキストをOCRして抽出します。
        /// </summary>
        /// <returns>OCRした結果のテキストを返します。</returns>
        /// <param name="imageUrl">Image URL.</param>
        /// <param name="mode">Mode.</param>
        public async Task<string> ExtractRemoteTextAsync(string imageUrl, OcrMode mode)
        {
            // Mode変換
            var ocrMode = GetOcrMode(mode);

            // ComputerVisionClient の準備
            var computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            };

            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return $"Invalid remoteImageUrl: {imageUrl}";

            // API でアクセスするための情報を取得
            var textHeaders = await computerVisionClient.RecognizeTextAsync(imageUrl, ocrMode);

            return await GetTextAsync(computerVisionClient, textHeaders.OperationLocation);
        }

        /// <summary>
        /// ローカルの画像のファイルパスからテキストをOCRして抽出します。
        /// </summary>
        /// <returns>OCRした結果のテキストを返します。</returns>
        /// <param name="imagePath">Image path.</param>
        /// <param name="mode">Mode.</param>
        public async Task<string> ExtractLocalTextAsync(string imagePath, OcrMode mode)
        {
            // Mode変換
            var ocrMode = GetOcrMode(mode);

            // ComputerVisionClient の準備
            var computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            };

            if (!File.Exists(imagePath))
                return $"Unable to open or read localImagePath: {imagePath}";

            using (var imageStream = File.OpenRead(imagePath))
            {
                // API でアクセスするための情報を取得
                var textHeaders = await computerVisionClient.RecognizeTextInStreamAsync(imageStream, ocrMode);

                return await GetTextAsync(computerVisionClient, textHeaders.OperationLocation);
            }
        }

        /// <summary>
        /// ローカルの画像のストリームからテキストをOCRして抽出します。
        /// </summary>
        /// <returns>OCRした結果のテキストを返します。</returns>
        /// <param name="imageStream">Image stream.</param>
        /// <param name="mode">Mode.</param>
        public async Task<string> ExtractLocalTextAsync(Stream imageStream, OcrMode mode)
        {
            // Mode変換
            var ocrMode = GetOcrMode(mode);

            // ComputerVisionClient の準備
            var computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            };

            var textHeaders = await computerVisionClient.RecognizeTextInStreamAsync(imageStream, ocrMode);

            return await GetTextAsync(computerVisionClient, textHeaders.OperationLocation);
        }


        private static async Task<string> GetTextAsync(ComputerVisionClient client, string operationLocation)
        {
            // API へのアクセス用の文字列（発行された URL の最後 36 文字）を取得
            var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            try
            {
                // API 呼び出し、結果取得
                var result = await client.GetTextOperationResultAsync(operationId);

                // OCR が完了するまで待つ（動く時と動かない時があるのはこれ？？）
                int i = 0;
                int maxRetries = 10;
                while ((result.Status == TextOperationStatusCodes.Running ||
                        result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
                {
                    await Task.Delay(1000);
                    result = await client.GetTextOperationResultAsync(operationId);
                }

                if (result.Status == TextOperationStatusCodes.Failed)
                    return $"Can not recognized.";

                // よしなに処理
                var lines = result.RecognitionResult.Lines;
                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    sb.Append(line.Text);
                }

                return sb.ToString();
            }
            catch (ComputerVisionErrorException e)
            {
                Debug.WriteLine($"imageUrl: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Unknown error: {e.Message}");
            }

            return $"Can not recognized.";
        }

        private TextRecognitionMode GetOcrMode(OcrMode ocrMode)
        {
            return ocrMode == OcrMode.Print ? TextRecognitionMode.Printed : TextRecognitionMode.Handwritten;
        }
    }
}
