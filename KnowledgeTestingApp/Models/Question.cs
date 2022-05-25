using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeTestingApp.Models {
  public class Question {
    public int Id { get; set; }

    [Column("Question")]
    public string Question_ { get; set; }

    public List<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }

    public List<Answer> Answers { get; set; }

  }
}
