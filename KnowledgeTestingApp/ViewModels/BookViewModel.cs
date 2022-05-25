using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KnowledgeTestingApp.ViewModels {
  public class BookViewModel {
    public int? Id { get; set; }

    [Required(ErrorMessage = "Заполните название книги")]
    public string Name { get; set; }

    public int AuthorId { get; set; }
    public int PublisherId { get; set; }

    public IFormFile File { get; set; }
    public IFormFile Image { get; set; }

  }
}
