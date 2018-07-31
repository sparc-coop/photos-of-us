using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class PhotoTagViewModel
    {
        string identifier { get; set; }
        int[] photosid { get; set; }
        int[] tagsid { get; set; }
    }
}
