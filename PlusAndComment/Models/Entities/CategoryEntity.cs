using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("Category")]
    public partial class CategoryEntity
    {
        public CategoryEntity()
        {
            //this.Categories = new HashSet<CategoryEntity>();
        }

        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual ICollection<CategoryEntity> Categories { get; set; }

        [ForeignKey("CatId")]
        public virtual ICollection<ProductEntity> Products { get; set; }
    }
}