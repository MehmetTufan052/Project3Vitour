using Microsoft.AspNetCore.Http;

namespace Project3Vitour.Helpers
{
    public static class RequestLanguageHelper
    {
        public static string GetCurrentLanguage(HttpContext? httpContext)
        {
            var lang = httpContext?.Request.Query["lang"].ToString();
            if (IsSupported(lang))
            {
                return lang!.ToLowerInvariant();
            }

            lang = httpContext?.Request.Cookies["site_lang"];
            if (IsSupported(lang))
            {
                return lang!.ToLowerInvariant();
            }

            return "tr";
        }

        private static bool IsSupported(string? lang)
        {
            return lang is "tr" or "en" or "fr" or "es";
        }
    }
}
