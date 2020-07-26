using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kuvio.Kernel.Core.Common;

namespace PhotosOfUs.Core.Orders
{
    public class OrderStatus : ValueObject<OrderStatus>
    {
        private OrderStatus(string value, string text) { Value = value; Text = text; }

        public string Value { get; private set; }
        public string Text { get; private set; }

        public static OrderStatus Open { get { return new OrderStatus("Open", "Open"); } }
        public static OrderStatus Pending { get { return new OrderStatus("Pending", "Pending"); } }
        public static OrderStatus Complete { get { return new OrderStatus("Complete", "Complete"); } }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Text;
        }

        public static List<OrderStatus> GetAll()
        {
            var list = new List<OrderStatus>();
            list.Add(Open);
            list.Add(Pending);
            list.Add(Complete);
            return list;
        }

        public static OrderStatus Get(string value)
        {
            var list = GetAll();
            return list.First(y => y.Value == value);
        }
    }
}
