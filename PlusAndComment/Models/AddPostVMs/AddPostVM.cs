using System;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Models.AddPostVMs
{
    //[Serializable]
    public class AddPostVM : PostVM
    {
        //public AddPostVM()
        //{
        //    Article = new AddArticleVM();
        //}

        public string Header { get; set; }
        public UIPostType Type { get; set; }
        public bool Removed { get; set; }

        public AddLinkVM Link { get; set; }
        public AddPictureFromDiskVM Picture { get; set; }
        public AddHumourVM Humour { get; set; }
        public AddSucharVM Suchar { get; set; }
        public AddArticleVM Article { get; set; }
    }
}