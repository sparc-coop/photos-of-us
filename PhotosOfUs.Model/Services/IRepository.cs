using System;
using System.Linq;
using PhotosOfUs.Model.Models;

namespace Kuvio.Kernel.Architecture
{
    public interface IRepository<T>
    {
        // Queries
        T Find(Func<T, bool> query);
        T Find(object id);
        IQueryable<T> Where(Func<T, bool> query);

        // Commands
        T Add(T item);
        void Delete(T item);

        void Commit();
    }
}