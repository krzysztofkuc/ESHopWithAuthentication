using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlusAndComment.Models
{
    public class MainMemVM
    {
        public int ID { get; set; }

        public MainPostVM Post { get; set; }

        public ICollection<MainPostVM> Comments { get; set; }
    }
}
