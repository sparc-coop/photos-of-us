using AutoMapper;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Photos.Queries;
using PhotosOfUs.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotosOfUs.Plugins.Mapping
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Photo, RandomPhotoModel>();

            //CreateMap<Photo, PhotoModel>();

            CreateMap<User, PhotographerModel>();
            CreateMap<Tag, PhotoTagModel>();
            CreateMap<Photo, PhotoModel>()
                .ForMember(dto => dto.PhotoTag, opt => opt.MapFrom(x => x.PhotoTag.Select(y => y.Tag).ToList()))
                .ForMember(dto => dto.Photographer, opt => opt.MapFrom(x => x.Photographer));
        }
    }
}
