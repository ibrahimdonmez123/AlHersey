using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AlHersey.Models
{
    public interface IOrderRepository
    {
        int ProductID { get; set; }
        int Quantity { get; set; }
        string? MyCart { get; set; }
        decimal UnitPrice { get; set; }
        string? ProductName { get; set; }
        int Kdv { get; set; }
        string? PhotoPath { get; set; }

        Task<List<Order>> OrderSelectAsync();
        Task<Order?> OrderDetailsAsync(int? id);
        Task UpdateQuantityInMyCartAsync(string productId, string newQuantity);
        Task<bool> AddToMyCartAsync(string id);
        Task DeleteFromMyCartAsync(string scid);
        Task<List<Cls_Order>> SelectMyCartAsync();
        Task<string> WriteToOrderTableAsync(string Email);
        Task<List<Order>> SelectMyOrdersAsync(string Email);
    }

    public class Cls_Order : IOrderRepository
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string? MyCart { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ProductName { get; set; }
        public int Kdv { get; set; }
        public string? PhotoPath { get; set; }

        private readonly AlHerseyContext context;

        public Cls_Order(AlHerseyContext _context)
        {
            context = _context;
        }

        public async Task<List<Order>> OrderSelectAsync()
        {
            List<Order> orders = await context.Orders.ToListAsync();
            return orders;
        }

        public async Task<Order?> OrderDetailsAsync(int? id)
        {
            Order? order = await context.Orders.FindAsync(id);
            return order;
        }

        public async Task UpdateQuantityInMyCartAsync(string productId, string newQuantity)
        {
            string[] MyCartArray = MyCart.Split('&');
            Dictionary<string, string> cartDictionary = new Dictionary<string, string>();

            for (int i = 0; i < MyCartArray.Length; i++)
            {
                string[] MyCartArrayLoop = MyCartArray[i].Split('=');

                if (MyCartArrayLoop.Length == 2)
                {
                    cartDictionary[MyCartArrayLoop[0]] = MyCartArrayLoop[1];
                }
            }

            cartDictionary[productId] = newQuantity;

            MyCart = string.Join("&", cartDictionary.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        public async Task<bool> AddToMyCartAsync(string id)
        {
            bool exists = false;

            if (MyCart == "")
            {
                MyCart = id + "=1";
            }
            else
            {
                string[] MyCartArray = MyCart.Split('&');
                for (int i = 0; i < MyCartArray.Length; i++)
                {
                    string[] MyCartArrayLoop = MyCartArray[i].Split('=');

                    if (MyCartArrayLoop[0] == id)
                    {
                        exists = true;
                    }
                }

                if (exists == false)
                {
                    MyCart = MyCart + "&" + id.ToString() + "=1";
                }
            }
            return exists;
        }

        public async Task DeleteFromMyCartAsync(string scid)
        {
            string[] MyCartArray = MyCart.Split('&');
            string NewMyCart = "";
            int count = 1;

            for (int i = 0; i < MyCartArray.Length; i++)
            {
                string[] MyCartArrayLoop = MyCartArray[i].Split('=');

                if (count == 1)
                {
                    if (MyCartArrayLoop[0] != scid)
                    {
                        NewMyCart += MyCartArrayLoop[0] + "=" + Convert.ToInt32(MyCartArrayLoop[1]);
                        count++;
                    }
                }
                else
                {
                    if (MyCartArrayLoop[0] != scid)
                    {
                        NewMyCart += "&" + MyCartArrayLoop[0] + "=" + Convert.ToInt32(MyCartArrayLoop[1]);
                        count++;
                    }
                }
            }

            MyCart = NewMyCart;
        }

        public async Task<List<Cls_Order>> SelectMyCartAsync()
        {
            List<Cls_Order> list = new List<Cls_Order>();

            string[] MyCartArray = MyCart.Split('&');

            if (MyCartArray[0] != "")
            {
                for (int i = 0; i < MyCartArray.Length; i++)
                {
                    string[] MyCartArrayLoop = MyCartArray[i].Split('=');

                    int productID = Convert.ToInt32(MyCartArrayLoop[0]);

                    Product? product = await context.Products.FirstOrDefaultAsync(p => p.ProductID == productID);

                    Cls_Order order = new Cls_Order(context);

                    order.ProductID = product.ProductID;
                    order.Quantity = Convert.ToInt32(MyCartArrayLoop[1]);
                    order.UnitPrice = product.UnitPrice;
                    order.ProductName = product.ProductName;
                    order.Kdv = product.Kdv;
                    order.PhotoPath = product.PhotoPath;

                    list.Add(order);
                }
            }

            return list;
        }

        public async Task<string> WriteToOrderTableAsync(string Email)
        {
            string OrderGroupGUID = DateTime.Now.ToString().Replace(":", "").Replace(".", "").Replace(" ", "").Replace(",", "");
            DateTime OrderDate = DateTime.Now;
            try
            {
                List<Cls_Order> orders = await SelectMyCartAsync();

                foreach (var item in orders)
                {
                    Order order = new Order();

                    order.OrderGroupGUID = OrderGroupGUID;
                    order.UserID = context.Users.FirstOrDefault(u => u.Email == Email).UserID;
                    order.ProductID = item.ProductID;
                    order.Qantity = item.Quantity;

                    context.Orders.Add(order);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                OrderGroupGUID = "Error";
            }
            return OrderGroupGUID;
        }

        public async Task<List<Order>> SelectMyOrdersAsync(string Email)
        {
            int UserID = (await context.Users.FirstOrDefaultAsync(u => u.Email == Email)).UserID;

            List<Order> orders = await context.Orders.Where(o => o.UserID == UserID).ToListAsync();

            return orders;
        }
    }
}
