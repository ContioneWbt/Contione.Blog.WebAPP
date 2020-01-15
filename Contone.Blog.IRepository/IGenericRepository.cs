using Contione.Blog.IRepository.Base;
using Contione.Blog.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contone.Blog.IRepository
{
    public interface IGenericRepository<T> : IBaseRepository<T, Guid> where T : AggregateRoot, new()
    {

    }
}
  