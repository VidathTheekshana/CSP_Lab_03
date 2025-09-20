using System.ComponentModel.DataAnnotations;

namespace AzureSqlCrudDemo.Models;

public class Student
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }
}
