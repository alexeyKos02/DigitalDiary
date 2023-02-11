﻿using DigitalDiary.AuthorizationAttributes;
using DigitalDiary.Controllers.Dto.Users;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalDiary.Controllers;

[Route("users")]
[AuthorizeByRole(Role.SchoolAdmin)]
public class UsersController : ControllerBase
{
	private readonly IUserRepository _userRepository;
	private readonly IEmailClient _emailClient;
	private readonly IPasswordManager _passwordManager;
	private readonly ISchoolRepository _schoolRepository;

	public UsersController(
		IUserRepository userRepository,
		IEmailClient emailClient,
		IPasswordManager passwordManager,
		ISchoolRepository schoolRepository)
	{
		_userRepository = userRepository;
		_emailClient = emailClient;
		_passwordManager = passwordManager;
		_schoolRepository = schoolRepository;
	}

	[HttpGet("teachers")]
	public async Task<IActionResult> GetTeachersAsync()
	{
		var schoolId = User.Claims.GetSchoolId();
		var users = (await _userRepository.GetUsersInSchoolByRoleAsync(schoolId, Role.Teacher))
			.MapUsersToDto();

		return Ok(users);
	}

	[HttpPost("teachers")]
	public async Task<IActionResult> AddTeacherAsync([FromBody] TeacherDto teacherDto)
	{
		var teacher = await CreateUserAsync<Teacher>(teacherDto);
		await _userRepository.CreateAsync(teacher);
		return Ok();
	}
	
	[HttpGet("students")]
	public async Task<IActionResult> GetStudentsAsync()
	{
		var schoolId = User.Claims.GetSchoolId();
		var users = (await _userRepository.GetUsersInSchoolByRoleAsync(schoolId, Role.Student))
			.MapUsersToDto();

		return Ok(users);
	}
	
	[HttpPost("students")]
	public async Task<IActionResult> AddStudentAsync([FromBody] StudentDto studentDto)
	{
		var student = await CreateUserAsync<Student>(studentDto);
		await _userRepository.CreateAsync(student);
		return Ok();
	}
	
	[HttpGet("parents")]
	public async Task<IActionResult> GetParentsAsync()
	{
		var schoolId = User.Claims.GetSchoolId();
		var users = (await _userRepository.GetUsersInSchoolByRoleAsync(schoolId, Role.Parent))
			.MapUsersToDto();

		return Ok(users);
	}
	
	[HttpPost("parents")]
	public async Task<IActionResult> AddParentAsync([FromBody] ParentDto parentDto)
	{
		var parent = await CreateUserAsync<Parent>(parentDto);
		await _userRepository.CreateAsync(parent);
		return Ok();
	}
	
	[HttpGet("admins")]
	public async Task<IActionResult> GetSchoolAdminsAsync()
	{
		var schoolId = User.Claims.GetSchoolId();
		var users = (await _userRepository.GetUsersInSchoolByRoleAsync(schoolId, Role.SchoolAdmin))
			.MapUsersToDto();

		return Ok(users);
	}
	
	[HttpPost("admins")]
	public async Task<IActionResult> AddSchoolAdminAsync([FromBody] UserDto userDto)
	{
		var admin = await CreateUserAsync<SchoolAdmin>(userDto);
		await _userRepository.CreateAsync(admin);
		return Ok();
	}

	private async Task<TUser> CreateUserAsync<TUser>(UserDto userDto) where TUser : User, new()
	{
		var password = _passwordManager.GenerateRandomPassword();
		var passwordHash = _passwordManager.GetPasswordHash(password, out var salt);

		var school = await _schoolRepository.GetAsync(userDto.SchoolId);
		var user = new TUser
		{
			FirstName = userDto.FirstName,
			LastName = userDto.LastName,
			Email = userDto.Email,
			School = school,
			PasswordHash = passwordHash,
			PasswordSalt = salt
		};
		
		Console.WriteLine($"Generated password for {user.Email} is {password}");
		await _emailClient.SendUserCreationEmailAsync(user, password);

		return user;
	}
}