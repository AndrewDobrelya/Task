using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IndividualTaskManagement.Models
{
    public class Subject
    {
        public int Id { get; set; }       
        [Display(Name = "Subject")]
        public string Name { get; set; }
    }
}