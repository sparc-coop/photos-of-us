using FluentValidation;
using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User cannot be null");
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Line1).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
        }
    }

    public partial class Address
    {
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

        public static IList<Error> Validate(int userId, string title, string line1, string line2, string city, string state, string zipCode, string country, string phone, string email)
        {
            var validator = new InlineValidator<Address>();
            validator.RuleFor(x => x.UserId).NotEmpty();

            RuleFor

            

            
            validator.RuleFor(x => x.Address.Line1).NotEqual("foo");

        }



        public int Id { get; protected set; }
        public int UserId { get; protected set; }
        public string Title { get; protected set; }
        public string Line1 { get; protected set; }
        public string Line2 { get; protected set; }
        public string City { get; protected set; }
        public string State { get; protected set; }
        public string ZipCode { get; protected set; }
        public string Country { get; protected set; }
        public string Phone { get; protected set; }
        public string Email { get; protected set; }

        //public ICollection<Order> OrderBillingAddress { get; protected set; }
        //public ICollection<Order> OrderShippingAddress { get; protected set; }
    }

    public class Result<T> : Result
    {
        public T Value { get; private set; }

        protected internal Result(T value, bool success, IList<Error> errors)
        : base(success, errors)
        {
            Value = value;
        }
    }

    public class Result
    {
        public bool Success { get; private set; }
        public IList<Error> Errors { get; private set; }

        protected Result(bool success, IList<Error> errors)
        {
            Errors = errors;
            Success = success;
        }

        public static Result<T> Fail<T>(IList<Error> errors)
        {
            return new Result<T>(default(T), false, errors);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, null);
        }

        public static Result Fail(IList<Error> errors)
        {
            return new Result(false, errors);
        }

        public static Result Ok()
        {
            return new Result(true, new List<Error>());
        }
    }

    public class Error
    {
        public string Field { get; set; }
        public string Message { get; set; }

        public Error(string field, string message)
        {
            Field = field;
            Message = message;
        }
    }
}
