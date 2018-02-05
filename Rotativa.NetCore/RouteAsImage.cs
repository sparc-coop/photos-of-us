using System;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Rotativa.NetCore
{
    public class RouteAsImage : AsImageResultBase
    {
        private readonly RouteValueDictionary routeValuesDict;
        private readonly object routeValues;
        private readonly string routeName;

        public RouteAsImage(string routeName)
        {
            this.routeName = routeName;
        }

        public RouteAsImage(string routeName, RouteValueDictionary routeValues)
            : this(routeName)
        {
            this.routeValuesDict = routeValues;
        }

        public RouteAsImage(string routeName, object routeValues)
            : this(routeName)
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
                actionUrl = urlHelper.RouteUrl(this.routeName, this.routeValuesDict);
            }
            else if (this.routeValues != null)
            {
                actionUrl = urlHelper.RouteUrl(this.routeName, this.routeValues);
            }
            else
            {
                actionUrl = urlHelper.RouteUrl(this.routeName);
            }

            var currentUri = new Uri(context.HttpContext.Request.GetDisplayUrl());
            var authority = currentUri.GetComponents(UriComponents.StrongAuthority, UriFormat.Unescaped);

            var url = string.Format("{0}://{1}{2}", context.HttpContext.Request.Scheme, authority, actionUrl);
            return url;
        }
    }
}
