using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using KnowledgeTestingApp.ViewModels;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using KnowledgeTestingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

namespace KnowledgeTestingApp.Controllers {
  public class QuestionnaireController : Controller {
    Context db;
    IWebHostEnvironment _env;

    public QuestionnaireController(Context context, IWebHostEnvironment env) {
      db = context;
      _env = env;
    }

    public IActionResult Questionnaire(int? id) {
      if (id != null)
        return View(db.Questionnaires.Single(q => q.Id == id));
      return View();
    }

    [HttpPost]
    public IActionResult Questionnaire(Questionnaire questionnaire, IFormFile image) {
      if (questionnaire.Id == 0) {
        questionnaire.Path = FileManager.UploadFileToServer(image, "questionnaires", _env.WebRootPath);
        db.Questionnaires.Add(questionnaire);
      } else {
        var newQuestionnaire = db.Questionnaires.Find(questionnaire.Id);
        newQuestionnaire.Name = questionnaire.Name;
        if (image != null)
          newQuestionnaire.Path = FileManager.UploadFileToServer(image, "questionnaires", _env.WebRootPath);
        db.Entry(newQuestionnaire).State = EntityState.Modified;
      }
      db.SaveChanges();
      return RedirectToAction("Questionnaires");
    }

    public IActionResult LoadQuestionsFromFile(int questionnaireId) {
      return View(db.Questionnaires.Where(q => q.Id == questionnaireId).Single());
    }

