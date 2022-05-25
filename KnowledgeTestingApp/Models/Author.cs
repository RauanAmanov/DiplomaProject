using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {
  public class Author {
    public int Id { get; set; }

    [Required(ErrorMessage = "Заполните поле Фамилия")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Заполните поле Имя")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Заполните поле Отчество")]
    public string Patronymic { get; set; }

    [NotMapped]
    public string Fullname { get { return $"{Lastname} {Firstname[0]}. {Patronymic[0]}."; } }
  }
}
