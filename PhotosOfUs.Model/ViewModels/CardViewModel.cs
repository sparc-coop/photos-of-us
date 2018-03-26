using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class CardViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }


        public static CardViewModel ToViewModel(Card entity)
        {
            CardViewModel model = new CardViewModel();
            model.Code = entity.Code;
            model.Url = "www.photosof.us/" + entity.Photographer.DisplayName.ToLower().Replace(" ","");
            model.Name = entity.Photographer.DisplayName;
            model.Email = entity.Photographer.Email;
            model.Id = entity.Id;
            model.CreatedDate = entity.CreatedDate;

            return model;
        }
    }
}
