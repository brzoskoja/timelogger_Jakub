using Timelogger.Api.Controllers;
using NUnit.Framework;
using Timelogger.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Timelogger.Api.Tests {
  [NonParallelizable]
  public class ProjectsControllerTests {

    private ApiContext _context;
    [OneTimeSetUp]
    public void Setup() {
      var builder = new DbContextOptionsBuilder<ApiContext>().UseInMemoryDatabase( "e-conomic interview" );
      _context = new ApiContext( builder.Options );
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
        TotalTimeSpentInMinutes = 200
      };

      var testProject3 = new Project {
        Id = 3,
        Name = "Project C",
        Customer = "Startup",
        IsCompleted = false,
        Deadline = DateTime.Parse( "4/5/2026" ),
      };

      _context.Projects.Add( testProject1 );
      _context.Projects.Add( testProject2 );
      _context.Projects.Add( testProject3 );

      var timeRegistration1 = new TimeRegistration {
        Id = 1,
        ProjectId = 2,
        TimeSpentInMinutes = 150,
        RegistrationCreated = DateTime.Now.AddDays( -6 ),
        Comment = "Kick off meeting"
      };

      var timeRegistration2 = new TimeRegistration {
        Id = 2,
        ProjectId = 2,
        TimeSpentInMinutes = 50,
        RegistrationCreated = DateTime.Now,
      };

      _context.TimeRegistrations.Add( timeRegistration1 );
      _context.TimeRegistrations.Add( timeRegistration2 );
      _context.SaveChanges();
    }

    [Test]
    public async Task TryAddValidTimeEntry_EntryCreated() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var timeEntry = new TimeEntry() {
        Comment = "AddValidTimeEntry_EntryCreated",
        Minutes = "60",
        Project = "Project A"
      };
      //Act
      var response = projectController.AddTimeEntry( timeEntry );
      //Assert
      Assert.IsInstanceOf<OkResult>( response );
      var match = await _context.TimeRegistrations.FirstAsync( timeRegistration => timeRegistration.Comment == "AddValidTimeEntry_EntryCreated" );
      Assert.AreEqual( "AddValidTimeEntry_EntryCreated", match.Comment );
    }
    [Test]
    public async Task TryAddInvalidTimeEntry_TimeTooShort_EntryNotCreated() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var timeEntry = new TimeEntry() {
        Comment = "TryAddInvalidTimeEntry_TimeTooShort_EntryNotCreated",
        Minutes = "15",
        Project = "Project A"
      };
      //Act
      var response = projectController.AddTimeEntry( timeEntry );
      //Assert
      Assert.IsInstanceOf<BadRequestObjectResult>( response );
      var match = await _context.TimeRegistrations.FirstOrDefaultAsync( timeRegistration => timeRegistration.Comment == "TryAddInvalidTimeEntry_TimeTooShort_EntryNotCreated" );
      Assert.IsNull( match );
    }
    [Test]
    public async Task TryAddInvalidTimeEntry_NonExistingProject_EntryNotCreated() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var timeEntry = new TimeEntry() {
        Comment = "TryAddInvalidTimeEntry_NonExistingProject_EntryNotCreated",
        Minutes = "75",
        Project = "NonExistingProject"
      };
      //Act
      var response = projectController.AddTimeEntry( timeEntry );
      //Assert
      Assert.IsInstanceOf<BadRequestObjectResult>( response );
      var match = await _context.TimeRegistrations.FirstOrDefaultAsync( timeRegistration => timeRegistration.Comment == "TryAddInvalidTimeEntry_NonExistingProject_EntryNotCreated" );
      Assert.IsNull( match );
    }

    [Test]
    public async Task CloseExistingNotCompletedProject_ProjectClosed() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var testProject4 = new Project {
        Id = 99,
        Name = "Project D",
        Customer =  "MediumCorp",
        IsCompleted = false,
        Deadline = DateTime.Parse( "4/5/2026" ),
      };
      _context.Projects.Add( testProject4 );
      _context.SaveChanges();
      //Act
      var response = projectController.CloseProject( testProject4.Id );
      //Assert
      Assert.IsInstanceOf<OkResult>( response );
      var match = await _context.Projects.FirstOrDefaultAsync( project => project.Id == testProject4.Id );
      Assert.IsNotNull( match );
      Assert.AreEqual( testProject4.Name, match.Name );
      Assert.IsTrue( testProject4.IsCompleted );
    }

    [Test]
    public async Task CloseExistingCompletedProject_BadRequest_ProjectClosed() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var testProject5 = new Project {
        Id = 5,
        Name = "Project E",
        Customer = "MediumCorp",
        IsCompleted = true,
        Deadline = DateTime.Parse( "4/5/2026" ),
      };
      _context.Projects.Add( testProject5 );
      _context.SaveChanges();
      //Act
      var response = projectController.CloseProject( testProject5.Id );
      //Assert
      Assert.IsInstanceOf<BadRequestObjectResult>( response );
      var match = await _context.Projects.FirstOrDefaultAsync( project => project.Id == testProject5.Id );
      Assert.IsNotNull( match );
      Assert.AreEqual( testProject5.Name, match.Name );
      Assert.IsTrue( testProject5.IsCompleted );
    }

    [Test]
    public async Task CloseNonExistingProject_BadRequestd() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      //Act
      var response = projectController.CloseProject( 99999 );
      //Assert
      Assert.IsInstanceOf<BadRequestObjectResult>( response );
      var match = await _context.Projects.FirstOrDefaultAsync( project => project.Id == 99999 );
      Assert.IsNull( match );
    }

    [Test]
    public async Task AddNewProject_ProjectCreated() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var addProjectEntry = new AddProjectEntry() {
        CustomerName = "MegaCorp",
        Deadline = "2025/02/02",
        ProjectName = "Totally new project"
      };
      //Act
      var response = projectController.AddProject( addProjectEntry );
      //Assert
      Assert.IsInstanceOf<OkResult>( response );
      var match = await _context.Projects.FirstOrDefaultAsync( project => project.Name == addProjectEntry.ProjectName );
      Assert.IsNotNull( match );
      Assert.AreEqual( addProjectEntry.CustomerName, match.Customer );
    }

    [Test]
    public async Task AddNewProject_ProjectExistsAlready_ProjectNotOverwritten() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var addProjectEntry = new AddProjectEntry() {
        CustomerName = "Umbrella Corp",
        Deadline = "2025/02/02",
        ProjectName = "Project A"
      };
      //Act
      var response = projectController.AddProject( addProjectEntry );
      //Assert
      Assert.IsInstanceOf<BadRequestObjectResult>( response );
      var match = await _context.Projects.FirstOrDefaultAsync( project => project.Name == addProjectEntry.ProjectName );
      Assert.IsNotNull( match );
      Assert.AreEqual( "MegaCorp", match.Customer );
    }

    [Test]
    public async Task AddNewProject_DeadlineInThePast_ProjectNotCreated() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var addProjectEntry = new AddProjectEntry() {
        CustomerName = "Umbrella Corp",
        Deadline = "2020/02/02",
        ProjectName = "Project G"
      };
      //Act
      var response = projectController.AddProject( addProjectEntry );
      //Assert
      Assert.IsInstanceOf<BadRequestObjectResult>( response );
      var match = await _context.Projects.FirstOrDefaultAsync( project => project.Name == addProjectEntry.ProjectName );
      Assert.IsNull( match );
    }

    [Test]
    public async Task AddNewProject_NoProjectName_ProjectNotCreated() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var addProjectEntry = new AddProjectEntry() {
        CustomerName = "Umbrella Corp",
        Deadline = "2025/02/02",
        ProjectName = ""
      };
      //Act
      var response = projectController.AddProject( addProjectEntry );
      //Assert
      Assert.IsInstanceOf<BadRequestObjectResult>( response );
      var match = await _context.Projects.FirstOrDefaultAsync( project => project.Name == addProjectEntry.ProjectName );
      Assert.IsNull( match );
    }

    [Test]
    public async Task GetProjects_AllProjectsReturned() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var lastProject = await _context.Projects.LastOrDefaultAsync();
      //Act
      var response = (OkObjectResult) projectController.Get(  );
      var payload = (List<Project>) response.Value;
      //Assert
      Assert.AreEqual( _context.Projects.Count(), payload.Count );
      Assert.AreEqual( lastProject.Name, payload.Last().Name );
    }

    [Test]
    public async Task GetTimeRegistrations_AllTimeRegistrationsReturned() {
      //Arrange
      ProjectsController projectController = new ProjectsController( _context );
      var lastTimeRegistration = await _context.TimeRegistrations.LastOrDefaultAsync();
      //Act
      var response = (OkObjectResult) projectController.GetTimeRegistrations();
      var payload = (List<TimeRegistration>) response.Value;
      //Assert
      Assert.AreEqual( _context.TimeRegistrations.Count(), payload.Count );
      Assert.AreEqual( lastTimeRegistration.ProjectId, payload.Last().ProjectId );
    }
  }
 }
