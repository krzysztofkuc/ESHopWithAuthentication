using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class CategoryAttributeVM
    {
        public int PKAttributeId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public int CategoryAttributeId { get; set; }

        public string AttributeType { get; set; }

        public CategoryVM CategoryAttribute { get; set; }

        public List<ProductAttributeVM> ProductAttributes { get; set; }

        public List<AttributeValueListVM> ComboboxValues { get; set; }
    }
}