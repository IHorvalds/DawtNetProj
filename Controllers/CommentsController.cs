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
    public class CommentsController : Controller
    {
        private App_Context db = new App_Context();

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Content")] Comment comment)
        {
            if (Request["articleId"] != null)
            {
                int id = Int16.Parse(Request["articleId"]);
                Article article = db.Articles.Find(id);
                if (article == null)
                {
                    return HttpNotFound();
                }

                if (TryValidateModel(comment))
                {
                    comment.LastEdit = DateTime.Now;
                    comment.article = article;
                    db.Comments.Add(comment);

                    article.Comments.Add(comment);

                    db.SaveChanges();

                    return RedirectToAction("Details", "Articles", new { id = article.ArticleId });
                } else
                {
                    return RedirectToAction("Details", "Articles", new { id = article.ArticleId });
                }

                
            }


            return RedirectToAction("Index", "Articles");
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Content,LastEdit")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            Article a = comment.article;
            a.Comments.Remove(comment);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Articles", new { id = a.ArticleId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
