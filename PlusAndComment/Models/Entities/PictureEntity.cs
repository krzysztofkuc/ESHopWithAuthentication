using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.ViewModel.Entities
{
    [Table("Picture")]
    public class PictureEntity
    {
        [Key]
        public int PictureId { get; set; }

        public string Path { get; set; }

        public int? ProdId { get; set; }

        //[ForeignKey("ProdId")]
        //public virtual ProductEntity Product { get; set; }
    }
}