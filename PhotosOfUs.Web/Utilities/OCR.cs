using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;



namespace PhotosOfUs.Web.Utilities
{
    public class OCRModelResult
    {
        public float MeanConfidence { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }
    }


    public class OCR
    {
        private IHostingEnvironment _environment;
        private PhotosOfUsContext _context;

        public OCR(PhotosOfUsContext context,IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        public OCRModelResult GetOCRResult(IFormFile imageFile, int photographerId)
        {

            OCRModelResult model = new OCRModelResult();


            using (var engine = new TesseractEngine(Path.Combine(_environment.WebRootPath, "tessdata"), "eng", EngineMode.Default))
            {
                using (var image = new Bitmap(imageFile.OpenReadStream()))
                {

                    using (var page = engine.Process(image))
                    {
                        model.MeanConfidence = page.GetMeanConfidence();
                        model.Text = page.GetText();
                        model.Code = ExtractCode(model.Text,photographerId);

                        return model;
                    }

                }
            }
            
        }

        private string ExtractCode(string cardText, int photographerId)
        {
            //sometimes the string has some strange characteres added
            var onlyLettersAndNumbers = new string(cardText.Where(c=>Char.IsLetter(c) || Char.IsNumber(c)).ToArray()).ToUpper();
            //considering the code is between Photos Of Us and the photographer URL
            Regex regex = new Regex("PHOTOSOFUS(.*)WWWPHOTOSOFUS");

            string potencialCode = regex.Match(onlyLettersAndNumbers).Groups[1].ToString();

            var code = _context.Card.Where(c => c.Code.ToUpper() == potencialCode.ToUpper());

            if(code.Count() > 0)
            {
                return potencialCode;
            }
            return string.Empty;  
        }
    }
}
