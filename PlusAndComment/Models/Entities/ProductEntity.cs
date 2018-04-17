using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("Product")]
    public class ProductEntity
    {
        public ProductEntity()
        {
            //this.Pictures = new HashSet<PictureEntity>();
        }

        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public string ThumbPath { get; set; }

        [ForeignKey("Category")]
        public int CatId { get; set; }

        //[ForeignKey("ProdId")]
        //public virtual ICollection<PictureEntity> Pictures { get; set; }

        public virtual CategoryEntity Category { get; set; }

    }
}