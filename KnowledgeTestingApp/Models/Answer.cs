using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeTestingApp.Models {
  public class Answer {
    public int Id { get; set; }

    [Column("Answer")]
    public string Answer_ { get; set; }
    
    public int QuestionId { get; set; }
    public Question Question { get; set; }

    public List<QuestionnaireAnswer> QuestionnaireAnswers { get; set; }
    public List<Response> Responses { get; set; }
  }
}
