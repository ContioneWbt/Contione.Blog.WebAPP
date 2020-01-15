﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Contione.Blog.IRepository.Base;
using Contione.Blog.Model;
using Contione.Blog.Model.DbContext;
using Contione.Blog.Model.DataModel;

namespace Contione.Blog.Repository.Base
{
    public class BaseRepository<T, TEntityKey> : IBaseRepository<T, TEntityKey> where T : AggregateRoot, new()
    {
        #region 成员及构造
        public ModelBaseContext _context { get; set; } //从httpcontext中获取ef上下文

        //下面这个方法是自己维护EF上下文，原理是将EF上下文放在HTTP上下文里，来保持每次请求使用同一个EF上下文，HTTP接口请求结束之后销毁HTTP上下文，EF也会跟着销毁
        //public ModelBaseContext _context = DataContextFactory.GetDataContext();

        public BaseRepository(ModelBaseContext contex)
        {
            _context = contex;//构造函数接收容器中的EF上下文
        }
        #endregion

        #region 增删改操作
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task Add(T entity)
        {
            await Task.Run(() =>
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task Delete(T entity)
        {
            await Task.Run(() =>
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task Update(T entity)
        {
            await Task.Run(() =>
            {
                _context.Set<T>().Update(entity);
                _context.SaveChanges();
            });
        }
        #endregion

        #region  SQL CURD操作
        /// <summary>
        /// SQL语句查询
        /// </summary>
        /// <param name="querySql">SQL语句</param>
        /// <returns></returns>
        public async Task<List<T>> QueryBySql(string querySql)
        {
            return await Task.Run(() => GetObjectSet().FromSql(querySql).ToList());
        }

        /// <summary>
        /// SQL语句分页查询
        /// </summary>
        /// <param name="querySql">SQL语句</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        public async Task<List<T>> QueryBySql(string querySql, int pageIndex, int pageSize)
        {
            return await Task.Run(() => GetObjectSet().FromSql(querySql).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList());
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task ExecuteSql(string sql)
        {
            await Task.Run(() => _context.Database.ExecuteSqlCommand(sql));
        }

        /// <summary>
        /// 执行SQL带参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task ExecuteSql(string sql, params object[] parameters)
        {
            await Task.Run(() => _context.Database.ExecuteSqlCommand(sql, parameters));
        }
        #endregion

        #region 跟踪查询
        /// <summary>
        /// 根据主键Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> QueryById(TEntityKey id)
        {
            return await Task.Run(() => _context.Set<T>().Find(id));
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> Query()
        {
            return await Task.Run(() => GetObjectSet().ToList());
        }

        /// <summary>
        /// 分页查询<see cref="T"/>所有数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<T>> Query(int pageIndex, int pageSize)
        {
            return await Task.Run(() => GetObjectSet().Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList());
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<List<T>> Query(Expression<Func<T, bool>> where)
        {
            return await Task.Run(() => GetObjectSet().Where(where).ToList());
        }

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> Query(Expression<Func<T, bool>> where, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                list = list.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                return new Tuple<List<T>, int>(list.ToList(), list.Count());
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> Query<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, bool isAsc)
        {
            return await Task.Run(() =>
            {
                if (isAsc)
                {
                    return GetObjectSet().Where(where).OrderBy(orderBy1).ToList();
                }
                else
                {
                    return GetObjectSet().Where(where).OrderByDescending(orderBy1).ToList();
                }
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> Query<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                if (isAsc)
                {
                    return new Tuple<List<T>, int>(GetObjectSet().Where(where).OrderBy(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), GetObjectSet().Where(where).Count());
                }
                else
                {
                    return new Tuple<List<T>, int>(GetObjectSet().Where(where).OrderByDescending(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), GetObjectSet().Where(where).Count());
                }
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> Query<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, bool isAsc)
        {
            return await Task.Run(() =>
            {
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).ToList();
                    }
                    else
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy1).ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy1).ToList();
                    }
                }
                else
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy2).ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy2).ToList();
                    }
                    else
                    {
                        return GetObjectSet().Where(where).ToList();//排序都为null
                    }
                }
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> Query<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                    }
                }
                else
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                    }
                    else
                    {
                        return new Tuple<List<T>, int>(list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                    }
                }
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> Query<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, Expression<Func<T, A>> orderBy3, bool isAsc)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).ThenBy<T, A>(orderBy3)
                                    .ToList();
                            else
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2)
                                    .ThenByDescending<T, A>(orderBy3).ToList();
                        }
                        else
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).ToList();
                            else
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).ToList();
                        }

                    }
                    else
                    {
                        if (isAsc)
                            return list.OrderBy<T, A>(orderBy1).ToList();
                        else
                            return list.OrderByDescending<T, A>(orderBy1).ToList();
                    }
                }
                return list.ToList();
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> Query<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, Expression<Func<T, A>> orderBy3, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).ThenBy<T, A>(orderBy3).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                            else
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2)
                                    .ThenByDescending<T, A>(orderBy3).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        }
                        else
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                            else
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        }

                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                    }
                }
                return new Tuple<List<T>, int>(list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="B"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> Query<A, B>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, bool isAsc)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).ToList();
                    }
                    else
                    {
                        if (isAsc)
                            return list.OrderBy<T, A>(orderBy1).ToList();
                        else
                            return list.OrderByDescending<T, A>(orderBy1).ToList();
                    }
                }
                else
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, B>(orderBy2).ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, B>(orderBy2).ToList();
                    }
                    else
                    {
                        return GetObjectSet().Where(where).ToList();
                    }
                }
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="B"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> Query<A, B>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(), list.Count());
                    }
                }
                return new Tuple<List<T>, int>(list.ToList(), list.Count());
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="B"/><see cref="C"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> Query<A, B, C>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, Expression<Func<T, C>> orderBy3, bool isAsc)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).ThenBy<T, C>(orderBy3)
                                    .ToList();
                            else
                            {
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2)
                                    .ThenByDescending<T, C>(orderBy3).ToList();
                            }


                        }
                        else
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).ToList();
                            else
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).ToList();
                        }
                    }
                    else
                    {
                        if (isAsc)
                            return list.OrderBy<T, A>(orderBy1).ToList();
                        else
                            return list.OrderByDescending<T, A>(orderBy1).ToList();
                    }

                }

                return list.ToList();
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="B"/><see cref="C"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> Query<A, B, C>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, Expression<Func<T, C>> orderBy3, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).ThenBy<T, C>(orderBy3).ToList(), list.Count());
                            else
                            {
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).ThenByDescending<T, C>(orderBy3).AsQueryable().ToList(), list.Count());
                            }
                        }
                        else
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).ToList(), list.Count());
                            else
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).ToList(), list.Count());
                        }
                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ToList(), list.Count());
                    }

                }
                return new Tuple<List<T>, int>(list.ToList(), list.Count());
            });
        }
        #endregion

        #region AsNoTracking非跟踪查询
        /// <summary>
        /// 根据主键Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> QueryAsNoTrackingById(TEntityKey id)
        {
            return await Task.Run(() => _context.Set<T>().Find(id));
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking()
        {
            return await Task.Run(() => GetObjectSet().AsNoTracking().ToList());
        }

        /// <summary>
        /// 分页查询<see cref="T"/>所有数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking(int pageIndex, int pageSize)
        {
            return await Task.Run(() => GetObjectSet().Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList());
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking(Expression<Func<T, bool>> where)
        {
            return await Task.Run(() => GetObjectSet().Where(where).AsNoTracking().ToList());
        }

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> QueryAsNoTracking(Expression<Func<T, bool>> where, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                list = list.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                return new Tuple<List<T>, int>(list.AsNoTracking().ToList(), list.Count());
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, bool isAsc)
        {
            return await Task.Run(() =>
            {
                if (isAsc)
                {
                    return GetObjectSet().Where(where).OrderBy(orderBy1).AsNoTracking().ToList();
                }
                else
                {
                    return GetObjectSet().Where(where).OrderByDescending(orderBy1).AsNoTracking().ToList();
                }
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> QueryAsNoTracking<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                if (isAsc)
                {
                    return new Tuple<List<T>, int>(GetObjectSet().Where(where).OrderBy(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), GetObjectSet().Where(where).Count());
                }
                else
                {
                    return new Tuple<List<T>, int>(GetObjectSet().Where(where).OrderByDescending(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), GetObjectSet().Where(where).Count());
                }
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, bool isAsc)
        {
            return await Task.Run(() =>
            {
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).AsNoTracking().ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).AsNoTracking().ToList();
                    }
                    else
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy1).AsNoTracking().ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy1).AsNoTracking().ToList();
                    }
                }
                else
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy2).AsNoTracking().ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy2).AsNoTracking().ToList();
                    }
                    else
                    {
                        return GetObjectSet().Where(where).AsNoTracking().ToList();//排序都为null
                    }
                }
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> QueryAsNoTracking<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                    }
                }
                else
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                    }
                    else
                    {
                        return new Tuple<List<T>, int>(list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                    }
                }
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, Expression<Func<T, A>> orderBy3, bool isAsc)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).ThenBy<T, A>(orderBy3)
                                    .AsNoTracking().ToList();
                            else
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2)
                                    .ThenByDescending<T, A>(orderBy3).AsNoTracking().ToList();
                        }
                        else
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).AsNoTracking().ToList();
                            else
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).AsNoTracking().ToList();
                        }

                    }
                    else
                    {
                        if (isAsc)
                            return list.OrderBy<T, A>(orderBy1).AsNoTracking().ToList();
                        else
                            return list.OrderByDescending<T, A>(orderBy1).AsNoTracking().ToList();
                    }
                }
                return list.AsNoTracking().ToList();
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="A"/><see cref="A"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> QueryAsNoTracking<A>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, A>> orderBy2, Expression<Func<T, A>> orderBy3, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).ThenBy<T, A>(orderBy3).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                            else
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2)
                                    .ThenByDescending<T, A>(orderBy3).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        }
                        else
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                            else
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, A>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        }

                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                    }
                }
                return new Tuple<List<T>, int>(list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="B"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking<A, B>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, bool isAsc)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).AsNoTracking().ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).AsNoTracking().ToList();
                    }
                    else
                    {
                        if (isAsc)
                            return list.OrderBy<T, A>(orderBy1).AsNoTracking().ToList();
                        else
                            return list.OrderByDescending<T, A>(orderBy1).AsNoTracking().ToList();
                    }
                }
                else
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return GetObjectSet().Where(where).OrderBy<T, B>(orderBy2).AsNoTracking().ToList();
                        else
                            return GetObjectSet().Where(where).OrderByDescending<T, B>(orderBy2).AsNoTracking().ToList();
                    }
                    else
                    {
                        return GetObjectSet().Where(where).AsNoTracking().ToList();
                    }
                }
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="B"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> QueryAsNoTracking<A, B>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList(), list.Count());
                    }
                }
                return new Tuple<List<T>, int>(list.AsNoTracking().ToList(), list.Count());
            });
        }

        /// <summary>
        /// 根据条件查询、并按照<see cref="A"/><see cref="B"/><see cref="C"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsNoTracking<A, B, C>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, Expression<Func<T, C>> orderBy3, bool isAsc)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).ThenBy<T, C>(orderBy3)
                                    .AsNoTracking().ToList();
                            else
                            {
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2)
                                    .ThenByDescending<T, C>(orderBy3).AsNoTracking().ToList();
                            }


                        }
                        else
                        {
                            if (isAsc)
                                return list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).AsNoTracking().ToList();
                            else
                                return list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).AsNoTracking().ToList();
                        }
                    }
                    else
                    {
                        if (isAsc)
                            return list.OrderBy<T, A>(orderBy1).AsNoTracking().ToList();
                        else
                            return list.OrderByDescending<T, A>(orderBy1).AsNoTracking().ToList();
                    }

                }

                return list.AsNoTracking().ToList();
            });
        }

        /// <summary>
        /// 根据条件分页查询、并按照<see cref="A"/><see cref="B"/><see cref="C"/>排序
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy1"></param>
        /// <param name="orderBy2"></param>
        /// <param name="orderBy3"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> QueryAsNoTracking<A, B, C>(Expression<Func<T, bool>> where, Expression<Func<T, A>> orderBy1, Expression<Func<T, B>> orderBy2, Expression<Func<T, C>> orderBy3, bool isAsc, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                var list = GetObjectSet();
                if (where != null)
                    list = GetObjectSet().Where(where);
                if (orderBy1 != null)
                {
                    if (orderBy2 != null)
                    {
                        if (orderBy3 != null)
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).ThenBy<T, C>(orderBy3).AsNoTracking().ToList(), list.Count());
                            else
                            {
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).ThenByDescending<T, C>(orderBy3).AsNoTracking().AsNoTracking().ToList(), list.Count());
                            }
                        }
                        else
                        {
                            if (isAsc)
                                return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).ThenBy<T, B>(orderBy2).AsNoTracking().ToList(), list.Count());
                            else
                                return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).ThenByDescending<T, B>(orderBy2).AsNoTracking().ToList(), list.Count());
                        }
                    }
                    else
                    {
                        if (isAsc)
                            return new Tuple<List<T>, int>(list.OrderBy<T, A>(orderBy1).AsNoTracking().ToList(), list.Count());
                        else
                            return new Tuple<List<T>, int>(list.OrderByDescending<T, A>(orderBy1).AsNoTracking().ToList(), list.Count());
                    }

                }
                return new Tuple<List<T>, int>(list.AsNoTracking().ToList(), list.Count());
            });
        }
        #endregion

        #region 公共
        public IQueryable<T> GetObjectSet()
        {
            return _context.Set<T>();
        }
        #endregion
    }
}
