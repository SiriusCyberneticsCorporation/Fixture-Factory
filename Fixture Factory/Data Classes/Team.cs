using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class Team
	{
		public Guid ID { get; set; }
		public Guid SeasonID { get; set; }
		public Guid LeagueID { get; set; }
		public string TeamName { get; set; }
		public List<int> PlayingDays { get; set; }
		public List<Guid> PairedTeams { get; set; }
		public List<NonPlayingDate> NonPlayingDates { get; set; }
	}
}
