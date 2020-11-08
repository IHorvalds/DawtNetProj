using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DawtNetProject.Models
{
    public class Article // id, title, desc, last_edit
    {
        [Key]
        public int Id { get; set; }
        public bool ProtectFromEditing { get; set; }
        

        public virtual ICollection<Domain> Domains { get; set; }
        public virtual Version CurrentVersion { get; set; }
        public virtual ICollection<Version> Versions { get; set; }

        public IEnumerable<SelectListItem> AllDomains { get; set; }
    }
}