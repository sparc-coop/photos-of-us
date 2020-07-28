using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace PhotosOfUs.Core.Orders
{
    //public class AddressValidator : AbstractValidator<Address>
    //{
    //    public AddressValidator()
    //    {
    //        RuleFor(x => x.UserId).NotEmpty().WithMessage("User cannot be null");
    //        RuleFor(x => x.Title)
    //        RuleFor(x => x.Line1).NotEmpty();
    //        RuleFor(x => x.City).NotEmpty();
    //    }
    //}

    public partial class Address
    {
        private int userId;
        private string title;
        private string line1;
        private string line2;
        private string city;
        private string state;
        private string zipCode;
        private string country;
        private string phone;
        private string email;

        public Address(int userId, string title, string line1, string line2, string city, string state, string zipCode, string country, string phone, string email)
        {
            UserId = userId;
            Title = title;
            Line1 = line1;
            Line2 = line2;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            Phone = phone;
            Email = email;
        }

        public int Id { get; protected set; }
        public int UserId
        {
            get => userId; protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                userId = value;
            }
        }
        public string Title
        {
            get => title; protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                title = value;
            }
        }
        public string Line1
        {
            get => line1; protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                line1 = value;
            }
        }
        public string Line2 { get => line2; protected set { line2 = value; } }
        public string City
        {
            get => city; protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                city = value;
            }
        }
        public string State
        {
            get => state; protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                state = value;
            }
        }
        public string ZipCode
        {
            get => zipCode; protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                zipCode = value;
            }
        }
        public string Country
        {
            get => country; protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                country = value;
            }
        }
        public string Phone
        {
            get => phone; 
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                phone = value;
            }
        }
        public string Email
        {
            get => email; protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                email = value;
            }
        }

        //public ICollection<Order> OrderBillingAddress { get; protected set; }
        //public ICollection<Order> OrderShippingAddress { get; protected set; }
    }
}
