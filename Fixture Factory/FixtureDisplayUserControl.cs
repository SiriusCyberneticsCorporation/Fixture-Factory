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
			Dictionary<string, Dictionary<string, Dictionary<string, int>>> fieldBreakdown = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
			Dictionary<string, Dictionary<string, Dictionary<string, int>>> slotBreakdown = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

			foreach (Fixture iFixture in fixtures)
			{
				if (iFixture is FixtureGame)
				{
					string league = iFixture.GameLeague.LeagueName;
					string homeTeam = ((FixtureGame)iFixture).HomeTeam.TeamName;
					string awayTeam = ((FixtureGame)iFixture).AwayTeam.TeamName;
					string field = ((FixtureGame)iFixture).Field.FieldName;
					string slot = ((FixtureGame)iFixture).GameTime.ToString("ddd HH:mm");

					if (!fieldBreakdown.ContainsKey(league))
					{
						fieldBreakdown.Add(league, new Dictionary<string, Dictionary<string, int>>());
					}

					if (!fieldBreakdown[league].ContainsKey(homeTeam))
					{
						fieldBreakdown[league].Add(homeTeam, new Dictionary<string, int>());
					}
					if (!fieldBreakdown[league][homeTeam].ContainsKey(field))
					{
						fieldBreakdown[league][homeTeam].Add(field, 0);
					}
					fieldBreakdown[league][homeTeam][field]++;

					if (!fieldBreakdown[league].ContainsKey(awayTeam))
					{
						fieldBreakdown[league].Add(awayTeam, new Dictionary<string, int>());
					}
					if (!fieldBreakdown[league][awayTeam].ContainsKey(field))
					{
						fieldBreakdown[league][awayTeam].Add(field, 0);
					}
					fieldBreakdown[league][awayTeam][field]++;


					if (!slotBreakdown.ContainsKey(league))
					{
						slotBreakdown.Add(league, new Dictionary<string, Dictionary<string, int>>());
					}

					if (!slotBreakdown[league].ContainsKey(homeTeam))
					{
						slotBreakdown[league].Add(homeTeam, new Dictionary<string, int>());
					}
					if (!slotBreakdown[league][homeTeam].ContainsKey(slot))
					{
						slotBreakdown[league][homeTeam].Add(slot, 0);
					}
					slotBreakdown[league][homeTeam][slot]++;

					if (!slotBreakdown[league].ContainsKey(awayTeam))
					{
						slotBreakdown[league].Add(awayTeam, new Dictionary<string, int>());
					}
					if (!slotBreakdown[league][awayTeam].ContainsKey(slot))
					{
						slotBreakdown[league][awayTeam].Add(slot, 0);
					}
					slotBreakdown[league][awayTeam][slot]++;
				}
				if (iFixture is FixtureTeamBye)
				{
					string league = iFixture.GameLeague.LeagueName;
					string teamWithBye = ((FixtureTeamBye)iFixture).TeamWithBye.TeamName;

					if (!fieldBreakdown.ContainsKey(league))
					{
						fieldBreakdown.Add(league, new Dictionary<string, Dictionary<string, int>>());
					}
					if (!fieldBreakdown[league].ContainsKey(teamWithBye))
					{
						fieldBreakdown[league].Add(teamWithBye, new Dictionary<string, int>());
					}
					if (!fieldBreakdown[league][teamWithBye].ContainsKey("Bye"))
					{
						fieldBreakdown[league][teamWithBye].Add("Bye", 0);
					}
					fieldBreakdown[league][teamWithBye]["Bye"]++;
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
			foreach (string league in fieldBreakdown.Keys)
			{
				DataTable fixtureBreakdown = new DataTable();
				DataTable timeSlotBreakdown = new DataTable();
				fixtureBreakdown.Columns.Add(new DataColumn(league));
				timeSlotBreakdown.Columns.Add(new DataColumn(league));

				foreach (string team in fieldBreakdown[league].Keys)
				{
					DataRow teamRow = fixtureBreakdown.NewRow();

					teamRow[league] = team;

					foreach (string field in fieldBreakdown[league][team].Keys)
					{
						if (!fixtureBreakdown.Columns.Contains(field))
						{
							fixtureBreakdown.Columns.Add(new DataColumn(field));
						}
						teamRow[field] = fieldBreakdown[league][team][field];
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


				foreach (string team in slotBreakdown[league].Keys)
				{
					DataRow teamRow = timeSlotBreakdown.NewRow();

					teamRow[league] = team;

					foreach (string slot in slotBreakdown[league][team].Keys)
					{
						if (!timeSlotBreakdown.Columns.Contains(slot))
						{
							timeSlotBreakdown.Columns.Add(new DataColumn(slot));
						}
						teamRow[slot] = slotBreakdown[league][team][slot];
					}
					timeSlotBreakdown.Rows.Add(teamRow);
				}
				if (display == 1)
				{
					TimeSlotDataGridView1.DataSource = timeSlotBreakdown;
				}
				else if (display == 2)
				{
					TimeSlotDataGridView2.DataSource = timeSlotBreakdown;
				}
				display++;
			}
		}
	}
}
