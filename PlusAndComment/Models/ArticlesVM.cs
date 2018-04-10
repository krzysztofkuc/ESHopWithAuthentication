using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models
{
    public class ArticlesVM
    {
        [Display(Name = "Tytuł")]
        public string Header { get; set; }
        public List<ArticleVM> Articles { get; set; }
    }
}