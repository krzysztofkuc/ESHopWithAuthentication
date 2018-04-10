using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlusAndComment.Models.Entities
{
    [Table("UserPosts")]
    public class UserPosts
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public string IdUser { get; set; }
        [ForeignKey("Post")]
        public int IdPost { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual PostEntity Post { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ApplicationUser User { get; set; }
    }
}