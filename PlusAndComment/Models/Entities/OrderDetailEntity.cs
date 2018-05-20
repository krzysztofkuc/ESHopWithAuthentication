using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("OrderDetail")]
    public class OrderDetailEntity
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderEntity Order { get; set; }
    }
}