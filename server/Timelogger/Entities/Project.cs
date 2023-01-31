using System;
using System.Collections.Generic;

namespace Timelogger.Entities
{
	public class Project
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Customer { get; set; }
		public DateTime Deadline { get; set; }
		public bool IsCompleted { get; set; }
		public int TotalTimeSpentInMinutes { get; set; }
	}
}
