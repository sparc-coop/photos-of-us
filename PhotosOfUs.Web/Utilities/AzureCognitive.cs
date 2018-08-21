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
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace PhotosOfUs.Web.Utilities
{


    public class AzureCognitive
    {
        private PhotosOfUsContext _context;

        public AzureCognitive(PhotosOfUsContext context)
        {
            _context = context;
        }

        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "";

        // You must use the same region in your REST call as you used to
        // get your subscription keys. For example, if you got your
        // subscription keys from westus, replace "westcentralus" in the URL
        // below with "westus".
        //
        // Free trial subscription keys are generated in the westcentralus region.
        // If you use a free trial subscription key, you shouldn't need to change
        // this region.
        const string uriBase =
            //"https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/ocr";
            "https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/";    
        //old - "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";

        //tutorial url: "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";

        public async Task<RootObject> MakeRequest(byte[] file, string cognitiveType)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters.
            //string requestParameters = "language=unk&detectOrientation=true";


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

            //string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = file;

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Make the REST API call.
                response = await client.PostAsync(uri, content);
            }

            // Get the JSON response.
            string contentString = await response.Content.ReadAsStringAsync();
                                              
            RootObject rootobject = JsonConvert.DeserializeObject<RootObject>(contentString);

            // Display the JSON response.
            //Console.WriteLine("\nResponse:\n\n{0}\n",
            //    JToken.Parse(contentString).ToString());

            return rootobject;
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>

        public string ExtractCardCode(RootObject result)
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
                                var code = _context.Card.Where(c => c.Code.ToUpper() == l.ToUpper());

                                if (code.Count() > 0)
                                {
                                    return code.First().Code;
                                }
                            }
                        }
                    }
                    
                }
                return string.Empty;
            }
            return string.Empty;
        }

        public List<string> ExtractTags(RootObject result)
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

        public static byte[] TransformImageIntoBytes(IFormFile file)
        {
            byte[] fileBytes;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
            }
            return fileBytes;
        }

        //static async void MakeRequest(string imageFilePath)
        //{
        //    var client = new HttpClient();
        //    var queryString = HttpUtility.ParseQueryString(string.Empty);

        //    // Request headers
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

        //    // Request parameters
        //    queryString["language"] = "unk";
        //    queryString["detectOrientation "] = "true";
        //    //var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + queryString;
        //    var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + queryString;

        //    HttpResponseMessage response;

        //    // Request body
        //    byte[] byteData = Encoding.UTF8.GetBytes("{body}");

        //    using (var content = new ByteArrayContent(byteData))
        //    {
        //        content.Headers.ContentType = new MediaTypeHeaderValue("< your content type, i.e. application/json >");
        //        response = await client.PostAsync(uri, content);
        //    }

        //    byte[] byteData = GetImageAsByteArray(imageFilePath);

        //    using (ByteArrayContent content = new ByteArrayContent(byteData))
        //    {
        //        // This example uses content type "application/octet-stream".
        //        // The other content types you can use are "application/json"
        //        // and "multipart/form-data".
        //        content.Headers.ContentType =
        //            new MediaTypeHeaderValue("application/octet-stream");

        //        // Make the REST API call.
        //        response = await client.PostAsync(uri, content);
        //    }

        //}


    }
}
