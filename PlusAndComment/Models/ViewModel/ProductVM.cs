using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class ProductVM
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public string ThumbPath { get; set; }

        public int CatId { get; set; }

        public CategoryVM Category { get; set; }

        public ICollection<PictureVM> Pictures { get; set; }
    }
}