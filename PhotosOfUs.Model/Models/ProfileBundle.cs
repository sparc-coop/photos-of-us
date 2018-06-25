using System;
using System.Collections.Generic;
using PhotosOfUs.Model.ViewModels;

namespace PhotosOfUs.Model.Models
{
    public partial class ProfileBundle
    {
        public User User { get; set; }
        public FolderViewModel Folder { get; set; }
    }
}
