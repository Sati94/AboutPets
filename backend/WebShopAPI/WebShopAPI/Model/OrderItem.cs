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

        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }

        public Product Product { get; set; }

    }
}
