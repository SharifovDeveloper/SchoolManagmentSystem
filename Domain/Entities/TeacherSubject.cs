using Domain.Common;

namespace Domain.Entities;

public class TeacherSubject : EntityBase
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }

    public Teacher Teacher { get; set; }
    public Subject Subject { get; set; }
}
