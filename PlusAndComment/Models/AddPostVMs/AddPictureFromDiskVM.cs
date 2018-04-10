using PlusAndComment.Common;
using System;
using System.ComponentModel.DataAnnotations;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Models.AddPostVMs
{
    [Serializable]
    public class AddPictureFromDiskVM : PostVM
    {
        public AddPictureFromDiskVM()
        {
            this.Picture = new Picture();
            this.Gif = new Gif();
        }

        [Required]
        [Display(Name = "Opis")]
        public string Header { get; set; }

        public Picture Picture { get; set; }

        public Gif Gif { get; set; }

        public string Type { get; set; }

        public string CommentParent { get { return UIPostType.MainMeme.ToFriendlyString(); } }
    }
}