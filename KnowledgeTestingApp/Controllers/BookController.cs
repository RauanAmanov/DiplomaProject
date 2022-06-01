using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System;
using System.IO;
using KnowledgeTestingApp.ViewModels;
using KnowledgeTestingApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KnowledgeTestingApp.Controllers {
  public static class FileManager {
    public static string UploadFileToServer(IFormFile file, string folderName, string serverDirPath) {
      string uploadFolderPath = serverDirPath + "/uploads/";
      if (!Directory.Exists(uploadFolderPath))
        Directory.CreateDirectory(uploadFolderPath);
      uploadFolderPath = Path.Combine(uploadFolderPath, folderName);
      if (!Directory.Exists(uploadFolderPath))
        Directory.CreateDirectory(uploadFolderPath);
      string path = string.Format("/uploads/{0}/{1}", folderName, file.FileName);
      using (var fs = new FileStream(Path.Combine(uploadFolderPath, file.FileName), FileMode.Create))
        file.CopyTo(fs);
      return path;
    }

    public static void RemoveFileFromServer(string path) {
      if (System.IO.File.Exists(path))
        System.IO.File.Delete(path);
    }

    public static byte[] FileToByteArray(IFormFile file) {
      byte[] result = null;
      using (BinaryReader br = new BinaryReader(file.OpenReadStream()))
        result = br.ReadBytes((int)file.Length);
      return result;
    }
  }

  public class BookController : Controller {
    Context db;
    IWebHostEnvironment _env;

    public BookController(Context context, IWebHostEnvironment env) {
      db = context;
      _env = env;
    }

    public IActionResult Books() {
      return View(db.Books.Include(b => b.Author).Include(b => b.Publisher).ToList());
    }



    [HttpPost]
    [DisableRequestSizeLimit]
    public IActionResult Book(BookViewModel bvm) {
      try {
        if ((bvm.File == null || bvm.Image == null) && bvm.Id == null)
          throw new Exception("Прикрепите все необходимые файлы");
        //edit
        if (bvm.Id.GetValueOrDefault() != 0) {
          Book book = db.Books.Single(b => b.Id == bvm.Id);
          book.Name = bvm.Name;
          book.AuthorId = bvm.AuthorId;
          book.PublisherId = bvm.PublisherId;
          if (bvm.File != null) {
            System.IO.File.Delete(_env.WebRootPath + book.Path);
            book.Path = FileManager.UploadFileToServer(bvm.File, "books", _env.WebRootPath);
          }
          if (bvm.Image != null)
            book.Image = FileManager.FileToByteArray(bvm.Image);
          db.SaveChanges();
        }//add 
        else {
          Book book = new Book() {
            Name = bvm.Name,
            Path = FileManager.UploadFileToServer(bvm.File, "books", _env.WebRootPath),
            Image = FileManager.FileToByteArray(bvm.Image),
            AuthorId = bvm.AuthorId,
            PublisherId = bvm.PublisherId
          };
          db.Books.Add(book);
          db.SaveChanges();
        }
      } catch (Exception e) {
        ModelState.AddModelError(string.Empty, e.Message);
        SelectList authors = new SelectList(db.Authors, "Id", "Fullname", bvm.AuthorId);
        SelectList publishers = new SelectList(db.Publishers, "Id", "Name", bvm.PublisherId);
        ViewBag.Authors = authors;
        ViewBag.Publishers = publishers;
        return View(new Book {
          Id = bvm.Id.GetValueOrDefault(), Name = bvm.Name,
          AuthorId = bvm.AuthorId, PublisherId = bvm.PublisherId
        });
      }
      return RedirectToAction("Books");
    }

    public IActionResult Book(int? id) {
      SelectList authors = new SelectList(db.Authors, "Id", "Fullname");
      SelectList publishers = new SelectList(db.Publishers, "Id", "Name");
      if (id == null) {
        ViewBag.Authors = authors;
        ViewBag.Publishers = publishers;
        return View();
      }
      Book book = db.Books.Where(b => b.Id == id).SingleOrDefault();
      authors.First(a => a.Value == book.AuthorId.ToString()).Selected = true;
      publishers.First(p => p.Value == book.PublisherId.ToString()).Selected = true;
      ViewBag.Authors = authors;
      ViewBag.Publishers = publishers;
      return View(book);
    }

    public IActionResult RemoveBook(int? id) {
      Book book = db.Books.Single(b => b.Id == id);
      string path = Path.Combine(_env.WebRootPath, new string(book.Path.Skip(1).ToArray()));
      FileManager.RemoveFileFromServer(path);
      db.Books.Remove(book);
      db.SaveChanges();
      return RedirectToAction("Books");
    }

    public IActionResult Index() {
      return View(db.Books.Include(b => b.Author).Include(b => b.Publisher).ToList());
    }

    public IActionResult DisplayBook(int? id) {
      if (id == null)
        return NotFound();
      return View(db.Books.Include(b => b.Publisher).Include(b => b.Author).
        SingleOrDefault(b => b.Id == id));
    }

  }
}
