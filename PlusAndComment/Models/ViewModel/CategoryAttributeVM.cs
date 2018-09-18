using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models.ViewModel
{
    public class CategoryAttributeVM
    {
        public int PKAttributeId { get; set; }

        [Display(Name = "Attribute name")]
        public string Name { get; set; }

        public string Value { get; set; }

        public double? numberFrom { get; set; }

        public double? numberTo { get; set; }

        public string dateFrom { get; set; }

        public string dateTo { get; set; }

        public int CategoryAttributeId { get; set; }

        public string AttributeType { get; set; }

        public CategoryVM CategoryAttribute { get; set; }

        public List<ProductAttributeVM> ProductAttributes { get; set; }

        public List<AttributeValueListVM> ComboboxValues { get; set; }
    }
}