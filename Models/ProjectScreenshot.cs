using System;
using System.Collections.Generic;

namespace ProjectAPI.Models
{
    public class ProjectScreenshot
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ImageUrl { get; set; }
        public string Caption { get; set; }
        public int DisplayOrder { get; set; }
        
        // Navigation property
        public Project Project { get; set; }
    }
}
