using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("ProductAttributes")]
    public class ProductAttriutesEntity
    {
        [Key]
        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        public int CategoryAttributeId { get; set; }

        public string Value { get; set; }

        public string AttributeType { get; set; }

        [ForeignKey("CategoryAttributeId")]
        CategoryEntity CategoryAttribute { get; set; }
    }
}