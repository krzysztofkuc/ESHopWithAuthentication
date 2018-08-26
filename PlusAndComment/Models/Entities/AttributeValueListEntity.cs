using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("AttributeValueList")]
    public class AttributeValueListEntity
    {
        [Key]
        public int Id { get; set; }

        public int ListId { get; set; }

        public string Value { get; set; }

        public int? FK_CategoryAttrId { get; set; }

        public int? FK_ProductAttrId { get; set; }
    }
}