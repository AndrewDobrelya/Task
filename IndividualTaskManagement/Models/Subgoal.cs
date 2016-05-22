﻿using System;
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
        public string EndDate { get; set; }
        public bool Overdue { get; set; }
        public bool AtTetm { get; set; }
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
        public string endDate { get; set; }
    }
}