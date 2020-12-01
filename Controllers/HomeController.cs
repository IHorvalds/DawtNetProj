using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DawtNetProject.Data;
using DawtNetProject.Models;

namespace DawtNetProject.Controllers
{
    public class HomeController : Controller
    {
        private App_Context db = new App_Context();

        public ActionResult Index()
        {
            //var domains = from domain in db.Domains.Include("Articles")
            //              orderby domain.LastArticlePublished descending
            //              select domain;

            var domains = db.Domains
                            .Include("Articles")
                            .OrderByDescending(d => d.Articles.OrderByDescending(a=>a.DatePublished).FirstOrDefault().DatePublished);

            var doms = domains.ToList();
            List<DomainArticleViewModel> daVMs = new List<DomainArticleViewModel>();
            foreach (var dom in doms)
            {
                DomainArticleViewModel daVM = new DomainArticleViewModel();
                daVM.DomainId = dom.Id;
                daVM.DomainName = dom.Title;
                daVM.articles = new List<ArticleVersionViewModel>();

                if (dom.Articles != null)
                {
                    daVM.LastArticlePublishDate = DateTime.MinValue;
                    List<Article> la = new List<Article>();
                    la = dom.Articles.OrderByDescending(a => a.DatePublished).Take(3).ToList();
                    daVM.LastArticlePublishDate = la.FirstOrDefault().DatePublished;
                    foreach (var article in la)
                    {
                        ArticleVersionViewModel av = new ArticleVersionViewModel();
                        av.ArticleId = article.ArticleId;
                        av.VersionId = article.CurrentVersionId;
                        av.Domains = article.Domains;
                        Models.Version v = db.Versions.Find(article.CurrentVersionId);
                        av.Title = v.Title;
                        av.DatePublished = article.DatePublished;
                        av.LastEdit = v.LastEdit;
                        daVM.articles.Add(av);
                    }

                    daVMs.Add(daVM);
                }

            }
            return View(daVMs);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}