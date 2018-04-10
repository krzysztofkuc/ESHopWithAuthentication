using System;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Common
{
    [Serializable]
    public class Gif
    {
        public string FileName { get; set; }

        public string PathAbsolute  { get; set; }

        public string PathRelative { get; set; }

        public string FirstFramePathAbsolute { get; set; }

        public string FirstFramePathRelative { get; set; }

        public string ThumbPathAbsolute { get; set; }

        public string ThumbPathRelative { get; set; }

        public string Url { get; set; }

        public string EmbedUrl { get; set; }

        public string Type { get { return PostType.gif.ToFriendlyString(); } }
    }
}