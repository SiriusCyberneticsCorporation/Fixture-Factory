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

		public void Initialise(SortedDictionary<DateTime, List<Fixture>> fixtures)
		{
			m_fixtureDisplay = new List<FixtureDisplay>();

			DateTime lastDate = DateTime.MinValue;
			DateTime roundDate = DateTime.MinValue;
			List<FixtureDisplay> byes = new List<FixtureDisplay>();
			Dictionary<string, Dictionary<string, Dictionary<string, int>>> fieldBreakdown = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
			Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>> teamBreakdown = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, int>>>();
			Dictionary<string, Dictionary<string, SortedDictionary<GameTime, int>>> slotBreakdown = new Dictionary<string, Dictionary<string, SortedDictionary<GameTime, int>>>();

			foreach (DateTime fixtureTime in fixtures.Keys)
			{
				foreach (Fixture iFixture in fixtures[fixtureTime])
				{
					if (iFixture.Round > 0)
					{
						if (iFixture is FixtureGame)
						{
							string league = iFixture.Grade;
							string homeTeam = ((FixtureGame)iFixture).HomeTeam;
							string awayTeam = ((FixtureGame)iFixture).AwayTeam;
							string umpiringTeam = ((FixtureGame)iFixture).UmpiringTeam;
							string field = ((FixtureGame)iFixture).Field;
							GameTime slot = new GameTime() { DayOfWeek = (int)((FixtureGame)iFixture).GameTime.DayOfWeek, StartTime = ((FixtureGame)iFixture).GameTime };

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

							if (umpiringTeam != null)
							{
								if (!fieldBreakdown[league].ContainsKey(umpiringTeam))
								{
									fieldBreakdown[league].Add(umpiringTeam, new Dictionary<string, int>());
								}
								if (!fieldBreakdown[league][umpiringTeam].ContainsKey("Umpiring"))
								{
									fieldBreakdown[league][umpiringTeam].Add("Umpiring", 0);
								}
								fieldBreakdown[league][umpiringTeam]["Umpiring"]++;
							}

							if (awayTeam != null)
							{
								if (!fieldBreakdown[league].ContainsKey(awayTeam))
								{
									fieldBreakdown[league].Add(awayTeam, new Dictionary<string, int>());
								}
								if (!fieldBreakdown[league][awayTeam].ContainsKey(field))
								{
									fieldBreakdown[league][awayTeam].Add(field, 0);
								}
								fieldBreakdown[league][awayTeam][field]++;
							}

							if(!teamBreakdown.ContainsKey(league))
							{
								teamBreakdown.Add(league, new SortedDictionary<string, SortedDictionary<string, int>>());
							}
							if (!teamBreakdown[league].ContainsKey(homeTeam))
							{
								teamBreakdown[league].Add(homeTeam, new SortedDictionary<string, int>());
							}

							if (awayTeam != null)
							{
								if (!teamBreakdown[league][homeTeam].ContainsKey(awayTeam))
								{
									teamBreakdown[league][homeTeam].Add(awayTeam, 0);
								}
								teamBreakdown[league][homeTeam][awayTeam]++;
							}
							else
							{
								if (!teamBreakdown[league][homeTeam].ContainsKey("Bye"))
								{
									teamBreakdown[league][homeTeam].Add("Bye", 0);
								}
								teamBreakdown[league][homeTeam]["Bye"]++;
							}

							if (!slotBreakdown.ContainsKey(league))
							{
								slotBreakdown.Add(league, new Dictionary<string, SortedDictionary<GameTime, int>>());
							}

							if (!slotBreakdown[league].ContainsKey(homeTeam))
							{
								slotBreakdown[league].Add(homeTeam, new SortedDictionary<GameTime, int>());
							}
							if (!slotBreakdown[league][homeTeam].ContainsKey(slot))
							{
								slotBreakdown[league][homeTeam].Add(slot, 0);
							}
							slotBreakdown[league][homeTeam][slot]++;

							if (awayTeam != null)
							{
								if (!slotBreakdown[league].ContainsKey(awayTeam))
								{
									slotBreakdown[league].Add(awayTeam, new SortedDictionary<GameTime, int>());
								}
								if (!slotBreakdown[league][awayTeam].ContainsKey(slot))
								{
									slotBreakdown[league][awayTeam].Add(slot, 0);
								}
								slotBreakdown[league][awayTeam][slot]++;
							}
						}
						else if (iFixture is FixtureTeamBye)
						{
							string league = iFixture.Grade;
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
					}

					if (iFixture.GameTime.Date != lastDate.Date && lastDate != DateTime.MinValue)
					{
						foreach (FixtureDisplay byeRow in byes)
						{
							m_fixtureDisplay.Add(byeRow);
						}
						byes.Clear();
						m_fixtureDisplay.Add(new FixtureDisplay());
					}
					lastDate = iFixture.GameTime;

					FixtureDisplay row = new FixtureDisplay();

					row.Day = iFixture.GameTime.ToString("ddd");
					row.Date = iFixture.GameTime.ToString("dd-MMM");
					if (iFixture is FixtureGame)
					{
						row.Time = iFixture.GameTime.ToString("hh:mm tt");
						row.Grade = iFixture.Grade;
						row.Round = iFixture.Round > 0 ? iFixture.Round.ToString() : "";
						row.Field = ((FixtureGame)iFixture).Field;
						row.Home = ((FixtureGame)iFixture).HomeTeam;
						row.Away = ((FixtureGame)iFixture).AwayTeam;
						if (((FixtureGame)iFixture).UmpiringTeam != null)
						{
							row.Umpiring = ((FixtureGame)iFixture).UmpiringTeam;
						}
						m_fixtureDisplay.Add(row);
					}
					else if (iFixture is FixtureGeneralBye)
					{
						if (iFixture.Grade != null)
						{
							row.Grade = iFixture.Grade;
						}
						row.Home = "General";
						row.Away = "Bye";
						row.Umpiring = ((FixtureGeneralBye)iFixture).Reason; ;
						byes.Add(row); //m_fixtureDisplay.Add(row);
					}
					else if (iFixture is FixtureTeamBye)
					{
						row.Grade = iFixture.Grade;
						row.Round = iFixture.Round > 0 ? iFixture.Round.ToString() : "";
						row.Home = ((FixtureTeamBye)iFixture).TeamWithBye.TeamName;
						row.Away = "Bye";
						byes.Add(row);
					}
				}				
			}
			foreach (FixtureDisplay byeRow in byes)
			{
				m_fixtureDisplay.Add(byeRow);
			}
			byes.Clear();

			BindingSource fixtureBindingSource = new BindingSource() { DataSource = m_fixtureDisplay };
			FixtureDataGridView.DataSource = fixtureBindingSource;

			foreach (string league in fieldBreakdown.Keys)
			{
				TabPage newTabPage = new TabPage(league);
				FixtureBreakdownUserControl fixtureBreakdownControl = new FixtureBreakdownUserControl();
				fixtureBreakdownControl.Dock = DockStyle.Fill;

				newTabPage.Controls.Add(fixtureBreakdownControl);

				BreakdownTabControl.TabPages.Add(newTabPage);

				fixtureBreakdownControl.Initialise(league, fieldBreakdown[league], teamBreakdown[league], slotBreakdown[league]);
			}
		}
	}
}
