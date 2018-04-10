using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlusAndComment.Models
{
    public class UserVM
    {
        public UserVM()
        {
            this.Rola = new RolaVM();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool Banned { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? BannEndDate { get; set; }
        public RolaVM Rola { get; set; }
    }
}