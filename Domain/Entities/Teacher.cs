﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Teachers")]
public class Teacher : User
{
	public Teacher() : base(Role.Teacher)
	{
	}
}