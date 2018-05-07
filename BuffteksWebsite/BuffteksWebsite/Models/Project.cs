using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

 namespace BuffteksWebsite.Models
{
    public class Project
    {
        [Key]
        public string ID { get; set; }
        [Display(Name = "Project Name")]                
        public string ProjectName { get; set; }
        [Display(Name = "Project Description")]                
        public string ProjectDescription { get; set; }
        public ICollection<ProjectRoster> Participants { get; set; }

        public override string ToString(){
            return $"Project Name: {this.ProjectName}\nProject Description: {this.ProjectDescription}";
        }
    }
}