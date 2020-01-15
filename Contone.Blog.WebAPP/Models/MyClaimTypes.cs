using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Contione.Blog.WebAPP.Models
{
    public class MyClaimTypes
    {
        public const string QQOpenId = ClaimTypes.NameIdentifier;
        public const string QQName = ClaimTypes.Name;
        public const string QQFigure = "urn:qq:figure";
        public const string QQGender = ClaimTypes.Gender;

    }
    public static class HttpContextExtension
    {
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
    }
}
