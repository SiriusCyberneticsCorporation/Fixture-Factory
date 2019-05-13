namespace Fixture_Factory
{
	partial class FixtureBreakdownUserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.TimeSlotDataGridView = new System.Windows.Forms.DataGridView();
			this.FieldBreakdownDataGridView = new System.Windows.Forms.DataGridView();
			this.TeamBreakdownDataGridView = new System.Windows.Forms.DataGridView();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.TimeSlotDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.FieldBreakdownDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TeamBreakdownDataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.TimeSlotDataGridView, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.FieldBreakdownDataGridView, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.TeamBreakdownDataGridView, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(150, 150);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// TimeSlotDataGridView
			// 
			this.TimeSlotDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.TimeSlotDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.TimeSlotDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TimeSlotDataGridView.Location = new System.Drawing.Point(3, 101);
			this.TimeSlotDataGridView.Name = "TimeSlotDataGridView";
			this.TimeSlotDataGridView.Size = new System.Drawing.Size(144, 46);
			this.TimeSlotDataGridView.TabIndex = 4;
			// 
			// FieldBreakdownDataGridView
			// 
			this.FieldBreakdownDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.FieldBreakdownDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.FieldBreakdownDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FieldBreakdownDataGridView.Location = new System.Drawing.Point(3, 3);
			this.FieldBreakdownDataGridView.Name = "FieldBreakdownDataGridView";
			this.FieldBreakdownDataGridView.Size = new System.Drawing.Size(144, 43);
			this.FieldBreakdownDataGridView.TabIndex = 1;
			// 
			// TeamBreakdownDataGridView
			// 
			this.TeamBreakdownDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.TeamBreakdownDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.TeamBreakdownDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TeamBreakdownDataGridView.Location = new System.Drawing.Point(3, 52);
			this.TeamBreakdownDataGridView.Name = "TeamBreakdownDataGridView";
			this.TeamBreakdownDataGridView.Size = new System.Drawing.Size(144, 43);
			this.TeamBreakdownDataGridView.TabIndex = 2;
			// 
			// FixtureBreakdownUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "FixtureBreakdownUserControl";
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.TimeSlotDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.FieldBreakdownDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TeamBreakdownDataGridView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.DataGridView TimeSlotDataGridView;
		private System.Windows.Forms.DataGridView FieldBreakdownDataGridView;
		private System.Windows.Forms.DataGridView TeamBreakdownDataGridView;
	}
}
