namespace PlusAndComment.Models.ViewModel
{
    public class ShoppingCartRemoveVM
    {
        public string Message { get; set; }
        public double CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
    }
}