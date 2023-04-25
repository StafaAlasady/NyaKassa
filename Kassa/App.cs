using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kassa
{
    public class App
    {
        private List<Product> _products = new List<Product>()
        {
            new Product
            {
                Id = 300,
                Name = "Mjölk",
                Price = 15.0m,
                Weight = 1,
                PromoPrices = new List<PromoPrice>()
                {
                    new PromoPrice { Price = 10.0m, StartDate = new DateTime(2023, 3, 18), EndDate = new DateTime(2023, 3, 19) }
                }
            },
            new Product
            {
                Id = 301,
                Name = "Bröd",
                Price = 20.0m,
                Weight = 1,
                PromoPrices = new List<PromoPrice>()
                {
                    new PromoPrice { Price = 15.0m, StartDate = new DateTime(2023, 3, 18), EndDate = new DateTime(2023, 3, 19) }
                }
            },
            new Product
            {
                Id = 302,
                Name = "Smör",
                Price = 25.0m,
                Weight = 0.5m,
                PromoPrices = new List<PromoPrice>()
                {
                    new PromoPrice { Price = 20.0m, StartDate = new DateTime(2023, 3, 18), EndDate = new DateTime(2023, 3, 19) }
                }
            },
            new Product
            {
                Id = 303,
                Name = "Havregryn",
                Price = 30.0m,
                Weight = 0.25m,
                PromoPrices = new List<PromoPrice>()
                {
                    new PromoPrice { Price = 20.0m, StartDate = new DateTime(2023, 3, 18), EndDate = new DateTime(2023, 3, 19) }
                }
            },

            new Product
            {
                Id = 304,
                Name = "ägg",
                Price = 1005.0m,
                Weight = 0.1M,
                PromoPrices = new List<PromoPrice>()
                {
                    new PromoPrice { Price = 10.0m, StartDate = new DateTime(2023, 3, 18), EndDate = new DateTime(2023, 3, 19) }
                }
            },
        };

        private Cart _cart = new Cart();

        private List<Receipt> _receipts = new List<Receipt>();

        private int _receiptNumber = 1;

        public void Run()
        {
            Console.WriteLine("Välkommen till hassans kassa!");

            while (true)
            {
                Console.WriteLine("Vad vill du göra?");
                Console.WriteLine("1. Visa alla produkter");
                Console.WriteLine("2. Lägg till produkt");
                Console.WriteLine("3. Betala");
                Console.WriteLine("4. Avsluta");
                Console.WriteLine("5. Visa Kvitton");

                string input = Console.ReadLine().ToUpper();

                switch (input)
                {
                    case "1":
                        DisplayProducts();
                        break;
                    case "2":
                        AddProduct();
                        break;
                    case "3":
                        Pay();
                        break;
                    case "4":
                        Console.WriteLine("Hej då!");
                        return;
                    case "5":
                        DisplayReceipts();
                        Console.WriteLine("Tryck på valfri tangent för att fortsätta.");
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Ogiltig inmatning, försök igen.");
                        break;
                }
            }
        }

        private void DisplayProducts()
        {
            Console.WriteLine("Alla produkter:");
            foreach (Product product in _products)
            {
                Console.WriteLine($"{product.Id} - {product.Name} ({product.Price:c})");
            }
        }

        private void AddProduct()
        {
            Console.WriteLine("Ange produkt-ID och antal (t.ex. 300 1):");
            string input = Console.ReadLine().ToUpper();

            string[] parts = input.Split(' ');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int id) || !int.TryParse(parts[1], out int quantity))
            {
                Console.WriteLine("Ogiltig inmatning, Försök igen.");
                return;
            }

            Product product = _products.Find(p => p.Id == id);
            if (product == null)
            {
                Console.WriteLine("Produkt hittades inte.");
                return;
            }

            decimal price = product.Price;

            // Kontrollera om det finns en kampanjpris för produkten och om det gäller just nu
            if (product.PromoPrices != null)
            {
                DateTime now = DateTime.Now.Date;
                PromoPrice promo = product.PromoPrices.FirstOrDefault(p => now >= p.StartDate.Date && now <= p.EndDate.Date);
                if (promo != null)
                {
                    price = promo.Price;
                }
            }

            _cart.AddItem(product, quantity);

            Console.WriteLine($"{product.Name} tillagt i kundvagnen. Totalt: {price * quantity:c}");
        }

        private void PrintReceipt(Cart cart)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Hassan's Kassa");
            Console.WriteLine("Kvittonummer: {0}", _receiptNumber++);
            Console.WriteLine("Datum: {0}", DateTime.Now);
            Console.WriteLine("========================================");


            foreach (Product item in cart.Items())
            {
                Console.Write("{0} x {1}", item.Name, cart.Items().Count(i => i.Id == item.Id));

                // Check if there is a promotion price for the item
                PromoPrice promo = item.PromoPrices?.FirstOrDefault(p => DateTime.Now >= p.StartDate && DateTime.Now <= p.EndDate);
                if (promo != null)
                {
                    Console.Write(" ({0:c} varje, promotion valid from {1:d} to {2:d})", promo.Price, promo.StartDate, promo.EndDate);
                }
                else
                {
                    Console.Write(" ({0:c} varje)", item.Price);
                }

                Console.WriteLine();
            }


            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Totalt: {0:c}", cart.Total(0));
            Console.WriteLine("========================================");
        }


        private void Pay()
        {
            Cart myCart = new Cart();
            Console.WriteLine("Ange vikt i kg (0 om ingen vikt):");
            decimal weightKG = decimal.Parse(Console.ReadLine());

            decimal total = _cart.Total(weightKG);
            Console.WriteLine($"Totalt pris: {total:c}");

            Console.WriteLine("Bekräfta betalning (ja/nej):");
            string input = Console.ReadLine().ToLower();

            if (input == "ja")
            {
                Receipt receipt = new Receipt(_cart.Items(), total, DateTime.Now);

                _receipts.Add(receipt);

                Console.WriteLine("Tack för ditt köp!");
                SaveReceipt();
                PrintReceipt(myCart);
                _receiptNumber++;
            }
            else
            {
                Console.WriteLine("Betalning avbruten.");
                SaveReceipt();
                PrintReceipt(myCart);
                _receiptNumber++;
            }
        }

        private void SaveReceipt()
        {

            string filename = $"RECEIPT_{DateTime.Now:yyyyMMdd}.txt";
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            using (StreamWriter writer = File.AppendText(filepath))
            {
                writer.WriteLine($"KVITTONUMMER: {_receiptNumber}");
                writer.WriteLine($"DATUM: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine("PRODUKTER:");

                foreach (Product product in _cart.Items())
                {
                    writer.WriteLine($"- {product.Name}: {product.Price:c}");
                }

                writer.WriteLine($"TOTALT: {_cart.Total(0),27:c}");
            }

            Console.WriteLine($"Kvitto sparades till filen {filename}");
        }


        private void DisplayReceipts()
        {
            Console.WriteLine("Tidigare kvitton:");

            foreach (Receipt receipt in _receipts)
            {
                Console.WriteLine($"Datum: {receipt.PurchaseDate}, Totalt pris: {receipt.Total:c}");
                Console.WriteLine("Varor:");
                foreach (Product item in receipt.Items)
                {
                    Console.WriteLine($"{item.Name} ({item.Price:c})");
                }
                Console.WriteLine();
            }
        }


    }
}