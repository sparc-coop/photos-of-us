using Kuvio.Kernel.Architecture;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotosOfUs.Pages.Events
{
    public class SettingsModel : PageModel
    {
        private readonly IRepository<Event> _events;

        public Event Event { get; set; }

        public SettingsModel(IRepository<Event> events)
        {
            _events = events;
        }

        public void OnGet()
        {
        }
    }
}