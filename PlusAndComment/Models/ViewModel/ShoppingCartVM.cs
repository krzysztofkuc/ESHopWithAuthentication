using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public ICollection<CartVM> CartItems { get; set; }
        public double CartTotal { get; set; }
    }
}