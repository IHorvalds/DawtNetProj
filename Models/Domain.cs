using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DawtNetProject.Models
{
    public class Domain
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Domain must have a name.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "A description is also necessary.")]
        public string Description { get; set; }
        public DateTime LastEdit { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}