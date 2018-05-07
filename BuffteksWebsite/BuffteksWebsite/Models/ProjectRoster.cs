using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuffteksWebsite.Models
{
    public class ProjectRoster
    {
        public string ProjectParticipantID { get; set; }
        [Display(Name = "Participant Name")]
        public ProjectParticipant ProjectParticipant { get; set; }
        public string ProjectID { get; set; }
        [Display(Name = "Project")]
        public Project Project { get; set; }
    }
}