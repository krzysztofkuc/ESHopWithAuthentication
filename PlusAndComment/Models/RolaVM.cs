using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models
{
    public class RolaVM
    {
        public string Id { get; set; }

        [DisplayFormat(NullDisplayText = "Nie ma roli")]
        public string Name { get; set; }
    }
}