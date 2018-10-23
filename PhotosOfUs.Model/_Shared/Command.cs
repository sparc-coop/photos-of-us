using System;

namespace Kuvio.Kernel.Architecture
{
    //public interface ICommand<T> : IDisposable
    //{
    //    //new void Dispose();

    //    void Commit();
    //}

    public class Command<T> //: ICommand<T>
    {
        protected IRepository<T> Set;
        
        public Command(IRepository<T> repository)
        {
            Set = repository;
        }

        public void Dispose() => Commit();

        protected void Commit() => Set.Commit();

        
    }
}