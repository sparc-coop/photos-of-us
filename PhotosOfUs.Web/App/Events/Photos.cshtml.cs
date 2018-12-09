using Kuvio.Kernel.Architecture;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace PhotosOfUs.Pages.Events
{
    public class PhotosModel : PageModel
    {
        private readonly IRepository<Event> _events;

        public string Code { get; private set; }
        public int EventId { get; set; }
        public List<Photo> Photos { get; set; }
        public User Photographer { get; set; }

        public PhotosModel(IRepository<Event> events)
        {
            _events = events;
        }

        public IActionResult OnGet(int eventId, string code, int? page = 1, int? photosPerPage = 8)
        {
            var ev = _events.Include("Photos.Photographer").Find(x => x.EventId == eventId);
            Code = code;
            EventId = eventId;
            Photos = ev.Photos.Where(x => x.Code == code).ToList();
            if (!Photos.Any()) return RedirectToPage("/Events/Search");

            Photographer = Photos.First().Photographer;

            return Page();
        }
    }
}