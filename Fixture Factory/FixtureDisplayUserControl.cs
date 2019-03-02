﻿using System;
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

					foreach (GameTime slot in slotBreakdown[league][team].Keys)
					{
						string slotText = slot.ToString();
						if (!timeSlotBreakdown.Columns.Contains(slotText))
						{
							timeSlotBreakdown.Columns.Add(new DataColumn(slotText));
						}
						teamRow[slotText] = slotBreakdown[league][team][slot];
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
