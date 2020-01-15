using System;
using System.Collections.Generic;
using System.Text;
using Contione.Blog.IRepository.Base;
using Contione.Blog.Model.DataModel;

namespace Contone.Blog.IRepository
{
    public interface IArticleRepository : IBaseRepository<Article, Guid>
    {

    }
}
