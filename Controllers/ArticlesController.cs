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
using Markdig;

namespace DawtNetProject.Models
{
    public class ArticlesController : Controller
    {
        private App_Context db = new App_Context();
        MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        // GET: Articles
        public ActionResult Index()
        {
            var articles = from article in db.Articles
                           select article;
            List<ArticleVersionViewModel> avs = new List<ArticleVersionViewModel>();
            var articlesList = articles.ToList();
            foreach(var article in articlesList)
            {
                ArticleVersionViewModel av = new ArticleVersionViewModel();
                Version v = db.Versions.Find(article.CurrentVersionId);
                av.ArticleId = article.ArticleId;
                av.VersionId = article.CurrentVersionId;
                av.Title = v.Title;
                av.Content = System.IO.File.ReadAllText(v.ContentPath, System.Text.Encoding.UTF8);
                av.Domains = article.Domains;
                avs.Add(av);
            }

            return View(avs);
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

            Version v = db.Versions.Find(article.CurrentVersionId);
            ArticleVersionViewModel av = new ArticleVersionViewModel();
            av.Title = v.Title;
            av.ArticleId = article.ArticleId;
            av.VersionId = article.CurrentVersionId;
            av.Comments = article.Comments;
            if (System.IO.File.Exists(v.ContentPath))
            {
                string content = System.IO.File.ReadAllText(v.ContentPath, System.Text.Encoding.UTF8);
                ViewBag.content = Markdown.ToHtml(content, this.pipeline);
                return View(av);
            }
            return View(av);
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            ArticleVersionViewModel avVM = new ArticleVersionViewModel();
            avVM.AllDomains = GetDomains();
            if (!avVM.AllDomains.Any())
            {
                TempData["errorMessage"] = "Create at least one domain first.";
                return RedirectToAction("Create", "Domains");
            }
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

            // domains
            var ds = db.Domains.Where(d => avViewModel.DomainIds.Contains(d.Id));
            if (avViewModel.DomainIds != null && avViewModel.DomainIds.Any())
            {
                if (ds != null && ds.Any())
                {
                    a.Domains = ds.ToList();
                }
                else
                {
                    ViewBag.Message = "Couldn't find selected domains.";
                    return View(avViewModel);
                }
            }

            // write file contents
            if (avViewModel.ContentFile != null && avViewModel.ContentFile.ContentLength > 0)
            {

                string content = ReadStream(avViewModel.ContentFile.InputStream, avViewModel.ContentFile.ContentLength);
                try
                {
                    v.ContentPath = WriteFile(content);
                } catch (Exception e)
                {
                    ViewBag.Message = "Error saving the file.";
                    return View(avViewModel);
                }

            } else if (avViewModel.Content != null && avViewModel.Content.Length > 0)
            {
                try
                {
                    v.ContentPath = WriteFile(avViewModel.Content);
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


            v.Title = avViewModel.Title;
            a.ProtectFromEditing = false;


            if (TryValidateModel(a))
            {
                db.Articles.Add(a);
                db.SaveChanges();

                v.Article = a;
                db.Versions.Add(v);
                db.SaveChanges();

                a.CurrentVersionId = v.Id;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // if we're here, something failed
            System.IO.File.Delete(v.ContentPath); // cleanup unused files

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


            Version v = db.Versions.Find(article.CurrentVersionId);
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
        public ActionResult Edit(int id, ArticleVersionViewModel avViewModel)
        {
            avViewModel.AllDomains = GetDomains();
            bool shouldUpdate = false;

            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            Version v = db.Versions.Find(article.CurrentVersionId);

            // domains
            var ds = db.Domains.Where(d => avViewModel.DomainIds.Contains(d.Id));
            if (avViewModel.DomainIds != null && avViewModel.DomainIds.Any())
            {
                if (ds != null && ds.Any())
                {
                    if (article.Domains != null)
                    {
                        List<Domain> l = ds.Where(p => !article.Domains.Contains(p)).ToList();
                        foreach (var dom in l)
                        {
                            article.Domains.Add(dom);
                            shouldUpdate = true;
                        }
                    } else
                    {
                        article.Domains = ds.ToList();
                    }
                }
                else
                {
                    ViewBag.Message = "Couldn't find selected domains.";
                    return View(avViewModel);
                }
            }

            if (v.Title != avViewModel.Title)
            {
                shouldUpdate = true;
            }

            string newContents = "";
            if (avViewModel.ContentFile != null && avViewModel.ContentFile.ContentLength > 0)
            {
                newContents = ReadStream(avViewModel.ContentFile.InputStream, avViewModel.ContentFile.ContentLength);
            } else if (avViewModel.Content != null && avViewModel.Content.Length > 0)
            {
                newContents = avViewModel.Content;
            }

            if (System.IO.File.Exists(v.ContentPath) && newContents != System.IO.File.ReadAllText(v.ContentPath, System.Text.Encoding.UTF8))
            {
                shouldUpdate = true;
            }
                
            if (shouldUpdate)
            {
                Version newV = new Version();

                newV.Title = avViewModel.Title;
                if (avViewModel.ContentFile != null && avViewModel.ContentFile.ContentLength > 0)
                {

                    try
                    {
                        string content = ReadStream(avViewModel.ContentFile.InputStream, avViewModel.ContentFile.ContentLength);
                        newV.ContentPath = WriteFile(content);
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
                        newV.ContentPath = WriteFile(avViewModel.Content);
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

                if (TryValidateModel(newV))
                {
                    db.Versions.Attach(newV);

                    newV.Article = article;
                    db.Versions.Add(newV);
                    db.SaveChanges();
                    
                    article.CurrentVersionId = newV.Id;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                System.IO.File.Delete(v.ContentPath);
                return View(avViewModel);
            }

            return View(avViewModel);

        }

        // GET: Articles/SetVersion/5/5
        public ActionResult SetVersion(int? id, int? reference)
        {
            Article a = db.Articles.Find(id);
            Version v = db.Versions.Find(reference);

            if (a == null || v == null)
            {
                return HttpNotFound();
            }

            ArticleVersionViewModel av = new ArticleVersionViewModel();
            
            av.Title = v.Title;
            av.ArticleId = a.ArticleId;
            av.VersionId = v.Id;
            ViewBag.currentVersionId = a.CurrentVersionId;

            av.AllVersions = GetVersions(a.ArticleId);
            if (System.IO.File.Exists(v.ContentPath))
            {
                string content = System.IO.File.ReadAllText(v.ContentPath, System.Text.Encoding.UTF8);
                ViewBag.content = Markdown.ToHtml(content, this.pipeline);
                return View(av);
            }
            return View(av);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetVersion(int id, int reference)
        {
            Article a = db.Articles.Find(id);
            Version v = db.Versions.Find(reference);

            if (a == null || v == null)
            {
                return HttpNotFound();
            }

            a.CurrentVersionId = v.Id;

            if (TryValidateModel(a))
            {
                db.SaveChanges();
                return RedirectToAction("Details", new { id = a.ArticleId });
            }

            ArticleVersionViewModel av = new ArticleVersionViewModel();

            av.Title = v.Title;
            av.ArticleId = a.ArticleId;
            av.VersionId = v.Id;
            ViewBag.currentVersionId = a.CurrentVersionId;

            av.AllVersions = GetVersions(a.ArticleId);
            if (System.IO.File.Exists(v.ContentPath))
            {
                string content = System.IO.File.ReadAllText(v.ContentPath, System.Text.Encoding.UTF8);
                ViewBag.content = Markdown.ToHtml(content, this.pipeline);
                return View(av);
            }

            return View(av);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            Version v = db.Versions.Find(article.CurrentVersionId);
            ArticleVersionViewModel av = new ArticleVersionViewModel();
            av.Title = v.Title;
            av.ArticleId = article.ArticleId;
            av.VersionId = article.CurrentVersionId;
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(av);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            var versions = from version in db.Versions
                           where version.Article.ArticleId == article.ArticleId
                           select version;

            List<Version> vs = versions.ToList();
            foreach(Version v in vs)
            {
                if (System.IO.File.Exists(v.ContentPath))
                {
                    System.IO.File.Delete(v.ContentPath);
                }
                db.Versions.Remove(v);
            }

            var comments = from comment in db.Comments
                           where comment.article.ArticleId == article.ArticleId
                           select comment;
            List<Comment> cmmts = comments.ToList();

            foreach(Comment c in cmmts)
            {
                db.Comments.Remove(c);
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

        [NonAction]
        public IEnumerable<SelectListItem> GetVersions(int articleId)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            var versions = from version in db.Versions 
                           where version.Article.ArticleId == articleId
                           select version;
            foreach (var version in versions)
            {
                selectList.Add(new SelectListItem
                {
                    Value = version.Id.ToString(),
                    Text = version.Title.ToString()
                });
            }

            return selectList;
        }

        [NonAction]
        private string WriteFile(string contents)
        {
            try
            {
                string path = Path.Combine(Server.MapPath("~/article_files"),
                                            System.Guid.NewGuid().ToString() + ".md");

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(contents);
                }
                return path;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [NonAction]
        private string ReadStream(Stream inputStream, int contentLength)
        {
            BinaryReader b = new BinaryReader(inputStream);
            byte[] binData = b.ReadBytes(contentLength);

            string content = System.Text.Encoding.UTF8.GetString(binData);
            return content;
        }

    }
}
