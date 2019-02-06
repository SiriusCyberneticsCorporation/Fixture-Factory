using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class OtherFixture
	{
		public Guid ID { get; set; }
		public DateTime GameTime { get; set; }
		public Guid FieldID { get; set; }
		public string Grade { get; set; }
		public string HomeTeam { get; set; }
		public string AwayTeam { get; set; }
	}
}
