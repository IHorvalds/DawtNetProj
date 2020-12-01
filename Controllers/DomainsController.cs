using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DawtNetProject.Data;

namespace DawtNetProject.Models
{
    public class DomainsController : Controller
    {
        private App_Context db = new App_Context();

        // GET: Domains
        public ActionResult Index()
        {
            return View(db.Domains.Include("Articles").ToList());
        }

        // GET: Domains/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string orderby = "";
            string asc = "";
            Domain domain = db.Domains
                                .Include("Articles")
                                .FirstOrDefault(d => d.Id == id);
            
            if (domain == null)
            {
                return HttpNotFound();
            }
            DomainArticleViewModel da = new DomainArticleViewModel();
            da.DomainId = domain.Id;
            da.DomainName = domain.Title;
            da.DomainDescription = domain.Description;
            da.articles = new List<ArticleVersionViewModel>();
            da.LastArticlePublishDate = DateTime.MinValue;

            if (domain.Articles != null)
            {
                List<Article> articleList = new List<Article>();
                if (Request["orderby"] != null && Request["orderby"] != "")
                {
                    orderby = Request["orderby"];
                    asc = (Request["asc"] != null && Request["asc"] != "") ? Request["asc"] : "asc";
                    if (orderby == "title")
                    {
                        if (asc == "asc")
                        {
                            articleList = domain.Articles?.OrderBy(a => GetVersionTitle(a.CurrentVersionId)).ToList();
                            ViewBag.asc = "asc";
                        } else
                        {
                            articleList = domain.Articles?.OrderByDescending(a => GetVersionTitle(a.CurrentVersionId)).ToList();
                            ViewBag.asc = "desc";
                        }
                        ViewBag.orderby = "title";
                    } else
                    {
                        if (asc == "asc")
                        {
                            articleList = domain.Articles?.OrderBy(a => a.DatePublished).ToList();
                            ViewBag.asc = "asc";
                        }
                        else
                        {
                            articleList = domain.Articles?.OrderByDescending(a => a.DatePublished).ToList();
                            ViewBag.asc = "desc";
                        }
                        ViewBag.orderby = "date";
                    }
                } else
                {
                    articleList = domain.Articles?.OrderByDescending(a => a.DatePublished).ToList();
                    ViewBag.asc = "desc";
                    ViewBag.orderby = "date";
                }

                foreach (var article in articleList)
                {
                    ArticleVersionViewModel av = new ArticleVersionViewModel();
                    av.ArticleId = article.ArticleId;
                    av.VersionId = article.CurrentVersionId;
                    av.Domains = article.Domains;
                    Models.Version v = db.Versions.Find(article.CurrentVersionId);
                    av.Title = v.Title;
                    if (article.DatePublished > da.LastArticlePublishDate)
                    {
                        da.LastArticlePublishDate = article.DatePublished;
                    }
                    av.DatePublished = article.DatePublished;
                    av.LastEdit = v.LastEdit;
                    da.articles.Add(av);
                }
            }

            return View(da);
        }

        // GET
        public ActionResult Create()
        {
            Domain d = new Domain();

            return View(d);
        }

        // POST: Domains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,LastEdit")] Domain domain)
        {
            domain.LastEdit = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Domains.Add(domain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(domain);
        }

        // GET: Domains/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Domain domain = db.Domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        // POST: Domains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,LastEdit")] Domain domain)
        {
            domain.LastEdit = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(domain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(domain);
        }

        // GET: Domains/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Domain domain = db.Domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        // POST: Domains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Domain domain = db.Domains
                                .Include("Articles")
                                .Where(d => d.Id == id)
                                .FirstOrDefault();

            if (domain.Articles != null)
            {
                var articles = domain.Articles.ToList();
                foreach(var art in articles)
                {
                    if (art.Domains.Count > 1)
                    {
                        art.Domains.Remove(domain);
                    } else
                    {
                        var versions = from version in db.Versions
                                       where version.Article.ArticleId == art.ArticleId
                                       select version;

                        List<Version> vs = versions.ToList();
                        foreach (Version v in vs)
                        {
                            if (System.IO.File.Exists(v.ContentPath))
                            {
                                System.IO.File.Delete(v.ContentPath);
                            }
                            db.Versions.Remove(v);
                        }
                        db.Articles.Remove(art);
                    }
                }
            }

            db.Domains.Remove(domain);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        private string GetVersionTitle(int versionId)
        {
            Version v = db.Versions.Find(versionId);
            if (v == null)
            {
                return "";
            }
            return v.Title;
        }
    }
}
