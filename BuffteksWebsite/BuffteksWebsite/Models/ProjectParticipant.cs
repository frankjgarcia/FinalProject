using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuffteksWebsite.Models {
    public class ProjectParticipant {
        [Key]
        public string ID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
        public ICollection<ProjectRoster> Projects { get; set; }

        public override string ToString() {
            return $"First Name: {this.FirstName} Last Name: {this.LastName}";
        }

        [NotMapped]
        public string FullName {
            get {
                return $"{this.FirstName} {this.LastName}";
            }
        }
        [NotMapped]
        public string Type { get; set; }
    }
}