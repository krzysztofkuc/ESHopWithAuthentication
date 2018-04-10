using System;

namespace PlusAndComment.Models
{
    public class FrameVM
    {
        public string Header { get; set; }
        public string LongDesc { get; set; }
        public string ThumbnailPath { get; set; }
        public Uri Uri { get; set; }
    }
}