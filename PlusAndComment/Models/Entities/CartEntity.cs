using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("Cart")]
    public class CartEntity
    {
        [Key]
        public int RecordId { get; set; }
        public string CartId { get; set; }

        
        public int CartProductId { get; set; }

        public int Number { get; set; }
        public System.DateTime DateCreated { get; set; }

        [ForeignKey("CartProductId")]
        public virtual ProductEntity Product { get; set; }
    }
}