﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;
using WebShopAPI.Model.CategoryClasses;

namespace WebShopAPI.Model
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal Discount { get; set; }
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
        public string ImageBase64 { get; set; }

        [JsonIgnore]
        public List<OrderItem> OrderItems { get; set; }
    }
}
