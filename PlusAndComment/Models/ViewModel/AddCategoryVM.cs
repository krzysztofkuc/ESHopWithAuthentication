using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models.ViewModel
{
    public class AddCategoryVM
    {
        public AddCategoryVM()
        {
            this.AllCategories = new List<CategoryVM>();
        }

        [UIHint("CategoryDropDOwn")]
        [DisplayName("Parent category")]
        public ICollection<CategoryVM> AllCategories { get; set; }

        public CategoryVM Category { get; set; }

        public int iteration { get; set; }
}
}