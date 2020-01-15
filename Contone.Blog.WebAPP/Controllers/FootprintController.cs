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

namespace Contione.Blog.WebAPP.Controllers
{
    public class FootprintController : Controller
    {
        public IActionResult Timeline()
        {
            return View();
        }

        public IActionResult Record()
        {
            return View();
        }

        public IActionResult Note()
        {
            return View();
        }

    }
}
