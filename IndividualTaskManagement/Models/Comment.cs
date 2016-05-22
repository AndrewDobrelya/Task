using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndividualTaskManagement.Models
{
    public class Comment
    {
        public int id { get; set; }

        public virtual DateTime date { get; set; }

        public string text { get; set; }

        public virtual Subgoal subgoal { get; set; }

        public virtual Comment previosComment { get; set; }

        public virtual ApplicationUser user { get; set; }
    }
}