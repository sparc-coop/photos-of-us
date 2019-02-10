using Kuvio.Kernel.Core;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace PhotosOfUs.Pages.Events
{
    public class PhotoModel : PageModel
    {
        private readonly IRepository<Event> _events;

        public Photo Photo { get; set; }
        public string PurchasePageUrl { get; set; }

        public PhotoModel(IRepository<Event> events)
        {
            _events = events;
        }

        public IActionResult OnGet(int eventId, string photoCode, int photoId)
        {
            var ev = _events.Find(eventId);
            Photo = ev.Cards.FirstOrDefault(x => x.Code == photoCode)?.Photos.FirstOrDefault(x => x.Id == photoId);
            if (Photo == null) return NotFound();

            PurchasePageUrl = Url.Page("Purchase", new { eventId, photoId });
            return Page();
        }
    }
}