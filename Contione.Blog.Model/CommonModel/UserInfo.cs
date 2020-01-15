using System;
using System.Collections.Generic;
using System.Text;

namespace Contione.Blog.Model.CommonModel
{
    public class UserInfo
    {
        public Guid SysUserId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int? UserType { get; set; }
        public string Telphone { get; set; }
    }
}
