using Domain.Common;

namespace Domain.Entities;

public class StudentSubject : EntityBase
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int Mark {  get; set; }
    public Student Student { get; set; }
    public Subject Subject { get; set; }
}
