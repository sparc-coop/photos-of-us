using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public class PaymentModel
    {
        public string StripeToken { get; set; }
        public int Amount { get; set; }

        public override string ToString()
        {
            return $"Token: {StripeToken}; Amount: {Amount}";
        }
    }
}
