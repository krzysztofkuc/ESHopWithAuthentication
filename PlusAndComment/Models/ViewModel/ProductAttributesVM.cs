using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace PlusAndComment.Models.ViewModel
{
    public class ProductAttributesVM
    {
        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        [ScriptIgnore]
        public int CategoryAttributeId { get; set; }

        [ScriptIgnore]
        public int? ProductOfAttributeId { get; set; }

        public string Value { get; set; }

        [ScriptIgnore]
        public string AttributeType { get; set; }

        [ScriptIgnore]
        public CategoryVM CategoryAttribute { get; set; }
    }
}