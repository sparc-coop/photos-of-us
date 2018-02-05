using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class CardViewModel
    {
        public string Code { get; set; }
        public string Url { get; set; }


        public static CardViewModel ToViewModel(Card entity)
        {
            CardViewModel model = new CardViewModel();
            model.Code = entity.Code;
            //todo logic to get displayname
            model.Url = "www.photosof.us/kieranparker";

            return model;
        }
    }
}
