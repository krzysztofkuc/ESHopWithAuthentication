using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class AddProductVM
    {
        public ICollection<ProductVM> AllProducts { get; set; }

        public ICollection<CategoryVM> AllCategories { get; set; }

        public ProductVM CurrentProduct { get; set; }

        public int iteration { get; set; }
    }
}