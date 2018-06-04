using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlusAndComment.Models.ViewModel
{
    public class CompanyInformationVM
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} musi mieć przynajmniej {2} znaków.", MinimumLength = 5)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public int PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Logo { get; set; }

        [Required]
        public string SmtpServer { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string Pop3Server { get; set; }

        [Required]
        public string EmailPassword { get; set; }

        [Required]
        public string PostalCode { get; set; }
    }
}