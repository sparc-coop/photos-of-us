using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PhotosOfUs.Model.Models;

namespace Kuvio.Kernel.Architecture
{
    public interface IMediaRepository<T> where T : IFile
    {
        Task<Uri> UploadAsync(T item);
    }
}