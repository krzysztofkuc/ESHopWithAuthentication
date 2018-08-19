using PlusAndComment.Models.ViewModel.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("Product")]
    public class ProductEntity
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public string ThumbPath { get; set; }

        public string Title { get; set; }

        [ForeignKey("Category")]
        public int CatId { get; set; }

        [ForeignKey("ProdId")]
        public virtual ICollection<PictureEntity> Pictures { get; set; }

        public virtual CategoryEntity Category { get; set; }

        [ForeignKey("ProductAttributeId")]
        public virtual ICollection<ProductAttributeEntity> Attributes { get; set; }

    }
}