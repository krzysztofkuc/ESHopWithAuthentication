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

        public enum AllAttributeTypes { number, numberFrom, numberTo, text, list, multiSelectList, date, dateFrom, dateTo };
        
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
    }
}