using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IndividualTaskManagement.Models
{
    public class Subgoal
    {
        public int Id { get; set; }
        public virtual Goal Goal { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ApplicationUser Student { get; set; }
        public DateTime EndDate { get; set; }
        public bool Overdue { get; set; }
        public bool AtTerm { get; set; }
    }

    public partial class CreateSubgoalModel
    {
        public CreateSubgoalModel(Subgoal subgoal)
        {
            id = subgoal.Id;
            name = subgoal.Name;           
            description = subgoal.Description;
            student_id = subgoal.Student.Id;
            endDate = subgoal.EndDate;           
        }

        public CreateSubgoalModel()
        {

        }

        public int id { get; set; }

        [Display(Name = "Task")]
        public string name { get; set; }
        public string student_id { get; set; }
        public string description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime endDate { get; set; }       
    }

    public partial class EditSubgoalModel
    {
        public EditSubgoalModel(Subgoal subgoal)
        {
            id = subgoal.Id;
            name = subgoal.Name;
            description = subgoal.Description;
            atTerm = subgoal.AtTerm;
            endDate = subgoal.EndDate;
        }

        public EditSubgoalModel()
        {

        }

        public int id { get; set; }

        [Display(Name = "Task")]
        public string name { get; set; }       
        public string description { get; set; }
        public bool atTerm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime endDate { get; set; }
    }
}