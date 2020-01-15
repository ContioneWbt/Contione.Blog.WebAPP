using System;
using System.Collections.Generic;
using System.Text;

namespace Contione.Blog.Infrastructure.StaticExtenions
{
    public class StaticConstraint
    {
        public static void Init(Func<string, string> func)
        {
            //数据库连接
            DBContextString = func("ConnectionStrings:DefaultConnection");

            //循环--反射的方式初始化多个
            string _desKey = func("DesKey");

            //加密解密
            RgbKey = ASCIIEncoding.ASCII.GetBytes(_desKey.Substring(0, 8));

            RgbIV = ASCIIEncoding.ASCII.GetBytes(_desKey.Insert(0, "w").Substring(0, 8));

            QQAppId = func("Authentication:QQ:AppId");

            QQAppKey = func("Authentication:QQ:AppKey");

            QQCallback = func("Authentication:QQ:CallbackPath");
        }


        /// <summary>
        /// 以前直接读配置文件 
        /// ConnectionStrings:ConnectionString
        /// </summary>
        public static string DBContextString = null;

        public static byte[] RgbKey = null;

        public static byte[] RgbIV = null;

        //qq登录信息授权
        public static string QQAppId = null;

        public static string QQAppKey = null;

        public static string QQCallback = null;

        #region  DLL

        public static string Repository => "Repository";
        public static string RepositoryDll => "Contione.Blog.Repository.dll";
        public static string AppService => "Contione.Blog.AppService.dll";
        public static string BlogModel => "Contione.Blog.Model";

        #endregion
    }
}
