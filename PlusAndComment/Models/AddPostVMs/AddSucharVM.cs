using PlusAndComment.Common;
using System;
using System.ComponentModel.DataAnnotations;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Models.AddPostVMs
{
    [Serializable]
    public class AddSucharVM
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public string AbsThumbPath { get; set; }

        public string RelThumbPath { get; set; }

        public string CommentParent { get { return UIPostType.Suchar.ToFriendlyString(); } }
    }
}