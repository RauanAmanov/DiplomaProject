using Microsoft.AspNetCore.Mvc;
using System;
using KnowledgeTestingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Controllers {
  public class AuthorController : Controller {
    Context db;

    public AuthorController(Context context) {
      db = context;
    }
    public IActionResult Authors() {
      return View(db.Authors.ToList());
    }

    public IActionResult Author(int? id) {
      if (id == null)
        return View();
      return View(db.Authors.Single(a => a.Id == id));
    }

    [HttpPost]
    public IActionResult Author(Author author) {
      if (ModelState.IsValid) {
        try {
          if (author.Id == 0) {
            db.Authors.Add(author);
            db.SaveChanges();
          } else {
            Author tmp = db.Authors.Find(author.Id);
            tmp.Lastname = author.Lastname;
            tmp.Firstname = author.Firstname;
            tmp.Patronymic = author.Patronymic;
            db.SaveChanges();
          }
          return RedirectToAction("Authors");
        } catch (Exception e) {
          ModelState.AddModelError(string.Empty, e.Message);
        }
      }
      return View();
    }

    public IActionResult RemoveAuthor(int? id) {
      if (id == null)
        return NotFound();
      db.Books.RemoveRange(db.Books.Where(b => b.AuthorId == id).ToList());
      db.Authors.Remove(db.Authors.Find(id));
      return RedirectToAction("Authors");
    }

  }
}