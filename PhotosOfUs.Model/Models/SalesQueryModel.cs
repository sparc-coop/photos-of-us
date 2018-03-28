using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotosOfUs.Model.Models
{
    public class SalesQueryModel
    {
        public string Total { get; set; } // number
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public bool? IsPhotographer { get; set; }
        public string Quantity { get; set; } // number

        public SalesQueryModel(string queryString)
        {
            Regex regex = new Regex("^\\?", RegexOptions.IgnoreCase);
            string cleaned = regex.Replace(queryString, "");
            var queryArray = cleaned.Split("&");

            foreach(string queryPair in queryArray)
            {
                string[] pairArray = queryPair.Split("=");
                var property = this.GetType().GetProperty(pairArray[0], System.Reflection.BindingFlags.IgnoreCase);

                if (property != null)
                {
                    property.SetValue(this, pairArray.GetValue(1));
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
