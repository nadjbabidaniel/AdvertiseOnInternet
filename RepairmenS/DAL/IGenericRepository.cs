using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> Get(
            Func<T, bool> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        T GetByID(object id);
        void Insert(T entity);
        void Delete(object id);
        void Delete(T entityToDelete);
        void Update(T entityToUpdate);
        bool Exists(object id);
    }
}
