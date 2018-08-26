using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("ProductAttribute")]
    public class ProductAttributeEntity
    {
        [Key]
        public int ProductAttributeId { get; set; }

        [ForeignKey("Product")]
        public int? ProductOfAttributeId { get; set; }

        [ForeignKey("CategoryAttribute")]
        public int? FK_CategoryAttributes { get; set; }

        public string Value { get; set; }

        public virtual ProductEntity Product { get; set; }

        public virtual CategoryAttributesEntity CategoryAttribute { get; set; }

        [ForeignKey("FK_ProductAttrId")]
        public virtual ICollection<AttributeValueListEntity> ComboboxValues { get; set; }


    }
}