namespace WebShopAPI.Model.OrderModel.DeliveryType
{
    public class UpdateOrderDeliveryRequest
    {
       
            public DeliveryTypes DeliveryType { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string StreetAddress { get; set; }
        
    }
}
