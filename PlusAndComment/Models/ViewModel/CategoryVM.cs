using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? ProdId { get; set; }

        public ICollection<CategoryVM> Categories { get; set; }

        public ICollection<ProductVM> Products { get; set; }

    }
}