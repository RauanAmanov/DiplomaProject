using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {
  public class Questionnaire {
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public List<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }
  }
}
