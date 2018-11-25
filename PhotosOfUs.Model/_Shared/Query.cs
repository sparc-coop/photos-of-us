using System;

namespace Kuvio.Kernel.Architecture
{
    public class Query<T>
    {
        protected IRepository<T> Set;
        
        public Query(IRepository<T> repository)
        {
            Set = repository;
        }
    }
}