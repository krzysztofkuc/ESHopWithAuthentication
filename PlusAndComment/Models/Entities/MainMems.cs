using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("MainMems")]
    public class MainMems
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("PostEntity")]
        public int PostEntity_ID { get; set; }

        public virtual PostEntity PostEntity { get; set; }

        [ForeignKey("MainMems_ID_Comment")]
        public virtual ICollection<PostEntity> Comments { get; set; }
    }
}