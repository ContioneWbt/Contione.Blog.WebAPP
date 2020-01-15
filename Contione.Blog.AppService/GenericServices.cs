using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Contione.Blog.AppService.Base;
using Contione.Blog.IAppService;
using Contione.Blog.IAppService.Base;
using Contione.Blog.IRepository;
using Contione.Blog.Model;
using Contione.Blog.Model.DataModel;
using Contone.Blog.IRepository;

namespace Contione.Blog.AppService
{
    public class GenericServices<T>: BaseServices<T, Guid>, IGenericServices<T> where T : AggregateRoot, new()
    {
        private readonly IGenericRepository<T> _iArticleRepository;

        public GenericServices(IGenericRepository<T> iGenericRepository)
        {
            base.BaseRepository = iGenericRepository;//如果要用基类封装的方法必须传值
            _iArticleRepository = iGenericRepository;
        }

        public async Task AddModel(T model)
        {
            await Add(model);
        }

        public async Task<Tuple<List<T>, int>> GetList()
        {
            var dt = await _iArticleRepository.Query(x => true, 1, 10);
            return dt;
        }
    }
}
