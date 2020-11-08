using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DawtNetProject.Models
{
    public class ArticleVersionViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime LastEdit { get; set; }
        public int[] DomainIds { get; set; }

    }
}