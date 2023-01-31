using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Timelogger.Entities;
using System.Collections.Generic;
using System;

namespace Timelogger.Api
{
	public class Startup
	{
		private readonly IWebHostEnvironment _environment;
		public IConfigurationRoot Configuration { get; }

		public Startup(IWebHostEnvironment env)
		{
			_environment = env;

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("e-conomic interview"));
			services.AddLogging(builder =>
			{
				builder.AddConsole();
				builder.AddDebug();
			});

			services.AddMvc(options => options.EnableEndpointRouting = false);

			if (_environment.IsDevelopment())
			{
				services.AddCors();
			}
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseCors(builder => builder
					.AllowAnyMethod()
					.AllowAnyHeader()
					.SetIsOriginAllowed(origin => true)
					.AllowCredentials());
			}

			app.UseMvc();


			var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
			using (var scope = serviceScopeFactory.CreateScope())
			{
				SeedDatabase(scope);
			}
		}

		private static void SeedDatabase(IServiceScope scope)
		{
			var context = scope.ServiceProvider.GetService<ApiContext>();
			var testProject1 = new Project {
				Id = 1,
				Name = "Project A",
				Customer = "MegaCorp",
				Deadline = DateTime.Parse( "1/1/2025" ),
				IsCompleted = false
				};

      var testProject2 = new Project {
        Id = 2,
        Name = "Project B",
        Customer = "MegaCorp",
        Deadline = DateTime.Parse( "1/11/2026" ),
        IsCompleted = true,
        TotalTimeSpentInMinutes = 200};

      var testProject3 = new Project {
				Id = 3,
				Name = "Project C",
				Customer = "Startup",
				IsCompleted = false,
				Deadline = DateTime.Parse( "4/5/2026" ),
			};

			context.Projects.Add(testProject1);
			context.Projects.Add( testProject2 );
			context.Projects.Add( testProject3 );

			var timeRegistration1 = new TimeRegistration {
				Id = 1,
				ProjectId = 2,
				TimeSpentInMinutes = 150,
				RegistrationCreated = DateTime.Now.AddDays(-6),
			};

			var timeRegistration2 = new TimeRegistration {
				Id = 2,
				ProjectId = 2,
				TimeSpentInMinutes = 50,
				RegistrationCreated = DateTime.Now,
			};

			context.TimeRegistrations.Add(timeRegistration1);
			context.TimeRegistrations.Add( timeRegistration2);
			context.SaveChanges();
		}
	}
}