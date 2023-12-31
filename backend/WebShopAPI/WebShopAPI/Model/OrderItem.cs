﻿using System.Text.Json.Serialization;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Model
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [JsonIgnore]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }

    }
}
