using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;

using Rotativa.NetCore.Extensions;

namespace Rotativa.NetCore
{
    public class ViewAsImage : AsImageResultBase
    {
        private string viewName;

        public string ViewName
        {
            get => this.viewName ?? string.Empty;
            set => this.viewName = value;
        }

        private string masterName;

        public string MasterName
        {
            get => this.masterName ?? string.Empty;
            set => this.masterName = value;
        }

        public object Model { get; set; }

        public ViewAsImage()
        {
            this.WkhtmlPath = string.Empty;
            this.MasterName = string.Empty;
            this.ViewName = string.Empty;
            this.Model = null;
        }

        public ViewAsImage(string viewName)
            : this()
        {
            this.ViewName = viewName;
        }

        public ViewAsImage(object model)
            : this()
        {
            this.Model = model;
        }

        public ViewAsImage(string viewName, object model)
            : this()
        {
            this.ViewName = viewName;
            this.Model = model;
        }

        public ViewAsImage(string viewName, string masterName, object model)
            : this(viewName, model)
        {
            this.MasterName = masterName;
        }

        protected override string GetUrl(ActionContext context)
        {
            return string.Empty;
        }

        protected virtual ViewEngineResult GetView(ActionContext context, string viewName, string masterName)
        {
            var compositeViewEngine = context.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            return compositeViewEngine.FindView(context, viewName, false);
        }

        protected override byte[] CallTheDriver(ActionContext context)
        {
            // use action name if the view name was not provided
            string viewName = this.ViewName;
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = context.ActionDescriptor.DisplayName;
            }

            ViewEngineResult viewResult = this.GetView(context, viewName, this.MasterName);
            string html = context.GetHtmlFromView(viewResult, viewName, this.Model);
            byte[] fileContent = WkhtmltoimageDriver.ConvertHtml(this.WkhtmlPath, this.GetConvertOptions(), html);
            return fileContent;
        }
    }
}