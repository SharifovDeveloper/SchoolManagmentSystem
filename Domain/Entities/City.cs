using Domain.Common;

namespace Domain.Entities;

public class City : EntityBase
{
    public string Name { get; set; }

    public ICollection<Student> Students { get; set; }
    public ICollection<Teacher> Teachers { get; set; }
}
