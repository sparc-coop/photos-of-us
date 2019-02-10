using System;
using System.Threading.Tasks;

namespace Kuvio.Kernel.Core
{
    public interface IMediaRepository<T> where T : IFile
    {
        Task<Uri> UploadAsync(T item);
    }
}