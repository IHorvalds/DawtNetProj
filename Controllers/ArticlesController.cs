using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DawtNetProject.Data;

namespace DawtNetProject.Models
{
    public class ArticlesController : Controller
    {
        private App_Context db = new App_Context();

        // GET: Articles
        public ActionResult Index()
        {
            var articles = from article in db.Articles.Include("Versions")
                           select article;
            return View(articles.ToList());
        }

        // GET: Articles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }

            Version v = article.CurrentVersion;
            if (System.IO.File.Exists(v.ContentPath))
            {
                string content = System.IO.File.ReadAllText(v.ContentPath, System.Text.Encoding.UTF8);
                ViewBag.content = content;
                return View(article);
            }
            return View(article);
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            ArticleVersionViewModel avVM = new ArticleVersionViewModel();
            avVM.AllDomains = GetDomains();
            return View(avVM);
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleVersionViewModel avViewModel)
        {

            avViewModel.AllDomains = GetDomains();
            Article a = new Article();
            Version v = new Version();

            v.Title = avViewModel.Title;
            List<Domain> ds = db.Domains.Where(d => avViewModel.DomainIds.Contains(d.Id)).ToList();
            a.Domains = ds;

            a.ProtectFromEditing = false;

            if (avViewModel.ContentFile != null && avViewModel.ContentFile.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/article_files"),
                                                System.Guid.NewGuid().ToString() + ".md");

                    avViewModel.ContentFile.SaveAs(path);
                    v.ContentPath = path;
                } catch (Exception e)
                {
                    ViewBag.Message = "Error saving the file.";
                    return View(avViewModel);
                }

            } else if (avViewModel.Content != null && avViewModel.Content.Length > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/article_files"),
                                                System.Guid.NewGuid().ToString() + ".md");

                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.Write(avViewModel.Content);
                    }
                    v.ContentPath = path;
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error saving the file.";
                    return View(avViewModel);
                }
            } else
            {
                ModelState.AddModelError("OneOfTwoFieldsShouldBeFilled", "Either you upload a file or type your article in the box. You can't have an empty article.");
                return View(avViewModel);
            }

            if (TryValidateModel(a) && TryValidateModel(v))
            {
                db.Articles.Add(a);
                db.Versions.Add(v);
                db.SaveChanges();

                a.CurrentVersion = v;
                v.Article = a;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(avViewModel);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }


            Version v = article.CurrentVersion;
            string content = "";
            if (System.IO.File.Exists(v.ContentPath))
            {
                content = System.IO.File.ReadAllText(v.ContentPath, System.Text.Encoding.UTF8);
            }

            ArticleVersionViewModel avViewModel = new ArticleVersionViewModel();
            avViewModel.Title = v.Title;
            avViewModel.Domains = article.Domains;
            avViewModel.Content = content;
            avViewModel.AllDomains = GetDomains();
            ViewBag.articleId = id;
      
            return View(avViewModel);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleVersionViewModel avViewModel)
        {
            avViewModel.AllDomains = GetDomains();
            if (Request["articleId"] != null)
            {
                int id = Int16.Parse(Request["articleId"]);
                Article article = db.Articles.Find(id);
                if (article == null)
                {
                    return HttpNotFound();
                }

                List<Domain> ds = db.Domains.Where(d => avViewModel.DomainIds.Contains(d.Id)).ToList();
                if (article.Domains != null)
                {
                    List<Domain> l = ds.Where(p => !article.Domains.Contains(p)).ToList();
                    foreach(var dom in l)
                    {
                        article.Domains.Add(dom);
                    }
                } else
                {
                    article.Domains = ds;
                }


                // TODO: Check if there were actually any changes made.
                Version v = new Version();

                v.Title = avViewModel.Title;
                if (avViewModel.ContentFile != null && avViewModel.ContentFile.ContentLength > 0)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/article_files"),
                                                    System.Guid.NewGuid().ToString() + ".md");

                        avViewModel.ContentFile.SaveAs(path);
                        v.ContentPath = path;
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = "Error saving the file.";
                        return View(avViewModel);
                    }

                }
                else if (avViewModel.Content != null && avViewModel.Content.Length > 0)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/article_files"),
                                                    System.Guid.NewGuid().ToString() + ".md");

                        using (StreamWriter sw = new StreamWriter(path))
                        {
                            sw.Write(avViewModel.Content);
                        }
                        v.ContentPath = path;
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = "Error saving the file.";
                        return View(avViewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("OneOfTwoFieldsShouldBeFilled", "Either you upload a file or type your article in the box. You can't have an empty article.");
                    return View(avViewModel);
                }

                if (TryValidateModel(v))
                {
                    db.Versions.Add(v);
                    db.SaveChanges();

                    article.CurrentVersion = v;
                    v.Article = article;
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                return View(avViewModel);
            }

            ModelState.AddModelError("ArticleNotFound", "Cannot find the id for the article you're trying to edit.");
            return View(avViewModel);

        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            var versions = from version in db.Versions
                           where version.Article.Id == article.Id
                           select version;
            article.CurrentVersion = null;
            List<Version> vs = versions.ToList();
            foreach(Version v in vs)
            {
                v.Article = null;
                if (System.IO.File.Exists(v.ContentPath))
                {
                    System.IO.File.Delete(v.ContentPath);
                }
            }
            db.SaveChanges();

            foreach(Version v in vs)
            {
                db.Versions.Remove(v);
            }

            db.Articles.Remove(article);
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
        public IEnumerable<SelectListItem> GetDomains()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            var domains = from dom in db.Domains select dom;
            foreach (var domain in domains)
            {
                selectList.Add(new SelectListItem
                {
                    Value = domain.Id.ToString(),
                    Text = domain.Title.ToString()
                });
            }

            return selectList;
        }

    }
}
