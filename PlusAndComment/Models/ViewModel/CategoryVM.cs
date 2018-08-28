using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace PlusAndComment.Models.ViewModel
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }

        [Display(Name = "Category name")]
        public string Name { get; set; }

        [ScriptIgnore]
        public int? ParentId { get; set; }

        [ScriptIgnore]
        public int? ProdId { get; set; }

        [ScriptIgnore]
        public ICollection<CategoryVM> Categories { get; set; }

        [ScriptIgnore]
        public ICollection<ProductVM> Products { get; set; }

        [ScriptIgnore]
        public ICollection<CategoryAttributeVM> Attributes { get; set; }

    }
}