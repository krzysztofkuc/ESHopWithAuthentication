using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("Order")]
    public partial class OrderEntity
    {
        [Key]
        public int OrderId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double Total { get; set; }
        public System.DateTime OrderDate { get; set; }

        public int? OrderOrderDetailId { get; set; }

        [ForeignKey("OrderDetailId")]
        public ICollection<OrderDetailEntity> OrderDetail { get; set; }
    }
}