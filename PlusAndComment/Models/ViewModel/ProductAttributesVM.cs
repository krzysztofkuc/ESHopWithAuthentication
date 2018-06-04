using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlusAndComment.Models.ViewModel
{
    public class ProductAttributeVM
    {
        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        public int CategoryAttributeId { get; set; }

        public string Value { get; set; }

        public string AttributeType { get; set; }

        CategoryVM CategoryAttribute { get; set; }
    }
}