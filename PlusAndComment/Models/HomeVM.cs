using PlusAndComment.Models.AddPostVMs;
using System.Collections.Generic;

namespace PlusAndComment.Models
{
    public class HomeVM
    {
        public ICollection<MainMemVM> MainMems { get; set; }
        public ICollection<ArticleVM> Articles { get; set; }
        public ICollection<SucharVM> Suchary { get; set; }
        public bool ShowNeedAgePics { get; set; }
        public int CurrentPage { get; set; }
        public int PostsCapacity { get; set; }
    }
}