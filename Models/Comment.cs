using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DawtNetProject.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Can't have an empty comment, can we?")]
        public string Content { get; set; }
        [Required]
        public DateTime LastEdit { get; set; }

        public virtual Article article { get; set; }
    }
}