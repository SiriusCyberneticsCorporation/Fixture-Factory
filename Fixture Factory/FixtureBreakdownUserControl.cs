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
	public partial class FixtureBreakdownUserControl : UserControl
	{
		public FixtureBreakdownUserControl()
		{
			InitializeComponent();
		}

		public void Initialise(string league,
								SortedDictionary<string, SortedDictionary<string, int>> fieldBreakdown,
								SortedDictionary<string, SortedDictionary<string, int>> teamBreakdown,
								SortedDictionary<string, SortedDictionary<GameTime, int>> slotBreakdown)
		{
			DataTable fieldBreakdownDataTable = new DataTable();
			DataTable teamBreakdownDataTable = new DataTable();
			DataTable slotBreakdownDataTable = new DataTable();
			fieldBreakdownDataTable.Columns.Add(new DataColumn(league));
			teamBreakdownDataTable.Columns.Add(new DataColumn(league));
			slotBreakdownDataTable.Columns.Add(new DataColumn(league));

			foreach (string team in fieldBreakdown.Keys)
			{
				DataRow teamRow = fieldBreakdownDataTable.NewRow();

				teamRow[league] = team;

				foreach (string field in fieldBreakdown[team].Keys)
				{
					if (!fieldBreakdownDataTable.Columns.Contains(field))
					{
						fieldBreakdownDataTable.Columns.Add(new DataColumn(field));
					}
					teamRow[field] = fieldBreakdown[team][field];
				}
				fieldBreakdownDataTable.Rows.Add(teamRow);
			}

			FieldBreakdownDataGridView.DataSource = fieldBreakdownDataTable;


			foreach (string homeTeam in teamBreakdown.Keys)
			{
				DataRow teamRow = teamBreakdownDataTable.NewRow();

				teamRow[league] = homeTeam;

				// Force the away teams to be in the same order as the home teams
				foreach (string dummyAwayTeam in teamBreakdown.Keys)
				{
					if (!teamBreakdownDataTable.Columns.Contains(dummyAwayTeam))
					{
						teamBreakdownDataTable.Columns.Add(new DataColumn(dummyAwayTeam));
					}
				}

				foreach (string awayTeam in teamBreakdown[homeTeam].Keys)
				{
					if (!teamBreakdownDataTable.Columns.Contains(awayTeam))
					{
						teamBreakdownDataTable.Columns.Add(new DataColumn(awayTeam));
					}
					teamRow[awayTeam] = teamBreakdown[homeTeam][awayTeam];
				}
				teamBreakdownDataTable.Rows.Add(teamRow);
			}

			TeamBreakdownDataGridView.DataSource = teamBreakdownDataTable;

			List<GameTime> timeSlots = new List<GameTime>();

			foreach (string team in slotBreakdown.Keys)
			{
				foreach (GameTime slot in slotBreakdown[team].Keys)
				{
					if (!timeSlots.Contains(slot))
					{
						timeSlots.Add(slot);
					}
				}
			}
			timeSlots.Sort();

			foreach (GameTime slot in timeSlots)
			{
				string slotText = slot.ToString();
				if (!slotBreakdownDataTable.Columns.Contains(slotText))
				{
					slotBreakdownDataTable.Columns.Add(new DataColumn(slotText));
				}
			}

			foreach (string team in slotBreakdown.Keys)
			{
				DataRow teamRow = slotBreakdownDataTable.NewRow();

				teamRow[league] = team;

				foreach (GameTime slot in slotBreakdown[team].Keys)
				{
					string slotText = slot.ToString();
					if (!slotBreakdownDataTable.Columns.Contains(slotText))
					{
						slotBreakdownDataTable.Columns.Add(new DataColumn(slotText));
					}
					teamRow[slotText] = slotBreakdown[team][slot];
				}
				slotBreakdownDataTable.Rows.Add(teamRow);
			}

			TimeSlotDataGridView.DataSource = slotBreakdownDataTable;
		}
	}
}
