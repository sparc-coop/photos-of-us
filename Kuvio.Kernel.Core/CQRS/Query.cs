namespace Kuvio.Kernel.Core
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