using System;
using KnowledgeTestingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.ViewModels {
  public class ResponseHelper {
    public Response Response { get; set; }
    public bool IsChecked { get; set; }

  }
  public class ResponseViewModel {
    public List<ResponseHelper> ResponseHelpers { get; set; }

    public int? PreviousId { get; set; }
    public int? NextId { get; set; }

    public List<Answer> Answers { get; set; }

    public QuestionnaireQuestion QuestionnaireQuestion { get; set; }

    public List<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }
    public List<Response> Responses { get; set; }
    public DateTime StartDate { get; set; }

    public int ResponseSessionId { get; set; }

    public ResponseViewModel() {
      Responses = new List<Response>();
    }
  }
}
