using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using KnowledgeTestingApp.Models;
using KnowledgeTestingApp.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KnowledgeTestingApp.Controllers 
{

  public class ResponseController : Controller 
  {
    Context db;

    [HttpGet]
    [RequestFormLimits(ValueCountLimit = int.MaxValue)]
    public int[] JsonSearch(int questionnaireId, int userId, int? fromResult, int? toResult,
      DateTime? fromStartDate, DateTime? toStartDate, DateTime? fromFinishDate, DateTime? toFinishDate,
      string orderBy) 
    {
      var temp = db.ResponseSessions.Include(rs => rs.Questionnaire).
        Include(rs => rs.User).ToList().Select((i, n) => new { ind = n, val = i }).ToList();


      if (questionnaireId != 0)
        temp = temp.Where(t => t.val.QuestionnaireId == questionnaireId).ToList();

      if (userId != 0)
        temp = temp.Where(t => t.val.UserId == userId).ToList();

      if (fromResult != null)
        temp = temp.Where(t => t.val.Result >= fromResult).ToList();

      if (toResult != null)
        temp = temp.Where(t => t.val.Result <= toResult).ToList();

      if (fromStartDate != null)
        temp = temp.Where(t => t.val.StartDate.Value.Date >= fromStartDate.Value.ToUniversalTime().Date).ToList();

      if (toStartDate != null)
        temp = temp.Where(t => t.val.StartDate.Value.Date <= toStartDate.Value.ToUniversalTime().Date).ToList();

      if (fromFinishDate != null)
        temp = temp.Where(t => t.val.FinishDate != null && (t.val.FinishDate.Value.Date >= fromFinishDate.Value.ToUniversalTime().Date)).ToList();

      if (toFinishDate != null)
        temp = temp.Where(t => t.val.FinishDate != null && t.val.FinishDate.Value.Date <= toFinishDate.Value.ToUniversalTime().Date).ToList();

      if (!string.IsNullOrWhiteSpace(orderBy)) 
      {
        if (orderBy == "result")
          temp = temp.OrderBy(t => t.val.Result).ToList();
        else if (orderBy == "questionnaire")
          temp = temp.OrderBy(t => t.val.Questionnaire.Name).ToList();
        else
          temp = temp.OrderBy(t => t.val.User.Username).ToList();
      }

      return temp.Select(t => t.ind).ToArray();
    }

    public IActionResult ResponseSessions() 
    {
      var questionnaires = db.Questionnaires.ToList();
      questionnaires.Add(new Questionnaire() { Id = 0, Name = "Все тестирования" });
      questionnaires = questionnaires.OrderBy(q => q.Id).ToList();
      var users = db.Users.ToList();
      users.Add(new Models.User() { Id = 0, Username = "Все пользователи" });
      users = users.OrderBy(u => u.Id).ToList();
      ViewBag.Questionnaires = new SelectList(questionnaires, "Id", "Name");
      ViewBag.Users = new SelectList(users, "Id", "Username");

      List<ResponseSession> responseSessions = db.ResponseSessions.Include(rs => rs.Questionnaire).
        Include(rs => rs.User).ToList();

      return View(responseSessions);
    }

    public ResponseController(Context context) 
    {
      db = context;
    }

    public IActionResult QuestionnaireQuestion(int? id) 
    {
      if (id == null)
        return NotFound();
      User user = System.Text.Json.JsonSerializer.Deserialize<User>
        (HttpContext.Session.GetString("user"));

      QuestionnaireQuestion questionnaireQuestion = db.QuestionnaireQuestions.
        Include(qq => qq.Questionnaire).Include(qq => qq.Question).ThenInclude(q => q.Answers).
        SingleOrDefault(qq => qq.Id == id);
      ResponseSession responseSession = null;
      List<QuestionnaireQuestion> questionnaireQuestions = db.QuestionnaireQuestions.
        Where(qq => qq.QuestionnaireId == questionnaireQuestion.QuestionnaireId).ToList();

      if (!db.ResponseSessions.Any(rs => rs.FinishDate == null &&
      rs.QuestionnaireId == questionnaireQuestion.QuestionnaireId && rs.UserId == user.Id)) 
      {
        responseSession = new ResponseSession() 
        {
          StartDate = DateTime.UtcNow,
          QuestionnaireId = questionnaireQuestion.QuestionnaireId,
          UserId = user.Id
        };
        db.ResponseSessions.Add(responseSession);
        db.SaveChanges();
      }
      responseSession = db.ResponseSessions.Include(rs => rs.Responses).
        FirstOrDefault(rs => rs.FinishDate == null && rs.QuestionnaireId == questionnaireQuestion.QuestionnaireId &&
        rs.UserId == user.Id);

      ResponseViewModel rvm = new ResponseViewModel() 
      {
        ResponseHelpers = responseSession.Responses.
        Where(r => r.QuestionnaireQuestionId == questionnaireQuestion.Id).
        Select(r => new ResponseHelper() { Response = r }).ToList(),
        PreviousId = questionnaireQuestions.Where(qq => qq.Id < questionnaireQuestion.Id).
        LastOrDefault()?.Id,
        NextId = questionnaireQuestions.Where(qq => qq.Id > questionnaireQuestion.Id).
        FirstOrDefault()?.Id,
        QuestionnaireQuestion = questionnaireQuestion,
        Answers = questionnaireQuestion.Question.Answers,
        QuestionnaireQuestions = questionnaireQuestions,
        Responses = responseSession.Responses,
        StartDate = responseSession.StartDate.GetValueOrDefault(),
        ResponseSessionId = responseSession.Id
      };

      return View(rvm);
    }

    public IActionResult Result(int? id) 
    {
      if (id.GetValueOrDefault() == 0)
        return NotFound();
      
      ResponseSession responseSession = db.ResponseSessions.Include(rs => rs.Responses).
        Include(rs => rs.User).Include(rs => rs.Questionnaire).ThenInclude(q => q.QuestionnaireQuestions).SingleOrDefault(rs => rs.Id == id);
      
      responseSession.Questionnaire.QuestionnaireQuestions.ForEach(qq => 
      {
        qq.Question = db.Questions.Include(q => q.Answers).Single(q => q.Id == qq.QuestionId);
        qq.QuestionnaireAnswers = db.QuestionnaireAnswers.
        Include(qa => qa.Answer).Where(qa => qa.QuestionnaireQuestionId == qq.Id).ToList();
      });

      return View(responseSession);
    }

    public IActionResult FinishQuestionnaire(int? id) 
    {
      if (id.GetValueOrDefault() == 0)
        return NotFound();
      DateTime finishDate = DateTime.UtcNow;
      ResponseSession responseSession = db.ResponseSessions.Include(rs => rs.Responses).SingleOrDefault(rs => rs.Id == id);
      var questionnaire = db.Questionnaires.Include(q => q.QuestionnaireQuestions).
        SingleOrDefault(q => q.Id == responseSession.QuestionnaireId);
      questionnaire.QuestionnaireQuestions.ForEach(qq => 
      {
        qq.QuestionnaireAnswers = db.QuestionnaireAnswers.
        Where(qa => qa.QuestionnaireQuestionId == qq.Id).ToList();
      });
      var responses = responseSession.Responses;
      int correctQuestionsCount = 0;
      foreach (var _questionnaireQuestion in questionnaire.QuestionnaireQuestions) 
      {
        int count = 0;
        _questionnaireQuestion.QuestionnaireAnswers.ForEach(qa => 
        {
          if (responses.Any(r => r.AnswerId == qa.AnswerId &&
          r.QuestionnaireQuestionId == _questionnaireQuestion.Id))
            count++;
        });
        if (_questionnaireQuestion.QuestionnaireAnswers.Count == count &&
          responses.Where(r => r.QuestionnaireQuestionId == _questionnaireQuestion.Id).
          Count() == count)
          correctQuestionsCount++;
      }
      decimal result = correctQuestionsCount * 100.0m / questionnaire.QuestionnaireQuestions.Count;
      responseSession = db.ResponseSessions.Find(responseSession.Id);
      responseSession.FinishDate = finishDate;
      responseSession.Result = result;
      db.SaveChanges();
      return RedirectToAction("Result", new { id = responseSession.Id });
    }

    [HttpPost]
    public IActionResult QuestionnaireQuestion(ResponseViewModel rvm) 
    {
      User user = null;
      if (HttpContext.Session.Keys.Contains("user"))
        user = System.Text.Json.JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));
      if (user == null)
        return RedirectToAction("Logout", "User");

      QuestionnaireQuestion questionnaireQuestion = db.QuestionnaireQuestions.
        Include(qq => qq.Questionnaire).ThenInclude(q => q.QuestionnaireQuestions).
        Include(q => q.QuestionnaireAnswers).Include(qq => qq.Question).ThenInclude(q => q.Answers).
        SingleOrDefault(qq => qq.Id == rvm.QuestionnaireQuestion.Id);

      ResponseSession responseSession = db.ResponseSessions.Include(rs => rs.Responses).
          FirstOrDefault(rs => rs.FinishDate == null && rs.QuestionnaireId == questionnaireQuestion.
          QuestionnaireId && rs.UserId == user.Id);

      if (questionnaireQuestion.IsMultiSelect) 
      {
        foreach (var rh in rvm.ResponseHelpers) 
        {
          //already exists in db:
          if (rh.Response.Id != 0) 
          {
            if (!rh.IsChecked) 
            {
              Response tmp = db.Responses.Find(rh.Response.Id);
              db.Responses.Remove(tmp);
              db.SaveChanges();
            }
          } 
          else
          {
            if (rh.IsChecked) 
            {
              Response response = new Response 
              {
                QuestionnaireQuestionId = questionnaireQuestion.Id,
                ResponseSessionId = responseSession.Id,
                AnswerId = rh.Response.AnswerId
              };
              db.Responses.Add(response);
              db.SaveChanges();
            }
          }
        }
      } 
      else 
      {
        Response response = rvm.ResponseHelpers.Single().Response;
        response.QuestionnaireQuestionId = questionnaireQuestion.Id;
        response.ResponseSessionId = responseSession.Id;

        if (response.Id == 0) 
        {
          var responsesToDelete = db.Responses.Where(r => r.ResponseSessionId == responseSession.Id && r.QuestionnaireQuestionId == questionnaireQuestion.Id).ToList();
          db.Responses.RemoveRange(responsesToDelete);
          db.Responses.Add(response);
          db.SaveChanges();
        } 
        else 
        {
          Response tmp = db.Responses.Find(response.Id);
          tmp.AnswerId = response.AnswerId;
          db.SaveChanges();
        }
      }

      var responses = db.ResponseSessions.Include(rs => rs.Responses).
          SingleOrDefault(rs => rs.Id == responseSession.Id).Responses;
      int questionsAnswered = responses.Select(r => r.QuestionnaireQuestionId).Distinct().Count();
      var questionnaire = db.Questionnaires.Include(q => q.QuestionnaireQuestions).
        ThenInclude(q => q.QuestionnaireAnswers).
        SingleOrDefault(q => q.Id == questionnaireQuestion.QuestionnaireId);
      if (questionsAnswered == questionnaire.QuestionnaireQuestions.Count)
        return RedirectToAction("FinishQuestionnaire", new { id = responseSession.Id });

      return RedirectToAction("QuestionnaireQuestion", new { id = rvm.NextId ?? questionnaireQuestion.Id });
    }
  }

}