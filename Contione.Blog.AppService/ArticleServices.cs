using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Contione.Blog.AppService.Base;
using Contione.Blog.IAppService;
using Contione.Blog.IAppService.Base;
using Contione.Blog.Infrastructure.Attribute;
using Contione.Blog.IRepository;
using Contione.Blog.Model.DataModel;
using Contone.Blog.IRepository;

namespace Contione.Blog.AppService
{
    public class ArticleServices : BaseServices<Article, Guid>, IArticleServices
    {
        private readonly IArticleRepository _iArticleRepository;

        public ArticleServices(IArticleRepository iArticleRepository)
        {
            base.BaseRepository = iArticleRepository;//如果要用基类封装的方法必须传值
            _iArticleRepository = iArticleRepository;
        }

        public async Task AddModel(Article model)
        {
            await Add(model);
            //await Add()
            //await _iMainRepository.Add(model);
        }

        [Caching(AbsoluteExpiration = 30)]

        public async Task<Tuple<List<Article>, int>> GetList()
        {
            System.Linq.Expressions.Expression<Func<Article, bool>> where1 = x => x.ID.ToString() != "";
            //System.Linq.Expressions.Expression<Func<SysUser, string>> orderby1 = x => x.SysUserId.ToString();
            var dt = await _iArticleRepository.Query(where1, 1, 10);
            return dt;
        }

        [Caching(AbsoluteExpiration = 30)]
        public async Task<List<Article>> Query()
        {
            return await _iArticleRepository.Query();
        }
    }
}
