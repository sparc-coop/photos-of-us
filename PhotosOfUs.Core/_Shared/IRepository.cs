using System;
using System.Linq;
using System.Linq.Expressions;
using PhotosOfUs.Model.Models;

namespace Kuvio.Kernel.Architecture
{
    public interface IRepository<T>
    {
        // Queries
        T Find(Func<T, bool> query);
        IQueryable<T> Where(Func<T, bool> query);

        // Commands
        T Add(T item);
        void Delete(T item);
        IRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> item);

        void Commit();
    }
}