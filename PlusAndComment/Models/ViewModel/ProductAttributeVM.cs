using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace PlusAndComment.Models.ViewModel
{
    public class ProductAttributeVM
    {
        public int ProductAttributeId { get; set; }

        public int? ProductOfAttributeId { get; set; }

        public int? FK_CategoryAttributes { get; set; }

        public string Value { get; set; }

        [ScriptIgnore]
        public ProductVM Product { get; set; }

        [ScriptIgnore]
        public CategoryAttributeVM CategoryAttribute { get; set; }

        [ScriptIgnore]
        public ICollection<AttributeValueListVM> ComboboxValues { get; set; }
    }
}