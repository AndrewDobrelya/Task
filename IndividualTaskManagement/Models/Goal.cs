using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IndividualTaskManagement.Models
{
    public class Goal
    {
        public int Id { get; set; }

        
        [Display(Name = "Task")]
        public string Name { get; set; }

        [Display(Name = "Date of publication")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Author")]
        public virtual ApplicationUser Author { get; set; }
      
        [Display(Name = "Subject")]
        public virtual Subject Subject { get; set; }

        [Display(Name = "Complete")]
        public int Completeness { get; set; }
    }

    public partial class AddGoalModel
    {
        public AddGoalModel(Goal goal)
        {
            id = goal.Id;
            name = goal.Name;          
            subject_id = goal.Subject.Id; 
        }

        public AddGoalModel()
        {

        }
        
        public int id { get; set; }

        [Required]
        [Display(Name = "Task")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public int subject_id { get; set; }      
    }
}