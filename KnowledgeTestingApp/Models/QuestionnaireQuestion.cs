using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {
  public class QuestionnaireQuestion {
    public int Id { get; set; }

    public int QuestionnaireId { get; set; }
    public Questionnaire Questionnaire { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; }

    public bool IsMultiSelect { get; set; }
    
    public List<QuestionnaireAnswer> QuestionnaireAnswers { get; set; }
    public List<Response> Responses { get; set; }
  }
}
