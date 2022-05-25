using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {
  public class ResponseSession {
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }

    public int? QuestionnaireId { get; set; }
    public Questionnaire Questionnaire { get; set; }

    public decimal? Result { get; set; }

    [JsonIgnore]
    public virtual List<Response> Responses { get; set; }

  }
}
