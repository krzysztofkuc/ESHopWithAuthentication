using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("CompanyInformation")]
    public class CompanyInformationEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Pop3Server { get; set; }
        public string EmailPassword { get; set; }
        public string PostalCode { get; set; }
    }
}