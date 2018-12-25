using System;
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
        public async Task<string> SearchRemoteImageAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return $"Invalid image URL: {imageUrl}";

            //var client = new VisualSearchClient(new ApiKeyServiceClientCredentials(Secrets.BingSearchApiKey),
            //    new System.Net.Http.DelegatingHandler[] { })
            //{
            //    Endpoint = Secrets.BingSearchEndPoint
            //};

            var client = new VisualSearchClient(new Microsoft.Azure.CognitiveServices.Search.VisualSearch.ApiKeyServiceClientCredentials(Secrets.BingVisualSearchApiKey));

            // The image can be specified via URL, in the ImageInfo object
            var imageInfo = new ImageInfo(url: imageUrl);

            // Optional filters inside the knowledgeRequest will restrict similar products and images to certain domains
            var filters = new Filters("pinterest.com");
            var knowledgeRequest = new KnowledgeRequest(filters);

            // An image binary is not necessary here, as the image is specified via URL
            var visualSearchRequest = new VisualSearchRequest(imageInfo, knowledgeRequest);

            try
            {
                var visualSearchResults = await client.Images.VisualSearchMethodAsync(knowledgeRequest: visualSearchRequest);

                if (visualSearchResults == null)
                    Debug.WriteLine($"No result.");

                // Visual Search results
                if (visualSearchResults.Image?.ImageInsightsToken != null)
                {
                    Debug.WriteLine($"Uploaded image insights token: {visualSearchResults.Image.ImageInsightsToken}");
                }

                // List of tags
                if (visualSearchResults.Tags.Count > 0)
                {
                    var firstTagResult = visualSearchResults.Tags.FirstOrDefault();

                    // List of actions in first tag
                    if (firstTagResult.Actions.Count > 0)
                    {
                        var firstActionResult = firstTagResult.Actions.FirstOrDefault();
                        Debug.WriteLine($"First tag action count: {firstTagResult.Actions.Count}");
                        Debug.WriteLine($"First tag action type: {firstActionResult.ActionType}");
                    }
                    else
                    {
                        Debug.WriteLine("Couldn't find tag actions!");
                    }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Unknown error: {e.Message}");
            }

            return null;

        }


        public async Task<string> SearchLocalImageAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
                return $"Unable to open or read ImagePath: {imagePath}";


            using (var client = new VisualSearchClient(new Microsoft.Azure.CognitiveServices.Search.VisualSearch.ApiKeyServiceClientCredentials(Secrets.BingVisualSearchApiKey)))
            {
                try
                {
                    using (var imageStream = File.OpenRead(imagePath))
                    {
                        // The knowledgeRequest parameter is not required if an image binary is passed in the request body
                        var visualSearchResults = await client.Images.VisualSearchMethodAsync(image: imageStream, knowledgeRequest: (string)null);

                        if (visualSearchResults == null)
                            Debug.WriteLine($"No result.");

                        // Visual Search results
                        if (visualSearchResults.Image?.ImageInsightsToken != null)
                        {
                            Debug.WriteLine($"Uploaded image insights token: {visualSearchResults.Image.ImageInsightsToken}");
                        }

                        // List of tags
                        if (visualSearchResults.Tags.Count > 0)
                        {
                            var firstTagResult = visualSearchResults.Tags.FirstOrDefault();

                            // List of actions in first tag
                            if (firstTagResult.Actions.Count > 0)
                            {
                                var firstActionResult = firstTagResult.Actions.FirstOrDefault();
                                Debug.WriteLine($"First tag action count: {firstTagResult.Actions.Count}");
                                Debug.WriteLine($"First tag action type: {firstActionResult.ActionType}");
                            }
                            else
                            {
                                Debug.WriteLine("Couldn't find tag actions!");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unknown error: {e.Message}");
                }

                return "";
            }

        }


    }
}

