using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DawtNetProject.Models
{
    public class DomainArticleViewModel
    {
        public int DomainId { get; set; }
        public string DomainName { get; set; }
        public string DomainDescription { get; set; }
        public DateTime LastArticlePublishDate { get; set; }
        public List<ArticleVersionViewModel> articles;
    }
}