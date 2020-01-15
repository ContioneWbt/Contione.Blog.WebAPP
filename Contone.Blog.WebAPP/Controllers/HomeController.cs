using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Contione.Blog.WebAPP.Models;
using Contione.Blog.IAppService;
using Contione.Blog.Model.DataModel;
using Contione.Blog.IAppService.Base;
using Contione.Blog.Infrastructure.Attribute;

namespace Contione.Blog.WebAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IArticleServices _iArticleServices;
        private readonly IGenericServices<Notification> _iNotificationServices;
        public HomeController(IArticleServices iArticleServices, IGenericServices<Notification> iNotificationServices)
        {
            _iArticleServices = iArticleServices;
            _iNotificationServices = iNotificationServices;
        }

        [ResponseCache(Duration = 60 * 60)]
        public async Task<IActionResult> Index()
        {

            var articles = await _iArticleServices.Query();

            var notifications = await _iNotificationServices.Query();

            return View(new Tuple<List<Notification>, List<Article>>
                (
                    notifications,
                    articles.OrderBy(o => o.Sort).ToList())
                );
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.Client, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
       
    }
}
