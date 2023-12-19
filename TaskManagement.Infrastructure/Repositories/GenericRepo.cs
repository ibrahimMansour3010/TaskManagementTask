using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Respiratory;
using TaskManagement.Infrastructure.Context;

namespace TaskManagement.Infrastructure.Repositories
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        #region CTOR & Definitions
        private readonly AppDBContext _context;
        private readonly DbSet<T> dbSet;
        public GenericRepo(AppDBContext context)
        {
            _context = context;
            dbSet = context.Set<T>();
        }
        #endregion

        #region GenericRepo Methods
        public virtual IQueryable<T> Get(Expression<Func<T, bool>>? filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrWhiteSpace(includeProperties))
                query = IncludeProperties(query, includeProperties);

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }
        public virtual IQueryable<T> GetNoTrack(Expression<Func<T, bool>>? filter = null,
         Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
         string includeProperties = "")
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter).AsNoTracking();
            }
            if (!string.IsNullOrWhiteSpace(includeProperties))
                query = IncludeProperties(query, includeProperties);

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }
        public virtual async Task<T?> GetOneAsyncNoTrack(Expression<Func<T, bool>>? filter = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
                query = IncludeProperties(query, includeProperties);

            return await query.AsNoTracking().FirstOrDefaultAsync();

        }
        public async Task<T> GetOneAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
                query = IncludeProperties(query, includeProperties);

            return await query.FirstOrDefaultAsync();

        }
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                return await query.Where(filter).AsNoTracking().CountAsync();
            else
                return await query.CountAsync();
        }
        public virtual async Task InsertAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }
        public virtual  T Update(T entity)
        {
            dbSet.Update(entity);
            return entity;
        }

        public virtual async Task<IQueryable<T>> GetPaginatedAsync(Expression<Func<T, bool>>? filter = null,
              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
              int? page = null,
              int? pageSize = null,
              string includeProperties = "")
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrWhiteSpace(includeProperties))
                query = IncludeProperties(query, includeProperties);
            if (page != null)
                query = query.Skip(((int)page - 1) * (int)pageSize).Take((int)pageSize);

            if (orderBy != null)
            {
                return  orderBy(query);
            }
            else
            {
                return  query;
            }
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        #endregion

        #region Private Methods
        private IQueryable<T> IncludeProperties(IQueryable<T> query, string includeProperties)
        {
            foreach (var include in includeProperties.Split(","))
            {
                query = query.Include(include.Trim());
            }
            return query;
        }
        #endregion
    }
}
