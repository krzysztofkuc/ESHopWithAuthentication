using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models
{
    public class ConfrimEmailVM
    {
        [Required]
        [Display(Name = "Nick")]
        public string UserName { get; set; }
    }
}