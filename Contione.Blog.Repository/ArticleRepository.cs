using Contione.Blog.Repository.Base;
using System;
using Contione.Blog.Model.DataModel;
using Contione.Blog.IRepository;
using Contione.Blog.Model.DbContext;
using Contone.Blog.IRepository;

namespace Contione.Blog.Repository
{
   public class ArticleRepository : BaseRepository<Article, Guid>, IArticleRepository
    {
        public ArticleRepository(ModelBaseContext context) : base(context)
        {
            //_context = context;
        }
    }
}
