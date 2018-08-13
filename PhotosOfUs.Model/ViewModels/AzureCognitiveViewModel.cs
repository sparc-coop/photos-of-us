using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class AzureCognitiveViewModel
    {
        public List<string> SuggestedTags { get; set; }
        public string Code { get; set; }
        public bool IsValid { get; set; }

        public static AzureCognitiveViewModel ToViewModel(string code, List<string> suggestedtags)
        {
            AzureCognitiveViewModel viewModel = new AzureCognitiveViewModel
            {
                Code = code,
                SuggestedTags = suggestedtags,
                IsValid = true
            };

            return viewModel;
        }
    }
}
