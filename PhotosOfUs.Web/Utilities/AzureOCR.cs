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

namespace PhotosOfUs.Web.Utilities
{
    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public List<Word> words { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public List<Line> lines { get; set; }
    }

    public class RootObject
    {
        public string language { get; set; }
        public string orientation { get; set; }
        public double textAngle { get; set; }
        public List<Region> regions { get; set; }
    }

    public class AzureOCR
    {
        private PhotosOfUsContext _context;

        public AzureOCR(PhotosOfUsContext context)
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
            "https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/ocr";
            //"https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";

        //tutorial url: "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";

        public async Task<RootObject> MakeOCRRequest(byte[] file)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters.
            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

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

        public static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
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
