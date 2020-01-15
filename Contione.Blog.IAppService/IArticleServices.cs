using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contione.Blog.IAppService.Base;
using Contione.Blog.Model.DataModel;

namespace Contione.Blog.IAppService
{
    public interface IArticleServices : IBaseServices<Article, Guid>
    {
        Task AddModel(Article model);
        Task<Tuple<List<Article>, int>> GetList();

    }
}
