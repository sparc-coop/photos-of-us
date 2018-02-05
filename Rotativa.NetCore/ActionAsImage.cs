using System;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Rotativa.NetCore
{
    public class ActionAsImage : AsImageResultBase
    {
        private readonly RouteValueDictionary routeValuesDict;
        private readonly object routeValues;
        private readonly string action;

        public ActionAsImage(string action)
        {
            this.action = action;
        }

        public ActionAsImage(string action, RouteValueDictionary routeValues)
            : this(action)
        {
            this.routeValuesDict = routeValues;
        }

        public ActionAsImage(string action, object routeValues)
            : this(action)
        {
            this.routeValues = routeValues;
        }

        protected override string GetUrl(ActionContext context)
        {
            var urlHelperFactory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(context);

            string actionUrl;
            if (this.routeValues == null)
            {
                actionUrl = urlHelper.Action(this.action, this.routeValuesDict);
            }
            else if (this.routeValues != null)
            {
                actionUrl = urlHelper.Action(this.action, this.routeValues);
            }
            else
            {
                actionUrl = urlHelper.Action(this.action);
            }

            var currentUri = new Uri(context.HttpContext.Request.GetDisplayUrl());
            var authority = currentUri.GetComponents(UriComponents.StrongAuthority, UriFormat.Unescaped);

            var url = string.Format("{0}://{1}{2}", context.HttpContext.Request.Scheme, authority, actionUrl);
            return url;
        }
    }
}
