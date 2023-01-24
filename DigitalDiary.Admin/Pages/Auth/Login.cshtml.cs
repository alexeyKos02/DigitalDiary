﻿using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalDiary.Admin.Pages.Auth;

public class LoginModel : PageModel
{
	private readonly IAdminRepository _adminRepository;

	public LoginModel(IAdminRepository adminRepository)
	{
		_adminRepository = adminRepository;
	}
	
	[BindProperty]
	public InputModel Input { get; set; }

	public class InputModel
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Электронная почта")]
		public string Email { get; set; }
		
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }
	}
	
	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");

		if (!ModelState.IsValid)
		{
			return Page();
		}
		var user = await _adminRepository.TryGetByEmailAsync(Input.Email);

		if (user == null || !AuthorizationHelper.VerifyPassword(Input.Password, user.PasswordHash, user.PasswordSalt))
		{
			ModelState.AddModelError(string.Empty, "Пользователя не существует или введен неверный пароль.");
			return Page();
		}

		var claims = new List<Claim> { new(ClaimTypes.Email, Input.Email) };
		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(claimsIdentity);

		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

		return LocalRedirect(returnUrl);

	}
}