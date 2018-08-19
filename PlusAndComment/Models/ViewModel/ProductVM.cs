using PlusAndComment.Models.Entities;
using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class ProductVM
    {
        public ProductVM()
        {
            Pictures = new List<PictureVM>();

            for(int i = 0; i< 4; i++)
            {
                Pictures.Add(new PictureVM());
            }
            
        }

        public int ProductId { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public string ThumbPath { get; set; }

        public int CatId { get; set; }

        public List<PictureVM> Pictures { get; set; }

        public CategoryVM Category { get; set; }

        public List<ProductAttributeVM> Attributes { get; set; }
    }
}