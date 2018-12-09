using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class PhotoViewModel
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public int FolderId { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        //public string PhotographerName { get; set; }
        public DateTime UploadDate { get; set; }
        public decimal? Price { get; set; }
        public string Resolution { get; set; }
        public string FileSize { get; set; }
        public User Photographer { get; set; }
        public string WaterMarkUrl { get; set; }
    }
}
