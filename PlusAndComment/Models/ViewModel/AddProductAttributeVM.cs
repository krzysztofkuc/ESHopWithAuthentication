﻿using PlusAndComment.Models.Entities;
using System.Collections.Generic;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Models.ViewModel
{
    public class AddProductAttributeVM
    {
        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string AttributeType { get; set; }

        public int? CategoryAttributeId { get; set; }

        public int? ProductOfAttributeId { get; set; }

        public ICollection<CategoryVM> AllCategories { get; set; }

        public AllAttributeTypes AllAttributeTypes { get; set; }

        public ICollection<AttributeValueListVM> ComboboxValues { get; set; }
    }
}