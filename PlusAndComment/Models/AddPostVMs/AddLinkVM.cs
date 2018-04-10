using PlusAndComment.Common;
using System;
using System.ComponentModel.DataAnnotations;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Models.AddPostVMs
{
    [Serializable]
    public class AddLinkVM : PostVM
    {
        public Picture Picture { get; set; }

        public Gif Gif { get; set; }

        [Url]
        [Required]
        public string Url { get; set; }

        [Url]
        public string EmbedUrl { get; set; }

        [Required]
        [Display(Name = "Adres URL linku")]
        public string ReferenceUrl { get; set; }

        [Required]
        public string Header { get; set; }

        public string Type { get; set; }

        public string CommentParent { get { return UIPostType.MainMeme.ToFriendlyString(); } }
    }
}