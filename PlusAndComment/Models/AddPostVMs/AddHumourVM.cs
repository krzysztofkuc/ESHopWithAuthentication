using PlusAndComment.Common;
using System;
using System.ComponentModel.DataAnnotations;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Models.AddPostVMs
{
    [Serializable]
    public class AddHumourVM : PostVM
    {
        [Required]
        public string Content { get; set; }

        public string Type { get { return "img"; } }

        public string CommentParent { get { return UIPostType.Humour.ToFriendlyString(); } }
    }
}