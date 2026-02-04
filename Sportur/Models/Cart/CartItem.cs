namespace Sportur.Models.Cart
{
    public class CartItem
    {
        public int VariantId { get; set; }

        public string ModelName { get; set; }
        public string Color { get; set; }
        public string FrameSize { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }

        public string PhotoUrl { get; set; }
    }


}
