using Contione.Blog.Repository.Base;
using System;
using Contione.Blog.Model.DataModel;
using Contione.Blog.IRepository;
using Contione.Blog.Model.DbContext;
using Contone.Blog.IRepository;
using Contione.Blog.Model;

namespace Contione.Blog.Repository
{
   public class GenericRepository<T> : BaseRepository<T, Guid>, IGenericRepository<T> where T : AggregateRoot, new()
    {
        public GenericRepository(ModelBaseContext context) : base(context)
        {
            //_context = context;
        }
    }
}
