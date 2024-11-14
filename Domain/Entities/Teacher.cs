﻿using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Teacher : EntityBase
{
    public string Name { get; set; }
    public int CityId { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }

    public City City { get; set; }
    public ICollection<TeacherSubject> TeacherSubjects { get; set; }
}
