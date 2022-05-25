using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeTestingApp.Models {
  public class Context : DbContext {
    public Context(DbContextOptions<Context> options) : base(options) {
      Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<QuestionnaireQuestion>().
        HasOne(qq => qq.Questionnaire).
        WithMany(q => q.QuestionnaireQuestions).
        HasForeignKey(qq => qq.QuestionnaireId);
      modelBuilder.Entity<QuestionnaireQuestion>().
        HasOne(qq => qq.Question).
        WithMany(q => q.QuestionnaireQuestions).
        HasForeignKey(qq => qq.QuestionId);

      modelBuilder.Entity<QuestionnaireAnswer>().
        HasOne(qa => qa.QuestionnaireQuestion).
        WithMany(qq => qq.QuestionnaireAnswers).
        HasForeignKey(qa => qa.QuestionnaireQuestionId);
      modelBuilder.Entity<QuestionnaireAnswer>().
        HasOne(qa => qa.Answer).
        WithMany(a => a.QuestionnaireAnswers).
        HasForeignKey(qa => qa.AnswerId);

      modelBuilder.Entity<Response>().
        HasOne(r => r.QuestionnaireQuestion).
        WithMany(qq => qq.Responses).
        HasForeignKey(r => r.QuestionnaireQuestionId);
      modelBuilder.Entity<Response>().
        HasOne(r => r.Answer).
        WithMany(a => a.Responses).
        HasForeignKey(r => r.AnswerId);
      modelBuilder.Entity<Response>().
        HasOne(r => r.ResponseSession).
        WithMany(rs => rs.Responses).
        HasForeignKey(r => r.ResponseSessionId);
      modelBuilder.Entity<ResponseSession>().
        HasOne(rs => rs.User).
        WithMany(u => u.ResponseSessions).
        HasForeignKey(rs => rs.UserId);

      modelBuilder.Entity<Answer>().
        HasOne(a => a.Question).
        WithMany(q => q.Answers).
        HasForeignKey(a => a.QuestionId);

      modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

      modelBuilder.Entity<Publisher>().HasData(
        new Publisher { Id = 1, Name = "БАСПРИНТ" },
        new Publisher { Id = 2, Name = "MACMILLAN" },
        new Publisher { Id = 3, Name = "ФОЛИАНТ" }
        );
      modelBuilder.Entity<Author>().HasData(
        new Author { Id = 1, Lastname = "Булгаков", Firstname = "Михаил", Patronymic = "Афанасьевич" },
        new Author { Id = 2, Lastname = "Пушкин", Firstname = "Александр", Patronymic = "Сергеевич" },
        new Author { Id = 3, Lastname = "Лермонтов", Firstname = "Михаил", Patronymic = "Юрьевич" },
        new Author { Id = 5, Lastname = "Чехов", Firstname = "Антон", Patronymic = "Павлович" },
        new Author { Id = 4, Lastname = "Сейфуллин", Firstname = "Сакен", Patronymic = "Сейфоллаевич" }
        );

      modelBuilder.Entity<Questionnaire>().HasData(
        new Questionnaire { Id = 1, Name = "Тест на знание C#" },
        new Questionnaire { Id = 2, Name = "Основы Java" }
        );

      modelBuilder.Entity<Question>().HasData(
       new Question { Id = 1, Question_ = @"Какой результат работы данного кода?
public static void main(String[] args) {       
        String[] array = new String[3];       
        System.out.println(array[2]);   
}" },
        new Question { Id = 2, Question_ = @"Что выполняет данный код?
public static void main(String[] args) {       
        int z = 0;       
        int number = 452;       
        while (number > 0) {           
             z += number % 10;           
             number /= 10;       
        }       
        System.out.println(z);   
}" },
        new Question {
          Id = 3, Question_ = @"Какой результат работы данного кода?
public static void main(String[] args) {
        int sum = 0;
        for (int i = 1; i <= 3; sum += i++) ;
        System.out.println(sum);
}"
        },
        new Question {
          Id = 4, Question_ = @"Какой результат работы данного кода?
public static void main(String[] args) {       
        double a = 5;       
        System.out.println((int) a / 2);   
}"
        },
        new Question {
          Id = 5, Question_ = @"Выберите вариант объявления массива в стиле Java."
        },
        new Question {
          Id = 6, Question_ = @"Какой результат работы данного кода?
        public static void main(String[] args) {       
        int[] array = new int[]{11, 5, -4, 8, 4, 7};       
        for (int i = 0; i < array.length; i += 2) {   System.out.print(array[i]+' ');   }
}"
        },
        new Question {
          Id = 7, Question_ = @"Какой результат работы данного кода?
public static void main(String[] args) { 
        String test = ''Hello''; 
        String test2 = ''Hello'';
        System.out.println(test == test2);
}"
        },
        new Question { Id = 8, Question_ = @"Какие из следующих объявлений переменных верны?" }
        );
      modelBuilder.Entity<Answer>().HasData(
        new Answer { Id = 1, QuestionId = 1, Answer_ = "2" },
        new Answer { Id = 2, QuestionId = 1, Answer_ = "0" },
        new Answer { Id = 3, QuestionId = 1, Answer_ = "null" },
        new Answer { Id = 4, QuestionId = 1, Answer_ = "1" },
        new Answer { Id = 5, QuestionId = 1, Answer_ = "ArrayIndexOutOfBoundsException" },
        new Answer { Id = 6, QuestionId = 2, Answer_ = "выводит на консоль сумму цифр числа" },
        new Answer { Id = 7, QuestionId = 2, Answer_ = "выводит на консоль сумму остатков от деления на 10 всех цифр числа" },
        new Answer { Id = 8, QuestionId = 2, Answer_ = "выводит на консоль число в обратном порядке" },
        new Answer { Id = 9, QuestionId = 2, Answer_ = "выводит на консоль количество цифр числа" },
        new Answer { Id = 10, QuestionId = 3, Answer_ = "3" },
        new Answer { Id = 11, QuestionId = 3, Answer_ = "compile error" },
        new Answer { Id = 12, QuestionId = 3, Answer_ = "6" },
        new Answer { Id = 13, QuestionId = 3, Answer_ = "5" },
        new Answer { Id = 14, QuestionId = 4, Answer_ = "unhandled exception" },
        new Answer { Id = 15, QuestionId = 4, Answer_ = "2.5" },
        new Answer { Id = 16, QuestionId = 4, Answer_ = "2" },
        new Answer { Id = 17, QuestionId = 4, Answer_ = "1" },
        new Answer { Id = 18, QuestionId = 5, Answer_ = "String birthdays [] = new String[10];" },
        new Answer { Id = 19, QuestionId = 5, Answer_ = "String birthdays [] = String[10];" },
        new Answer { Id = 20, QuestionId = 5, Answer_ = "String [] birthdays = new String[10];" },
        new Answer { Id = 21, QuestionId = 5, Answer_ = "String birthdays = String[];" },
        new Answer { Id = 22, QuestionId = 6, Answer_ = "11 -4 4" },
        new Answer { Id = 23, QuestionId = 6, Answer_ = "0 2 4" },
        new Answer { Id = 24, QuestionId = 6, Answer_ = "ArrayIndexOutOfBoundsException" },
        new Answer { Id = 25, QuestionId = 6, Answer_ = "11" },
        new Answer { Id = 26, QuestionId = 7, Answer_ = "false" },
        new Answer { Id = 27, QuestionId = 7, Answer_ = "Hello" },
        new Answer { Id = 28, QuestionId = 7, Answer_ = "true" },
        new Answer { Id = 29, QuestionId = 7, Answer_ = "null" },
        new Answer { Id = 30, QuestionId = 8, Answer_ = "double t; int x;" },
        new Answer { Id = 31, QuestionId = 8, Answer_ = "String book1; book2;" },
        new Answer { Id = 32, QuestionId = 8, Answer_ = "int f,f;" },
        new Answer { Id = 33, QuestionId = 8, Answer_ = "int x; double X;" },
        new Answer { Id = 34, QuestionId = 8, Answer_ = "int 1x;" },
        new Answer { Id = 35, QuestionId = 8, Answer_ = "int x,X; double a; a1;" },
        new Answer { Id = 36, QuestionId = 8, Answer_ = "char symbol1,symbol2,symbol_3;" },
        new Answer { Id = 37, QuestionId = 8, Answer_ = "int x,a,b; double y,z,x;" }
        );

      modelBuilder.Entity<QuestionnaireQuestion>().HasData(
        new QuestionnaireQuestion { Id = 1, QuestionId = 1, QuestionnaireId = 2 },
        new QuestionnaireQuestion { Id = 2, QuestionId = 2, QuestionnaireId = 2 },
        new QuestionnaireQuestion { Id = 3, QuestionId = 3, QuestionnaireId = 2 },
        new QuestionnaireQuestion { Id = 4, QuestionId = 4, QuestionnaireId = 2 },
        new QuestionnaireQuestion { Id = 5, QuestionId = 5, QuestionnaireId = 2 },
        new QuestionnaireQuestion { Id = 6, QuestionId = 6, QuestionnaireId = 2 },
        new QuestionnaireQuestion { Id = 7, QuestionId = 7, QuestionnaireId = 2 },
        new QuestionnaireQuestion { Id = 8, QuestionId = 8, QuestionnaireId = 2, IsMultiSelect = true }
        );

      modelBuilder.Entity<QuestionnaireAnswer>().HasData(
        new QuestionnaireAnswer { Id = 1, QuestionnaireQuestionId = 1, AnswerId = 3 },
        new QuestionnaireAnswer { Id = 2, QuestionnaireQuestionId = 2, AnswerId = 6 },
        new QuestionnaireAnswer { Id = 3, QuestionnaireQuestionId = 3, AnswerId = 12 },
        new QuestionnaireAnswer { Id = 4, QuestionnaireQuestionId = 4, AnswerId = 16 },
        new QuestionnaireAnswer { Id = 5, QuestionnaireQuestionId = 5, AnswerId = 20 },
        new QuestionnaireAnswer { Id = 6, QuestionnaireQuestionId = 6, AnswerId = 22 },
        new QuestionnaireAnswer { Id = 7, QuestionnaireQuestionId = 7, AnswerId = 26 },
        new QuestionnaireAnswer { Id = 8, QuestionnaireQuestionId = 8, AnswerId = 30 },
        new QuestionnaireAnswer { Id = 10, QuestionnaireQuestionId = 8, AnswerId = 33 },
        new QuestionnaireAnswer { Id = 11, QuestionnaireQuestionId = 8, AnswerId = 36 }
        );

      modelBuilder.Entity<User>().HasData(
        new User() {
          Id = 1, Lastname = "Amanov",
          Firstname = "Rauan",
          Username = "rauan1405",
          Password = "qwerty",
          Birthday = DateTime.ParseExact("14.05.2000", "dd.MM.yyyy", null),
          Email = "rauan1405@mail.ru",
          IsAdmin = true
        },
        new User() {
          Id = 2, Lastname = "Saliyev",
          Firstname = "Shokan",
          Username = "shokan",
          Password = "123456",
          Birthday = DateTime.ParseExact("12.04.2000", "dd.MM.yyyy", null),
          Email = "shokan1204@inbox.ru",
          IsAdmin = false
        }
        );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Questionnaire> Questionnaires { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }
    public DbSet<QuestionnaireAnswer> QuestionnaireAnswers { get; set; }
    public DbSet<ResponseSession> ResponseSessions { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }

  }
}
