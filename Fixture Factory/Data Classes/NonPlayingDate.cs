using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class NonPlayingDate
	{
		public Guid ID { get; set; }
		public DateTime Date { get; set; }
		public string Reason { get; set; }
	}
}
