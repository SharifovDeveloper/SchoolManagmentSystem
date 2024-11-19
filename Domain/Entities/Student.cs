using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Student : EntityBase
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public int CurrentGradeLevel { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    public ICollection<StudentSubject> StudentSubjects { get; set; }
}
