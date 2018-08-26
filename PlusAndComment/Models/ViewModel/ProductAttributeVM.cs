using System.Collections.Generic;

namespace PlusAndComment.Models.ViewModel
{
    public class ProductAttributeVM
    {
        public int ProductAttributeId { get; set; }

        public int? ProductOfAttributeId { get; set; }

        public int? FK_CategoryAttributes { get; set; }

        public string Value { get; set; }

        public ProductVM Product { get; set; }

        public CategoryAttributeVM CategoryAttribute { get; set; }

        public ICollection<AttributeValueListVM> ComboboxValues { get; set; }
    }
}