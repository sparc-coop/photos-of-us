using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/PhotographerApi")]
    public class PhotographerApiController : Controller
    {
        private readonly PhotosOfUsContext _context;
        private readonly IViewRenderService _viewRenderService;

        public PhotographerApiController(PhotosOfUsContext context, IViewRenderService viewRenderService)
        {
            _context = context;
            _viewRenderService = viewRenderService;
        }

        // GET: api/PhotographerApi
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PhotographerApi/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("{query}")]
        [Route("SalesHistory")]
        public async Task<IActionResult> SalesHistory(string query)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;
            var user = _context.User.Find(userId);

            List<Order> queriedOrders;
            if(null == query || query.Equals(""))
                queriedOrders = new OrderRepository(_context).GetOrders(user.Id);
            else
                queriedOrders = new OrderRepository(_context).SearchOrders(user.Id, query);

            var viewModel = SalesHistoryViewModel.ToViewModel(user, queriedOrders);

            var result = await _viewRenderService.RenderToStringAsync("Photographer/Partials/_SalesHistoryPartial", viewModel);

            return Content(result);
        }
    }
}
