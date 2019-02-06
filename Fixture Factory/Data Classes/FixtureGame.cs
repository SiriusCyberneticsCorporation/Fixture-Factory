using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class FixtureGame : Fixture
	{
		public PlayingField Field { get; set; }
		public Team HomeTeam { get; set; }
		public Team AwayTeam { get; set; }
		public Team UmpiringTeam { get; set; }
		public Team TechBenchTeam { get; set; }
		public bool IsFriendly { get; set; }
	}
}
