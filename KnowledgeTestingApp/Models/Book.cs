using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeTestingApp.Models {
  public class Book {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public byte[] Image { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; }

    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; }

  }
}