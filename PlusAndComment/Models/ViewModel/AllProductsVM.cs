using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class AllProductsVM
    {
        public ICollection<ProductVM> Products { get; set; }
    }
}