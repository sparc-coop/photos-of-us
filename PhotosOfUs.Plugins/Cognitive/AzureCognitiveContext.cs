using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using PhotosOfUs.Model.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;

namespace PhotosOfUs.Connectors.Cognitive
{
    public class AzureCognitiveContext : ICognitiveContext
    {
        const string subscriptionKey = "";
        const string uriBase = "https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/";    

        public async Task<List<string>> GetSuggestedTags(byte[] file)
        {
            var result = await MakeRequest(file, "tags");
            return ExtractTags(result);
        }

        public async Task<string> GetPhotoCode(byte[] file, List<string> possibleCodes)
        {
            var result = await MakeRequest(file, "ocr");
            return ExtractCardCode(result, possibleCodes);
        }

        private async Task<CognitiveResponse> MakeRequest(byte[] file, string cognitiveType)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            var tagsQueryString = "analyze?visualFeatures=Tags&language=en";
            var ocrQueryString = "ocr?language=unk&detectOrientation=true";

            string uri = "";

            // Assemble the URI for the REST API Call.
            if (cognitiveType == "ocr")
            {
                uri = uriBase + ocrQueryString;
            }
            else if (cognitiveType == "tags")
            {
                uri = uriBase + tagsQueryString;
            }

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = file;

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Make the REST API call.
                response = await client.PostAsync(uri, content);
            }

            // Get the JSON response.
            string contentString = await response.Content.ReadAsStringAsync();
                                              
            CognitiveResponse rootobject = JsonConvert.DeserializeObject<CognitiveResponse>(contentString);
            return rootobject;
        }

        public string ExtractCardCode(CognitiveResponse result, List<string> possibleCodes)
        {
            foreach (Region region in result.regions)
            {
                foreach (Line line in region.lines)
                {
                    foreach (Word word in line.words)
                    {
                        var onlyLettersAndNumbers = new string(word.text.Where(c => Char.IsLetter(c) || Char.IsNumber(c)).ToArray()).ToUpper();

                        string[] numbers = Regex.Split(onlyLettersAndNumbers, @"\D+");

                        foreach (var l in numbers)
                        {
                            if (l.Count() == 7)
                            {
                                var code = possibleCodes.Find(x => x.ToUpper() == l.ToUpper());

                                if (code != null)
                                {
                                    return code;
                                }
                            }
                        }
                    }
                }
                return null;
            }
            return null;
        }

        private List<string> ExtractTags(CognitiveResponse result)
        {
            var sortedresults = result.tags.OrderBy(o => o.confidence).ToList();

            var tags = new List<string>();

            foreach (AzureTag tag in sortedresults)
            {
                tags.Add(tag.name);
            }

            return tags;
        }

        public static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
