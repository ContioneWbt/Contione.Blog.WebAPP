﻿namespace Contione.Blog.Model.CommonModel.ResultModel
{
    /// <summary>
    /// 可选项目
    /// </summary>
    /// <typeparam name="IText">标签数据的类型</typeparam>
    /// <typeparam name="IValue">值数据的类型</typeparam>
    public interface IOptionItem<TText, TValue>
    {
        /// <summary>
        /// 标签
        /// </summary>
        TText Text { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        TValue Value { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        bool Disabled { get; set; }
    }
}
