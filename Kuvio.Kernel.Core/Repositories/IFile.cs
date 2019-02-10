using System.IO;

namespace Kuvio.Kernel.Core
{
    public interface IFile {
        string Filename { get; }
        string FolderName { get; }
        Stream Stream { get; }
    }
}