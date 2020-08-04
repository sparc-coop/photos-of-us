using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;

namespace PhotosOfUs.WA.Server.Utils
{
    public class ScssFileProvider : IFileProvider
    {
        private readonly IFileProvider _contentRootFileProvider;

        public ScssFileProvider(IFileProvider contentRootFileProvider)
        {
            _contentRootFileProvider = contentRootFileProvider;
        }

        IFileInfo IFileProvider.GetFileInfo(string subpath)
        {
            var wwwRootIndex = subpath.IndexOf("/wwwroot/");
            if (wwwRootIndex != -1)
            {
                subpath = subpath.Substring(wwwRootIndex);
            }
            return _contentRootFileProvider.GetFileInfo(subpath);
        }

        IDirectoryContents IFileProvider.GetDirectoryContents(string subpath) => throw new NotImplementedException();

        IChangeToken IFileProvider.Watch(string filter) => throw new NotImplementedException();
    }
}
