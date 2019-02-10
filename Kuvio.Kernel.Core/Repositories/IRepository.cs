using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kuvio.Kernel.Core
{
    public interface IRepository<T>
    {
        IQueryable<T> Query { get; }
        T Find(object id);
        
        // Commands
        T Add(T item);
        //void Update(T item);
        void Delete(T item);
        void Execute(object id, Action<T> action);
        Task ExecuteAsync(object id, Action<T> action);
    }
}