using Microsoft.AspNetCore.Mvc;
using System;
using KnowledgeTestingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace KnowledgeTestingApp.Controllers {
  public class UserController : Controller {
    Context db;

    public UserController(Context context) {
      db = context;
    }

    //Авторизация
    public IActionResult Login() {
      if (HttpContext.Session.Keys.Contains("user"))
        return RedirectToAction("Index", "Home");

      return View();
    }

    [HttpPost]
    public IActionResult Login(User user) {
      ModelState.Remove("Lastname");
      ModelState.Remove("Firstname");
      ModelState.Remove("Email");

      if (ModelState.IsValid) {
        if (db.Users.Any(u => u.Username == user.Username && u.Password == user.Password)) {
          user = db.Users.First(u => u.Username == user.Username && u.Password == user.Password);
          var session = HttpContext.Session;
          session.SetString("user", JsonSerializer.Serialize<User>(user));
          return RedirectToAction("Index");
        } else
          ModelState.AddModelError(string.Empty, 
            "Авторизоваться не вышло: возможно были неверно введены логин или пароль");
      }
      return View(user);
    }

    //Регистрация
    public IActionResult Register() {
      return View();
    }

    [HttpPost]
    public IActionResult Register(User newUser) {
      if (ModelState.IsValid) {
        newUser = new User {
          Username = newUser.Username.Trim(),
          Lastname = newUser.Lastname.Trim(),
          Firstname = newUser.Firstname.Trim(),
          Email = newUser.Email.Trim(),
          Password = newUser.Password.Trim(),
          Birthday = newUser.Birthday
        };
        try {
          if (string.IsNullOrWhiteSpace(newUser.Lastname) || string.IsNullOrWhiteSpace(newUser.Firstname)
            || string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.Password) ||
            string.IsNullOrWhiteSpace(newUser.Email))
            throw new Exception("Заполните все поля!");
          if (db.Users.Any(u => u.Username == newUser.Username))
            throw new Exception("Аккаунт с таким никнеймом уже зарегистрирован");
          db.Users.Add(newUser);
          db.SaveChanges();
          ViewBag.Display = "normal";
          return View();
        } catch (Exception e) {
          ModelState.AddModelError(string.Empty, $"Зарегистрироваться не вышло: {e.Message}");
        }
      }
      return View(newUser);
    }

    //Личный кабинет пользователя
    public IActionResult Index(int? id) {
      if (id != null)
        return View(db.Users.Include(u => u.ResponseSessions).ThenInclude(rs => rs.Questionnaire).
          ThenInclude(q => q.QuestionnaireQuestions).SingleOrDefault(u => u.Id == id));

      return RedirectToAction("Index", "Home");
    }

    //Выйти из аккаунта
    public IActionResult Logout() {
      if (HttpContext.Session.Keys.Contains("user"))
        HttpContext.Session.Remove("user");
      return RedirectToAction("Login");
    }




    public IActionResult Users(string message = null, string alertType = null) {
      ViewBag.Message = message;
      ViewBag.AlertType = alertType;
      return View(db.Users.ToList());
    }

    public IActionResult DeleteUser(int id) {
      string msg = null;
      string alertType = null;
      try {
        var user = db.Users.Find(id);
        var responses = db.Responses.Include(r => r.ResponseSession).Where(r => r.ResponseSession.UserId == id).ToList();
        var responseSessions = responses.Select(rs => rs.ResponseSession).ToList();
        db.Responses.RemoveRange(responses);
        db.ResponseSessions.RemoveRange(responseSessions);
        db.Users.Remove(user);
        db.SaveChanges();
        msg = "Пользователь был удалён";
        alertType = "success";
      } catch (Exception e) {
        msg = "Удалить не удалось: " + e.Message;
      }
      return RedirectToAction("Users", new { message = msg, alertType = alertType });
    }

    [HttpPost]
    public IActionResult AddUser(User newUser, int status) {
      string msg = null;
      string alertType = null;
      try {
        if (string.IsNullOrWhiteSpace(newUser.Lastname) || string.IsNullOrWhiteSpace(newUser.Firstname) || 
          string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.Password) || 
          string.IsNullOrWhiteSpace(newUser.Email))
          throw new Exception("Заполните все поля!");
        if (db.Users.Any(u => u.Username == newUser.Username))
          throw new Exception("Аккаунт с таким никнеймом уже зарегистрирован");
        newUser.IsAdmin = Convert.ToBoolean(status);
        db.Users.Add(newUser);
        db.SaveChanges();
        msg = "Пользователь был успешно добавлен";
        alertType = "success";
      } catch (Exception e) {
        msg = "Добавить пользователя не удалось: " + e.Message;
      }
      return RedirectToAction("Users", new { message = msg, alertType = alertType });
    }

    public IActionResult EditUser(int? id) {
      User user = db.Users.Find(id);
      var statuses = Enum.GetValues<Status>().Select(s => new {
        ID = (int)s, Name = s.GetType().
        GetMember(s.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name
      });
      ViewBag.Statuses = new SelectList(statuses, "ID", "Name", Convert.ToInt32(user.IsAdmin));
      return View(user);
    }


    [HttpPost]
    public IActionResult EditUser(User user, int status) {
      string msg = null;
      string alertType = null;
      try {
        if (string.IsNullOrWhiteSpace(user.Lastname) || string.IsNullOrWhiteSpace(user.Firstname) || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.Email))
          throw new Exception("Заполните все поля!");
        if (db.Users.Any(u => u.Username == user.Username && u.Id != user.Id))
          throw new Exception("Аккаунт с таким никнеймом уже зарегистрирован");
        User newUser = db.Users.Find(user.Id);
        newUser.Lastname = user.Lastname;
        newUser.Firstname = user.Firstname;
        newUser.Username = user.Username;
        newUser.Password = user.Password;
        newUser.Email = user.Email;
        newUser.Birthday = user.Birthday;
        newUser.IsAdmin = Convert.ToBoolean(status);
        db.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        db.SaveChanges();
        msg = "Пользователь был успешно изменён";
        alertType = "success";
        return RedirectToAction("Users", new { message = msg, alertType = alertType });
      } catch (Exception e) {
        msg = "Изменить пользователя не удалось: " + e.Message;
        ViewData["Message"] = msg;
        ViewData["AlertType"] = alertType;
        var statuses = Enum.GetValues<Status>().Select(s => new { ID = (int)s, Name = s.GetType().GetMember(s.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name });
        ViewBag.Statuses = new SelectList(statuses, "ID", "Name", Convert.ToInt32(user.IsAdmin));
        return View(user);
      }
    }
  }
}
