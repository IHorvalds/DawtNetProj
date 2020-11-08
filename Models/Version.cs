using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DawtNetProject.Models
{
    public class Version
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int VersionNumber { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ContentPath { get; set; }

        public virtual Article Article { get; set; }
    }
}