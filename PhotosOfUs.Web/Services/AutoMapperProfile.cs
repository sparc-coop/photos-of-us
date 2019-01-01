using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Models;

namespace PhotosOfUs.Web.Utilities
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<Photo, PhotoViewModel>();
            CreateMap<Folder, FolderViewModel>();
            CreateMap<Tag, TagViewModel>();
        }
    }

    public static class AutoMapperExtensions
    {
        public static List<TViewModel> ToViewModel<TViewModel>(this IQueryable query)
        {
            return query.ProjectTo<TViewModel>().ToList();
        }

        public static TViewModel ToViewModel<TViewModel>(this object item)
        {
            return AutoMapper.Mapper.Map<TViewModel>(item);
        }
    }

}