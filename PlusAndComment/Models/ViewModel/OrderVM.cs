using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class OrderVM
    {
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
            public ICollection<OrderDetailVM> OrderDetails { get; set; }
    }
}