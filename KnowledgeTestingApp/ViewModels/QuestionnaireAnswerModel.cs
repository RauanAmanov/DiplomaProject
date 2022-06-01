using System;
using KnowledgeTestingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.ViewModels {
  public class QuestionnaireAnswerModel {
    public Answer Answer { get; set; }
    public bool IsCorrect { get; set; }
    public Microsoft.AspNetCore.Http.IFormFile Image { get; set; }

  }
}
