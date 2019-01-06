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
        public IEnumerable<int> Opacities { get; private set; }

        public SettingsModel(IRepository<Event> events)
        {
            _events = events;
        }

        public void OnGet()
        {
            Opacities = Enumerable.Range(0, 101).Where(x => x % 5 == 0);
        }
    }
}