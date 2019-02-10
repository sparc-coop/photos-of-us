using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PhotosOfUs
{
    public class AngularModelViewComponent : ViewComponent
    {
        public string AppName { get; set; }
        public string ModelName { get; set; }
        public HtmlString Json { get; set; }
        public bool HasClientSideData { get; set; }
        
        public IViewComponentResult Invoke(object model, string appName = "app", string modelName = "model")
        {
            AppName = appName;
            ModelName = modelName;
            var clientSideModels = model.GetType().GetProperties()
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(ClientSideAttribute)))
                .Select(x => new {
                    x.Name,
                    Value = x.GetValue(model)
                });

            var json = "{" + 
                string.Join(", ", clientSideModels.Select(x => $"\"{x.Name}\": {JsonConvert.SerializeObject(x.Value)}")) + 
            "}";
            Json = new HtmlString(json);
            HasClientSideData = clientSideModels.Any();

            return View();
        }
    }
}