using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class SearchViewModel
    {
        public List<PhotoViewModel> Photos { get; set; }
        public List<TagViewModel> Tags { get; set; }
    }
}
