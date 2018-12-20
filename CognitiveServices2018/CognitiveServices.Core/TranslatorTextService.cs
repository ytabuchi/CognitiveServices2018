using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CognitiveServices.Core
{
    /// <summary>
    /// Translator text service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/translator/quickstart-csharp-translate</remarks>
    public class TranslatorTextService
    {
        private readonly string route = "/translate?api-version=3.0";

        public enum ToLanguage
        {
            en,
            ja
        }

        /// <summary>
        /// テキストを翻訳します
        /// </summary>
        /// <returns>翻訳されたテキスト</returns>
        /// <param name="text">Text.</param>
        /// <param name="to">To Language.</param>
        public async Task<string> TranslateTextAsync(string text, ToLanguage to)
        {
            var body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        // POSTで通信
                        request.Method = HttpMethod.Post;

                        // URLを生成
                        request.RequestUri = new Uri(Secrets.TranslatorTextRestEndpoint + route + "&to=" + to.ToString());

                        // リクエストコンテンツを追加
                        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                        // 認証ヘッダーを追加
                        request.Headers.Add("Ocp-Apim-Subscription-Key", Secrets.TranslatorTextRestApiKey);

                        // リクエストを送付しJSONのレスポンスを取得
                        var response = await client.SendAsync(request);
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        // オブジェクトに戻して最初のテキストを取得
                        var translated = JsonConvert.DeserializeObject<List<TranslatedObject>>(jsonResponse);
                        return translated[0].Translations[0].Text;
                    }
                }
            }
            catch (JsonException e)
            {
                Debug.WriteLine($"JSON error: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Unknown error: {e.Message}");
            }

            return "Could not translated.";
        }
    }

    public class DetectedLanguage
    {
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }

    public class Translation
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
    }

    public class TranslatedObject
    {
        [JsonProperty("detectedLanguage")]
        public DetectedLanguage DetectedLanguage { get; set; }
        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }
    }
}
