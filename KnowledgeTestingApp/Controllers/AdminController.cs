using Microsoft.AspNetCore.Mvc;
using System;
using KnowledgeTestingApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using KnowledgeTestingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace KnowledgeTestingApp.Controllers {
  public class AdminController : Controller {
    Context db;

    public AdminController(Context context) {
      db = context;
    }
    
    public IActionResult Index() {
      return View();
    }    
  }
}