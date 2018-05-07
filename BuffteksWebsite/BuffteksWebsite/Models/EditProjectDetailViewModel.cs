using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace BuffteksWebsite.Models
{
    public class EditProjectDetailViewModel
    {

        public Project TheProject {get; set;}

        public string SelectedID { get; set; }

        public List<SelectListItem> ProjectClientsList { get; set; }
        
        public List<SelectListItem> ProjectMembersList { get; set; }

        public string ErrorMessage { get; set; }
    }
}