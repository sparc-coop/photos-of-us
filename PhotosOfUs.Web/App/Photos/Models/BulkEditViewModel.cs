using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class BulkEditViewModel
    {
        public List<int> photoIds { get; set; }
        public List<TagViewModel> tags { get; set; }
        public decimal? newPrice { get; set; }
    }
}
