using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa
{
    public class Cart
    {
        private List<Product> _items = new List<Product>();
        private decimal _total = 0m;

        public void AddItem(Product product, decimal quantity)
        {
            _items.Add(product);
            _total += product.Price * quantity;
        }

        public decimal Total(decimal weightKG)
        {
            decimal total = _total;

            if (weightKG > 0)
            {
                decimal weightPrice = 0m;
                foreach (Product product in _items)
                {
                    weightPrice += product.Weight * weightKG;
                }

                total += weightPrice;
            }

            return total;
        }

        public void Clear()
        {
            _items.Clear();
            _total = 0m;
        }

        public List<Product> Items()
        {
            return _items;
        }
    }
}
