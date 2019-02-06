using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fixture_Factory.Data_Classes;

namespace Fixture_Factory
{
	public partial class FixtureDisplayUserControl : UserControl
	{
		private class FixtureDisplay
		{
			public string Day { get; set; }
			public string Date { get; set; }
			public string Time { get; set; }
			public string Field { get; set; }
			public string Grade { get; set; }
			public string Round { get; set; }
			public string Home { get; set; }
			public string Away { get; set; }
			public string Umpiring { get; set; }
			public string TechBench { get; set; }
		}

		List<FixtureDisplay> m_fixtureDisplay = new List<FixtureDisplay>();

		public FixtureDisplayUserControl()
		{
			InitializeComponent();
		}

		public void Initialise(List<Fixture> fixtures)
		{
			m_fixtureDisplay = new List<FixtureDisplay>();

			DateTime lastDate = DateTime.MinValue;
			Dictionary < string, Dictionary<string, Dictionary<string, int>>> breakdown = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

			foreach (Fixture iFixture in fixtures)
			{
				if (iFixture is FixtureGame)
				{
					string league = iFixture.GameLeague.LeagueName;
					string homeTeam = ((FixtureGame)iFixture).HomeTeam.TeamName;
					string awayTeam = ((FixtureGame)iFixture).AwayTeam.TeamName;
					string field = ((FixtureGame)iFixture).Field.FieldName;

					if (!breakdown.ContainsKey(league))
					{
						breakdown.Add(league, new Dictionary<string, Dictionary<string, int>>());
					}

					if (!breakdown[league].ContainsKey(homeTeam))
					{
						breakdown[league].Add(homeTeam, new Dictionary<string, int>());
					}
					if (!breakdown[league][homeTeam].ContainsKey(field))
					{
						breakdown[league][homeTeam].Add(field, 0);
					}
					breakdown[league][homeTeam][field]++;

					if (!breakdown[league].ContainsKey(awayTeam))
					{
						breakdown[league].Add(awayTeam, new Dictionary<string, int>());
					}
					if (!breakdown[league][awayTeam].ContainsKey(field))
					{
						breakdown[league][awayTeam].Add(field, 0);
					}
					breakdown[league][awayTeam][field]++;
				}
				if (iFixture is FixtureTeamBye)
				{
					string league = iFixture.GameLeague.LeagueName;
					string teamWithBye = ((FixtureTeamBye)iFixture).TeamWithBye.TeamName;

					if (!breakdown.ContainsKey(league))
					{
						breakdown.Add(league, new Dictionary<string, Dictionary<string, int>>());
					}
					if (!breakdown[league].ContainsKey(teamWithBye))
					{
						breakdown[league].Add(teamWithBye, new Dictionary<string, int>());
					}
					if (!breakdown[league][teamWithBye].ContainsKey("Bye"))
					{
						breakdown[league][teamWithBye].Add("Bye", 0);
					}
					breakdown[league][teamWithBye]["Bye"]++;
				}

				if(iFixture.GameTime.Date != lastDate.Date && lastDate != DateTime.MinValue)
				{
					m_fixtureDisplay.Add(new FixtureDisplay());
				}
				lastDate = iFixture.GameTime;

				FixtureDisplay row = new FixtureDisplay();

				row.Day = iFixture.GameTime.ToString("ddd");
				row.Date = iFixture.GameTime.ToString("dd-MMM");
				if (iFixture is FixtureGame)
				{
					row.Time = iFixture.GameTime.ToString("HH:mm");
					row.Grade = iFixture.GameLeague.LeagueName;
					row.Round = iFixture.Round.ToString();
					row.Field = ((FixtureGame)iFixture).Field.FieldName;
					row.Home = ((FixtureGame)iFixture).HomeTeam.TeamName;
					row.Away = ((FixtureGame)iFixture).AwayTeam.TeamName;
				}
				else if (iFixture is FixtureGeneralBye)
				{
					if(iFixture.GameLeague != null)
					{
						row.Grade = iFixture.GameLeague.LeagueName;
					}
					row.Home = "General";
					row.Away = "Bye";
					row.Umpiring = ((FixtureGeneralBye)iFixture).Reason; ;
				}
				else if (iFixture is FixtureTeamBye)
				{
					row.Grade = iFixture.GameLeague.LeagueName;
					row.Round = iFixture.Round.ToString();
					row.Home = ((FixtureTeamBye)iFixture).TeamWithBye.TeamName;
					row.Away = "Bye";
				}
				m_fixtureDisplay.Add(row);
			}

			BindingSource fixtureBindingSource = new BindingSource() { DataSource = m_fixtureDisplay };
			FixtureDataGridView.DataSource = fixtureBindingSource;

			int display = 1;
			foreach (string league in breakdown.Keys)
			{
				DataTable fixtureBreakdown = new DataTable();
				fixtureBreakdown.Columns.Add(new DataColumn(league));

				foreach (string team in breakdown[league].Keys)
				{
					DataRow teamRow = fixtureBreakdown.NewRow();

					teamRow[league] = team;

					foreach (string field in breakdown[league][team].Keys)
					{
						if (!fixtureBreakdown.Columns.Contains(field))
						{
							fixtureBreakdown.Columns.Add(new DataColumn(field));
						}
						teamRow[field] = breakdown[league][team][field];
					}
					fixtureBreakdown.Rows.Add(teamRow);
				}
				if (display == 1)
				{
					BreakdownDataGridView1.DataSource = fixtureBreakdown;
				}
				else if (display == 2)
				{
					BreakdownDataGridView2.DataSource = fixtureBreakdown;
				}
				display++;
			}
		}
	}
}
