using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlusAndComment.Models.ViewModel
{
    public class AddCategoryAttributeVM
    {
        public int PKAttributeId { get; set; }

        [Display(Name = "Category name")]
        public string CategoryName { get; set; }

        public string Value { get; set; }

        public int CategoryAttributeId { get; set; }

        public string AttributeType { get; set; }

        public CategoryVM Category { get; set; }

        public List<ProductAttributeVM> ProductAttributes { get; set; }

        public List<AttributeValueListVM> ComboboxValues { get; set; }
    }
}