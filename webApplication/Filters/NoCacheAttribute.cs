using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;

namespace webApplication.Filters 
{
    public class NoCacheAttribute : ActionFilterAttribute   //As you know, dotnet is a little bit bad for handling with turn back button which 
    //is not correctly working when you log out, so we must use this template filter class 
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers[HeaderNames.Pragma] = "no-cache";
            context.HttpContext.Response.Headers[HeaderNames.Expires] = "-1"; 

            base.OnResultExecuting(context);
        }
    }
}