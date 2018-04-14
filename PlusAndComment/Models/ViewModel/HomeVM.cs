using PlusAndComment.Models.ViewModel;
using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class HomeVM
    {
        public ICollection<ProductVM> Prducts { get; set; }
    }
}