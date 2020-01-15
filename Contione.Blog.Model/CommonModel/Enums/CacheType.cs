using System;
using System.Collections.Generic;
using System.Text;

namespace Contione.Blog.Model.CommonModel.Enums
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// .Net Core 自带缓存
        /// </summary>
        InMemory = 0,
        /// <summary>
        /// Redis 分布式缓存
        /// </summary>
        Redis = 1
    }
}
