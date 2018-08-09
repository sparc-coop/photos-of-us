using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class PhotoTagViewModel
    {
        public List<Photo> photos { get; set; }
        public List<TagViewModel> tags { get; set; }
    }
}
