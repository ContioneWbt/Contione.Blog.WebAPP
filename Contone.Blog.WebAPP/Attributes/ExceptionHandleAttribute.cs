using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Contione.Blog.IAppService;
using Contione.Blog.Model.DataModel;

namespace Contione.Blog.WebAPP.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionFilterInvokeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionExecutedContext)
        {
            base.OnActionExecuting(actionExecutedContext);
        }
        public async override void  OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            var pathUrl = context.HttpContext.Request.Path.ToString();
            if (pathUrl?.Contains("/article/detail") ?? false)
            {
                var id = pathUrl.Split('/').LastOrDefault();

                var article = await _iArticleServices.Query(o => o.IdentityId.ToString() == id);

                if (article != null)
                {
                    Article updateArticle = article.FirstOrDefault();
                    updateArticle.ViewNumber = updateArticle.ViewNumber + 1;
                    await _iArticleServices.Update(updateArticle);
                }
            }
        }

        public ActionFilterInvokeAttribute(IGenericServices<Article> iArticleServices)
        {
            _iArticleServices = iArticleServices;
        }
        private readonly IGenericServices<Article> _iArticleServices;
    }

    /// <summary>
    /// 异常处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionHandleAttribute : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ILogger<ExceptionHandleAttribute> _logger;
        /// <summary>
        /// ioc来的
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="modelMetadataProvider"></param>
        public ExceptionHandleAttribute(
            IHostingEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider, ILogger<ExceptionHandleAttribute> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
            _logger = logger;
        }

        /// <summary>
        /// 没有处理的异常，就会进来
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)//异常有没有被处理过
            {
                string controllerName = (string)filterContext.RouteData.Values["controller"];
                string actionName = (string)filterContext.RouteData.Values["action"];
                string errorMsg = filterContext.Exception.Message;
                string msgTemplate = "在执行 controller[{0}] 的 action[{1}] 时产生异常,异常信息{2}";
                _logger.LogError(string.Format(msgTemplate, controllerName, actionName, errorMsg), filterContext.Exception);
                if (this.IsAjaxRequest(filterContext.HttpContext.Request))//检查请求头
                {
                    filterContext.Result = new JsonResult(
                         new
                         {
                             Result = false,
                             PromptMsg = "系统出现异常，请联系管理员",
                             DebugMessage = filterContext.Exception.Message
                         }//这个就是返回的结果
                    );
                }
                else
                {
                    var result = new ViewResult { ViewName = "~/Views/Home/Error.cshtml" };
                    result.ViewData = new ViewDataDictionary(_modelMetadataProvider, filterContext.ModelState);
                    result.ViewData.Add("Exception", filterContext.Exception);
                    filterContext.Result = result;
                }
                filterContext.ExceptionHandled = true;
            }
        }


        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
