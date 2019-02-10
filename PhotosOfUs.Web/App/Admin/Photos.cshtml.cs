using Kuvio.Kernel.Core;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotosOfUs.Pages.Admin
{
    public class PhotosModel : PageModel
    {
        private readonly IRepository<Event> _events;

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Card> Cards { get; set; }

        public PhotosModel(IRepository<Event> events)
        {
            _events = events;
        }

        public void OnGet(int eventId)
        {
            var ev = _events.Find(eventId);
            Id = ev.EventId;
            PhotographerId = ev.UserId;
            Name = ev.Name;
            CreatedDate = ev.CreatedDate;
            Cards = ev.Cards.ToList();
        }
    }
}