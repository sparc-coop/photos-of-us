using AutoMapper;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Photos.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Plugins.Mapping
{
    class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Photo, RandomPhotoModel>();
        }
    }
}
