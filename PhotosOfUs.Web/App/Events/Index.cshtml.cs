using Kuvio.Kernel.Architecture;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace PhotosOfUs.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly IRepository<Event> _events;

        public int EventId { get; private set; }
        public string ThemeName { get; set; }

        public IndexModel(IRepository<Event> events)
        {
            _events = events;
        }

        public void OnGet(string eventName)
        {
            var ev = _events.Find(x => x.Url == eventName);
            EventId = ev.EventId;
            ThemeName = "_" + ev.HomepageTemplate;
        }

        public IActionResult OnPost(int eventId, string code)
        {
            return RedirectToPage("Photos", new { eventId, code });
        }
    }
}