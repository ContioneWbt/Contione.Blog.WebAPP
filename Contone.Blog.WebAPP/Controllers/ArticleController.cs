using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Contione.Blog.Model.DataModel;
using Contione.Blog.IRepository.Base;
using Contione.Blog.IAppService.Base;
using Contione.Blog.IAppService;

namespace Contione.Blog.WebAPP.Controllers
{
    
    public class ArticleController : Controller
    {

        private readonly IGenericServices<Article> _iArticleServices;
        private readonly IGenericServices<ArticleType> _iArticleTypeServices;

        public ArticleController(IGenericServices<Article> iArticleServices, IGenericServices<ArticleType> iArticleTypeServices)
        {
            _iArticleServices = iArticleServices;
            _iArticleTypeServices = iArticleTypeServices;
        }

        public async Task<IActionResult> All(int IdentityId)
        {
            var articles = await _iArticleServices.Query();

            var articleType = await _iArticleTypeServices.Query();

            return View(new Tuple<List<Article>, List<ArticleType>>
                (
                     articles.OrderBy(o => o.Sort).ToList(),
                     articleType.ToList()
                ));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var result = await _iArticleServices.Query(o => o.IdentityId == id);
            if (result != null)

                return View(result.FirstOrDefault());
            else
                throw new Exception("没找到相关的文章！");
        }

        public async Task<IActionResult> Cate(int id)
        {
            var articles = await _iArticleServices.Query(o => o.ArticleType == id); 

            var articleTypes = await _iArticleTypeServices.Query();

            //获取标签Name
            ViewBag.Name = articleTypes?.FirstOrDefault(o => o.TypeId == id)?.Name;

            return View(new Tuple<List<Article>, List<ArticleType>>
                (
                    articles.OrderBy(o => o.Sort).ToList(),
                    articleTypes
                ));
        }

    }
}
