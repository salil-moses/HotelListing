﻿using HotelListing.Models;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.IRepository
{
    public interface IGenericRepository<T> where T: class
    {
        /*
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
        );
        */
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null 
        );

        /*Task<IPagedList<T>> GetPagedList(RequestParams requestParams, List<string> includes = null);*/
        Task<IPagedList<T>> GetPagedList(
            RequestParams requestParams,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null
        );



        /*param - 'expression' is a generic representation of the condition by which the table is filtered
         could be get by name property, or id property etc.
        param - 'includes' is a generic representation of the entity properties that should be searched through
         */
        /*Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null);*/
        Task<T> Get(
            Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null
        );

        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(int id);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);

    }
}
