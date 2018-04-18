namespace PlusAndComment.Models.ViewModel
{
    public class CartVM
    {
        public int RecordId { get; set; }
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int Number { get; set; }
        public System.DateTime DateCreated { get; set; }
        public virtual ProductVM Product { get; set; }
    }
}