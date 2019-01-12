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
using PhotosOfUs.Model.Events;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using PhotosOfUs.Pages.Admin;
using Microsoft.AspNetCore.Http;
using PhotosOfUs.Model.Photos.Commands;
using System.Threading.Tasks;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Event")]
    [Authorize]
    public class EventApiController : Controller
    {
        private readonly IRepository<Event> _events;

        public EventApiController(IRepository<Event> events)
        {
            _events = events;
        }

        [HttpGet]
        [Route("{eventId:int}")]
        public Event Get(int eventId)
        {
            return _events.Find(x => x.EventId == eventId);
        }

        [HttpPost]
        [Route("Events")]
        public IActionResult Save([FromBody]Event evt)
        {
            _events.Add(evt);
            _events.Commit();
            return Ok(evt.EventId);
        }

        [HttpGet]
        [Route("{eventId:int}/Cards")]
        public List<Card> GetEventCards(int eventId)
        {
            return _events.Include(x => x.Cards).Find(x => x.EventId == eventId).Cards.ToList();
        }

        [HttpPost]
        [Route("{eventId:int}/Cards")]
        public List<Card> CreateEventCards(int eventId, [FromBody]int quantity)
        {
            var ev = _events.Include(x => x.Cards).Find(x => x.EventId == eventId);
            ev.AddNewCards(quantity);
            _events.Commit();

            return ev.Cards.ToList();
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

        [HttpPost]
        [Route("{eventId:int}/Photos")]
        public async Task<UploadPhotoCommand.UploadPhotoCommandResult> UploadPhotoAsync(int eventId, string photoCode, IFormFile file, [FromServices]UploadPhotoCommand command)
        {
            return await command.ExecuteAsync(eventId, User.ID(), file.FileName, file.OpenReadStream(), photoCode);
        }

        [HttpPost]
        [Route("Photos")]
        public async Task<UploadPhotoCommand.UploadPhotoCommandResult> UploadCoverPhotoAsync(IFormFile file, [FromServices]UploadPhotoCommand command)
        {
            return await command.ExecuteAsync(User.ID(), file.FileName, file.OpenReadStream());
        }

        [HttpGet]
        [Route("{eventId:int}/Tags")]
        public List<TagViewModel> GetAllTags(int eventId)
        {
            return _events.Find(x => x.EventId == eventId)
            .Cards
            .SelectMany(x => x.Photos)
            .SelectMany(x => x.PhotoTag.Select(y => y.Tag))
            .Distinct()
            .AsQueryable()
            .ToViewModel<TagViewModel>();
        }

        [HttpPost]
        [Route("{eventId:int}/BulkEdit")]
        public List<TagViewModel> BulkEdit(int eventId, List<int> photoIds)
        {
             return _events.Find(x => x.EventId == eventId)
            .Photos
            .Where(y => photoIds.Contains(y.Id))
            .SelectMany(x => x.PhotoTag.Select(y => y.Tag))
            .Distinct()
            .AsQueryable()
            .ToViewModel<TagViewModel>();
        }

        [HttpPut]
        [Route("{eventId:int}/BulkEdit")]
        public void BulkEditSave(int eventId, List<int> photoIds, [FromBody]BulkEditModel model, [FromServices]BulkEditCommand command)
        {
            var tags = model.tags.Select(x => x.Name).ToList();
            command.Execute(eventId, photoIds, tags, model.newPrice);
        }

        [HttpDelete]
        [Route("{eventId:int}/BulkEdit")]
        public void BulkDelete(int eventId, List<int> photoIds)
        {
            var ev = _events.Include(x => x.Photos).Find(x => x.EventId == eventId);
            ev.DeletePhotos(photoIds);
            _events.Commit();
        }

        [Route("CardsPdf")]
        public ActionResult CardsPdf(string json)
        {
            var newString = json;
            var cards = JsonConvert.DeserializeObject<List<CardsPdfModel>>(json);
            return View(cards);
        }
    }
}