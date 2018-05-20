namespace PlusAndComment.Models.ViewModel
{
    public class OrderDetailVM
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public ProductVM Product { get; set; }
        public OrderVM Order { get; set; }
    }
}