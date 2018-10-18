using System.IO;

namespace PhotosOfUs.Model.Models
{
    public interface IFile {
        string Filename { get; }
        string FolderName { get; }
        Stream Stream { get; }
    }
}