using System;
using System.Linq;
using System.Linq.Expressions;
using Kuvio.Kernel.Core;
using Microsoft.EntityFrameworkCore;

namespace PhotosOfUs.Connectors.Database
{
    public class DbRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext context;
        private DbSet<T> Command;
        private IQueryable<T> Query;

        public DbRepository(DbContext context)
        {
            this.context = context;
            this.Command = context.Set<T>();
            this.Query = context.Set<T>();
        }

        // Queries
        public T Find(Func<T, bool> query) => Query.FirstOrDefault(query);

        public IQueryable<T> Where(Func<T, bool> query) => Query.Where(query).AsQueryable();

        // Commands
        public T Add(T item)
        {
            Command.Add(item);
            Commit(); // In order to populate any DB-generated IDs
            return item;
        }

        //public void Update(T item)
        //{
        //    Command.Attach(item);
        //}

        public void Delete(T item)
        {
            Command.Remove(item);
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public IRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> item)
        {
            Query = Query.Include(item);

            return this;
        }

        public IRepository<T> Include(string propertyPath)
        {
            Query = Query.Include(propertyPath);

            return this;
        }


    }
}