using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contione.Blog.IAppService.Base;
using Contione.Blog.Model.DataModel;

namespace Contione.Blog.IAppService
{
    public interface IArticleTypeServices : IBaseServices<ArticleType, Guid>
    {
        Task AddModel(ArticleType model);
        Task<Tuple<List<ArticleType>, int>> GetList();
    }
}
