//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using Kuvio.Kernel.Core;
//using Microsoft.EntityFrameworkCore;

//namespace PhotosOfUs.Connectors.Database
//{
//    public class DbRepository<T> : IDbRepository<T> where T : class
//    {
//        private readonly DbContext context;
//        private readonly DbSet<T> Command;
//        public IQueryable<T> Query { get; private set; }

//        public T Find(object id) => Command.Find(id);

//        public DbRepository(DbContext context)
//        {
//            this.context = context;
//            Command = context.Set<T>();
//            Query = context.Set<T>().AsNoTracking();
//        }

//        // Commands
//        public T Add(T item)
//        {
//            Command.Add(item);
//            Commit(); // In order to populate any DB-generated IDs
//            return item;
//        }

//        //public void Update(T item)
//        //{
//        //    Command.Attach(item);
//        //}

//        public void Delete(T item)
//        {
//            Command.Remove(item);
//        }

//        public void Execute(object id, Action<T> action)
//        {
//            var entity = context.Set<T>().Find(id);
//            action(entity);
//            Commit();
//        }

//        public async Task ExecuteAsync(object id, Action<T> action)
//        {
//            var entity = await context.Set<T>().FindAsync(id);
//            action(entity);
//            await context.SaveChangesAsync();
//        }

//        private void Commit()
//        {
//            context.SaveChanges();
//        }
//    }
//}