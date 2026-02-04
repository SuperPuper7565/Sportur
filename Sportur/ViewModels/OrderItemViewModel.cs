namespace Sportur.ViewModels
{
    public class OrderItemViewModel
    {
        public string ModelName { get; set; }
        public string Color { get; set; }
        public string FrameSize { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
