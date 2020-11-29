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
        public int ArticleId { get; set; }
        public int VersionId { get; set; }
        [Required(ErrorMessage = "Your article has to have a title.")]
        public string Title { get; set; }
        public string Content { get; set; }
        public HttpPostedFileBase ContentFile { get; set; }
        public int[] DomainIds { get; set; }
        public virtual ICollection<Domain> Domains { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public IEnumerable<SelectListItem> AllDomains { get; set; }
        public IEnumerable<SelectListItem> AllVersions { get; set; }
    }
}