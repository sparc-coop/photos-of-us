namespace Kuvio.Kernel.Core
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