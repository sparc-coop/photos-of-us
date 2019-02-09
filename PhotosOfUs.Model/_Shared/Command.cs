using System;

namespace Kuvio.Kernel.Architecture
{
    public class Command<T>
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