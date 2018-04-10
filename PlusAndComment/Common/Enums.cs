using System;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Common
{
    [Serializable]
    public static class Enums
    {
        //leave it in the same order
        public enum PostType {img, gif, mov, link};

        public enum UIPostType { Humour, MainMeme, Suchar, Article };

        public enum PictureType { img, gif };
    }

    public static class PosTypeExtensions
    {
        public static string ToFriendlyString(this PostType me)
        {
            switch (me)
            {
                case PostType.img:
                    return "img";
                case PostType.gif:
                    return "gif";
                case PostType.mov:
                    return "mov";
                case PostType.link:
                    return "link";
                default:
                    return "damn";
            }
        }

        public static string ToFriendlyString(this UIPostType me)
        {
            switch (me)
            {
                case UIPostType.Humour:
                    return "Humour";
                case UIPostType.MainMeme:
                    return "MainMeme";
                case UIPostType.Suchar:
                    return "Suchar";
                case UIPostType.Article:
                    return "Article";
                default:
                    return "damn";
            }
        }
    }
}