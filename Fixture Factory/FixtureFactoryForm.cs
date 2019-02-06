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
		BindingSource m_leaguesBindingSource = null;
		BindingSource m_teamsBindingSource = null;
		BindingSource m_gameTimesBindingSource = null;
		BindingSource m_otherFixturesBindingSource = null;

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
					foreach (Season iSeason in seasons)
					{
						ToolStripMenuItem seasonItem = new ToolStripMenuItem(iSeason.SeasonTitle);
						seasonItem.Tag = iSeason;
						seasonItem.Click += SeasonItem_Click;
						seasonToolStripMenuItem.DropDownItems.Add(seasonItem);
					}

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
						Leagues = new List<League>(),
						OtherFixtures = new List<OtherFixture>(),
						Grades = new List<StringValue>() { new StringValue("Year 3-5"), new StringValue("Year 6-8"), new StringValue("Year 9-12"), new StringValue("A2"), new StringValue("Masters") }
					};
				}

				DisplayCurrentSeason();
			}

			m_loadingData = false;
		}

		private void newSeasonToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Season newSeason = new Season()
			{
				ID = Guid.NewGuid(),
				SeasonStartDate = DateTime.Now,
				SeasonEndDate = DateTime.Now,
				PlayingFields = new List<PlayingField>(),
				NonPlayingDates = new List<NonPlayingDate>(),
				Leagues = new List<League>(),
				OtherFixtures = new List<OtherFixture>(),
				Grades = new List<StringValue>() { new StringValue("Year 3-5"), new StringValue("Year 6-8"), new StringValue("Year 9-12"), new StringValue("A2"), new StringValue("Masters") }
			};

			newSeason.Grades = m_currentSeason.Grades;
			newSeason.Leagues = m_currentSeason.Leagues;
			newSeason.NonPlayingDates = m_currentSeason.NonPlayingDates;
			newSeason.OtherFixtures = m_currentSeason.OtherFixtures;
			newSeason.PlayingFields = m_currentSeason.PlayingFields;
			m_currentSeason = newSeason;

			m_seasons.Insert(m_currentSeason);

			DisplayCurrentSeason();
		}

		private void DisplayCurrentSeason()
		{
			if (m_currentSeason != null)
			{
				m_loadingData = true;

				if (m_currentSeason.OtherFixtures == null)
				{
					m_currentSeason.OtherFixtures = new List<OtherFixture>();
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

				SeasonTitleTextBox.DataBindings.Clear();
				SeasonTitleTextBox.DataBindings.Add("Text", m_currentSeason, "SeasonTitle");
				SeasonStartDateTimePicker.DataBindings.Clear();
				SeasonStartDateTimePicker.DataBindings.Add("Value", m_currentSeason, "SeasonStartDate");
				SeasonEndDateTimePicker.DataBindings.Clear();
				SeasonEndDateTimePicker.DataBindings.Add("Value", m_currentSeason, "SeasonEndDate");


				//m_currentSeason.Grades = new List<StringValue>() { new StringValue("Year 3-5"), new StringValue("Year 6-8"), new StringValue("Year 9-12"), new StringValue("A2"), new StringValue("Masters") };
				BindingSource gradesBindingSource = new BindingSource() { DataSource = m_currentSeason.Grades };
				SeasonGradesDataGridView.DataSource = gradesBindingSource;
				SeasonGradesDataGridView.ClearSelection();

				BindingSource playingFieldsBindingSource = new BindingSource() { DataSource = m_currentSeason.PlayingFields };
				SeasonPlayingFieldsDataGridView.DataSource = playingFieldsBindingSource;
				SeasonPlayingFieldsDataGridView.ClearSelection();

				BindingSource nonPlayingDatesBindingSource = new BindingSource() { DataSource = m_currentSeason.NonPlayingDates };
				SeasonNonPlayingDatesDataGridView.AutoGenerateColumns = false;
				SeasonNonPlayingDatesDataGridView.DataSource = nonPlayingDatesBindingSource;
				//SeasonNonPlayingDatesDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
				SeasonNonPlayingDatesDataGridView.ClearSelection();


				//BindingSource seasonPlayingFieldsBindingSource = new BindingSource() { DataSource = m_currentSeason.PlayingFields };
				OtherFixtureFieldColumn.DataSource = m_currentSeason.PlayingFields; //seasonPlayingFieldsBindingSource
				OtherFixtureFieldColumn.DisplayMember = "FieldName";
				OtherFixtureFieldColumn.ValueMember = "ID";

				m_otherFixturesBindingSource = new BindingSource() { DataSource = m_currentSeason.OtherFixtures };
				m_otherFixturesBindingSource.AddingNew += OtherFixturesBindingSource_AddingNew;
				OtherFixturesDataGridView.DataSource = m_otherFixturesBindingSource;
				OtherFixturesDataGridView.ClearSelection();

				m_leaguesBindingSource = new BindingSource() { DataSource = m_currentSeason.Leagues };
				m_leaguesBindingSource.AddingNew += LeaguesBindingSource_AddingNew;
				LeaguesDataGridView.AutoGenerateColumns = false;
				LeaguesDataGridView.DataSource = m_leaguesBindingSource;
				LeaguesDataGridView.ClearSelection();

				m_teamsBindingSource = new BindingSource();
				m_teamsBindingSource.AddingNew += TeamsBindingSource_AddingNew;
				TeamsDataGridView.AutoGenerateColumns = false;
				TeamsDataGridView.DataSource = m_teamsBindingSource;
				TeamsDataGridView.ClearSelection();

				m_gameTimesBindingSource = new BindingSource();
				m_gameTimesBindingSource.AddingNew += GameTimesBindingSource_AddingNew;

			}

			m_loadingData = false;
		}

		private void SeasonItem_Click(object sender, EventArgs e)
		{
			if(sender is ToolStripMenuItem)
			{
				ToolStripMenuItem seasonItem = (ToolStripMenuItem)sender;

				if(seasonItem.Tag is Season)
				{
					/*
					((Season)seasonItem.Tag).Leagues = m_currentSeason.Leagues;
					((Season)seasonItem.Tag).NonPlayingDates = m_currentSeason.NonPlayingDates;
					((Season)seasonItem.Tag).OtherFixtures = m_currentSeason.OtherFixtures;
					((Season)seasonItem.Tag).PlayingFields = m_currentSeason.PlayingFields;
					*/
					m_currentSeason = (Season)seasonItem.Tag;
					DisplayCurrentSeason();
				}
			}
		}

		private void OtherFixturesBindingSource_AddingNew(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new OtherFixture()
			{
				ID = Guid.NewGuid()
			};
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

				LeagueGradeComboBox.DataSource = new BindingSource() { DataSource = m_currentSeason.Grades };
				LeagueGradeComboBox.DisplayMember = "Value";
				LeagueGradeComboBox.ValueMember = "Value";

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

					LeagueGradeComboBox.DataBindings.Clear();
					LeagueGradeComboBox.DataBindings.Add("SelectedValue", m_currentLeague, "Grade");

					GameDurationTextBox.Text = m_currentLeague.GameDurationMinutes.ToString();

					m_gameTimesBindingSource.DataSource = m_currentLeague.GameTimes;
					LeagueGameTimesDataGridView.AutoGenerateColumns = false;
					LeagueGameTimesDataGridView.DataSource = m_gameTimesBindingSource;
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

					m_teamsBindingSource.DataSource = m_currentLeague.Teams;
					TeamsDataGridView.DataSource = m_teamsBindingSource;
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
						if (m_currentLeague.Grade == iLeague.Grade)
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

		private void GameTimesBindingSource_AddingNew(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new GameTime()
			{
				ID = Guid.NewGuid()
			};
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
					PlayingDays = new List<Guid>(),
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
					PlayingDays = new List<Guid>(),
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
						PlayingDays = new List<Guid>()
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

				TeamGameTimesCheckedListBox.Items.Clear();
				foreach (GameTime iGameTime in m_currentLeague.GameTimes)
				{
					TeamGameTimesCheckedListBox.Items.Add(iGameTime, m_currentTeam.PlayingDays.Contains(iGameTime.ID));
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
			if (m_loadingData)
			{
				return;
			}

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
			Save();
		}

		private void SeasonNonPlayingDatesDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			Save();
		}

		private void SeasonPlayingFieldsDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			Save();
		}

		private void LeaguesDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			Save();
		}

		private void LeagueGameTimesDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			Save();
		}

		private void TeamsDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			Save();
		}

		private void OtherFixturesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			Save();
		}

		private void OtherFixturesDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
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

		private void LeaguePlayingFieldsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			m_currentLeague.PlayingFields.Clear();
			for (int index = 0; index < LeaguePlayingFieldsCheckedListBox.Items.Count; index++)
			{
				Guid fieldID = ((PlayingField)LeaguePlayingFieldsCheckedListBox.Items[index]).ID;

				if ((index == e.Index && e.NewValue == CheckState.Checked) ||
					(index != e.Index && LeaguePlayingFieldsCheckedListBox.CheckedIndices.Contains(index)))
				{
					m_currentLeague.PlayingFields.Add(fieldID);
				}
			}
			/*
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
			*/
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
					PlayingDays = new List<Guid>()
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
			for (int index = 0; index < PairedLeaguesCheckedListBox.Items.Count; index++)
			{
				Guid leagueID = ((League)PairedLeaguesCheckedListBox.Items[index]).ID;

				if ((index == e.Index && e.NewValue == CheckState.Checked) ||
					(index != e.Index && PairedLeaguesCheckedListBox.CheckedIndices.Contains(index)))
				{
					m_currentLeague.PairedLeagues.Add(leagueID);
				}
			}
			/*
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
			*/
			Save();
		}

		private void PairedTeamsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			m_currentTeam.PairedTeams.Clear();
			for (int index = 0; index < PairedTeamsCheckedListBox.Items.Count; index++)
			{
				Guid teamID = ((League)PairedTeamsCheckedListBox.Items[index]).ID;

				if ((index == e.Index && e.NewValue == CheckState.Checked) ||
					(index != e.Index && PairedTeamsCheckedListBox.CheckedIndices.Contains(index)))
				{
					m_currentTeam.PairedTeams.Add(teamID);
				}
			}
			/*
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
			*/
			Save();
		}

		private void TeamGameTimesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			m_currentTeam.PlayingDays.Clear();
			for(int index = 0; index < TeamGameTimesCheckedListBox.Items.Count; index++)
			{
				Guid gameTimeID = ((GameTime)TeamGameTimesCheckedListBox.Items[index]).ID;

				if ((index == e.Index && e.NewValue == CheckState.Checked) ||
					(index != e.Index && TeamGameTimesCheckedListBox.CheckedIndices.Contains(index)))
				{
					m_currentTeam.PlayingDays.Add(gameTimeID);
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

		private void LeagueNonPlayingDatesDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
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

		private void TeamNonPlayingDatesDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			if (m_loadingData)
			{
				return;
			}

			Save();
		}

		private Dictionary<string, List<Fixture>> m_fixtures = new Dictionary<string, List<Fixture>>();
		
		private void GenerateFixturesButton_Click(object sender, EventArgs e)
		{
			m_fixtures = new Dictionary<string, List<Fixture>>();

			List<Guid> leaguesScheduled = new List<Guid>();
			Dictionary<string, List<FixtureDetails>> leagueDictionary = new Dictionary<string, List<FixtureDetails>>();

			foreach (League iLeague in m_currentSeason.Leagues)
			{
				int playingWeeks = 0;
				FixtureDetails iFixtureDetails = new FixtureDetails() { m_league = iLeague };

				foreach (GameTime iGameTime in iLeague.GameTimes)
				{
					if (!iFixtureDetails.playingDays.Contains((DayOfWeek)iGameTime.DayOfWeek))
					{
						iFixtureDetails.playingDays.Add((DayOfWeek)iGameTime.DayOfWeek);
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

					foreach (DayOfWeek day in iFixtureDetails.playingDays)
					{
						string reason = string.Empty;
						DateTime gameDate = date.AddDays((int)day - (int)date.DayOfWeek);

						if (AddDate(gameDate, iLeague, ref reason))
						{
							//gameDates.Add(gameDate);
							dateAdded = true;
						}
						else if (!iFixtureDetails.nonPlayingDates.ContainsKey(gameDate.Date))
						{
							iFixtureDetails.nonPlayingDates.Add(gameDate.Date, reason);
						}
					}

					if (dateAdded)
					{
						playingWeeks++;
					}
				}

				iFixtureDetails.round = 1;
				iFixtureDetails.rounds = playingWeeks / (iLeague.Teams.Count());// - 1);
				iFixtureDetails.friendlies = playingWeeks % (iLeague.Teams.Count());// - 1);
				iFixtureDetails.roundTeams = new List<Team>();
				iFixtureDetails.roundTeams.AddRange(iLeague.Teams.ToArray());

				string gradeName = iLeague.LeagueName;

				if (iLeague.PairedLeagues.Count > 0)
				{
					string sameLeague = string.Empty;
					string existingLeagueName = string.Empty;

					foreach (StringValue grade in m_currentSeason.Grades)
					{
						if(iLeague.LeagueName.StartsWith(grade.Value))
						{
							gradeName = grade.Value;
							existingLeagueName = iLeague.LeagueName;
							break;
						}
					}
					if (gradeName != iLeague.LeagueName)
					{
						if (leagueDictionary.ContainsKey(existingLeagueName))
						{
							List<FixtureDetails> existingFixtureDetails = leagueDictionary[existingLeagueName];
							leagueDictionary.Remove(existingLeagueName);
							leagueDictionary.Add(gradeName, existingFixtureDetails);
						}
					}
				}

				if (!leagueDictionary.ContainsKey(gradeName))
				{
					leagueDictionary.Add(gradeName, new List<FixtureDetails>());
				}
				leagueDictionary[gradeName].Add(iFixtureDetails);
			}

			foreach (string leagueName in leagueDictionary.Keys)
			{
				for (DateTime date = m_currentSeason.SeasonStartDate; date <= m_currentSeason.SeasonEndDate; date = date.AddDays(7))
				{
					// Ensure that the hours, minutes and seconds are zero.
					date = date.AddHours(-date.Hour);
					date = date.AddMinutes(-date.Minute);
					date = date.AddSeconds(-date.Second);
					date = date.AddMilliseconds(-date.Millisecond);

					if (!m_fixtures.ContainsKey(leagueName))
					{
						m_fixtures.Add(leagueName, new List<Fixture>());
					}

					Dictionary<DateTime, List<Guid>> slotsUsed = new Dictionary<DateTime, List<Guid>>();
					Dictionary<DateTime, int> gameSlots = new Dictionary<DateTime, int>();

					foreach (FixtureDetails iFixtureDetails in leagueDictionary[leagueName])
					{
						foreach (GameTime iGameTime in iFixtureDetails.m_league.GameTimes)
						{
							DateTime gameTime = date.AddDays((int)iGameTime.DayOfWeek - (int)date.DayOfWeek);
							gameTime = gameTime.AddHours(iGameTime.StartTime.Hour);
							gameTime = gameTime.AddMinutes(iGameTime.StartTime.Minute);

							if (!gameSlots.ContainsKey(gameTime))
							{
								gameSlots.Add(gameTime, iGameTime.Priority);
							}
						}
					}

					foreach (FixtureDetails iFixtureDetails in leagueDictionary[leagueName])
					{
						bool generalBye = false;
						// If the whole day is a non-playing days then log it and skip to the next week.
						foreach (DateTime gameSlot in gameSlots.Keys)
						{
							if (iFixtureDetails.nonPlayingDates.ContainsKey(gameSlot.Date))
							{
								m_fixtures[leagueName].Add(new FixtureGeneralBye() { GameTime = gameSlot, Reason = iFixtureDetails.nonPlayingDates[gameSlot.Date] });
								generalBye = true;
								break;
							}
						}

						if(generalBye)
						{
							break;
						}
						if (iFixtureDetails.friendlies > 0)
						{
							// Need to determine what to do about friendlies.
							foreach (DayOfWeek day in iFixtureDetails.playingDays)
							{
								DateTime gameDate = date.AddDays((int)day - (int)date.DayOfWeek);

								if (iFixtureDetails.nonPlayingDates.ContainsKey(gameDate.Date))
								{
									m_fixtures[leagueName].Add(new FixtureGeneralBye() { GameTime = gameDate, Reason = iFixtureDetails.nonPlayingDates[gameDate.Date] });
								}
								else
								{
									m_fixtures[leagueName].Add(new FixtureGeneralBye() { GameTime = gameDate, Reason = "Friendly", GameLeague = iFixtureDetails.m_league });
								}
							}

							iFixtureDetails.friendlies--;
							continue;
						}
						else
						{
							int teamPosition = 0;
							bool roundComplete = false;
							DateTime gameDate = DateTime.MinValue;

							foreach (DateTime gameSlot in gameSlots.Keys)
							{
								gameDate = gameSlot.Date;
								List<Guid> availableFields = new List<Guid>();
								foreach (Guid fieldID in iFixtureDetails.m_league.PlayingFields)
								{
									if (gameSlots[gameSlot] == GetField(fieldID).Priority)
									{
										availableFields.Add(fieldID);
									}
								}
								
								foreach (Guid fieldID in iFixtureDetails.m_league.PlayingFields)
								{
									if (gameSlots[gameSlot] != GetField(fieldID).Priority)
									{
										availableFields.Add(fieldID);
									}
								}
								
								if (slotsUsed.ContainsKey(gameSlot))
								{
									bool allFieldsUsed = true;
									foreach (Guid fieldID in availableFields)
									{
										if (!slotsUsed[gameSlot].Contains(fieldID))
										{
											allFieldsUsed = false;
										}
									}
									if (allFieldsUsed)
									{
										continue;
									}
								}

								foreach (Guid fieldID in availableFields)
								{
									Fixture fixture;
									if (teamPosition == (iFixtureDetails.roundTeams.Count - 1))
									{
										fixture = new FixtureTeamBye()
										{
											GameTime = gameSlot,
											TeamWithBye = iFixtureDetails.roundTeams[teamPosition],
											Round = iFixtureDetails.round,
											GameLeague = iFixtureDetails.m_league
										};
										teamPosition++;
									}
									else
									{
										// If one of the teams cannot play in this time slot then move it and its opposition
										// to the next slot.
										Guid gameTimeID = Guid.Empty;
										foreach (GameTime leagueGameTime in iFixtureDetails.m_league.GameTimes)
										{
											if (leagueGameTime.StartTime.Hour == gameSlot.Hour &&
												leagueGameTime.StartTime.Minute == gameSlot.Minute)
											{
												gameTimeID = leagueGameTime.ID;
												break;
											}
										}
										if (!iFixtureDetails.roundTeams[teamPosition].PlayingDays.Contains(gameTimeID) ||
											!iFixtureDetails.roundTeams[teamPosition + 1].PlayingDays.Contains(gameTimeID) &&
											teamPosition + 2 < iFixtureDetails.roundTeams.Count)
										{
											Team homeTeam = iFixtureDetails.roundTeams[teamPosition];
											Team awayTeam = iFixtureDetails.roundTeams[teamPosition + 1];
											iFixtureDetails.roundTeams.RemoveAt(teamPosition);
											iFixtureDetails.roundTeams.RemoveAt(teamPosition);
											iFixtureDetails.roundTeams.Insert(teamPosition, awayTeam);
											iFixtureDetails.roundTeams.Insert(teamPosition, homeTeam);
										}
										fixture = new FixtureGame()
										{
											GameTime = gameSlot,
											Round = iFixtureDetails.round,
											GameLeague = iFixtureDetails.m_league,
											Field = GetField(fieldID),
											HomeTeam = iFixtureDetails.roundTeams[teamPosition],
											AwayTeam = iFixtureDetails.roundTeams[teamPosition + 1]
										};
										teamPosition += 2;
									}
									m_fixtures[leagueName].Add(fixture);

									if (fixture is FixtureGame)
									{
										if (!slotsUsed.ContainsKey(gameSlot))
										{
											slotsUsed.Add(gameSlot, new List<Guid>());
										}
										slotsUsed[gameSlot].Add(fieldID);
									}

									if (teamPosition >= iFixtureDetails.roundTeams.Count)
									{
										roundComplete = true;
										break;
									}
								}
								if (roundComplete)
								{
									break;
								}
							}

							if (teamPosition == (iFixtureDetails.roundTeams.Count - 1))
							{
								Fixture fixture = new FixtureTeamBye()
								{
									GameTime = gameDate,
									TeamWithBye = iFixtureDetails.roundTeams[teamPosition],
									Round = iFixtureDetails.round,
									GameLeague = iFixtureDetails.m_league
								};
								m_fixtures[leagueName].Add(fixture);
							}

							// Shuffle the first team to the end.
							Team firstTeam = iFixtureDetails.roundTeams[0];
							iFixtureDetails.roundTeams.RemoveAt(0);
							iFixtureDetails.roundTeams.Add(firstTeam);

							// Next round...
							iFixtureDetails.round++;
						}
					}
				}
			}

			int tabPageCount = FixtureTabControl.TabPages.Count;
			while (tabPageCount > 1)
			{
				FixtureTabControl.TabPages.RemoveAt(1);
				tabPageCount--;
			}

			foreach (string leagueName in leagueDictionary.Keys)
			{
				TabPage newTabPage = new TabPage(leagueName);
				FixtureDisplayUserControl fixtureDisplay = new FixtureDisplayUserControl();
				fixtureDisplay.Dock = DockStyle.Fill;

				newTabPage.Controls.Add(fixtureDisplay);

				FixtureTabControl.TabPages.Add(newTabPage);

				fixtureDisplay.Initialise(m_fixtures[leagueName]);
			}				

			foreach (string leagueName in leagueDictionary.Keys)
			{
				foreach (FixtureDetails iFixtureDetails in leagueDictionary[leagueName])
				{
					DateTime fixtureDate = DateTime.MinValue;
					bool addDate = false;
					StreamWriter fixtureWriter = new StreamWriter(leagueName + ".csv");
					foreach (Fixture iFixture in m_fixtures[leagueName])
					{
						string line = string.Empty;
						if (iFixture is FixtureGame && fixtureDate.Date != iFixture.GameTime.Date)
						{
							fixtureDate = iFixture.GameTime;
							addDate = true;
							fixtureWriter.WriteLine();
							fixtureWriter.WriteLine(",Time,Home Team,Away Team,Field,League,Round");
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
							line += ",";
							line += ((FixtureGame)iFixture).GameLeague.LeagueName;
							line += ",";
							line += ((FixtureGame)iFixture).Round.ToString();
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
							line += ",,,";
							line += ((FixtureTeamBye)iFixture).GameLeague.LeagueName;
							line += ",";
							line += ((FixtureTeamBye)iFixture).Round.ToString();
						}
						fixtureWriter.WriteLine(line);
					}
					fixtureWriter.Close();
				}
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

		private void OtherFixturesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

		private void LeagueGameTimesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

		private void LeaguePlayingFieldsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

		private void SeasonTitleTextBox_TextChanged(object sender, EventArgs e)
		{
			m_currentSeason.SeasonTitle = SeasonTitleTextBox.Text;
		}

		private void LeagueGradeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(!m_loadingData)
			{
				Save();
			}
		}
	}
}
