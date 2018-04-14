using System.Collections.Generic;
using System.ComponentModel;

namespace PlusAndComment.Models.ViewModel
{
    public class AddCategoryVM
    {
        public AddCategoryVM()
        {
            this.AllCategories = new List<CategoryVM>();
        }

        [DisplayName("Parent category")]
        public ICollection<CategoryVM> AllCategories { get; set; }

        public CategoryVM Category { get; set; }
    }
}