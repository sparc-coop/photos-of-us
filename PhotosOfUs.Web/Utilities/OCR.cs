using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public OCR(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public OCRModelResult GetOCRResult(IFormFile imageFile)
        {
           
                OCRModelResult model = new OCRModelResult();

                // for now just fail hard if there's any error however in a propper app I would expect a full demo.

                using (var engine = new TesseractEngine(Path.Combine(_environment.WebRootPath, "tessdata"), "eng", EngineMode.Default))
                {
                //have to load Pix via a bitmap since Pix doesn't support loading a stream.
                using (var image = new Bitmap(new FileStream(Path.Combine(_environment.WebRootPath, imageFile.FileName), FileMode.Create)))
                {

                    using (var page = engine.Process(image))
                    {
                        model.MeanConfidence = page.GetMeanConfidence();
                        model.Text = page.GetText();
                    }

                }
            }
                
            

            return null;
        }
    }
}
