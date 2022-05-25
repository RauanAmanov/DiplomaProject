using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {
  public class QuestionnaireAnswer {
    public int Id { get; set; }

    public int? QuestionnaireQuestionId { get; set; }

    public QuestionnaireQuestion QuestionnaireQuestion { get; set; }

    public int AnswerId { get; set; }
    public Answer Answer { get; set; }

  }
}
