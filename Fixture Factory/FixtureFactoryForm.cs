using Fixture_Factory.Data_Classes;
using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fixture_Factory
{
	public partial class FixtureFactoryForm : Form
	{
		private class WeekDay
		{
			public int ID { get; set; }
			public string Name { get; set; }
		}
		private List<WeekDay> m_weekDays = new List<WeekDay>()
		{
			new WeekDay() { ID = (int)DayOfWeek.Thursday, Name = DayOfWeek.Thursday.ToString() },
			new WeekDay() { ID = (int)DayOfWeek.Friday, Name = DayOfWeek.Friday.ToString() },
			new WeekDay() { ID = (int)DayOfWeek.Saturday, Name = DayOfWeek.Saturday.ToString() },
			new WeekDay() { ID = (int)DayOfWeek.Sunday, Name = DayOfWeek.Sunday.ToString() },
			new WeekDay() { ID = (int)DayOfWeek.Monday, Name = DayOfWeek.Monday.ToString() },
			new WeekDay() { ID = (int)DayOfWeek.Tuesday, Name = DayOfWeek.Tuesday.ToString() },
			new WeekDay() { ID = (int)DayOfWeek.Wednesday, Name = DayOfWeek.Wednesday.ToString() },
		};

		private const string FIXTURE_DATABASE = "Fixtures.db";

		private bool m_loadingData = false;
		private LiteDatabase m_fixturesDatabase = null;
		private LiteCollection<Season> m_seasons = null;
		//private LiteCollection<League> m_leagues = null;
		//private LiteCollection<Team> m_teams = null;
		//private LiteCollection<GameTime> m_gameTimes = null;
		//private LiteCollection<PlayingField> m_playingFields = null;

		Season m_currentSeason = null;
		League m_currentLeague = null;
		Team m_currentTeam = null;
		BindingSource leaguesBindingSource = null;
		BindingSource teamsBindingSource = null;

		public FixtureFactoryForm()
		{
			InitializeComponent();
		}

		private void FixtureFactoryForm_Load(object sender, EventArgs e)
		{
			m_fixturesDatabase = new LiteDatabase(FIXTURE_DATABASE);
			m_seasons = m_fixturesDatabase.GetCollection<Season>();
			//m_leagues = m_fixturesDatabase.GetCollection<League>();
			//m_teams = m_fixturesDatabase.GetCollection<Team>();
			//m_gameTimes = m_fixturesDatabase.GetCollection<GameTime>();
			//m_playingFields = m_fixturesDatabase.GetCollection<PlayingField>();
		}

		private void FixtureFactoryForm_Shown(object sender, EventArgs e)
		{
			m_loadingData = true;

			if (m_seasons != null)
			{
				IEnumerable<Season> seasons = m_seasons.FindAll();

				if (seasons.Count() > 0)
				{
					m_currentSeason = seasons.Last();
				}

				if (m_currentSeason == null)
				{
					m_currentSeason = new Season()
					{
						SeasonStartDate = DateTime.Now,
						SeasonEndDate = DateTime.Now,
						PlayingFields = new List<PlayingField>(),
						NonPlayingDates = new List<NonPlayingDate>(),
						Leagues = new List<League>()
					};
				}
				/*
				foreach(League iLeague in m_currentSeason.Leagues)
				{
					if (iLeague.LeagueName != "Masters" && !iLeague.LeagueName.Contains("A2"))
					{
						if(iLeague.NonPlayingDates == null)
						{
							iLeague.NonPlayingDates = new List<NonPlayingDate>();
						}
						iLeague.NonPlayingDates.AddRange(m_currentSeason.NonPlayingDates.ToArray());
					}
				}
				*/
				SeasonTitleTextBox.DataBindings.Add("Text", m_currentSeason, "SeasonTitle");
				SeasonStartDateTimePicker.DataBindings.Add("Value", m_currentSeason, "SeasonStartDate");
				SeasonEndDateTimePicker.DataBindings.Add("Value", m_currentSeason, "SeasonEndDate");
				//SeasonTitleTextBox.Text = m_currentSeason.SeasonTitle;
				//SeasonStartDateTimePicker.Value = m_currentSeason.SeasonStartDate;
				//SeasonEndDateTimePicker.Value = m_currentSeason.SeasonEndDate;

				BindingSource playingFieldsBindingSource = new BindingSource() { DataSource = m_currentSeason.PlayingFields };
				SeasonPlayingFieldsDataGridView.DataSource = playingFieldsBindingSource;
				SeasonPlayingFieldsDataGridView.ClearSelection();

				BindingSource nonPlayingDatesBindingSource = new BindingSource() { DataSource = m_currentSeason.NonPlayingDates };
				SeasonNonPlayingDatesDataGridView.AutoGenerateColumns = false;
				SeasonNonPlayingDatesDataGridView.DataSource = nonPlayingDatesBindingSource;
				//SeasonNonPlayingDatesDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
				SeasonNonPlayingDatesDataGridView.ClearSelection();

				leaguesBindingSource = new BindingSource() { DataSource = m_currentSeason.Leagues };
				leaguesBindingSource.AddingNew += LeaguesBindingSource_AddingNew;
				LeaguesDataGridView.AutoGenerateColumns = false;
				LeaguesDataGridView.DataSource = leaguesBindingSource;
				LeaguesDataGridView.ClearSelection();

				teamsBindingSource = new BindingSource();
				teamsBindingSource.AddingNew += TeamsBindingSource_AddingNew;
				TeamsDataGridView.AutoGenerateColumns = false;
				TeamsDataGridView.DataSource = teamsBindingSource;
				TeamsDataGridView.ClearSelection();
			}

			m_loadingData = false;
		}

		private void LeaguesBindingSource_AddingNew(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new League()
			{
				ID = Guid.NewGuid(),
				SeasonID = m_currentSeason.ID,
				GameTimes = new List<GameTime>(),
				PlayingFields = new List<Guid>(),
				PairedLeagues = new List<Guid>(),
				Teams = new List<Team>(),
				NonPlayingDates = new List<NonPlayingDate>()
			};
		}

		private void LeagueDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}
			m_loadingData = true;

			if (LeaguesDataGridView.Rows[e.RowIndex] != null && LeaguesDataGridView.Rows[e.RowIndex].Cells["LeagueIDColumn"].Value != null)
			{
				Guid leagueID = (Guid)(LeaguesDataGridView.Rows[e.RowIndex].Cells["LeagueIDColumn"].Value);

				m_currentLeague = null;
				LeagueNameTextBox.DataBindings.Clear();
				LeagueGameTimesDataGridView.DataSource = null;
				LeaguePlayingFieldsCheckedListBox.DataSource = null;
				PairedLeaguesCheckedListBox.DataSource = null;
				TeamsDataGridView.DataSource = null;
				PairedTeamsCheckedListBox.DataSource = null;
				GameDurationTextBox.Text = "";

				if (leagueID != Guid.Empty)
				{
					foreach (League iLeague in m_currentSeason.Leagues)
					{
						/*
						// TEMP
						if (iLeague.PairedLeagues == null)
						{
							iLeague.PairedLeagues = new List<Guid>();
						}
						*/
						if (iLeague.ID == leagueID)
						{
							m_currentLeague = iLeague;
							break;
						}
					}

					if (m_currentLeague == null)
					{
						m_currentLeague = new League()
						{
							ID = leagueID,
							SeasonID = m_currentSeason.ID,
							GameTimes = new List<GameTime>(),
							PlayingFields = new List<Guid>(),
							Teams = new List<Team>()
						};
					}

					LeagueNameTextBox.DataBindings.Add("Text", m_currentLeague, "LeagueName");
					GameDurationTextBox.Text = m_currentLeague.GameDurationMinutes.ToString();

					BindingSource gameTimesBindingSource = new BindingSource() { DataSource = m_currentLeague.GameTimes };
					LeagueGameTimesDataGridView.AutoGenerateColumns = false;
					LeagueGameTimesDataGridView.DataSource = gameTimesBindingSource;
					LeagueGameTimesDataGridView.ClearSelection();

					GameDOWColumn.DataSource = m_weekDays;
					GameDOWColumn.DisplayMember = "Name";
					GameDOWColumn.ValueMember = "ID";

					BindingSource seasonPlayingFieldsBindingSource = new BindingSource() { DataSource = m_currentSeason.PlayingFields };
					LeaguePlayingFieldsCheckedListBox.DataSource = seasonPlayingFieldsBindingSource;
					LeaguePlayingFieldsCheckedListBox.DisplayMember = "FieldName";
					LeaguePlayingFieldsCheckedListBox.ValueMember = "ID";

					for (int i = 0; i < LeaguePlayingFieldsCheckedListBox.Items.Count; i++)
					{
						PlayingField field = (PlayingField)LeaguePlayingFieldsCheckedListBox.Items[i];

						if (m_currentLeague.PlayingFields.Contains(field.ID))
						{
							LeaguePlayingFieldsCheckedListBox.SetItemChecked(i, true);
						}
					}

					BindingSource pairedLeaguesBindingSource = new BindingSource() { DataSource = m_currentSeason.Leagues.Where(l => l.ID != m_currentLeague.ID) };
					PairedLeaguesCheckedListBox.DataSource = pairedLeaguesBindingSource;
					PairedLeaguesCheckedListBox.DisplayMember = "LeagueName";
					PairedLeaguesCheckedListBox.ValueMember = "ID";

					for (int i = 0; i < PairedLeaguesCheckedListBox.Items.Count; i++)
					{
						League iLeague = (League)PairedLeaguesCheckedListBox.Items[i];

						if (m_currentLeague.PairedLeagues.Contains(iLeague.ID))
						{
							PairedLeaguesCheckedListBox.SetItemChecked(i, true);
						}
					}

					teamsBindingSource.DataSource = m_currentLeague.Teams;
					TeamsDataGridView.DataSource = teamsBindingSource;
					TeamsDataGridView.ClearSelection();

					List<Team> pairedTeams = new List<Team>();
					foreach (League iLeague in m_currentSeason.Leagues)
					{
						/*
						// TEMP
						if (iLeague.Teams == null)
						{
							iLeague.Teams = new List<Team>();
						}
						*/
						if (m_currentLeague.PairedLeagues.Contains(iLeague.ID))
						{
							pairedTeams.AddRange(iLeague.Teams);
						}
					}

					//BindingSource pairedTeamsBindingSource = new BindingSource() { DataSource = m_currentLeague.Teams };
					BindingSource pairedTeamsBindingSource = new BindingSource() { DataSource = pairedTeams };
					PairedTeamsCheckedListBox.DataSource = pairedTeamsBindingSource;
					PairedTeamsCheckedListBox.DisplayMember = "TeamName";
					PairedTeamsCheckedListBox.ValueMember = "ID";

					BindingSource nonPlayingDatesBindingSource = new BindingSource() { DataSource = m_currentLeague.NonPlayingDates };
					LeagueNonPlayingDatesDataGridView.AutoGenerateColumns = false;
					LeagueNonPlayingDatesDataGridView.DataSource = nonPlayingDatesBindingSource;
					//LeagueNonPlayingDatesDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
					LeagueNonPlayingDatesDataGridView.ClearSelection();
				}
			}

			m_loadingData = false;
		}

		private void TeamsBindingSource_AddingNew(object sender, AddingNewEventArgs e)
		{
			if (m_currentLeague != null)
			{
				e.NewObject = new Team()
				{
					ID = Guid.NewGuid(),
					LeagueID = m_currentLeague.ID,
					SeasonID = m_currentSeason.ID,
					PlayingDays = new List<int>(),
					PairedTeams = new List<Guid>(),
					NonPlayingDates = new List<NonPlayingDate>()
				};
			}
			else
			{
				e.NewObject = m_currentTeam = new Team()
				{
					ID = Guid.NewGuid(),
					SeasonID = m_currentSeason.ID,
					PlayingDays = new List<int>(),
					PairedTeams = new List<Guid>(),
					NonPlayingDates = new List<NonPlayingDate>()
				};
			}
		}

		private void TeamsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}
			m_loadingData = true;

			if (TeamsDataGridView.Rows[e.RowIndex] != null && TeamsDataGridView.Rows[e.RowIndex].Cells["TeamIDColumn"].Value != null)
			{
				Guid teamID = (Guid)(TeamsDataGridView.Rows[e.RowIndex].Cells["TeamIDColumn"].Value);

				m_currentTeam = null;

				foreach (Team iTeam in m_currentLeague.Teams)
				{
					if (iTeam.ID == teamID)
					{
						m_currentTeam = iTeam;
						break;
					}
				}


				if (m_currentTeam == null)
				{
					m_currentTeam = new Team()
					{
						SeasonID = m_currentSeason.ID,
						PairedTeams = new List<Guid>(),
						PlayingDays = new List<int>()
					};
					if (m_currentLeague != null)
					{
						m_currentTeam.LeagueID = m_currentLeague.ID;
					}
				}

				TeamNameTextBox.DataBindings.Clear();
				TeamNameTextBox.DataBindings.Add("Text", m_currentTeam, "TeamName");

				TeamLeagueTextBox.DataBindings.Clear();
				TeamLeagueTextBox.DataBindings.Add("Text", m_currentLeague, "LeagueName");

				for (int i = 0; i < PairedTeamsCheckedListBox.Items.Count; i++)
				{
					PairedTeamsCheckedListBox.SetItemChecked(i, false);
				}
				for (int i = 0; i < PairedTeamsCheckedListBox.Items.Count; i++)
				{
					Team iTeam = (Team)PairedTeamsCheckedListBox.Items[i];

					if (m_currentTeam.PairedTeams.Contains(iTeam.ID))
					{
						PairedTeamsCheckedListBox.SetItemChecked(i, true);
					}
				}

				BindingSource nonPlayingDatesBindingSource = new BindingSource() { DataSource = m_currentTeam.NonPlayingDates };
				TeamNonPlayingDatesDataGridView.AutoGenerateColumns = false;
				TeamNonPlayingDatesDataGridView.DataSource = nonPlayingDatesBindingSource;
				//TeamNonPlayingDatesDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
				TeamNonPlayingDatesDataGridView.ClearSelection();
			}
			m_loadingData = false;
		}

		private void Save()
		{
			if (m_currentSeason != null)
			{
				if (m_currentSeason.ID == Guid.Empty)
				{
					m_currentSeason.ID = Guid.NewGuid();
					m_seasons.Insert(m_currentSeason);
				}
				else
				{
					m_seasons.Update(m_currentSeason);
				}
			}
		}

		private void SeasonTextBox_Leave(object sender, EventArgs e)
		{
			Save();
		}

		private void SeasonStartDateTimePicker_Leave(object sender, EventArgs e)
		{
			Save();
		}

		private void SeasonEndDateTimePicker_Leave(object sender, EventArgs e)
		{
			Save();
		}

		private void SeasonPlayingFieldsDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}
		}

		private void SeasonPlayingFieldsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			if (m_loadingData)
			{
				return;
			}

			if (SeasonPlayingFieldsDataGridView.Rows[e.RowIndex].Cells["PlayingFieldIDColumn"].Value == null ||
				SeasonPlayingFieldsDataGridView.Rows[e.RowIndex].Cells["PlayingFieldIDColumn"].Value.ToString() == Guid.Empty.ToString())
			{
				SeasonPlayingFieldsDataGridView.Rows[e.RowIndex].Cells["PlayingFieldIDColumn"].Value = Guid.NewGuid();
			}

			Save();
		}

		private void SeasonNonPlayingDatesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			Save();
		}

		private void LeaguesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			if (m_loadingData)
			{
				return;
			}

			if (m_currentLeague == null)
			{
				m_currentLeague = new League()
				{
					ID = Guid.NewGuid(),
					SeasonID = m_currentSeason.ID,
					GameTimes = new List<GameTime>(),
					PlayingFields = new List<Guid>()
				};
			}

			m_currentLeague.LeagueName = LeaguesDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

			Save();
		}

		private void LeagueGameTimesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}
			Save();
		}

		private void GameDurationTextBox_Leave(object sender, EventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}
			if (m_currentLeague != null)
			{
				int minutes = m_currentLeague.GameDurationMinutes;
				if (int.TryParse(GameDurationTextBox.Text, out minutes))
				{
					m_currentLeague.GameDurationMinutes = minutes;
					Save();
				}
			}
		}

		private void LeagueGameTimesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

		private void LeaguePlayingFieldsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

		private void LeaguePlayingFieldsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			m_currentLeague.PlayingFields.Clear();
			foreach (PlayingField field in LeaguePlayingFieldsCheckedListBox.CheckedItems)
			{
				m_currentLeague.PlayingFields.Add(field.ID);
			}

			if (e.NewValue == CheckState.Checked)
			{
				Guid fieldID = ((PlayingField)LeaguePlayingFieldsCheckedListBox.Items[e.Index]).ID;
				if (!m_currentLeague.PlayingFields.Contains(fieldID))
				{
					m_currentLeague.PlayingFields.Add(fieldID);
				}
			}
			Save();
		}

		private void TeamsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			if (m_loadingData)
			{
				return;
			}

			if (m_currentTeam == null)
			{
				m_currentTeam = new Team()
				{
					SeasonID = m_currentSeason.ID,
					PairedTeams = new List<Guid>(),
					PlayingDays = new List<int>()
				};
				if (m_currentLeague != null)
				{
					m_currentTeam.LeagueID = m_currentLeague.ID;
				}
			}

			Save();
		}

		private void PairedLeaguesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			m_currentLeague.PairedLeagues.Clear();
			foreach (League iLeague in PairedLeaguesCheckedListBox.CheckedItems)
			{
				m_currentLeague.PairedLeagues.Add(iLeague.ID);
			}

			if (e.NewValue == CheckState.Checked)
			{
				Guid leagueID = ((League)PairedLeaguesCheckedListBox.Items[e.Index]).ID;
				if (!m_currentLeague.PairedLeagues.Contains(leagueID))
				{
					m_currentLeague.PairedLeagues.Add(leagueID);
				}
			}
			Save();
		}

		private void PairedTeamsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			m_currentTeam.PairedTeams.Clear();
			foreach (Team iTeam in PairedTeamsCheckedListBox.CheckedItems)
			{
				m_currentTeam.PairedTeams.Add(iTeam.ID);
			}

			if (e.NewValue == CheckState.Checked)
			{
				Guid teamID = ((Team)PairedTeamsCheckedListBox.Items[e.Index]).ID;
				if (!m_currentTeam.PairedTeams.Contains(teamID))
				{
					m_currentTeam.PairedTeams.Add(teamID);
				}
			}
			Save();
		}

		private void LeagueNonPlayingDatesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			Save();
		}

		private void TeamNonPlayingDatesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			Save();
		}

		private Dictionary<Guid, List<Fixture>> m_fixtures = new Dictionary<Guid, List<Fixture>>();

		private void GenerateFixturesButton_Click(object sender, EventArgs e)
		{
			foreach (League iLeague in m_currentSeason.Leagues)
			{
				int playingWeeks = 0;
				List<DayOfWeek> playingDays = new List<DayOfWeek>();
				List<DateTime> gameDates = new List<DateTime>();
				Dictionary<DateTime, string> nonPlayingDates = new Dictionary<DateTime, string>();

				foreach (GameTime iGameTime in iLeague.GameTimes)
				{
					if (!playingDays.Contains((DayOfWeek)iGameTime.DayOfWeek))
					{
						playingDays.Add((DayOfWeek)iGameTime.DayOfWeek);
					}
				}

				for (DateTime date = m_currentSeason.SeasonStartDate; date <= m_currentSeason.SeasonEndDate; date = date.AddDays(7))
				{
					bool dateAdded = false;

					// Ensure that the hours, minutes and seconds are zero.
					date = date.AddHours(-date.Hour);
					date = date.AddMinutes(-date.Minute);
					date = date.AddSeconds(-date.Second);
					date = date.AddMilliseconds(-date.Millisecond);
					
					foreach (DayOfWeek day in playingDays)
					{
						string reason = string.Empty;
						DateTime gameDate = date.AddDays((int)day - (int)date.DayOfWeek);

						if (AddDate(gameDate, iLeague, ref reason))
						{
							gameDates.Add(gameDate);
							dateAdded = true;
						}
						else if(!nonPlayingDates.ContainsKey(gameDate))
						{
							nonPlayingDates.Add(gameDate, reason);
						}
					}

					if (dateAdded)
					{
						playingWeeks++;
					}
				}

				int round = 1;
				int rounds = playingWeeks / (iLeague.Teams.Count() - 1);
				int friendlies = playingWeeks % (iLeague.Teams.Count() - 1);
				List<Team> roundTeams = new List<Team>();
				roundTeams.AddRange(iLeague.Teams.ToArray());

				for (DateTime date = m_currentSeason.SeasonStartDate; date <= m_currentSeason.SeasonEndDate; date = date.AddDays(7))
				{
					// Ensure that the hours, minutes and seconds are zero.
					date = date.AddHours(-date.Hour);
					date = date.AddMinutes(-date.Minute);
					date = date.AddSeconds(-date.Second);
					date = date.AddMilliseconds(-date.Millisecond);

					if (!m_fixtures.ContainsKey(iLeague.ID))
					{
						m_fixtures.Add(iLeague.ID, new List<Fixture>());
					}

					if (friendlies > 0)
					{
						// Need to determine what to do about friendlies.
						foreach (DayOfWeek day in playingDays)
						{
							DateTime gameDate = date.AddDays((int)day - (int)date.DayOfWeek);

							if (nonPlayingDates.ContainsKey(gameDate))
							{
								m_fixtures[iLeague.ID].Add(new FixtureGeneralBye() { GameTime = gameDate, Reason = nonPlayingDates[gameDate] });
							}
							else
							{
								m_fixtures[iLeague.ID].Add(new FixtureGeneralBye() { GameTime = gameDate, Reason = "Friendly" });
							}
						}

						friendlies--;
						continue;
					}
					else
					{
						int teamPosition = 0;
						bool roundComplete = false;
						bool byeEncountered = false;

						foreach (GameTime iGameTime in iLeague.GameTimes)
						{
							DateTime gameTime = date.AddDays((int)iGameTime.DayOfWeek - (int)date.DayOfWeek);

							// If the whole day is a non-playing days then log it and skip to the next week.
							if (nonPlayingDates.ContainsKey(gameTime))
							{
								m_fixtures[iLeague.ID].Add(new FixtureGeneralBye() { GameTime = gameTime, Reason = nonPlayingDates[gameTime] });
								byeEncountered = true;
								break;
							}
							else
							{
								gameTime = gameTime.AddHours(iGameTime.StartTime.Hour);
								gameTime = gameTime.AddMinutes(iGameTime.StartTime.Minute);

								if (nonPlayingDates.ContainsKey(gameTime))
								{
									m_fixtures[iLeague.ID].Add(new FixtureGeneralBye() { GameTime = gameTime, Reason = nonPlayingDates[gameTime] });
								}
								else
								{
									List<Guid> availableFields = new List<Guid>();
									foreach (Guid fieldID in iLeague.PlayingFields)
									{
										if (iGameTime.Priority == GetField(fieldID).Priority)
										{
											availableFields.Add(fieldID);
										}
									}

									foreach (Guid fieldID in availableFields)
									{
										Fixture fixture;
										if (teamPosition == (roundTeams.Count - 1))
										{
											fixture = new FixtureTeamBye() { TeamWithBye = roundTeams[teamPosition], Round = round };
											teamPosition++;
										}
										else
										{
											fixture = new FixtureGame()
											{
												GameTime = gameTime,
												Round = round,
												GameLeague = iLeague,
												Field = GetField(fieldID),
												HomeTeam = roundTeams[teamPosition],
												AwayTeam = roundTeams[teamPosition + 1]
											};
											teamPosition += 2;
										}
										m_fixtures[iLeague.ID].Add(fixture);

										if (teamPosition >= roundTeams.Count)
										{
											roundComplete = true;
											break;
										}
									}
								}
								if (roundComplete)
								{
									break;
								}
							}
						}

						if (!byeEncountered)
						{
							if (teamPosition == (roundTeams.Count - 1))
							{
								Fixture fixture = new FixtureTeamBye() { TeamWithBye = roundTeams[teamPosition], Round = round };
								m_fixtures[iLeague.ID].Add(fixture);
							}

							// Shuffle the first team to the end.
							Team firstTeam = roundTeams[0];
							roundTeams.RemoveAt(0);
							roundTeams.Add(firstTeam);

							// Next round...
							round++;
						}
					}
				}
				//m_fixtures

				round = 0;
				bool addDate = false;
				StreamWriter fixtureWriter = new StreamWriter(iLeague.LeagueName + ".csv");
				foreach(Fixture iFixture in m_fixtures[iLeague.ID])
				{
					string line = string.Empty;
					if (iFixture.Round > 0 && iFixture.Round != round)
					{
						round = iFixture.Round;
						addDate = true;
						fixtureWriter.WriteLine();
						fixtureWriter.WriteLine("Round " + round.ToString() + ",Time,Home Team,Away Team,Field");
					}

					if (iFixture is FixtureGame)
					{
						if (addDate)
						{
							line += iFixture.GameTime.ToString("dd-MMM-yy");
						}
						line += ",";
						line += iFixture.GameTime.ToString("HH:mm");
						line += ",";
						line += ((FixtureGame)iFixture).HomeTeam.TeamName;
						line += ",";
						line += ((FixtureGame)iFixture).AwayTeam.TeamName;
						line += ",";
						line += ((FixtureGame)iFixture).Field.FieldName;
					}
					else if (iFixture is FixtureGeneralBye)
					{
						fixtureWriter.WriteLine();
						line += iFixture.GameTime.ToString("dd-MMM-yy");
						line += ",,General Bye - ";
						line += ((FixtureGeneralBye)iFixture).Reason;
					}
					else if (iFixture is FixtureTeamBye)
					{
						line += ",";
						line += "Bye";
						line += ",";
						line += ((FixtureTeamBye)iFixture).TeamWithBye.TeamName;
					}
					fixtureWriter.WriteLine(line);
				}
				fixtureWriter.Close();
			}
		}

		private PlayingField GetField(Guid fieldID)
		{
			foreach(PlayingField field in m_currentSeason.PlayingFields)
			{
				if(field.ID == fieldID)
				{
					return field;
				}
			}
			return null;
		}

		private bool AddDate(DateTime gameDate, League iLeague, ref string reason)
		{
			bool addDate = true;

			foreach (NonPlayingDate skipDate in m_currentSeason.NonPlayingDates)
			{
				if (skipDate.Date.Date == gameDate.Date)
				{
					if (skipDate.Date.Hour == 0 && skipDate.Date.Minute == 0)
					{
						reason = skipDate.Reason;
						addDate = false;
						break;
					}
					else
					{
						foreach (GameTime iGameTime in iLeague.GameTimes)
						{
							if (iGameTime.StartTime.Hour == skipDate.Date.Hour && iGameTime.StartTime.Minute == skipDate.Date.Minute)
							{
								reason = skipDate.Reason;
								addDate = false;
								break;
							}
						}
					}
				}
				if (!addDate)
				{
					break;
				}
			}

			if (addDate)
			{
				// TEMP
				if (iLeague.NonPlayingDates == null)
				{
					iLeague.NonPlayingDates = new List<NonPlayingDate>();
				}

				foreach (NonPlayingDate skipDate in iLeague.NonPlayingDates)
				{
					if (skipDate.Date.Date == gameDate.Date)
					{
						if (skipDate.Date.Hour == 0 && skipDate.Date.Minute == 0)
						{
							reason = skipDate.Reason;
							addDate = false;
							break;
						}
						else
						{
							foreach (GameTime iGameTime in iLeague.GameTimes)
							{
								if (iGameTime.StartTime.Hour == skipDate.Date.Hour && iGameTime.StartTime.Minute == skipDate.Date.Minute)
								{
									reason = skipDate.Reason;
									addDate = false;
									break;
								}
							}
						}
					}
					if (!addDate)
					{
						break;
					}
				}
			}

			return addDate;
		}



	}
}
