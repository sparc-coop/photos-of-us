using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class FolderViewModel
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<PhotoViewModel> Photo { get; set; }
    }
}
