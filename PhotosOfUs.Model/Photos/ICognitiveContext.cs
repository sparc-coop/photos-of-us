using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PhotosOfUs.Core.Photos
{
    public interface ICognitiveContext {
        Task<List<string>> GetSuggestedTags(byte[] file);
        Task<string> GetPhotoCode(byte[] file, List<string> possibleCodes);
    }
}