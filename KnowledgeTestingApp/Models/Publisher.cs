using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {
  public class Publisher {
    public int Id { get; set; }

    [Required(ErrorMessage = "Заполните поле Название")]
    public string Name { get; set; }

  }
}
