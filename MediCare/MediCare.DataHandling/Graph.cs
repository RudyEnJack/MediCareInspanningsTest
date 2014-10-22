using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace MediCare.DataHandling
{
    public class Graph
    {
        private System.Windows.Forms.DataVisualization.Charting.Chart GraphChart;
        private System.Windows.Forms.DataVisualization.Charting.Series[] ChartData;
        private System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1;
        private System.Windows.Forms.DataVisualization.Charting.Legend legend1;
        private System.Windows.Forms.CheckBox Time_Running_CheckBox;
        private System.Windows.Forms.CheckBox Speed_CheckBox;
        private System.Windows.Forms.CheckBox Distance_CheckBox;
        private System.Windows.Forms.CheckBox Brake_CheckBox;
        private System.Windows.Forms.CheckBox Power_CheckBox;
        private System.Windows.Forms.CheckBox Energy_CheckBox;
        private System.Windows.Forms.CheckBox HeartBeats_CheckBox;
        private System.Windows.Forms.CheckBox RPM_CheckBox;
        private bool[] checkbox_Status = { false, false, false, false, false, false, false, false };
        public Graph()
        {
            this.GraphChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.GraphChart)).BeginInit();
            chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.Time_Running_CheckBox = new System.Windows.Forms.CheckBox();
            this.Speed_CheckBox = new System.Windows.Forms.CheckBox();
            this.Distance_CheckBox = new System.Windows.Forms.CheckBox();
            this.Brake_CheckBox = new System.Windows.Forms.CheckBox();
            this.Power_CheckBox = new System.Windows.Forms.CheckBox();
            this.Energy_CheckBox = new System.Windows.Forms.CheckBox();
            this.HeartBeats_CheckBox = new System.Windows.Forms.CheckBox();
            this.RPM_CheckBox = new System.Windows.Forms.CheckBox();
            this.ChartData = new System.Windows.Forms.DataVisualization.Charting.Series[8];
        }
        public void InitializeChart_Client()
        {
            //chartArea1.AxisX.Maximum = 120D;
            //chartArea1.AxisX.Minimum = 0D;
            //chartArea1.AxisX.Title = "Time in seconds";
            //chartArea1.AxisY.Maximum = 100D;
            //chartArea1.AxisY.Minimum = 0D;
            chartArea1.Name = "ChartArea1";
            this.GraphChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.GraphChart.Legends.Add(legend1);
            this.GraphChart.Location = new System.Drawing.Point(501, 60);
            this.GraphChart.Name = "Graph";
            this.GraphChart.Size = new System.Drawing.Size(737, 323);
            this.GraphChart.TabIndex = 26;
            this.GraphChart.Text = "Graph";
        }
        public void InitializeChart_Doctor()
        {
            //chartArea1.AxisX.Maximum = 120D;
            //chartArea1.AxisX.Minimum = 0D;
            //chartArea1.AxisX.Title = "Time in seconds";
            //chartArea1.AxisY.Maximum = 100D;
            //chartArea1.AxisY.Minimum = 0D;
            //chartArea1.Name = "ChartArea1";
            this.GraphChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.GraphChart.Legends.Add(legend1);
            this.GraphChart.Location = new System.Drawing.Point(450, 20);
            this.GraphChart.Name = "Graph";
            this.GraphChart.Size = new System.Drawing.Size(737, 323);
            this.GraphChart.TabIndex = 26;
            this.GraphChart.Text = "Graph";
        }
        public void Initialize_Checkboxes_Client()
        {
            //
            // Distance_CheckBox
            //
            this.Distance_CheckBox.AutoSize = true;
            this.Distance_CheckBox.Location = new System.Drawing.Point(480, 155);
            this.Distance_CheckBox.Name = "Distance_CheckBox";
            this.Distance_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Distance_CheckBox.TabIndex = 29;
            this.Distance_CheckBox.UseVisualStyleBackColor = true;
            this.Distance_CheckBox.CheckedChanged += new System.EventHandler(this.on_Distance_CheckBox_Click);
            //
            // Brake_CheckBox
            //
            this.Brake_CheckBox.AutoSize = true;
            this.Brake_CheckBox.Location = new System.Drawing.Point(480, 195);
            this.Brake_CheckBox.Name = "Brake_CheckBox";
            this.Brake_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Brake_CheckBox.TabIndex = 30;
            this.Brake_CheckBox.UseVisualStyleBackColor = true;
            this.Brake_CheckBox.CheckedChanged += new System.EventHandler(this.on_Brake_CheckBox_Click);
            //
            // Power_CheckBox
            //
            this.Power_CheckBox.AutoSize = true;
            this.Power_CheckBox.Location = new System.Drawing.Point(480, 235);
            this.Power_CheckBox.Name = "Power_CheckBox";
            this.Power_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Power_CheckBox.TabIndex = 31;
            this.Power_CheckBox.UseVisualStyleBackColor = true;
            this.Power_CheckBox.CheckedChanged += new System.EventHandler(this.on_Power_CheckBox_Click);
            //
            // Energy_CheckBox
            //
            this.Energy_CheckBox.AutoSize = true;
            this.Energy_CheckBox.Location = new System.Drawing.Point(480, 275);
            this.Energy_CheckBox.Name = "Energy_CheckBox";
            this.Energy_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Energy_CheckBox.TabIndex = 32;
            this.Energy_CheckBox.UseVisualStyleBackColor = true;
            this.Energy_CheckBox.CheckedChanged += new System.EventHandler(this.on_Energy_CheckBox_Click);
            //
            // HeartBeats_CheckBox
            //
            this.HeartBeats_CheckBox.AutoSize = true;
            this.HeartBeats_CheckBox.Location = new System.Drawing.Point(480, 315);
            this.HeartBeats_CheckBox.Name = "HeartBeats_CheckBox";
            this.HeartBeats_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.HeartBeats_CheckBox.TabIndex = 33;
            this.HeartBeats_CheckBox.UseVisualStyleBackColor = true;
            this.HeartBeats_CheckBox.CheckedChanged += new System.EventHandler(this.on_HeartBeats_CheckBox_Click);
            //
            // RPM_CheckBox
            //
            this.RPM_CheckBox.AutoSize = true;
            this.RPM_CheckBox.Location = new System.Drawing.Point(480, 355);
            this.RPM_CheckBox.Name = "RPM_CheckBox";
            this.RPM_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.RPM_CheckBox.TabIndex = 34;
            this.RPM_CheckBox.UseVisualStyleBackColor = true;
            this.RPM_CheckBox.CheckedChanged += new System.EventHandler(this.on_RPM_CheckBox_Click);
            //
            // Time_Running_CheckBox
            //
            this.Time_Running_CheckBox.AutoSize = true;
            this.Time_Running_CheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Time_Running_CheckBox.Location = new System.Drawing.Point(480, 75);
            this.Time_Running_CheckBox.Name = "Time_Running_CheckBox";
            this.Time_Running_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Time_Running_CheckBox.TabIndex = 27;
            this.Time_Running_CheckBox.UseVisualStyleBackColor = true;
            this.Time_Running_CheckBox.CheckedChanged += new System.EventHandler(this.on_Time_Running_CheckBox_Click);
            //
            // Speed_CheckBox
            //
            this.Speed_CheckBox.AutoSize = true;
            this.Speed_CheckBox.Location = new System.Drawing.Point(480, 115);
            this.Speed_CheckBox.Name = "Speed_CheckBox";
            this.Speed_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Speed_CheckBox.TabIndex = 28;
            this.Speed_CheckBox.UseVisualStyleBackColor = true;
            this.Speed_CheckBox.CheckedChanged += new System.EventHandler(this.on_Speed_CheckBox_Click);
        }
        public void Initialize_Checkboxes_Doctor()
        {
            int CorrectionY = 45;
            int CorrectionX = 50;
            //
            // Distance_CheckBox
            //
            this.Distance_CheckBox.AutoSize = true;
            this.Distance_CheckBox.Location = new System.Drawing.Point(425, 98);
            this.Distance_CheckBox.Name = "Distance_CheckBox";
            this.Distance_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Distance_CheckBox.TabIndex = 29;
            this.Distance_CheckBox.UseVisualStyleBackColor = true;
            this.Distance_CheckBox.CheckedChanged += new System.EventHandler(this.on_Distance_CheckBox_Click);
            //
            // Brake_CheckBox
            //
            this.Brake_CheckBox.AutoSize = true;
            this.Brake_CheckBox.Location = new System.Drawing.Point(425, 133);
            this.Brake_CheckBox.Name = "Brake_CheckBox";
            this.Brake_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Brake_CheckBox.TabIndex = 30;
            this.Brake_CheckBox.UseVisualStyleBackColor = true;
            this.Brake_CheckBox.CheckedChanged += new System.EventHandler(this.on_Brake_CheckBox_Click);
            //
            // Power_CheckBox
            //
            this.Power_CheckBox.AutoSize = true;
            this.Power_CheckBox.Location = new System.Drawing.Point(425, 167);
            this.Power_CheckBox.Name = "Power_CheckBox";
            this.Power_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Power_CheckBox.TabIndex = 31;
            this.Power_CheckBox.UseVisualStyleBackColor = true;
            this.Power_CheckBox.CheckedChanged += new System.EventHandler(this.on_Power_CheckBox_Click);
            //
            // Energy_CheckBox
            //
            this.Energy_CheckBox.AutoSize = true;
            this.Energy_CheckBox.Location = new System.Drawing.Point(425, 202);
            this.Energy_CheckBox.Name = "Energy_CheckBox";
            this.Energy_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Energy_CheckBox.TabIndex = 32;
            this.Energy_CheckBox.UseVisualStyleBackColor = true;
            this.Energy_CheckBox.CheckedChanged += new System.EventHandler(this.on_Energy_CheckBox_Click);
            //
            // HeartBeats_CheckBox
            //
            this.HeartBeats_CheckBox.AutoSize = true;
            this.HeartBeats_CheckBox.Location = new System.Drawing.Point(425, 238);
            this.HeartBeats_CheckBox.Name = "HeartBeats_CheckBox";
            this.HeartBeats_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.HeartBeats_CheckBox.TabIndex = 33;
            this.HeartBeats_CheckBox.UseVisualStyleBackColor = true;
            this.HeartBeats_CheckBox.CheckedChanged += new System.EventHandler(this.on_HeartBeats_CheckBox_Click);
            //
            // RPM_CheckBox
            //
            this.RPM_CheckBox.AutoSize = true;
            this.RPM_CheckBox.Location = new System.Drawing.Point(425, 275);
            this.RPM_CheckBox.Name = "RPM_CheckBox";
            this.RPM_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.RPM_CheckBox.TabIndex = 34;
            this.RPM_CheckBox.UseVisualStyleBackColor = true;
            this.RPM_CheckBox.CheckedChanged += new System.EventHandler(this.on_RPM_CheckBox_Click);
            //
            // Time_Running_CheckBox
            //
            this.Time_Running_CheckBox.AutoSize = true;
            this.Time_Running_CheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Time_Running_CheckBox.Location = new System.Drawing.Point(425, 30);
            this.Time_Running_CheckBox.Name = "Time_Running_CheckBox";
            this.Time_Running_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Time_Running_CheckBox.TabIndex = 27;
            this.Time_Running_CheckBox.UseVisualStyleBackColor = true;
            this.Time_Running_CheckBox.CheckedChanged += new System.EventHandler(this.on_Time_Running_CheckBox_Click);
            //
            // Speed_CheckBox
            //
            this.Speed_CheckBox.AutoSize = true;
            this.Speed_CheckBox.Location = new System.Drawing.Point(425, 63);
            this.Speed_CheckBox.Name = "Speed_CheckBox";
            this.Speed_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.Speed_CheckBox.TabIndex = 28;
            this.Speed_CheckBox.UseVisualStyleBackColor = true;
            this.Speed_CheckBox.CheckedChanged += new System.EventHandler(this.on_Speed_CheckBox_Click);
        }
        private void on_Time_Running_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[0] = !checkbox_Status[0];
            updateGraph(0);
        }
        private void on_Speed_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[1] = !checkbox_Status[1];
            updateGraph(1);
        }
        private void on_Distance_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[2] = !checkbox_Status[2];
            updateGraph(2);
        }
        private void on_Brake_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[3] = !checkbox_Status[3];
            updateGraph(3);
        }
        private void on_Power_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[4] = !checkbox_Status[4];
            updateGraph(4);
        }
        private void on_Energy_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[5] = !checkbox_Status[5];
            updateGraph(5);
        }
        private void on_HeartBeats_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[6] = !checkbox_Status[6];
            updateGraph(6);
        }
        private void on_RPM_CheckBox_Click(object sender, EventArgs e)
        {
            checkbox_Status[7] = !checkbox_Status[7];
            updateGraph(7);
        }
        private void updateGraph(int box_ID)
        {
            GraphChart.Series.Clear();
            for (int i = 0; i < checkbox_Status.Length; i++)
            {
                if (checkbox_Status[i])
                {
                    GraphChart.Series.Add(ChartData[i]);
                }
                else
                {
                    //do nothing
                }
            }
        }
        public void InitializeGraph()
        {
            for (int i = 0; i < ChartData.Length; i++)
            {
                System.Windows.Forms.DataVisualization.Charting.Series s = new System.Windows.Forms.DataVisualization.Charting.Series();
                s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                switch (i)
                {
                    case 0:
                        s.Color = Color.BurlyWood;
                        s.Name = "Time Running";
                        break;
                    case 1:
                        s.Color = Color.Blue;
                        s.Name = "Speed";
                        break;
                    case 2:
                        s.Color = Color.Cyan;
                        s.Name = "Distance";
                        break;
                    case 3:
                        s.Color = Color.DarkOrange;
                        s.Name = "Brake";
                        break;
                    case 4:
                        s.Color = Color.ForestGreen;
                        s.Name = "Power";
                        break;
                    case 5:
                        s.Color = Color.Gold;
                        s.Name = "Energy";
                        break;
                    case 6:
                        s.Color = Color.Magenta;
                        s.Name = "Heart Beats";
                        break;
                    case 7:
                        s.Color = Color.MistyRose;
                        s.Name = "RPM";
                        break;
                    default:
                        s.Color = Color.Black;
                        s.Name = "ERROR NON EXISTEND ITEM LOADED";
                        break;
                }
                ChartData[i] = s;
                ChartData[0].Points.Add(1, 1);
            }
        }
        /**************************************\
        * TODO: Implement Small Cashing System *
        \**************************************/
        public void process_Graph_Data(String[] data)
        {
            // if (data.Length != 1) // maybe not needed if called from updatevalues
            // {
            for (int i = 0; i < data.Length; i++)
            {
                ChartData[i].Points.Add(double.Parse(data[i]));
                //ChartData[i].Points.
            }
            // }
        }
        public object[] getComponents()
        {
            return new object[] { GraphChart, Time_Running_CheckBox, Distance_CheckBox, Brake_CheckBox, Speed_CheckBox, Power_CheckBox, Energy_CheckBox, HeartBeats_CheckBox, RPM_CheckBox };
        }
        public void SetVisibibility(bool v)
        {
            GraphChart.Visible = v;
            Time_Running_CheckBox.Visible = v;
            Speed_CheckBox.Visible = v;
            Distance_CheckBox.Visible = v;
            Brake_CheckBox.Visible = v;
            Power_CheckBox.Visible = v;
            Energy_CheckBox.Visible = v;
            HeartBeats_CheckBox.Visible = v;
            RPM_CheckBox.Visible = v;
        }
    }
}