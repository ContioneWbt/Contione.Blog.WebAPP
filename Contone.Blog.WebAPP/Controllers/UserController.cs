using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Contione.Blog.WebAPP.Models;
using Contione.Blog.IAppService;
using Contione.Blog.Model.DataModel;
using Contione.Blog.Infrastructure.Utility;

namespace H_Bolg.Controllers
{
    public class UserController : Controller
    {
        #region IOC_DI

        private readonly IGenericServices<QQUser> _iQQUserServices;

        public UserController(IGenericServices<QQUser> iQQUserServices)
        {
            _iQQUserServices = iQQUserServices;
        }


        #endregion

        [AllowAnonymous]
        public IActionResult Login(string provider = "QQ", string returnUrl = null)
        {
            //第三方登录成功后跳转的地址
            var redirectUrl = Url.Action(nameof(UserCallback), new { returnUrl });
            var properties = new AuthenticationProperties()
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> UserCallback(string returnUrl = null)
        {
            //QQ认证后会默认登录，如果你想自定义登录，可以先注销第三方登录的身份
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            string openId = "", name = "", figure = "", gender = "";
            foreach (var item in HttpContext.User.Claims)
            {
                switch (item.Type)
                {
                    case MyClaimTypes.QQOpenId:
                        openId = item.Value;
                        break;
                    case MyClaimTypes.QQName:
                        name = item.Value;
                        break;
                    case MyClaimTypes.QQFigure:
                        figure = item.Value;
                        break;
                    case MyClaimTypes.QQGender:
                        gender = item.Value;
                        break;
                    default:
                        break;
                }
            }
            //获取到OpenId后进行登录或者注册
            if (!openId.IsNullOrEmpty())
            {
                //去数据库查询该QQ是否绑定用户
                var user = await _iQQUserServices.Query(o => o.QQOpenId == openId);

                if (user.IsNoContent())
                {
                    QQUser userInfo = user.FirstOrDefault();
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Sid, userInfo.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Name));
                    identity.AddClaim(new Claim(MyClaimTypes.QQFigure, userInfo.Avatar));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.AddHours(3) // 有效时间
                    });
                    userInfo.LastLoginIP = HttpContext.GetUserIp();
                    userInfo.LastLoginTime = DateTime.Now;

                    ////更新登录信息
                    await _iQQUserServices.Update(userInfo);

                    if (returnUrl != null)
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("index", "home");
                }
                else
                {

                    QQUser userModel = new QQUser();
                    userModel.QQOpenId = openId;
                    userModel.Name = name;
                    userModel.Avatar = figure;
                    userModel.Gender = gender;
                    userModel.CreateTime = DateTime.Now;
                    //注册
                    await _iQQUserServices.Add(userModel);
                    #region 注册后自动登陆
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Sid, userModel.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userModel.Name));
                    identity.AddClaim(new Claim(MyClaimTypes.QQFigure, userModel.Avatar));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.AddHours(3)// 有效时间
                    });
                    userModel.LastLoginIP = HttpContext.GetUserIp();
                    userModel.LastLoginTime = DateTime.Now;
                    ////更新登录信息
                    await _iQQUserServices.Update(userModel);
                    #endregion
                    if (returnUrl != null)
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("index", "home");
                }
            }
            else
                throw new Exception("OpenId is null");
        }

        public async Task<ActionResult> Logout(string returnUrl)
        {
            await base.HttpContext.SignOutAsync();
            return this.Redirect(returnUrl);
        }
    }
}
