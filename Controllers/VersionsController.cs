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
    public class VersionsController : Controller
    {
        private App_Context db = new App_Context();
        MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        // GET: Versions
        public ActionResult Index()
        {
            return View(db.Versions.ToList());
        }

        // GET: Versions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Version version = db.Versions.Find(id);
            if (version == null)
            {
                return HttpNotFound();
            }
            if (System.IO.File.Exists(version.ContentPath))
            {
                string content = System.IO.File.ReadAllText(version.ContentPath, System.Text.Encoding.UTF8);
                ViewBag.content = Markdown.ToHtml(content, this.pipeline);
                return View(version);
            }
            return View(version);
        }

        // GET: Versions/Create
        public ActionResult Create()
        {
            Version v = new Version();
            v.AllArticles = GetArticles();
            return View(v);
        }

        // POST: Versions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Version version)
        {
            version.AllArticles = GetArticles();

            if (version.UploadFile != null && version.UploadFile.ContentLength > 0)
            {
                string content = ReadStream(version.UploadFile.InputStream, version.UploadFile.ContentLength);
                version.ContentPath = WriteFile(content);
                ModelState.Remove("ContentPath");
            } else
            {
                version.ContentPath = null;
            }

            if (version.SelectedArticle != 0)
            {
                Article a = db.Articles.Find(version.SelectedArticle);
                version.Article = a;
            }

            if (TryValidateModel(version))
            {
                db.Versions.Add(version);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (version.ContentPath != null)
            {
                System.IO.File.Delete(version.ContentPath);
            }
            return View(version);
        }

        // GET: Versions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Version version = db.Versions.Find(id);
            if (version == null)
            {
                return HttpNotFound();
            }
            return View(version);
        }

        // POST: Versions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Version version = db.Versions.Find(id);
            Article a = version.Article;
            db.Versions.Remove(version);

            var versions = from v in db.Versions
                           where v.Article.ArticleId == a.ArticleId
                           select v;
            if (versions.Count() == 0)
            { 
                TempData["errorMessage"] = "Cannot remove last version. Remove the article instead.";
                return RedirectToAction("Delete", "Articles", new { id = a.ArticleId });
            }
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
        public IEnumerable<SelectListItem> GetArticles()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            var articles = from art in db.Articles select art;
            var arts = articles.ToList();
            foreach (var article in arts)
            {
                Version v = db.Versions.Find(article.CurrentVersionId);
                selectList.Add(new SelectListItem
                {
                    Value = article.ArticleId.ToString(),
                    Text = v.Title.ToString()
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
