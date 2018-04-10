using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("PostEntity")]
    public partial class PostEntity
    {
        public PostEntity()
        {
            this.Posts = new HashSet<PostEntity>();
        }

        [Key]
        public int ID { get; set; }
        public string Header { get; set; }
        public string FilePath { get; set; }

        [ForeignKey("User")]
        public string ApplicationUser_Id { get; set; }

        [ForeignKey("Posts")]
        public Nullable<int> PostEntity_ID { get; set; }

        public int PlusAmount { get; set; }
        public int MinusAmount { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ReferenceUrl { get; set; }
        public string Url { get; set; }
        public bool Removed { get; set; }
        public string EmbedUrl { get; set; }
        public string PostType { get; set; }
        public bool? NeedAge { get; set; }
        public string CommentParent { get; set; }
        public Nullable<int> Article_ID { get; set; }

        [ForeignKey("MainMem")]
        public Nullable<int> MainMems_ID_Comment { get; set; }

        public bool IsMainComment { get; set; }
        

        public virtual ICollection<PostEntity> Posts { get; set; }

        public virtual MainMems MainMem { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}