using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DawtNetProject.Models
{
    public class Version
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Articolul trebuie sa aiba un titlu.")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "File Path")]
        public string ContentPath { get; set; }
        public DateTime LastEdit { get; set; }
        
        public virtual Article Article { get; set; }

        [NotMapped]
        public int SelectedArticle { get; set; }
        public IEnumerable<SelectListItem> AllArticles { get; set; }
        [NotMapped]
        public HttpPostedFileBase UploadFile { get; set; }
    }
}