    [HttpPost]
    public IActionResult LoadQuestionsFromFile(int Id, IFormFile file) {
      string path = Path.Combine(_env.WebRootPath, file.FileName);
      using (FileStream fs = new FileStream(path, FileMode.Create))
        file.CopyTo(fs);
      using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted)) {
        try {
          List<QuestionnaireQuestion> questionnaireQuestions = new List<QuestionnaireQuestion>();
          using (var reader = new StreamReader(path)) {
            string line;
            QuestionnaireQuestion questionnaireQuestion = new QuestionnaireQuestion() {
              QuestionnaireId = Id
            };
            List<Answer> answers = new List<Answer>();
            List<int> answerIds = new List<int>();

            Question question = new Question();
            string pattern = "^(" + string.Join(".|", Enumerable.Range(65, 90 - 65 + 1).
              Select(n => (char)n).ToArray()) + ".)";

            while ((line = reader.ReadLine()) != null) {
              if (string.IsNullOrWhiteSpace(line)) {
                db.Questions.Add(question);
                db.SaveChanges();

                questionnaireQuestion.QuestionId = question.Id;
                questionnaireQuestion.IsMultiSelect = answerIds.Count > 1;
                db.QuestionnaireQuestions.Add(questionnaireQuestion);
                db.SaveChanges();

                answers.ForEach(a => a.QuestionId = question.Id);

                for (int i = 0; i < answers.Count; ++i) {
                  db.Answers.Add(answers[i]);
                  db.SaveChanges();
                }

                foreach (var n in answerIds) {
                  db.QuestionnaireAnswers.Add(new QuestionnaireAnswer() {
                    AnswerId = answers[n].Id,
                    QuestionnaireQuestionId = questionnaireQuestion.Id
                  });
                }

                db.SaveChanges();

                questionnaireQuestion = new QuestionnaireQuestion() {
                  QuestionnaireId = Id
                };

                question = new Question();
                answers = new List<Answer>();
                answerIds = new List<int>();
              } else if (line.StartsWith("ANSWER: ")) {
                char answer = line.Substring(8, 1).Single();
                int answerId = ((int)answer) - 65;
                answerIds.Add(answerId);
              } else if (Regex.IsMatch(line, pattern)) {
                string option = line.Substring(3);
                answers.Add(new Answer() { Answer_ = option });
              } else {
                question.Question_ = line;
              }
            }
            transaction.Commit();
            return RedirectToAction("QuestionnaireQuestions", new { id = Id });
          }
        } catch (Exception e) {
          transaction.Rollback();
          ModelState.AddModelError(string.Empty, "Загрузить вопросы не удалось: " + e.Message);
        } finally {
          System.IO.File.Delete(path);
        }
      }
      return View(db.Questionnaires.Where(q => q.Id == Id).Single());
    }

    [HttpPost]
    public IActionResult QuestionnaireQuestion(QuestionnaireQuestion questionnaireQuestion,
      List<QuestionnaireAnswerModel> options, IFormFile image) {
      if (questionnaireQuestion.Id != 0) {
        var oldQuestionnaireQuestion = db.QuestionnaireQuestions.Find(questionnaireQuestion.Id);
        oldQuestionnaireQuestion.IsMultiSelect = questionnaireQuestion.IsMultiSelect;
        if (image != null)
          oldQuestionnaireQuestion.Image = FileManager.FileToByteArray(image);
        var question = db.Questions.Find(oldQuestionnaireQuestion.QuestionId);
        question.Question_ = questionnaireQuestion.Question.Question_;
        List<int> idsToKeep = new List<int>();
        options.ToList().ForEach(o => {
          if (o.Answer.Id != 0) {
            Answer answer;
            answer = db.Answers.Find(o.Answer.Id);
            answer.Answer_ = o.Answer.Answer_;
            if (o.Image != null)
              answer.Image = FileManager.FileToByteArray(o.Image);
            if (!o.IsCorrect)
              db.QuestionnaireAnswers.
              RemoveRange(db.QuestionnaireAnswers.
              Where(qq => qq.AnswerId == answer.Id && qq.QuestionnaireQuestionId == oldQuestionnaireQuestion.Id));
            else {
              if (!db.QuestionnaireAnswers.Any(qa => qa.AnswerId == answer.Id && qa.QuestionnaireQuestionId == oldQuestionnaireQuestion.Id))
                db.QuestionnaireAnswers.Add(new QuestionnaireAnswer { AnswerId = answer.Id, QuestionnaireQuestionId = oldQuestionnaireQuestion.Id });
            }
            db.SaveChanges();
            idsToKeep.Add(answer.Id);
          } else {
            Answer answer = new Answer();
            answer.QuestionId = question.Id;
            answer.Answer_ = o.Answer.Answer_;
            if (o.Image != null)
              answer.Image = FileManager.FileToByteArray(o.Image);
            o.Answer.QuestionId = question.Id;
            db.Answers.Add(answer);
            db.SaveChanges();
            if (o.IsCorrect) {
              db.QuestionnaireAnswers.Add(new QuestionnaireAnswer { AnswerId = answer.Id, QuestionnaireQuestionId = oldQuestionnaireQuestion.Id });
              db.SaveChanges();
            }
            idsToKeep.Add(answer.Id);
          }
        });
        var recordsToDelete = db.Answers.Where(a => a.QuestionId == question.Id && !idsToKeep.Any(n => n == a.Id)).ToList();
        var answersToDelete = db.QuestionnaireAnswers.ToList().Where(qa => recordsToDelete.Any(a => a.Id == qa.AnswerId)).ToList();
        db.QuestionnaireAnswers.RemoveRange(answersToDelete);
        db.Answers.RemoveRange(recordsToDelete);
        db.SaveChanges();
      } else {
        Question question = new Question() { Question_ = questionnaireQuestion.Question.Question_ };
        db.Questions.Add(question);
        db.SaveChanges();
        questionnaireQuestion.QuestionId = question.Id;
        questionnaireQuestion.Question = null;
        if (image != null)
          questionnaireQuestion.Image = FileManager.FileToByteArray(image);
        db.QuestionnaireQuestions.Add(questionnaireQuestion);
        db.SaveChanges();
        options.ToList().ForEach(o => {
          Answer answer = new Answer() { Answer_ = o.Answer.Answer_, QuestionId = question.Id };
          if (o.Image != null)
            answer.Image = FileManager.FileToByteArray(o.Image);
          db.Answers.Add(answer);
          db.SaveChanges();
          if (o.IsCorrect)
            db.QuestionnaireAnswers.Add(new QuestionnaireAnswer() { AnswerId = answer.Id, QuestionnaireQuestionId = questionnaireQuestion.Id });
          db.SaveChanges();
        });
      }
      return RedirectToAction("QuestionnaireQuestions", new { id = questionnaireQuestion.QuestionnaireId });
    }
    public IActionResult RemoveQuestionnaireQuestion(int id) {
      QuestionnaireQuestion questionnaireQuestion = db.QuestionnaireQuestions.Find(id);
      var responses = db.Responses.Where(r => r.QuestionnaireQuestionId == questionnaireQuestion.Id).
        ToList();
      var answers = db.QuestionnaireAnswers.
        Where(qa => qa.QuestionnaireQuestionId == questionnaireQuestion.Id).ToList();
      db.Responses.RemoveRange(responses);
      db.QuestionnaireAnswers.RemoveRange(answers);            
      db.QuestionnaireQuestions.Remove(questionnaireQuestion);
      db.SaveChanges();
      return RedirectToAction("QuestionnaireQuestions",
        new { id = questionnaireQuestion.QuestionnaireId });
    }
    public IActionResult QuestionnaireQuestion(int? id, int? questionnaireId) {
      if (id != null) {
        var questionnaireQuestion = db.QuestionnaireQuestions.Include(qq => qq.Question).
          Where(qq => qq.Id == id).Single();
        var questionnareAnswers = db.QuestionnaireAnswers.
          Where(qa => qa.QuestionnaireQuestionId == questionnaireQuestion.Id).ToList();
        var options = db.Answers.Where(a => a.QuestionId == questionnaireQuestion.QuestionId).
          ToList();

        ViewBag.Options = options;
        return View(questionnaireQuestion);
      }
      return View(new QuestionnaireQuestion() { QuestionnaireId = questionnaireId.Value });
    }

    public IActionResult QuestionnaireQuestions(int id) {
      Questionnaire questionnaire = db.Questionnaires.Include(q => q.QuestionnaireQuestions).ThenInclude(qq => qq.Question).SingleOrDefault(q => q.Id == id);
      return View(questionnaire);
    }



    public IActionResult Questionnaires() {
      return View(db.Questionnaires.ToList());
    }

    public IActionResult RemoveQuestionnaire(int? id) {
      var questionnaire = db.Questionnaires.Find(id);
      var questionnaireQuestions = db.QuestionnaireQuestions.
        Where(qq => qq.QuestionnaireId == questionnaire.Id).ToList();
      var questionnaireAnswers = db.QuestionnaireAnswers.ToList().
        Where(qa => questionnaireQuestions.Any(qq => qq.Id == qa.QuestionnaireQuestionId)).
        ToList();
      var responseSessions = db.ResponseSessions.
        Where(rs => rs.QuestionnaireId == questionnaire.Id);
      var responses = db.Responses.Where(r => responseSessions.
      Any(rs => rs.Id == r.ResponseSessionId));
      db.Responses.RemoveRange(responses);
      db.ResponseSessions.RemoveRange(responseSessions);
      db.QuestionnaireAnswers.RemoveRange(questionnaireAnswers);
      db.QuestionnaireQuestions.RemoveRange(questionnaireQuestions);
      string path = Path.Combine(_env.WebRootPath, new string(questionnaire.Path.Skip(1).ToArray()));
      FileManager.RemoveFileFromServer(path);
      db.Questionnaires.Remove(questionnaire);
      db.SaveChanges();
      return RedirectToAction("Questionnaires");
    }

    public IActionResult Index() {
      return View(db.Questionnaires.Include(q => q.QuestionnaireQuestions).
        ToList());
    }

  }
}