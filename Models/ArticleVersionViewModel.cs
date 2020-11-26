using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DawtNetProject.Models
{
    public class ArticleVersionViewModel
    {
        [Required(ErrorMessage = "Your article has to have a title.")]
        public string Title { get; set; }
        public string Content { get; set; }
        public HttpPostedFileBase ContentFile { get; set; }
        public int[] DomainIds { get; set; }
        public virtual ICollection<Domain> Domains { get; set; }

        public IEnumerable<SelectListItem> AllDomains { get; set; }
    }
}