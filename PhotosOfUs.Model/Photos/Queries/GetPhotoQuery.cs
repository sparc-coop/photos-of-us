using AutoMapper;
using Kuvio.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosOfUs.Core.Photos.Queries
{
    public class PhotographerModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    public class PhotoTagModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PhotoModel
    {
        public PhotoModel()
        {
            Photographer = new PhotographerModel();
            PhotoTag = new List<PhotoTagModel>();
        }

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public PhotographerModel Photographer { get; set; }
        public List<PhotoTagModel> PhotoTag { get; set; }
        public string Url { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Resolution { get; set; }
        public string FileSize { get; set; }
        public DateTime UploadDateUtc { get; set; }
        public bool PublicProfile { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class GetPhotoQuery
    {
        readonly IDbRepository<Photo> _photoRepository;
        readonly IMapper _mapper;

        public GetPhotoQuery(IDbRepository<Photo> photoRepository, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _mapper = mapper;
        }

        public async Task<PhotoModel> Execute(int id)
        {
            var photo = _photoRepository.Include("Photographer", "PhotoTag.Tag").Where(x => x.Id == id).FirstOrDefault();

            return _mapper.Map<PhotoModel>(photo);
        }
    }
}
