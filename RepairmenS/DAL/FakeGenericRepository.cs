using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using RepairmenModel;
using System.Reflection;
using System.Data.Entity.Core;


namespace DAL
{
    public class FakeGenericRepository<TEntity> : IGenericRepository<TEntity>
    {

        //internal IEnumerable<TEntity> dbSet;
        private Dictionary<Guid, TEntity> context;

        public FakeGenericRepository(Dictionary<Guid, TEntity> context)
        {
            this.context = context;
            // this.dbSet = context.Values;
        }

        public IEnumerable<TEntity> Get(
            Func<TEntity, bool> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = context.Values.AsQueryable();

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

        public TEntity GetByID(object id)
        {

            //return context.Where(x => x.Key == (Guid)id).Select(y => y.Value).FirstOrDefault();

            var result = context.Where(x => x.Key == (Guid)id).Select(y => y.Value).FirstOrDefault();
            if (result == null)
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }

        public void Insert(TEntity entity)
        {
            try
            {
                Type type = entity.GetType();
                PropertyInfo id = type.GetProperty("Id");
                context.Add((Guid)id.GetValue(entity), entity);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void Delete(object id)
        {
            context.Remove((Guid)id);
        }

        public void Delete(TEntity entityToDelete)
        {
            try
            {
                Type type = entityToDelete.GetType();
                PropertyInfo id = type.GetProperty("Id");
                Guid key = (Guid)id.GetValue(entityToDelete);
                context.Remove(key);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void Update(TEntity entityToUpdate)
        {
            try
            {
                Type type = entityToUpdate.GetType();
                PropertyInfo id = type.GetProperty("Id");
                Guid key = (Guid)id.GetValue(entityToUpdate);
              
                context[key] = entityToUpdate;
              
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Exists(object id)
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
        public List<TEntity> Context { get; set; }

    }
}
