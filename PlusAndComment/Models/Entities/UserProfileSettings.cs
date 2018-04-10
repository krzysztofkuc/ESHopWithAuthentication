using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("UserProfileSettings")]
    public class UserProfileSettings
    {
        [Key]
        public int Id { get; set; }
        public bool ShowNeedAgePics { get; set; }
    }
}