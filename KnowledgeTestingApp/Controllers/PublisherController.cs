using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using KnowledgeTestingApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Controllers {
  public class PublisherController : Controller {
    public Context db { get; set; }

    public PublisherController(Context context) {
      db = context;
    }

    public IActionResult RemovePublisher(int? id) {
      if (id == null)
        return NotFound();
      Publisher publisher = db.Publishers.Single(p => p.Id == id);
      db.Books.RemoveRange(db.Books.Where(b => b.PublisherId == publisher.Id).ToList());
      db.Publishers.Remove(publisher);
      db.SaveChanges();
      return RedirectToAction("Publishers");
    }

    public IActionResult Publisher(int? id) {
      if (id == null)
        return View();
      return View(db.Publishers.SingleOrDefault(p => p.Id == id));
    }

    [HttpPost]
    public IActionResult Publisher(Publisher publisher) {
      if (ModelState.IsValid) {
        try {
          if (publisher.Id != 0) {
            Publisher temp = db.Publishers.Find(publisher.Id);
            temp.Name = publisher.Name;
            db.SaveChanges();
          } else {
            //add
            db.Publishers.Add(publisher);
            db.SaveChanges();
          }
          return RedirectToAction("Publishers");
        } catch (Exception e) {
          ModelState.AddModelError(string.Empty, e.Message);
        }
      }
      return View(publisher);
    }

    public IActionResult Publishers() {
      return View(db.Publishers.ToList());
    }
  }
}
