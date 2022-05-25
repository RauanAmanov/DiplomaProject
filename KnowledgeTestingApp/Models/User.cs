using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {

  public enum Status {
    [Display(Name = "Обычный")]
    Common,
    [Display(Name = "Администратор")]
    Admin
  }

  public class User {
    public int Id { get; set; }

    [Required(ErrorMessage = "Заполните поле Логин")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Заполните поле Пароль")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Заполните поле Фамилия")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Заполните поле Имя")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Заполните поле Почта")]
    public string Email { get; set; }

    public DateTime Birthday { get; set; }

    public bool IsAdmin { get; set; }

    [JsonIgnore]
    public List<ResponseSession> ResponseSessions { get; set; }

  }
}
