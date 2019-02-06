using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class League
	{
		public Guid ID { get; set; }
		public Guid SeasonID { get; set; }
		public string LeagueName { get; set; }
		public int GameDurationMinutes { get; set; }
		public List<GameTime> GameTimes { get; set; }
		public List<Guid> PlayingFields { get; set; }
		public List<Guid> PairedLeagues { get; set; }
		public List<Team> Teams { get; set; }
		public List<NonPlayingDate> NonPlayingDates { get; set; }
		public string Grade { get; set; }
	}
}
