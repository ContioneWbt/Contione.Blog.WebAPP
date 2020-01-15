using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contione.Blog.IAppService.Base;
using Contione.Blog.Model;
using Contione.Blog.Model.DataModel;

namespace Contione.Blog.IAppService
{
    public interface IGenericServices<T> : IBaseServices<T, Guid> where T : AggregateRoot, new()
    {
        Task AddModel(T model);
        Task<Tuple<List<T>, int>> GetList();
    }
}
