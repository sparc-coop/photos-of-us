using System.Collections.Generic;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Pages.Events
{
    public class BulkEditModel
    {
        public List<int> photoIds { get; set; }
        public List<TagModel> tags { get; set; }
        public decimal? newPrice { get; set; }
    }

    public class TagModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}