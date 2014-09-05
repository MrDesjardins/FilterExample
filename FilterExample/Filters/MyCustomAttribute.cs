using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FilterExample.Filters
{
    public class MyCustomAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            if (response.ContentType == "text/html")
            {
                response.Filter = new MyCustomStream(response.Filter);
            }
        }
  
    }
}