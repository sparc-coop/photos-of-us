using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Checkout")]
    public class CheckoutApiController
    {
        private PhotosOfUsContext _context;

        public CheckoutApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("SaveAddress")]
        public AddressViewModel GetPhoto(AddressViewModel vm)
        {
            var address = AddressViewModel.ToEntity(vm);
            address = new AddressRepository(_context).Create(address);
            return AddressViewModel.ToViewModel(address);
        }
    }
}
