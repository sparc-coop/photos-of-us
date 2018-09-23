using System;
using System.Linq;
using Kuvio.Kernel.Architecture;
using Microsoft.EntityFrameworkCore;

namespace PhotosOfUs.Connectors.Database
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext context;

        public Repository(DbContext context)
        {
            this.context = context;
        }
        
        private DbSet<T> Set => context.Set<T>();

        // Queries
        public T Find(object id) => Set.Find(id);

        public T Find(Func<T, bool> query) => Set.FirstOrDefault(query);

        public IQueryable<T> Where(Func<T, bool> query) => Set.Where(query).AsQueryable();

        // Commands
        public T Add(T item)
        {
            Set.Add(item);
            Commit(); // In order to populate any DB-generated IDs
            return item;
        }

        public void Delete(T item)
        {
            Set.Remove(item);
        }

        public void Commit()
        {
            context.SaveChanges();
        }
    }
}