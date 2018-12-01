using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace CognitiveService.Core
{
    public class ComputerVisionService
    {
        private const int numberOfCharsInOperationId = 36;

        public async Task<string> ExtractRemoteTextAsync(string imageUrl)
        {
            // ComputerVisionClient の準備
            var cvClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Secrets.ComputerVisionApiKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Secrets.ComputerVisionEndpoint
            };

            // API でアクセスするための情報を取得
            var textHeaders = await cvClient.RecognizeTextAsync(imageUrl, TextRecognitionMode.Handwritten);
            var operationLocation = textHeaders.OperationLocation;
            var operationId = textHeaders.OperationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // API 呼び出し、結果取得
            var result = await cvClient.GetTextOperationResultAsync(operationId);
            var lines = result.RecognitionResult.Lines;

            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.Append(line.Text);
            }

            return sb.ToString();
        }
    }
}
