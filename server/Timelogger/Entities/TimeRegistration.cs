using System;

namespace Timelogger.Entities {
  public class TimeRegistration {
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int TimeSpentInMinutes { get; set; }
    public DateTime RegistrationCreated { get; set; }
  }
}
