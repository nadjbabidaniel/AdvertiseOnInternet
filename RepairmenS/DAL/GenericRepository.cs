using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using RepairmenModel;
using System.Data.Entity.Core;
using System.Reflection;


namespace DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal repairmenEntities context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(repairmenEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Func<TEntity, bool> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter).AsQueryable();
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (query == null || !query.Any())
            {
                throw new ObjectNotFoundException();
            }
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            var result = dbSet.Find(id);
            if (result == null)
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            ////dbSet.Attach(entityToUpdate);
            //if (context.Entry(entityToUpdate).State != EntityState.Added)
            //{
            //    context.Entry(entityToUpdate).State = EntityState.Modified;
            //}
            Type type = entityToUpdate.GetType();
            PropertyInfo id = type.GetProperty("Id");
            Guid key = (Guid)id.GetValue(entityToUpdate);
            var entity = GetByID(key);
            context.Entry(entity).CurrentValues.SetValues(entityToUpdate);
        }

        public virtual bool Exists(object id)
        {
            try
            {
                GetByID(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
