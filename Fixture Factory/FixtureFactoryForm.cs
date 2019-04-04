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

		private Random m_randomiser = new Random();
		private Dictionary<Guid, List<Team>> m_umpiringTeamDictionary = new Dictionary<Guid, List<Team>>();
		private Dictionary<Guid, Dictionary<Team, int>> m_umpiringCountDictionary = new Dictionary<Guid, Dictionary<Team, int>>();
		private Dictionary<string, List<DateTime>> m_allocatedFixtures = new Dictionary<string, List<DateTime>>();

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

					bool save = false;
					foreach(League iLeague in m_currentSeason.Leagues)
					{
						if(iLeague.Grade == "A2")
						{
							iLeague.Grade = "Senior";
							save = true;
						}
					}
					if (save)
					{
						Save();
					}

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
						Grades = new List<StringValue>() { new StringValue("Year 3-5"), new StringValue("Year 6-8"), new StringValue("Year 9-12"), new StringValue("Senior"), new StringValue("Masters") }
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
				Grades = new List<StringValue>() { new StringValue("Year 3-5"), new StringValue("Year 6-8"), new StringValue("Year 9-12"), new StringValue("Senior"), new StringValue("Masters") }
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
					if (iLeague.LeagueName != "Masters" && !iLeague.LeagueName.Contains("Senior"))
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


				//m_currentSeason.Grades = new List<StringValue>() { new StringValue("Year 3-5"), new StringValue("Year 6-8"), new StringValue("Year 9-12"), new StringValue("Senior"), new StringValue("Masters") };
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
			if (sender is ToolStripMenuItem)
			{
				ToolStripMenuItem seasonItem = (ToolStripMenuItem)sender;

				if (seasonItem.Tag is Season)
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
						if (m_currentLeague.Grade == iLeague.Grade && m_currentLeague.ID != iLeague.ID)
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
				Guid teamID = ((Team)PairedTeamsCheckedListBox.Items[index]).ID;

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
			for (int index = 0; index < TeamGameTimesCheckedListBox.Items.Count; index++)
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

		private Dictionary<string, SortedDictionary<DateTime, List<Fixture>>> m_fixtures = new Dictionary<string, SortedDictionary<DateTime, List<Fixture>>>();

		private void AddFixture(string league, DateTime slot, Fixture iFixture)
		{
			if (!m_fixtures.ContainsKey(league))
			{
				m_fixtures.Add(league, new SortedDictionary<DateTime, List<Fixture>>());
			}
			if (!m_fixtures[league].ContainsKey(slot))
			{
				m_fixtures[league].Add(slot, new List<Fixture>());
			}
			m_fixtures[league][slot].Add(iFixture);

			if (iFixture is FixtureGame)
			{
				string field = ((FixtureGame)iFixture).Field;
				if (!m_allocatedFixtures.ContainsKey(field))
				{
					m_allocatedFixtures.Add(field, new List<DateTime>());
				}
				m_allocatedFixtures[field].Add(iFixture.GameTime);
			}
		}

		private bool GameTimeMatch(DateTime slot1, DateTime slot2)
		{
			return (slot1.Day == slot2.Day && slot1.Month == slot2.Month && slot1.Year == slot2.Year &&
					slot1.Day == slot2.Day && slot1.Hour == slot2.Hour && slot1.Minute == slot2.Minute);
		}

		private int GetDayShift(DateTime date, int gameDayOfWeek)
		{
			int dayShift = 0;
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					dayShift = gameDayOfWeek;
					break;
				case DayOfWeek.Monday:
					switch (gameDayOfWeek)
					{
						case (int)DayOfWeek.Sunday:
							dayShift = 6;
							break;
						case (int)DayOfWeek.Monday:
						case (int)DayOfWeek.Tuesday:
						case (int)DayOfWeek.Wednesday:
						case (int)DayOfWeek.Thursday:
						case (int)DayOfWeek.Friday:
						case (int)DayOfWeek.Saturday:
							dayShift = gameDayOfWeek - 1;
							break;
					}
					break;
				case DayOfWeek.Tuesday:
					switch (gameDayOfWeek)
					{
						case (int)DayOfWeek.Tuesday:
						case (int)DayOfWeek.Wednesday:
						case (int)DayOfWeek.Thursday:
						case (int)DayOfWeek.Friday:
						case (int)DayOfWeek.Saturday:
							dayShift = gameDayOfWeek - 2;
							break;
						case (int)DayOfWeek.Sunday:
						case (int)DayOfWeek.Monday:
							dayShift = 5 + gameDayOfWeek;
							break;
					}
					break;
				case DayOfWeek.Wednesday:
					switch (gameDayOfWeek)
					{
						case (int)DayOfWeek.Wednesday:
						case (int)DayOfWeek.Thursday:
						case (int)DayOfWeek.Friday:
						case (int)DayOfWeek.Saturday:
							dayShift = gameDayOfWeek - 3;
							break;
						case (int)DayOfWeek.Sunday:
						case (int)DayOfWeek.Monday:
						case (int)DayOfWeek.Tuesday:
							dayShift = 4 + gameDayOfWeek;
							break;
					}
					break;
				case DayOfWeek.Thursday:
					switch (gameDayOfWeek)
					{
						case (int)DayOfWeek.Thursday:
						case (int)DayOfWeek.Friday:
						case (int)DayOfWeek.Saturday:
							dayShift = gameDayOfWeek - 4;
							break;
						case (int)DayOfWeek.Sunday:
						case (int)DayOfWeek.Monday:
						case (int)DayOfWeek.Tuesday:
						case (int)DayOfWeek.Wednesday:
							dayShift = 3 + gameDayOfWeek;
							break;
					}
					break;
				case DayOfWeek.Friday:
					switch (gameDayOfWeek)
					{
						case (int)DayOfWeek.Friday:
						case (int)DayOfWeek.Saturday:
							dayShift = gameDayOfWeek - 5;
							break;
						case (int)DayOfWeek.Sunday:
						case (int)DayOfWeek.Monday:
						case (int)DayOfWeek.Tuesday:
						case (int)DayOfWeek.Wednesday:
						case (int)DayOfWeek.Thursday:
							dayShift = 2 + gameDayOfWeek;
							break;
					}
					break;
				case DayOfWeek.Saturday:
					switch (gameDayOfWeek)
					{
						case (int)DayOfWeek.Saturday:
							dayShift = gameDayOfWeek - 6;
							break;
						case (int)DayOfWeek.Sunday:
						case (int)DayOfWeek.Monday:
						case (int)DayOfWeek.Tuesday:
						case (int)DayOfWeek.Wednesday:
						case (int)DayOfWeek.Thursday:
						case (int)DayOfWeek.Friday:
							dayShift = 1 + gameDayOfWeek;
							break;
					}
					break;
			}

			return dayShift;
		}

		private League GetLeague(Guid ID)
		{
			League result = null;

			foreach (League iLeague in m_currentSeason.Leagues)
			{
				if (iLeague.ID == ID)
				{
					result = iLeague;
					break;
				}
			}

			return result;
		}
		private bool DesignatedGameTime(Team iTeam, DateTime gameSlot)
		{
			bool slotAvailable = false;
			League iLeague = GetLeague(iTeam.LeagueID);
			foreach (Guid gameTimeID in iTeam.PlayingDays)
			{
				foreach (GameTime iGameTime in iLeague.GameTimes)
				{
					if (iGameTime.ID == gameTimeID)
					{
						if (iGameTime.DayOfWeek == (int)gameSlot.DayOfWeek &&
							iGameTime.StartTime.Hour == gameSlot.Hour &&
							iGameTime.StartTime.Minute == gameSlot.Minute)
						{
							slotAvailable = true;
							break;
						}
					}
				}
				if (slotAvailable)
				{
					break;
				}
			}
			// Make sure the game time is not in the Team non-playing times.
			if (slotAvailable && iTeam.NonPlayingDates != null)
			{
				foreach(NonPlayingDate npd in iTeam.NonPlayingDates)
				{
					if (gameSlot.Date == npd.Date.Date &&
						gameSlot.Hour == npd.Date.Hour &&
						gameSlot.Minute == npd.Date.Minute)
					{
						slotAvailable = false;
						break;
					}
				}
			}
			if (!slotAvailable && iTeam.TeamName != "Margaret River")
			{
				int x = 0;
			}
			return slotAvailable;
		}

		public static void Swap<T>(IList<T> list, int indexA, int indexB)
		{
			T tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}

		private void GenerateFixturesButton_Click(object sender, EventArgs e)
		{
			m_fixtures = new Dictionary<string, SortedDictionary<DateTime, List<Fixture>>>();
			m_allocatedFixtures.Clear();

			FixtureCalculator iFixtureCalculator = new FixtureCalculator();
			List<Guid> leaguesScheduled = new List<Guid>();
			Dictionary<string, Dictionary<Guid, FixtureDetails>> leagueDictionary = new Dictionary<string, Dictionary<Guid, FixtureDetails>>();

			foreach (League iLeague in m_currentSeason.Leagues)
			{
				int playingWeeks = 0;
				List<DateTime> gameDates = new List<DateTime>();
				FixtureDetails iFixtureDetails = new FixtureDetails() { m_league = iLeague };

				iFixtureDetails.Fixtures = iFixtureCalculator.GenerateFixtures(iLeague.Teams.Count);

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
						DateTime gameDate = date.AddDays(GetDayShift(date, (int)day));

						if (AddDate(gameDate, iLeague, ref reason))
						{
							if (!dateAdded)
							{
								gameDates.Add(gameDate);
								playingWeeks++;
							}
							dateAdded = true;
							//break;
						}
						else if (!iFixtureDetails.nonPlayingDates.ContainsKey(gameDate.Date))
						{
							iFixtureDetails.nonPlayingDates.Add(gameDate.Date, reason);
							FixtureGeneralBye bye = new FixtureGeneralBye() { GameTime = gameDate, Reason = reason, Grade = iLeague.LeagueName };
							AddFixture(iFixtureDetails.m_league.Grade, gameDate, bye);
						}
					}
				}

				iFixtureDetails.Round = 1;
				iFixtureDetails.Rounds = iFixtureDetails.Fixtures.Count;

				int roundRepeats = playingWeeks / iFixtureDetails.Rounds;
				int numberOfGames = roundRepeats * iFixtureDetails.Rounds;
				iFixtureDetails.FirstGameDate = gameDates[playingWeeks - numberOfGames];

				//iFixtureDetails.Friendlies = playingWeeks % iFixtureDetails.Fixtures.Count;
				iFixtureDetails.RoundTeams = new List<Team>();
				iFixtureDetails.RoundTeams.AddRange(iLeague.Teams.ToArray());

				if (!leagueDictionary.ContainsKey(iLeague.Grade))
				{
					leagueDictionary.Add(iLeague.Grade, new Dictionary<Guid, FixtureDetails>());
				}
				if (!leagueDictionary[iLeague.Grade].ContainsKey(iLeague.ID))
				{
					leagueDictionary[iLeague.Grade][iLeague.ID] = iFixtureDetails;
				}
			}

			int swapCounter = 0;

			foreach (string leagueName in leagueDictionary.Keys)
			{
				Dictionary<Guid, FixtureDetails> fixtureDetailsList = leagueDictionary[leagueName];

				for (DateTime date = m_currentSeason.SeasonStartDate; date <= m_currentSeason.SeasonEndDate; date = date.AddDays(7))
				{
					// Ensure that the hours, minutes and seconds are zero.
					date = date.AddHours(-date.Hour);
					date = date.AddMinutes(-date.Minute);
					date = date.AddSeconds(-date.Second);
					date = date.AddMilliseconds(-date.Millisecond);

					List<List<KeyValuePair<Team, Team>>> teamPairLists = new List<List<KeyValuePair<Team, Team>>>();
					List<KeyValuePair<DateTime, Guid>> gameSlots = new List<KeyValuePair<DateTime, Guid>>();

					// Generate teamPairLists for this round and game slots for the day.
					foreach (Guid leagueID in leagueDictionary[leagueName].Keys)
					{
						FixtureDetails iFixtureDetails = leagueDictionary[leagueName][leagueID];
						List<KeyValuePair<Team, Team>> teamPairList = new List<KeyValuePair<Team, Team>>();

						int round = (iFixtureDetails.Round - 1) % iFixtureDetails.Fixtures.Count;
						int index = 0;

						foreach (KeyValuePair<int, int> teams in iFixtureDetails.Fixtures[round])
						{
							if (teams.Value == -1)
							{
								teamPairList.Add(new KeyValuePair<Team, Team>(iFixtureDetails.RoundTeams[teams.Key], null));
							}
							else if (round >= iFixtureDetails.Fixtures.Count)
							{
								teamPairList.Add(new KeyValuePair<Team, Team>(iFixtureDetails.RoundTeams[teams.Value], iFixtureDetails.RoundTeams[teams.Key]));
							}
							else
							{
								teamPairList.Add(new KeyValuePair<Team, Team>(iFixtureDetails.RoundTeams[teams.Key], iFixtureDetails.RoundTeams[teams.Value]));
							}
							index++;
						}

						// Move any team with a bye to the end of the list
						for (int teamIndex = 0; teamIndex < teamPairList.Count; teamIndex++)
						{
							KeyValuePair<Team, Team> teamPair = teamPairList[teamIndex];
							if (teamPair.Value == null)
							{
								teamPairList.RemoveAt(teamIndex);
								teamPairList.Add(teamPair);
								break;
							}
						}

						teamPairLists.Add(teamPairList);

						for (int priority = 1; priority >= 0; priority--)
						{
							foreach (Guid fieldID in iFixtureDetails.m_league.PlayingFields)
							{
								PlayingField field = GetField(fieldID);

								foreach (GameTime iGameTime in iFixtureDetails.m_league.GameTimes)
								{
									DateTime gameTime = date.AddDays(GetDayShift(date, iGameTime.DayOfWeek));
									gameTime = gameTime.AddHours(iGameTime.StartTime.Hour);
									gameTime = gameTime.AddMinutes(iGameTime.StartTime.Minute);

									bool addGameTime = true;

									if (priority == 1)
									{
										foreach (OtherFixture iOtherFixture in m_currentSeason.OtherFixtures)
										{
											if (GameTimeMatch(iOtherFixture.GameTime, gameTime))
											{
												addGameTime = false;

												if (!m_fixtures.ContainsKey(leagueName) || !m_fixtures[leagueName].ContainsKey(gameTime))
												{
													FixtureGame fixture = new FixtureGame()
													{
														GameTime = gameTime,
														Round = 0,
														Grade = iOtherFixture.Grade,
														Field = GetField(iOtherFixture.FieldID).FieldName,
														HomeTeam = iOtherFixture.HomeTeam,
														AwayTeam = iOtherFixture.AwayTeam
													};
													AddFixture(leagueName, gameTime, fixture);
												}
											}
										}
									}

									if (m_allocatedFixtures.ContainsKey(field.FieldName))
									{
										if(m_allocatedFixtures[field.FieldName].Contains(gameTime))
										{
											addGameTime = false;
										}
									}

									if (addGameTime)
									{
										if (!iFixtureDetails.nonPlayingDates.ContainsKey(gameTime.Date))
										{
											if (field.Priority == priority && iGameTime.Priority == priority)
											{
												bool alreadyAdded = false;
												foreach (KeyValuePair<DateTime, Guid> existing in gameSlots)
												{
													if (GameTimeMatch(existing.Key, gameTime) && existing.Value == fieldID)
													{
														alreadyAdded = true;
														break;
													}
												}
												if (!alreadyAdded)
												{
													gameSlots.Add(new KeyValuePair<DateTime, Guid>(gameTime, fieldID));
												}
											}											
										}
									}
								}
							}
						}
					}

					if (gameSlots.Count == 0)
					{
						continue;
					}

					bool roundComplete = false;
					DateTime gameDate = DateTime.MinValue;
					Team previousHomeTeam = null;
					Team previousAwayTeam = null;

					List<KeyValuePair<Team, Team>> pairedTeams = new List<KeyValuePair<Team, Team>>();

					if (teamPairLists.Count == 1)
					{
						pairedTeams.AddRange(teamPairLists[0]);
					}
					else if (teamPairLists.Count == 2)
					{
						if (swapCounter % 2 == 1)
						{
							Swap(teamPairLists, 0, 1);
						}

						int sourceLeague = 0;
						int pairedLeague = 1;

						foreach (KeyValuePair<Team, Team> teamPair in teamPairLists[sourceLeague])
						{
							pairedTeams.Add(teamPair);

							// Ignore byes
							if (teamPair.Value != null)
							{								
								// Try to match paired teams...
								for (int pi = 0; pi < teamPairLists[pairedLeague].Count; pi++)
								{
									KeyValuePair<Team, Team> possiblePair = teamPairLists[pairedLeague][pi];

									// Ignore byes
									if (possiblePair.Value != null)
									{
										// If both teams have a paired team of the source league then add them as the next pair.
										if ((possiblePair.Key.PairedTeams.Contains(teamPair.Key.ID) && possiblePair.Value.PairedTeams.Contains(teamPair.Value.ID)) ||
											(possiblePair.Key.PairedTeams.Contains(teamPair.Value.ID) && possiblePair.Value.PairedTeams.Contains(teamPair.Key.ID)))
										{
											pairedTeams.Add(possiblePair);
											teamPairLists[pairedLeague].RemoveAt(pi);
											break;
										}
									}
								}
								for (int pi = 0; pi < teamPairLists[pairedLeague].Count; pi++)
								{
									KeyValuePair<Team, Team> possiblePair = teamPairLists[pairedLeague][pi];

									// Ignore byes
									if (possiblePair.Value != null)
									{
										// If either team has a paired team of the source league then add them as the next pair.
										if ((possiblePair.Key.PairedTeams.Contains(teamPair.Key.ID) || possiblePair.Value.PairedTeams.Contains(teamPair.Key.ID)) ||
											(possiblePair.Key.PairedTeams.Contains(teamPair.Value.ID) || possiblePair.Value.PairedTeams.Contains(teamPair.Value.ID)))
										{
											pairedTeams.Add(possiblePair);											
											teamPairLists[pairedLeague].RemoveAt(pi);
											//break;
										}
									}
								}
							}
						}
						// Add any remaining matches to the end.
						for (int pi = 0; pi < teamPairLists[pairedLeague].Count; pi++)
						{
							pairedTeams.Add(teamPairLists[pairedLeague][pi]);
						}
					}
					else
					{
						throw new NotImplementedException();
					}

					ShuffleTeamsToMeetNonPlayingDays(pairedTeams, gameSlots);

					m_umpiringTeamDictionary.Clear();
					foreach (KeyValuePair<Team, Team> teamPair in pairedTeams)
					{
						// Teams with a bye do not umpire
						if (teamPair.Value != null)
						{
							if (!m_umpiringTeamDictionary.ContainsKey(teamPair.Key.LeagueID))
							{
								m_umpiringTeamDictionary.Add(teamPair.Key.LeagueID, new List<Team>());
							}
							m_umpiringTeamDictionary[teamPair.Key.LeagueID].Add(teamPair.Key);
							m_umpiringTeamDictionary[teamPair.Key.LeagueID].Add(teamPair.Value);
						}
					}

					int remainingSlots = gameSlots.Count + 1;
					foreach (KeyValuePair<DateTime, Guid> slot in gameSlots)
					{
						DateTime gameSlot = slot.Key;
						Guid fieldID = slot.Value;
						Guid leagueID = pairedTeams[0].Key.LeagueID;

						remainingSlots--;
						gameDate = gameSlot.Date;

						while(LeagueNonPlayingDate(gameSlot, leagueID))
						{
							pairedTeams.RemoveAt(0);

							if (pairedTeams.Count == 0)
							{
								roundComplete = true;
								break;
							}
							leagueID = pairedTeams[0].Key.LeagueID;
						}
						if (pairedTeams.Count == 0)
						{
							roundComplete = true;
							break;
						}

						if (EvenRoundsOnlyCheckBox.Checked && fixtureDetailsList[leagueID].FirstGameDate.Date > gameSlot.Date)
						{
							Fixture friendly = new FixtureGeneralBye() { GameTime = gameDate, Reason = "Friendly", Grade = fixtureDetailsList[leagueID].m_league.LeagueName };

							fixtureDetailsList[leagueID].Friendly = true;
							AddFixture(fixtureDetailsList[leagueID].m_league.Grade, gameSlot, friendly);							
							pairedTeams.RemoveAt(0);

							while (true)
							{
								bool matchRemoved = false;
								for (int pti = 0; pti < pairedTeams.Count; pti++)
								{
									if(pairedTeams[pti].Key.LeagueID == leagueID)
									{
										pairedTeams.RemoveAt(pti);
										matchRemoved = true;
										break;
									}
								}
								if(!matchRemoved)
								{
									break;
								}
							}

							if (pairedTeams.Count > 0)
							{
								leagueID = pairedTeams[0].Key.LeagueID;

								if (EvenRoundsOnlyCheckBox.Checked && fixtureDetailsList[leagueID].FirstGameDate.Date > gameSlot.Date)
								{
									friendly = new FixtureGeneralBye() { GameTime = gameDate, Reason = "Friendly", Grade = fixtureDetailsList[leagueID].m_league.LeagueName };

									fixtureDetailsList[leagueID].Friendly = true;
									AddFixture(fixtureDetailsList[leagueID].m_league.Grade, gameSlot, friendly);
									pairedTeams.RemoveAt(0);

									while (true)
									{
										bool matchRemoved = false;
										for (int pti = 0; pti < pairedTeams.Count; pti++)
										{
											if (pairedTeams[pti].Key.LeagueID == leagueID)
											{
												pairedTeams.RemoveAt(pti);
												matchRemoved = true;
												break;
											}
										}
										if (!matchRemoved)
										{
											break;
										}
									}
								}
							}
						}

						if (pairedTeams.Count == 0)
						{
							roundComplete = true;
							break;
						}
						else
						{
							leagueID = pairedTeams[0].Key.LeagueID;
						}

						
						while (pairedTeams[0].Value == null)
						{
							Fixture fixture = new FixtureTeamBye()
							{
								GameTime = gameSlot,
								TeamWithBye = pairedTeams[0].Key,
								Round = fixtureDetailsList[leagueID].Round,
								Grade = fixtureDetailsList[leagueID].m_league.LeagueName
							};
							pairedTeams.RemoveAt(0);
							AddFixture(fixtureDetailsList[leagueID].m_league.Grade, gameSlot, fixture);

							if (pairedTeams.Count == 0)
							{
								roundComplete = true;
								break;
							}
							else
							{
								leagueID = pairedTeams[0].Key.LeagueID;
							}
						}
						if (pairedTeams.Count > 0)
						{
							
							if (fixtureDetailsList[pairedTeams[0].Key.LeagueID].m_league.LeagueName.Contains("Senior"))
							{
										int x = 0;
							}
							


							// If one of the teams cannot play in this time slot the find the first team that can play in this time slot 
							// and move it to the start of the list.
							if (!DesignatedGameTime(pairedTeams[0].Key, gameSlot) || !DesignatedGameTime(pairedTeams[0].Value, gameSlot) ||
								LeagueNonPlayingDate(gameSlot, pairedTeams[0].Key.LeagueID) || LeagueNonPlayingDate(gameSlot, pairedTeams[0].Value.LeagueID))
							{
								bool swapMade = false;
								for (int pti = 1; pti < pairedTeams.Count; pti++)
								{
									if (pairedTeams[pti].Value != null &&
										!LeagueNonPlayingDate(gameSlot, pairedTeams[pti].Key.LeagueID) &&
										!LeagueNonPlayingDate(gameSlot, pairedTeams[pti].Value.LeagueID) &&
										DesignatedGameTime(pairedTeams[pti].Key, gameSlot) &&
										DesignatedGameTime(pairedTeams[pti].Value, gameSlot))
									{
										Swap(pairedTeams, 0, pti);
										swapMade = true;
										break;
									}
								}

								if(!swapMade && remainingSlots >= pairedTeams.Count)
								{
									continue;
								}
								leagueID = pairedTeams[0].Key.LeagueID;
							}

							Fixture fixture = new FixtureGame()
							{
								GameTime = gameSlot,
								Round = fixtureDetailsList[leagueID].Round,
								Grade = fixtureDetailsList[leagueID].m_league.LeagueName,
								Field = GetField(fieldID).FieldName,
								HomeTeam = pairedTeams[0].Key.TeamName,
								AwayTeam = pairedTeams[0].Value.TeamName,
							};
							if(fixtureDetailsList[leagueID].m_league.LeagueName.Contains("Senior"))
							{
								((FixtureGame)fixture).UmpiringTeam = GetUmpiringTeam(pairedTeams);
							}
							previousHomeTeam = pairedTeams[0].Key;
							previousAwayTeam = pairedTeams[0].Value;
							pairedTeams.RemoveAt(0);
							AddFixture(fixtureDetailsList[leagueID].m_league.Grade, gameSlot, fixture);
						}

						if (pairedTeams.Count == 0)
						{
							roundComplete = true;
							break;
						}

						if (roundComplete)
						{
							break;
						}						
					}

					if(gameDate.Month == 8 && gameDate.Day == 31)
					{
						int x = 0;
					}

					//if (gameSlots.Count > 0)
					{
						// Add in any dangling teams as byes.
						while (pairedTeams.Count > 0)
						{
							KeyValuePair<Team, Team> match = pairedTeams[0];
							if (match.Value == null)
							{
								Guid leagueID = match.Key.LeagueID;
								Fixture fixture = new FixtureTeamBye()
								{
									GameTime = gameDate,
									TeamWithBye = match.Key,
									Round = fixtureDetailsList[leagueID].Round,
									Grade = fixtureDetailsList[leagueID].m_league.LeagueName
								};
								AddFixture(fixtureDetailsList[leagueID].m_league.Grade, gameDate, fixture);
								pairedTeams.RemoveAt(0);
							}
							else
							{
								break;
							}
						}
					}
					
					if (pairedTeams.Count == 0)
					{
						roundComplete = true;
					}

					if (roundComplete)
					{
						swapCounter++;
						roundComplete = false;
						// Increment the round.
						foreach (Guid leagueID in leagueDictionary[leagueName].Keys)
						{
							if (!fixtureDetailsList[leagueID].Friendly)
							{
								fixtureDetailsList[leagueID].Round++;
							}
							fixtureDetailsList[leagueID].Friendly = false;
						}
					}
				}
			}

			EvenOutTurfUsage(m_fixtures["Year 3-5"]);
			EvenOutTurfUsage(m_fixtures["Year 6-8"]);			

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

				DocumentGenerator iDocumentGenerator = new DocumentGenerator(leagueName, m_fixtures[leagueName]);
			}

			foreach (string leagueName in leagueDictionary.Keys)
			{
				foreach (Guid leagueID in leagueDictionary[leagueName].Keys)
				{
					FixtureDetails iFixtureDetails = leagueDictionary[leagueName][leagueID];
					DateTime fixtureDate = DateTime.MinValue;
					bool addDate = false;
					StreamWriter fixtureWriter = new StreamWriter(leagueName + ".csv");
					SortedDictionary<DateTime, List<Fixture>> fixtures = m_fixtures[leagueName];
					foreach (DateTime fixtureTime in fixtures.Keys)
					{
						string line = string.Empty;
						foreach (Fixture iFixture in fixtures[fixtureTime])
						{
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
								line += ((FixtureGame)iFixture).HomeTeam;
								line += ",";
								line += ((FixtureGame)iFixture).AwayTeam;
								line += ",";
								line += ((FixtureGame)iFixture).Field;
								line += ",";
								line += ((FixtureGame)iFixture).Grade;
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
								line += ((FixtureTeamBye)iFixture).Grade;
								line += ",";
								line += ((FixtureTeamBye)iFixture).Round.ToString();
							}
							fixtureWriter.WriteLine(line);
						}
					}
					fixtureWriter.Close();
				}
			}
		}

		private void EvenOutTurfUsage(SortedDictionary<DateTime, List<Fixture>> fixtures)
		{
			int minGames = 999;
			int maxGames = 0;
			const string TURF = "Turf";
			List<string> leagues = new List<string>();
			List<DateTime> datesAltered = new List<DateTime>();

			foreach (DateTime fixtureTime in fixtures.Keys)
			{
				foreach (Fixture iFixture in fixtures[fixtureTime])
				{
					if (iFixture.Round > 0 && iFixture is FixtureGame)
					{
						if (((FixtureGame)iFixture).Field == TURF)
						{
							string league = iFixture.Grade;

							if (!leagues.Contains(league))
							{
								leagues.Add(league);
							}
						}
					}
				}
			}

			string previousMinTeam = string.Empty;
			string previousMaxTeam = string.Empty;
			List<string> teamsSwapped = new List<string>();

			foreach (string league in leagues)
			{
				int repeatCount = 1;

				while (true)
				{
					Dictionary<string, Dictionary<string, int>> turfGames = new Dictionary<string, Dictionary<string, int>>();

					foreach (DateTime fixtureTime in fixtures.Keys)
					{
						foreach (Fixture iFixture in fixtures[fixtureTime])
						{
							if (iFixture.Round > 0 && iFixture is FixtureGame)
							{
								if (((FixtureGame)iFixture).Grade == league && ((FixtureGame)iFixture).Field == TURF)
								{
									string homeTeam = ((FixtureGame)iFixture).HomeTeam;
									string awayTeam = ((FixtureGame)iFixture).AwayTeam;

									if (!turfGames.ContainsKey(league))
									{
										turfGames.Add(league, new Dictionary<string, int>());
									}
									if (!turfGames[league].ContainsKey(homeTeam))
									{
										turfGames[league].Add(homeTeam, 0);
									}
									if (!turfGames[league].ContainsKey(awayTeam))
									{
										turfGames[league].Add(awayTeam, 0);
									}
									turfGames[league][homeTeam]++;
									turfGames[league][awayTeam]++;
								}
							}
						}
					}

					List<string> minGameTeams = new List<string>();
					List<string> maxGameTeams = new List<string>();

					minGames = 999;
					maxGames = 0;

					foreach (string team in turfGames[league].Keys)
					{
						if (turfGames[league][team] < minGames)
						{
							minGames = turfGames[league][team];
						}
						if (turfGames[league][team] > maxGames)
						{
							maxGames = turfGames[league][team];
						}
					}

					if(Math.Abs(maxGames - minGames) <= 1)
					{
						break;
					}

					int tempMinGames = minGames;
					int tempMaxGames = maxGames;

					while (minGameTeams.Count == 0)
					{
						foreach (string team in turfGames[league].Keys)
						{
							if (turfGames[league][team] == tempMinGames && (repeatCount < 5 || !teamsSwapped.Contains(team)))
							{
								minGameTeams.Add(team);
							}
						}
						tempMinGames++;
					}
					//minGameTeams = Randomize(minGameTeams);

					while (maxGameTeams.Count == 0)
					{
						foreach (string team in turfGames[league].Keys)
						{
							if (turfGames[league][team] == tempMaxGames && (repeatCount < 5 || !teamsSwapped.Contains(team)))
							{
								maxGameTeams.Add(team);
							}
						}
						tempMaxGames--;
					}
					//maxGameTeams = Randomize(maxGameTeams);

					FixtureGame minTeamFixture = null;
					FixtureGame maxTeamFixture = null;
					DateTime previousGameDate = DateTime.MinValue.Date;
					bool changeMade = false;

					foreach (DateTime fixtureTime in fixtures.Keys)
					{
						if(datesAltered.Contains(fixtureTime.Date))
						{
							continue;
						}
						if (fixtureTime.Date != previousGameDate)
						{
							minTeamFixture = null;
							maxTeamFixture = null;
							previousGameDate = fixtureTime.Date;
						}

						foreach (Fixture iFixture in fixtures[fixtureTime])
						{
							if (iFixture.Round > 0 && iFixture is FixtureGame && ((FixtureGame)iFixture).Grade == league)
							{
								string homeTeam = ((FixtureGame)iFixture).HomeTeam;
								string awayTeam = ((FixtureGame)iFixture).AwayTeam;
								string field = ((FixtureGame)iFixture).Field;

								if (field == TURF)
								{
									if (maxTeamFixture == null && 
										(maxGameTeams[0] == homeTeam || maxGameTeams[0] == awayTeam) &&
										!(minGameTeams[0] == homeTeam || minGameTeams[0] == awayTeam))
									{
										maxTeamFixture = (FixtureGame)iFixture;
									}
								}
								else if (minTeamFixture == null)
								{
									if ((minGameTeams[0] == homeTeam || minGameTeams[0] == awayTeam) &&
										!(maxGameTeams[0] == homeTeam || maxGameTeams[0] == awayTeam))
									{
										minTeamFixture = (FixtureGame)iFixture;
									}
								}
							}
						}

						if (minTeamFixture != null && maxTeamFixture != null)
						{
							string maxHomeTeam = maxTeamFixture.HomeTeam;
							string maxAwayTeam = maxTeamFixture.AwayTeam;

							teamsSwapped.Clear();
							teamsSwapped.Add(maxGameTeams[0]);
							teamsSwapped.Add(minGameTeams[0]);

							if (maxGameTeams.Contains(maxTeamFixture.HomeTeam))
							{
								maxGameTeams.Remove(maxTeamFixture.HomeTeam);
							}
							if (maxGameTeams.Contains(maxTeamFixture.AwayTeam))
							{
								maxGameTeams.Remove(maxTeamFixture.AwayTeam);
							}

							if (minGameTeams.Contains(minTeamFixture.HomeTeam))
							{
								minGameTeams.Remove(minTeamFixture.HomeTeam);
							}
							if (minGameTeams.Contains(minTeamFixture.AwayTeam))
							{
								minGameTeams.Remove(minTeamFixture.AwayTeam);
							}
							maxTeamFixture.HomeTeam = minTeamFixture.HomeTeam;
							maxTeamFixture.AwayTeam = minTeamFixture.AwayTeam;

							minTeamFixture.HomeTeam = maxHomeTeam;
							minTeamFixture.AwayTeam = maxAwayTeam;

							changeMade = true;
							datesAltered.Add(fixtureTime.Date);
							//if (maxGameTeams.Count == 0 || minGameTeams.Count == 0)
							{
								break;
							}
						}
					}
					if(!changeMade)
					{
						repeatCount++;
						if(repeatCount > 10)
						{
							break;
						}
						datesAltered.Clear();
					}
				}
			}
		}

		public List<string> Randomize(List<string> source)
		{
			Random rnd = new Random();
			return source.OrderBy<string, int>((item) => rnd.Next()).ToList();
		}

		private string GetUmpiringTeam(List<KeyValuePair<Team, Team>> pairedTeams)
		{
			string umpiringTeam = string.Empty;
			Guid teamLeague = pairedTeams[0].Key.LeagueID;

			if (pairedTeams.Count > 0 && m_umpiringTeamDictionary.ContainsKey(teamLeague))
			{
				List<int> umpiresIndexList = new List<int>();

				for (int index = 0; index < m_umpiringTeamDictionary[teamLeague].Count; index++)
				{
					if (m_umpiringTeamDictionary[teamLeague][index] != pairedTeams[0].Key &&
						m_umpiringTeamDictionary[teamLeague][index] != pairedTeams[0].Value)
					{
						umpiresIndexList.Add(index);
					}
					if (umpiresIndexList.Count >= 2)
					{
						break;
					}
				}

				if (umpiresIndexList.Count >= 2)
				{
					int team1Index = umpiresIndexList[0];
					int team2Index = umpiresIndexList[1];

					if (!m_umpiringCountDictionary.ContainsKey(teamLeague))
					{
						m_umpiringCountDictionary.Add(teamLeague, new Dictionary<Team, int>());
					}
					if (!m_umpiringCountDictionary[teamLeague].ContainsKey(m_umpiringTeamDictionary[teamLeague][team1Index]))
					{
						m_umpiringCountDictionary[teamLeague].Add(m_umpiringTeamDictionary[teamLeague][team1Index], 0);
					}
					if (!m_umpiringCountDictionary[teamLeague].ContainsKey(m_umpiringTeamDictionary[teamLeague][team2Index]))
					{
						m_umpiringCountDictionary[teamLeague].Add(m_umpiringTeamDictionary[teamLeague][team2Index], 0);
					}

					int count1 = m_umpiringCountDictionary[teamLeague][m_umpiringTeamDictionary[teamLeague][team1Index]];
					int count2 = m_umpiringCountDictionary[teamLeague][m_umpiringTeamDictionary[teamLeague][team2Index]];

					if (count1 > count2)
					{
						umpiringTeam = m_umpiringTeamDictionary[teamLeague][team2Index].TeamName;
						m_umpiringCountDictionary[teamLeague][m_umpiringTeamDictionary[teamLeague][team2Index]]++;
						m_umpiringTeamDictionary[teamLeague].RemoveAt(team2Index);
					}
					else
					{
						umpiringTeam = m_umpiringTeamDictionary[teamLeague][team1Index].TeamName;
						m_umpiringCountDictionary[teamLeague][m_umpiringTeamDictionary[teamLeague][team1Index]]++;
						m_umpiringTeamDictionary[teamLeague].RemoveAt(team1Index);
					}
				}
				else
				{
					int team1Index = umpiresIndexList[0];

					if (!m_umpiringCountDictionary.ContainsKey(teamLeague))
					{
						m_umpiringCountDictionary.Add(teamLeague, new Dictionary<Team, int>());
					}
					if (!m_umpiringCountDictionary[teamLeague].ContainsKey(m_umpiringTeamDictionary[teamLeague][team1Index]))
					{
						m_umpiringCountDictionary[teamLeague].Add(m_umpiringTeamDictionary[teamLeague][team1Index], 0);
					}

					umpiringTeam = m_umpiringTeamDictionary[teamLeague][team1Index].TeamName;
					m_umpiringCountDictionary[teamLeague][m_umpiringTeamDictionary[teamLeague][team1Index]]++;
					m_umpiringTeamDictionary[teamLeague].RemoveAt(team1Index);					
				}
			}

			return umpiringTeam;
		}

		// If a team cannot play in one of the game slots them move that team pair to the front of the line in the
		// hope that we can find a slot before they are all used up.
		private void ShuffleTeamsToMeetNonPlayingDays(List<KeyValuePair<Team, Team>> pairedTeams, List<KeyValuePair<DateTime, Guid>> gameSlots)
		{
			int startIndex = 0;
			while (startIndex < pairedTeams.Count)
			{
				int npdIndex = -1;
				for (int index = startIndex; index < pairedTeams.Count; index++)
				{
					KeyValuePair<Team, Team> teamPair = pairedTeams[index];

					if (teamPair.Key.NonPlayingDates.Count > 0)
					{
						foreach (NonPlayingDate npd in teamPair.Key.NonPlayingDates)
						{
							foreach (KeyValuePair<DateTime, Guid> slot in gameSlots)
							{
								if (slot.Key.Date == npd.Date.Date &&
									slot.Key.Hour == npd.Date.Hour &&
									slot.Key.Minute == npd.Date.Minute)
								{
									npdIndex = index;
									break;
								}
							}
							if(npdIndex > 0)
							{
								break;
							}
						}
					}
					if (npdIndex > 0)
					{
						break;
					}
					if (teamPair.Value != null && teamPair.Value.NonPlayingDates.Count > 0)
					{
						foreach (NonPlayingDate npd in teamPair.Value.NonPlayingDates)
						{
							foreach (KeyValuePair<DateTime, Guid> slot in gameSlots)
							{
								if (slot.Key.Date == npd.Date.Date &&
									slot.Key.Hour == npd.Date.Hour &&
									slot.Key.Minute == npd.Date.Minute)
								{
									npdIndex = index;
									break;
								}
							}
							if (npdIndex > 0)
							{
								break;
							}
						}
					}
					if (npdIndex > 0)
					{
						break;
					}
				}
				if (npdIndex > 0)
				{
					KeyValuePair<Team, Team> teamPair = pairedTeams[npdIndex];
					pairedTeams.RemoveAt(npdIndex);
					pairedTeams.Insert(0, teamPair);
					startIndex++;
				}
				else
				{
					break;
				}
			}
		}

		private bool LeagueNonPlayingDate(DateTime gameDate, Guid leagueID)
		{
			bool nonPlayingDate = false;
			League iLeague = GetLeague(leagueID);

			foreach (NonPlayingDate skipDate in iLeague.NonPlayingDates)
			{
				if (skipDate.Date.Date == gameDate.Date)
				{
					if ((skipDate.Date.Hour == 0 && skipDate.Date.Minute == 0) ||
						(skipDate.Date.Hour == gameDate.Hour && skipDate.Date.Minute == gameDate.Minute))
					{
						nonPlayingDate = true;
						break;
					}
				}
			}
			return nonPlayingDate;
		}

		private PlayingField GetField(Guid fieldID)
		{
			foreach (PlayingField field in m_currentSeason.PlayingFields)
			{
				if (field.ID == fieldID)
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
			if (!m_loadingData)
			{
				Save();
			}
		}
	}
}
