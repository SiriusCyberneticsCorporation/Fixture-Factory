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
		public int round = 1;
		public int rounds = 0;
		public int friendlies = 0;
		public List<Team> roundTeams = new List<Team>();
	}
}
