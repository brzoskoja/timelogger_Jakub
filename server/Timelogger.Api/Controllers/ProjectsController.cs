using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Timelogger.Entities;

namespace Timelogger.Api.Controllers
{
	[Route("api/[controller]")]
	public class ProjectsController : Controller
	{
		private readonly ApiContext _context;

		public ProjectsController(ApiContext context)
		{
			_context = context;
		}

		[HttpPost]
		[Route("addTimeEntry")]
		public IActionResult AddTimeEntry( [FromBody] TimeEntry timeEntry )
		{
      if ( ValidateTimeEntry( timeEntry ) ) {
				var project = _context.Projects.First( project => project.Name == timeEntry.Project );
				project.TotalTimeSpentInMinutes += int.Parse(timeEntry.Minutes);
				_context.TimeRegistrations.Add( new TimeRegistration {
					TimeSpentInMinutes = int.Parse( timeEntry.Minutes ),
					RegistrationCreated = DateTime.Now,
					ProjectId = project.Id
				} ); ;
				_context.SaveChanges();
				return Ok();
			}
			return BadRequest("Can not add time to project");
		}

		// GET api/projects
		[HttpGet]
		public IActionResult Get()
		{
			return Ok(_context.Projects);
		}

		// GET api/projects/getTimeRegistrations
		[HttpGet]
		[Route( "getTimeRegistrations" )]
		public IActionResult GetTimeRegistrations() {
			return Ok( _context.TimeRegistrations );
		}

		private bool ValidateTimeEntry ( TimeEntry timeEntry ) {
			return timeEntry != null &&
					int.TryParse( timeEntry.Minutes, out int minutes ) &&
					minutes >= 30 &&
					_context.Projects.Any( project => project.Name == timeEntry.Project ) &&
					!_context.Projects.First( project => project.Name == timeEntry.Project ).IsCompleted;
		}
	}
}
