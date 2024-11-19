using Domain.Common;

namespace Domain.Entities;

public class StudentSubject : EntityBase
{
    public int Mark {  get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
}
