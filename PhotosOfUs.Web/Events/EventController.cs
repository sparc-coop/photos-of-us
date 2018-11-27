using System;
using System.Collections.Generic;
using System.Linq;
using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Events")]
    public class EventApiController : Controller
    {
        private readonly IRepository<Event> _events;

        public EventApiController(IRepository<Event> eventRepository)
        {
        }

        [HttpPost]
        [Route("{eventId:int}/Cards/{quantity:int}")]
        public List<CardViewModel> Create(int eventId, int quantity)
        {
            var ev = _events.Find(x => x.EventId == eventId);
            ev.AddNewCards(quantity);
            _events.Commit();

            return ev.Cards.ToList().Select(CardViewModel.ToViewModel).ToList();
        }

        [HttpGet]
        [Route("{eventId:int}/Cards/{code}")]
        public List<PhotoViewModel> GetCodePhotos(int eventId, string code)
        {
            return _events.Find(x => x.EventId == eventId)
            .Cards
            .FirstOrDefault(x => x.Code == code)
            ?.Photos.AsQueryable()
            .ToViewModel<PhotoViewModel>();
        }

        [HttpGet]
        [Route("{eventId:int}/Tags")]
        public List<TagViewModel> GetAllTags(int eventId)
        {
            return _events.Find(x => x.EventId == eventId)
            .Cards
            .SelectMany(x => x.Photos)
            .SelectMany(x => x.Tag)
            .Distinct()
            .ToViewModel<List<TagViewModel>>();
        }
    }
}