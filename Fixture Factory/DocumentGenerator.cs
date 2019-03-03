using Fixture_Factory.Data_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fixture_Factory
{
	public class DocumentGenerator
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
		//private static Excel.Application m_excel = new Excel.Application();

		public DocumentGenerator(string grade, SortedDictionary<DateTime, List<Fixture>> fixtures)
		{
			List<float> columnWidths = new List<float>() { 8, 6, 10, 10, 8, 15, 16, 16, 16, 16 };
			ExcelExporter iExcelExporter = new ExcelExporter(grade + ".xlsx", "Fixtures", grade + " Fixtures", "", DateTime.Now.ToLongDateString(), true, columnWidths);

			iExcelExporter.AddRow();
			iExcelExporter.AddText("", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Day", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Date", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Time", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Field", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Grade", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Home", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Away", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Umpiring", 1, true, false, false, HorizontalAlignment.Center);
			iExcelExporter.AddText("Tech Bench", 1, true, false, false, HorizontalAlignment.Center);

			List<FixtureDisplay> m_fixtureDisplay = new List<FixtureDisplay>();

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

			foreach(FixtureDisplay row in m_fixtureDisplay)
			{
				System.Drawing.Color backgroundColour = System.Drawing.Color.White;
				if (row.Grade != null)
				{
					if (row.Grade.Contains("6-8 Boys"))
					{
						backgroundColour = System.Drawing.Color.LightGreen;
					}
					else if (row.Grade.Contains("6-8 Girls"))
					{
						backgroundColour = System.Drawing.Color.LightYellow;
					}
					if (row.Grade.Contains("9-12 Boys"))
					{
						backgroundColour = System.Drawing.Color.Violet;
					}
					else if (row.Grade.Contains("9-12 Girls"))
					{
						backgroundColour = System.Drawing.Color.Turquoise;
					}
					else if (row.Grade.Contains("Men"))
					{
						backgroundColour = System.Drawing.Color.LightBlue;
					}
					else if (row.Grade.Contains("Women"))
					{
						backgroundColour = System.Drawing.Color.LightPink;
					}
				}

				iExcelExporter.AddRow();
				iExcelExporter.AddText("", 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Day, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Date, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Time, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Field, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Grade, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Home, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Away, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.Umpiring, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
				iExcelExporter.AddText(row.TechBench, 1, false, false, false, HorizontalAlignment.Center, backgroundColour);
			}

			List<float> breakdownColumnWidths = new List<float>() { 20, 12, 12, 12, 12, 12, 12, 12 };
			iExcelExporter.AddWorkSheet("Breakdown", grade + " Breakdown", "", DateTime.Now.ToLongDateString(), true, breakdownColumnWidths);

			iExcelExporter.AddRow();

			foreach (string league in fieldBreakdown.Keys)
			{
				iExcelExporter.AddText(league, 1, true, false, false, HorizontalAlignment.Center);

				int column = 1;
				Dictionary<string, int> fields = new Dictionary<string, int>();
				foreach (string team in fieldBreakdown[league].Keys)
				{
					foreach (string field in fieldBreakdown[league][team].Keys)
					{
						if(!fields.ContainsKey(field))
						{
							fields.Add(field, column);
							column++;
						}
					}
				}

				foreach (string field in fields.Keys)
				{
					iExcelExporter.AddText(field, 1, true, false, false, HorizontalAlignment.Center);
				}

				int row = 0;
				string[,] fieldGrid = new string[fieldBreakdown[league].Keys.Count, fields.Count + 1];

				foreach (string team in fieldBreakdown[league].Keys)
				{
					fieldGrid[row, 0] = team;

					foreach (string field in fieldBreakdown[league][team].Keys)
					{
						fieldGrid[row, fields[field]] = fieldBreakdown[league][team][field].ToString();
					}
					row++;
				}

				for (row = 0; row < fieldBreakdown[league].Keys.Count; row++)
				{
					iExcelExporter.AddRow();
					for (column = 0; column < fields.Count + 1; column++)
					{
						if (column == 0)
						{
							iExcelExporter.AddText(fieldGrid[row, column], 1, true, false, false, HorizontalAlignment.Right);
						}
						else
						{
							iExcelExporter.AddText(fieldGrid[row, column], 1, false, false, false, HorizontalAlignment.Center);
						}
					}
				}

				iExcelExporter.AddRow();
				iExcelExporter.AddRow();
			}

			iExcelExporter.AddRow();

			foreach (string league in fieldBreakdown.Keys)
			{
				iExcelExporter.AddText(league, 1, true, false, false, HorizontalAlignment.Center);

				int column = 1;
				Dictionary<string, int> slots = new Dictionary<string, int>();

				foreach (string team in slotBreakdown[league].Keys)
				{
					foreach (GameTime slot in slotBreakdown[league][team].Keys)
					{
						string slotText = slot.ToString();
						if(!slots.ContainsKey(slotText))
						{
							slots.Add(slotText, column);
							column++;
						}
					}
				}

				foreach (string slot in slots.Keys)
				{
					iExcelExporter.AddText(slot, 1, true, false, false, HorizontalAlignment.Center);
				}

				int row = 0;
				string[,] slotGrid = new string[slotBreakdown[league].Keys.Count, slots.Count + 1];

				foreach (string team in slotBreakdown[league].Keys)
				{
					slotGrid[row, 0] = team;

					foreach (GameTime slot in slotBreakdown[league][team].Keys)
					{
						string slotText = slot.ToString();
						slotGrid[row, slots[slotText]] = slotBreakdown[league][team][slot].ToString();
					}
					row++;
				}

				for (row = 0; row < slotBreakdown[league].Keys.Count; row++)
				{
					iExcelExporter.AddRow();
					for (column = 0; column < slots.Count + 1; column++)
					{
						if (column == 0)
						{
							iExcelExporter.AddText(slotGrid[row, column], 1, true, false, false, HorizontalAlignment.Right);
						}
						else
						{
							iExcelExporter.AddText(slotGrid[row, column], 1, false, false, false, HorizontalAlignment.Center);
						}
					}
				}

				iExcelExporter.AddRow();
				iExcelExporter.AddRow();
			}

			iExcelExporter.Finish();
		}
	}
}
