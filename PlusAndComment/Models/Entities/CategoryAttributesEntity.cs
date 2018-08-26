using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("CategoryAttributes")]
    public class CategoryAttributesEntity
    {
        [Key]
        public int PKAttributeId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        [ForeignKey("CategoryAttribute")]
        public int CategoryAttributeId { get; set; }

        public string AttributeType { get; set; }

        public virtual CategoryEntity CategoryAttribute { get; set; }

        [ForeignKey("ProductAttributeId")]
        public virtual ICollection<ProductAttributeEntity> ProductAttributes { get; set; }

        [ForeignKey("FK_CategoryAttrId")]
        public virtual ICollection<AttributeValueListEntity> ComboboxValues { get; set; }
    }
}