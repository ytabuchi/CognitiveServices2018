using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CognitiveServices.Core
{
    /// <summary>
    /// Text analytics service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/text-analytics/quickstarts/csharp</remarks>
    public class TextAnalyticsService
    {
        /// <summary>
        /// 英語テキストのセンチメントを解析します
        /// </summary>
        /// <returns>センチメント値</returns>
        /// <param name="text">Text.</param>
        public async Task<double> AnalyzeSentimentAsync(string text)
        {
            // クライアント作成
            using (var client = new TextAnalyticsClient(new TextAnalyticsApiKeyServiceClientCredentials(),
                new DelegatingHandler[] { })
            {
                Endpoint = Secrets.CognitiveApiEndpoint
            })
            {
                // 英語かどうかチェック
                if (!await IsEnglishText(client, text))
                    return 0;

                try
                {
                    // API 呼び出し、結果取得
                    var result = await client.SentimentAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>
                    {
                        new MultiLanguageInput("en", "0", text),
                    }));

                    var sentiment = result.Documents[0].Score ?? 0;

                    return sentiment;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Analyze error: {e.Message}");
                }

                return 0;
            }
        }

        /// <summary>
        /// 各メソッドで英語を引数で受け取ることを想定しているので、英語と認識したかどうかをチェックするメソッドです。
        /// </summary>
        /// <returns>英語の場合はTrue</returns>
        /// <param name="client">TextAnalyticsClient.</param>
        /// <param name="text">Text.</param>
        private async Task<bool> IsEnglishText(TextAnalyticsClient client, string text)
        {
            try
            {
                var result = await client.DetectLanguageAsync(new BatchInput(
                new List<Input>()
                {
                    new Input("0", text),
                }));

                var detectedLanguage = result.Documents[0].DetectedLanguages[0].Iso6391Name;

                return detectedLanguage == "en";
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Detection error: {e.Message}");
            }

            return false;
        }
    }

    class TextAnalyticsApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Ocp-Apim-Subscription-Key", Secrets.TextAnalyticsApiKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }

}
