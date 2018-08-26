namespace PlusAndComment.Models.ViewModel
{
    public class AttributeValueListVM
    {
        public int? Id { get; set; }

        public int? ListId { get; set; }

        public string Value { get; set; }

        public int? FK_CategoryAttrId { get; set; }

        public int? FK_ProductAttrId { get; set; }

        public bool Checked { get; set; }
    }
}