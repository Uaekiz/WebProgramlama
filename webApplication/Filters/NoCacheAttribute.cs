using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;

namespace webApplication.Filters // Projenizin ana ad alanına (namespace) uyumlu
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // Tarayıcıya önbellekleme yapmamasını söyleyen HTTP başlıkları ekle
            context.HttpContext.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers[HeaderNames.Pragma] = "no-cache";
            context.HttpContext.Response.Headers[HeaderNames.Expires] = "-1"; // Hemen sona erdir (süresi geçmiş)

            base.OnResultExecuting(context);
        }
    }
}