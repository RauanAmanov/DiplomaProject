using KnowledgeTestingApp.Models;
using KnowledgeTestingApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KnowledgeTestingApp.Controllers {
  public class HomeController : Controller {
    Context db;

    public HomeController(Context context) {
      db = context;
    }

    public IActionResult Index() {
      return View(new BooksQuestionnairesViewModel {
        Books = db.Books.ToList(),
        Questionnaires = db.Questionnaires.Include(q => q.QuestionnaireQuestions).ToList()
      });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

  }
}