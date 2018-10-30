using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class AddressViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public ICollection<Order> OrderBillingAddress { get; set; }
        public ICollection<Order> OrderShippingAddress { get; set; }

        //public static AddressViewModel ToViewModel(Address entity)
        //{
        //    AddressViewModel viewModel = new AddressViewModel();

        //    viewModel.Id = entity.Id;
        //    viewModel.UserId = entity.UserId;
        //    viewModel.FullName = entity.FullName;
        //    viewModel.Address1 = entity.Address1;
        //    viewModel.Address2 = entity.Address2;
        //    viewModel.City = entity.City;
        //    viewModel.State = entity.State;
        //    viewModel.City = entity.City;
        //    viewModel.ZipCode = entity.ZipCode;
        //    viewModel.Country = entity.Country;
        //    viewModel.Phone = entity.Phone;
        //    viewModel.Email = entity.Email;

        //    return viewModel;
        //}

        //public static Address ToEntity(AddressViewModel viewModel)
        //{
        //    Address entity = new Address();

        //    entity.Id = viewModel.Id;
        //    entity.UserId = viewModel.UserId;
        //    entity.FullName = viewModel.FullName;
        //    entity.Address1 = viewModel.Address1;
        //    entity.Address2 = viewModel.Address2;
        //    entity.City = viewModel.City;
        //    entity.State = viewModel.State;
        //    entity.City = viewModel.City;
        //    entity.ZipCode = viewModel.ZipCode;
        //    entity.Country = viewModel.Country;
        //    entity.Phone = viewModel.Phone;
        //    entity.Email = viewModel.Email;

        //    return entity;
        //}
    }
}
