using Domain.Common;

namespace Domain.Entities;

public class Subject : EntityBase
{
    public string Name { get; set; }
    public int GradeLevel { get; set; }

    public ICollection<StudentSubject> StudentSubjects { get; set; }
    public ICollection<TeacherSubject> TeacherSubjects { get; set; }
}
