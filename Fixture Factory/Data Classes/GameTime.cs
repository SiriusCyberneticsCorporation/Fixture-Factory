using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class GameTime
	{
		public Guid ID { get; set; }
		public int DayOfWeek { get; set; }
		public DateTime StartTime { get; set; }
		public int Priority { get; set; }

		public override string ToString()
		{
			return ((System.DayOfWeek)DayOfWeek).ToString() + " - " + StartTime.ToString("HH:mm");
		}
	}
}
