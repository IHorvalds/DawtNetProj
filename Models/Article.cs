using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DawtNetProject.Models
{
    public class Article // id, title, desc, last_edit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }
        public bool ProtectFromEditing { get; set; }
        
        [Required]
        public virtual ICollection<Domain> Domains { get; set; }
        [Required]
        public int CurrentVersionId { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}