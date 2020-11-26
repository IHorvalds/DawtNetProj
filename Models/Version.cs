using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DawtNetProject.Models
{
    public class Version
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Articolul trebuie sa aiba un titlu.")]
        public string Title { get; set; }
        [Required]
        public string ContentPath { get; set; }

        public virtual Article Article { get; set; }
    }
}