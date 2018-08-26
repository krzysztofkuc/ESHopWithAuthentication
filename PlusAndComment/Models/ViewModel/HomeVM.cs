using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class HomeVM
    {
        public ICollection<ProductVM> Prducts { get; set; }
        public ICollection<CategoryVM> Categories { get; set; }
        public List<CategoryAttributeVM> CurrentAttributes { get; set; }
    }
}