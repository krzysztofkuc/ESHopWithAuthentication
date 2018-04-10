using System;
using System.ComponentModel;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Models
{
    [Serializable]
    public class PostVM
    {
        public int ID { get; set; }

        public int ParentPostID { get; set; }

        [DisplayName("18+")]
        public bool NeedAge { get; set; }

        public DateTime? AddedTime { get { return DateTime.Now; } }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}