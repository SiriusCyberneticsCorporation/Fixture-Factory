using Fixture_Factory.Data_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory
{
	public class FixtureDetails
	{
		public League m_league;
		public List<DayOfWeek> playingDays = new List<DayOfWeek>();
		public Dictionary<DateTime, string> nonPlayingDates = new Dictionary<DateTime, string>();
		public int Round = 1;
		public int PreviousRound = 1;
		public int Rounds = 0;
		public int Friendlies = 0;
		public List<Team> RoundTeams = new List<Team>();
		public Dictionary<int, List<KeyValuePair<int, int>>> Fixtures;
	}
}
