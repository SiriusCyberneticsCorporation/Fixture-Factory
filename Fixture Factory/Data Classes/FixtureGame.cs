using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class FixtureGame : Fixture
	{
		public string Field { get; set; }
		public string HomeTeam { get; set; }
		public string AwayTeam { get; set; }
		public string UmpiringTeam { get; set; }
		public string TechBenchTeam { get; set; }
		public bool IsFriendly { get; set; }
	}
}
