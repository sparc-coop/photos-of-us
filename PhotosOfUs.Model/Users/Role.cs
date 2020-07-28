using Kuvio.Kernel.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Core.Users
{
    public partial class Role : ValueObject<Role>
    {
        private Role(string value, string text) { Value = value; Text = text; }

        public string Value { get; private set; }
        public string Text { get; private set; }

        public static Role Photographer { get { return new Role("Photographer", "Photographer"); } }
        public static Role Customer { get { return new Role("Customer", "Customer"); } }
        public static Role Admin { get { return new Role("Admin", "Admin"); } }


        public static List<Role> GetAll()
        {
            var list = new List<Role>();
            list.Add(Photographer);
            list.Add(Customer);
            list.Add(Admin);
            return list;
        }

        public static Role Get(string value)
        {
            var list = GetAll();
            return list.First(y => y.Value == value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
