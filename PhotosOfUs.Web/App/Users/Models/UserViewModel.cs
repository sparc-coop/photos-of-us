using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Web.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string JobPosition { get; set; }
        public string Bio { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Dribbble { get; set; }
        public bool? IsPhotographer { get; set; }
        public bool? IsDeactivated { get; set; }
        public bool? PurchaseTour { get; set; }
        public bool? DashboardTour { get; set; }
        public bool? PhotoTour { get; set; }
    }
}
