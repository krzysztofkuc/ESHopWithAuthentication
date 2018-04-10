using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models
{
    public class SucharyVM
    {
        [Display(Name = "Tytuł")]
        public string Header { get; set; }
        public List<SucharVM> Suchary { get; set; }
    }
}