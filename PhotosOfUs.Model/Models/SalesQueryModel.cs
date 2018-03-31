using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PhotosOfUs.Model.Models
{
    public class SalesQueryModel
    {
        public string Total { get; set; } // number
        public string PhotoName { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? OrderDateEarliest { get; set; }
        public DateTime? OrderDateLatest { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public bool? IsPhotographer { get; set; }
        public string QuantityMin { get; set; } // number
        public string QuantityMax { get; set; } // number

        public SalesQueryModel(string queryString)
        {
            Regex regex = new Regex("^\\?", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex("\\+", RegexOptions.IgnoreCase);
            string cleaned = regex.Replace(queryString, "");
            var queryArray = cleaned.Split("&");

            foreach(string queryPair in queryArray)
            {
                string[] pairArray = queryPair.Split("=");
                if (pairArray.Length != 2) return;

                var property = this.GetType().GetProperty(pairArray[0]);

                if (property != null && !String.IsNullOrEmpty(pairArray[1]))
                {
                    Debug.WriteLine("NOT NULL PAIR: {0}:{1}", pairArray[0], pairArray[1]);
                    if(property.PropertyType == typeof(Nullable<DateTime>))
                    {
                        property.SetValue(this, DateTime.Parse(pairArray[1]));
                    } else
                    if (property.PropertyType == typeof(Nullable<bool>))
                    {
                        property.SetValue(this, pairArray[1] == "true");
                    }
                    else
                    {
                        property.SetValue(this, regex2.Replace(pairArray[1], " "));
                    }
                } else
                {
                    Debug.WriteLine("NULL PAIR: {0}:{1}", pairArray[0], pairArray[1]);
                }
            }
        }

// structure of an Order
/*
int Id-
int UserId-
int ShippingAddressId-
int BillingAddressId-
decimal Total
string OrderStatus
DateTime OrderDate

Address BillingAddress-
Address ShippingAddress-
User User
    public int Id-
    string AzureId-
    string Email
    string FirstName
    string LastName
    string DisplayName
    DateTime CreateDate-
    DateTime? LastLoginDate-
    bool? IsPhotographer

    ICollection<Card> Card-
    ICollection<Folder> Folder-
    ICollection<Order> Order-
    ICollection<Photo> Photo-
    ICollection<PrintPrice> PrintPrice-
    Address Address-
ICollection<OrderDetail> OrderDetail
    int Id-
    int OrderId-
    int PhotoId-
    int Quantity
    int PrintTypeId-
    decimal UnitPrice-

    Order Order-
    Photo Photo-
    PrintType PrintType-
        int Id
        string Type
        string Height
        string Length
        string Icon
*/
    }
}
