using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models.ViewModel
{
    public class OrderVM
    {
            public int OrderId { get; set; }

        public string Username { get; set; }
        [Required]
        [DisplayName("Imię")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Nazwisko")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("Ulica")]
        public string Address { get; set; }
        [Required]
        [DisplayName("Miasto")]
        public string City { get; set; }
        [Required]
        [DisplayName("Województwo")]
        public string State { get; set; }
        [Required]
        [DisplayName("Kod pocztowy")]
        public string PostalCode { get; set; }
        [Required]
        [DisplayName("Państwo")]
        public string Country { get; set; }
        [Required]
        [DisplayName("Numer telefonu")]
        public string Phone { get; set; }
        [Required]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy adres email")]
        public string Email { get; set; }
        [Required]
        [DisplayName("Suma")]
        public double Total { get; set; }
        
        public System.DateTime OrderDate { get; set; }
        public ICollection<OrderDetailVM> OrderDetails { get; set; }
    }
}