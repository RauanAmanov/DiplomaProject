using System;
using System.Collections.Generic;
using KnowledgeTestingApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.ViewModels {
  public class BooksQuestionnairesViewModel {
    public List<Questionnaire> Questionnaires { get; set; }
    public List<Book> Books { get; set; }

  }
}
