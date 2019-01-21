using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Search.VisualSearch;
using Microsoft.Azure.CognitiveServices.Search.VisualSearch.Models;

namespace CognitiveServices.Core
{
    /// <summary>
    /// Bing visual search service.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/ja-jp/azure/cognitive-services/bing-visual-search/visual-search-sdk-c-sharp</remarks>
    public class BingVisualSearchService
    {
        ObservableCollection<FoundImage> _images = new ObservableCollection<FoundImage>();

        /// <summary>
        /// 画像 URL から類似画像や関連商品を検索します。
        /// </summary>
        /// <returns>見つかった画像情報のコレクション</returns>
        /// <param name="imageUrl">Image URL.</param>
        public async Task<ObservableCollection<FoundImage>> SearchRemoteImageAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return null;

            // クライアント作成
            using (var client = new VisualSearchClient(new ApiKeyServiceClientCredentials(Secrets.BingVisualSearchApiKey)))
            {
                // 画像を URL で指定する場合はImageInfoオブジェクトを利用します。
                var imageInfo = new ImageInfo(url: imageUrl);

                // オプションで KnowledgeRequest に検索するドメインを指定するなどができます。
                var filters = new Filters("pinterest.com");
                var knowledgeRequest = new KnowledgeRequest(filters);

                try
                {
                    // 画像を URL で指定する場合は画像のバイナリデータは不要です。
                    var visualSearchRequest = new VisualSearchRequest(imageInfo, knowledgeRequest);

                    // VisualSearchMethodAsync に KnowledgeRequest の引数を付けて API 呼び出し、結果取得
                    // market: "en-us" が無いと 2019年1月の時点では "ProductVisualSearch" が得られないようなので付けています。
                    var visualSearchResults = await client.Images.VisualSearchMethodAsync(knowledgeRequest: visualSearchRequest, market: "en-us");

                    if (visualSearchResults == null)
                        Debug.WriteLine($"No result.");

                    // Debug用。解析対象の画像トークンを取得しています。ImageKnowledge オブジェクトが返ってきていれば存在するはず。
                    if (visualSearchResults.Image?.ImageInsightsToken != null)
                    {
                        Debug.WriteLine($"Uploaded image insights token: {visualSearchResults.Image.ImageInsightsToken}");
                    }

                    // "ProductVisualSearch" または "VisualSearch" が有る場合と無い場合で処理を分岐
                    if (IsIncluding(visualSearchResults, "ProductVisualSearch"))
                    {
                        _images = GetOfferedProducts(visualSearchResults);
                    }
                    else if (IsIncluding(visualSearchResults, "VisualSearch"))
                    {
                        _images = GetSimilarImages(visualSearchResults);
                    }
                    else
                        return null;

                    return _images;

                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return null;
            }
        }

        /// <summary>
        /// ローカル画像の PATH から類似画像や関連商品を検索します。
        /// </summary>
        /// <returns>見つかった画像情報のコレクション</returns>
        /// <param name="imagePath">Image path.</param>
        public async Task<ObservableCollection<FoundImage>> SearchLocalImageAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
                return null;

            // クライアント作成
            using (var client = new VisualSearchClient(new ApiKeyServiceClientCredentials(Secrets.BingVisualSearchApiKey)))
            {
                try
                {
                    // ファイルから ImageStream を生成
                    using (var imageStream = File.OpenRead(imagePath))
                    {
                        // 画像のストリームを引数で渡す場合は、KnowledgeRequest は必須ではありませんので、ImageStream の引数を付けて API 呼び出し、結果取得
                        // market: "en-us" が無いと 2019年1月の時点では "ProductVisualSearch" が得られないようなので付けています。
                        var visualSearchResults = await client.Images.VisualSearchMethodAsync(image: imageStream,　market: "en-us", knowledgeRequest: (string)null);

                        if (visualSearchResults == null)
                            return null;

                        // Debug用。解析対象の画像トークンを取得しています。ImageKnowledge オブジェクトが返ってきていれば存在するはず。
                        if (visualSearchResults.Image?.ImageInsightsToken != null)
                        {
                            Debug.WriteLine($"Uploaded image insights token: {visualSearchResults.Image.ImageInsightsToken}");
                        }

                        // "ProductVisualSearch" または "VisualSearch" が有る場合と無い場合で処理を分岐
                        if (IsIncluding(visualSearchResults, "ProductVisualSearch"))
                        {
                            _images = GetOfferedProducts(visualSearchResults);
                        }
                        else if (IsIncluding(visualSearchResults, "VisualSearch"))
                        {
                            _images = GetSimilarImages(visualSearchResults);
                        }
                        else
                            return null;

                        return _images;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return null;
            }

        }

        private ObservableCollection<FoundImage> GetOfferedProducts(ImageKnowledge visualSearchResults)
        {
            var images = new ObservableCollection<FoundImage>();

            // 取得した ProductVisualSearch の Actions の中から価格のオファーが含まれているデータだけを抽出
            var foundItems = visualSearchResults.Tags.FirstOrDefault().Actions
                .Where(x => x.ActionType.Contains("ProductVisualSearch"))
                .Cast<ImageModuleAction>()
                .FirstOrDefault()
                .Data.Value
                .Where(x => x.InsightsMetadata.AggregateOffer != null)
                .Select(x => x);

            // FoundImage のコレクションに移し替え
            foreach (var item in foundItems)
            {
                images.Add(new FoundImage
                {
                    Name = item.InsightsMetadata.AggregateOffer.Offers.FirstOrDefault().Name,
                    ImageUrl = item.ContentUrl,
                    ProductUrl = item.InsightsMetadata.AggregateOffer.Offers.FirstOrDefault().Url,
                    ProductDescription = item.InsightsMetadata.AggregateOffer.Offers.FirstOrDefault().Description,
                    Price = item.InsightsMetadata.AggregateOffer.Offers.FirstOrDefault().Price ?? 0d,
                    PriceCurrency = item.InsightsMetadata.AggregateOffer.Offers.FirstOrDefault().PriceCurrency
                });
            }

            return images;
        }

        private ObservableCollection<FoundImage> GetSimilarImages(ImageKnowledge visualSearchResults)
        {
            var images = new ObservableCollection<FoundImage>();

            // 取得した VisualSearch の Actions の中から類似画像のデータを抽出
            var foundItems = visualSearchResults.Tags.FirstOrDefault().Actions
                .Where(x => x.ActionType.Contains("VisualSearch"))
                .Cast<ImageModuleAction>()
                .FirstOrDefault()
                .Data.Value
                .Select(x => x);

            // FoundImage のコレクションに移し替え
            foreach (var item in foundItems)
            {
                images.Add(new FoundImage
                {
                    ImageUrl = item.ContentUrl,
                });
            }

            return images;
        }

        private bool IsIncluding(ImageKnowledge visualSearchResults, string parameter)
        {
            return visualSearchResults.Tags.FirstOrDefault().Actions
                .Any(x => x.ActionType.Contains(parameter));
        }
    }

    public class FoundImage
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string ProductUrl { get; set; }
        public string ProductDescription { get; set; }
        public double? Price { get; set; }
        public string PriceCurrency { get; set; }
    }
}

