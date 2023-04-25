using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa
{
    public class Receipt
    {
        public List<Product> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime PurchaseDate { get; set; }

        public Receipt(List<Product> items, decimal total, DateTime purchaseDate)
        {
            Items = items;
            Total = total;
            PurchaseDate = purchaseDate;
        }
    }
}
