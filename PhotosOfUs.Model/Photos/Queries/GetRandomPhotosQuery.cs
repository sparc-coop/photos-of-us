using AutoMapper;
using Kuvio.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosOfUs.Core.Photos.Queries
{
    public class RandomPhotoModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public class GetRandomPhotosQuery
    {
        readonly IDbRepository<Photo> _photoRepository;
        readonly IMapper _mapper;

        public GetRandomPhotosQuery(IDbRepository<Photo> photoRepository, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _mapper = mapper;
        }

        public async Task<List<RandomPhotoModel>> Execute(int count)
        {
            Random rand = new Random();
            int toSkip = rand.Next(0, (await _photoRepository.CountAsync(x => x.Id > 0)) - count);

            return _photoRepository.Query
                .Skip(toSkip)
                .Take(count)
                .Select(y => new RandomPhotoModel() { Url = y.Url, Name = y.Name, ThumbnailUrl = y.ThumbnailUrl })
                .ToList();

            //return _mapper.Map<List<RandomPhotoModel>>(photos);
        }
    }
}
