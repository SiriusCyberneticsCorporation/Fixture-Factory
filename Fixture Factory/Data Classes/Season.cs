using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class Season
	{
		public Guid ID { get; set; }
		public string SeasonTitle { get; set; }
		public DateTime SeasonStartDate { get; set; }
		public DateTime SeasonEndDate { get; set; }
		public List<PlayingField> PlayingFields { get; set; }
		public List<NonPlayingDate> NonPlayingDates { get; set; }
		public List<League> Leagues { get; set; }
	}
}
