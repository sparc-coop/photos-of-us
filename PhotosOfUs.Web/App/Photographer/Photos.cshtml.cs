using Kuvio.Kernel.Architecture;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotosOfUs.Pages.Photographer
{
    public class PhotosModel : PageModel
    {
        private readonly IRepository<Event> _events;

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Photo> Photos { get; set; }

        public PhotosModel(IRepository<Event> events)
        {
            _events = events;
        }

        public void OnGet(int eventId)
        {
            var ev = _events.Include(x => x.Photos).Find(x => x.EventId == eventId);
            Id = ev.EventId;
            PhotographerId = ev.UserId;
            Name = ev.Name;
            CreatedDate = ev.CreatedDate;
            Photos = ev.Photos.ToList();
        }
    }
}