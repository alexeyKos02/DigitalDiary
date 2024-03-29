﻿using System.Net;
using System.Net.Mail;
using Domain.Entities;
using Infrastructure.Config;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public interface IEmailClient
{
	Task SendUserCreationEmailAsync(User user, string password);
}

public class EmailClientStub : IEmailClient
{
	public Task SendUserCreationEmailAsync(User user, string password)
	{
		Console.WriteLine($"Generated password for {user.Email} is {password}");
		return Task.CompletedTask;
	}
}

public class EmailClient : IEmailClient
{
	private EmailConfig _emailConfig;

	public EmailClient(IOptions<EmailConfig> emailConfig)
	{
		_emailConfig = emailConfig.Value;
	}
	
	public async Task SendUserCreationEmailAsync(User user, string password)
	{
		var subject = "Доступ в Цифровой Дневник";
		var text = "Здравствуйте!\n" +
		           $"Вас зарегистрировали в системе «Цифровой Дневник» в качестве {GetRoleString(user.Role)}.\n" +
		           "Цифровой Дневник доступен по адресу: https://digitaldiary.site\n" +
		           $"Для входа в систему используйте вашу почту и сгенерированный пароль: {password}";
		
		using var smtpClient = new SmtpClient(_emailConfig.SmtpHost)
		{
			Port = 587,
			Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password),
			EnableSsl = true
		};

		await smtpClient.SendMailAsync(
			_emailConfig.Username,
			user.Email,
			subject,
			text);
	}

	private string GetRoleString(Role role) => role switch
	{
		Role.SchoolAdmin => "администратора",
		Role.Student => "ученика",
		Role.Parent => "родителя",
		Role.Teacher => "учителя",
	};
}