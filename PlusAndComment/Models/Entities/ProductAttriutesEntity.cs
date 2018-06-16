using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("ProductAttributes")]
    public class ProductAttributesEntity
    {
        [Key]
        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        [ForeignKey("CategoryAttribute")]
        public int CategoryAttributeId { get; set; }

        public string Value { get; set; }

        public string AttributeType { get; set; }


        public virtual CategoryEntity CategoryAttribute { get; set; }
    }
}