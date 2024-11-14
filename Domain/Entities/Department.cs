using Domain.Common;

namespace Domain.Entities;

public class Department : EntityBase
{
    public string Name { get; set; }

    public ICollection<Student> Students { get; set; }
}
