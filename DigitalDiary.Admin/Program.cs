using Core.Interfaces;
using Core.Services;
using Infrastructure.Config;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.Configure<EmailConfig>(config =>
{
	config.Password = "bEnyKG1E5AnHjvdYtqXu";
	// config.Password = Environment.GetEnvironmentVariable("DIGITAL_DIARY_Email:Password");
	config.Username = builder.Configuration.GetValue<string>("Email:Username");
	config.SmtpHost = builder.Configuration.GetValue<string>("Email:SmtpHost");
});

services.AddRazorPages();
services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Default");
services.AddDbContext(connectionString);
services.AddRepositories();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/auth/login";
	});

services.AddScoped<IPasswordManager, PasswordManager>();
services.AddScoped<IEmailClient, EmailClient>();
services.AddScoped<ISchoolCreateRequestService, SchoolCreateRequestService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();