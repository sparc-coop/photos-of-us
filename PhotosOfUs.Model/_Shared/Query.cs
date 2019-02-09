using System;
using System.Linq;

namespace Kuvio.Kernel.Architecture
{
    public abstract class Query<T> 
    {
        protected IRepository<T> Set;
        
        public Query(IRepository<T> repository)
        {
            Set = repository;
        }
    }
}