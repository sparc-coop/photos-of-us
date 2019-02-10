using System;
using System.Linq;
using System.Linq.Expressions;

namespace Kuvio.Kernel.Core
{
    public interface IRepository<T>
    {
        // Queries
        T Find(Func<T, bool> query);
        IQueryable<T> Where(Func<T, bool> query);

        // Commands
        T Add(T item);
        //void Update(T item);
        void Delete(T item);
        IRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> item);

        IRepository<T> Include(string propertyPath);

        void Commit();
    }
}