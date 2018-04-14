using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlusAndComment.Models.Entities
{
    [Table("Articles")]
    public class ArticleEntity
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }
        public string Header { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string AbsThumbPath { get; set; }
        public string RelThumbPath { get; set; }
        public DateTime? AddedTime { get; set; }
        public string PostType { get; set; }
        public string ThumbType { get; set; }
    }
}