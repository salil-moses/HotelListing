using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _db = context.Set<T>();
        }
        public async Task Delete(int id)
        {
            // throw new NotImplementedException();
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            //throw new NotImplementedException();
            _db.RemoveRange(entities);
        }

        // public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        public async Task<T> Get(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)        
        {
            // throw new NotImplementedException();
            IQueryable<T> query = _db;
            /*Get the list of entity properties that the query is applied upon*/
            if(includes != null)
            {
                /*
                foreach(var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
                */
                query = includes(query); // This replaces above loop in our typed UOW
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        // public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            // throw new NotImplementedException();
            IQueryable<T> query = _db;

            if(expression != null)
            {
                query = query.Where(expression);
            }

            /*Get the list of entity properties that the query is applied upon*/
            if (includes != null)
            {
                /*
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
                */
                query = includes(query); // This replaces above loop in our typed UOW
            }

            if(orderBy != null)
            {
                query = orderBy(query);
            }


            return await query.AsNoTracking().ToListAsync();
        }

        // public async Task<IPagedList<T>> GetPagedList(RequestParams requestParams, List<string> includes = null)
        public async Task<IPagedList<T>> GetPagedList(RequestParams requestParams, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            // throw new NotImplementedException();
            IQueryable<T> query = _db;          

            /*Get the list of entity properties that the query is applied upon*/
            if (includes != null)
            {
                /*
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
                */
                query = includes(query); // This replaces above loop in our typed UOW
            }
           

            return await query.AsNoTracking().ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
        }

        public async Task Insert(T entity)
        {
            // throw new NotImplementedException();
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            // throw new NotImplementedException();
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            // throw new NotImplementedException();
            _db.Attach(entity);  // Tracks changes to the entity
            _context.Entry(entity).State = EntityState.Modified;  // Mark the entity as updated
        }
    }
}
