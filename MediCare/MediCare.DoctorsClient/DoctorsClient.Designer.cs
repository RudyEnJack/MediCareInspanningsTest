using System;
using System.Security.AccessControl;

namespace MediCare.ArtsClient
{
    partial class DoctorClient
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.IndexTab = new System.Windows.Forms.TabPage();
            this.Filelist = new System.Windows.Forms.ListBox();
            this.OverviewTable = new System.Windows.Forms.DataGridView();
            this.ClientID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Signup_Button = new System.Windows.Forms.Button();
            this.typeBox = new System.Windows.Forms.TextBox();
            this.OverviewLabel = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SendMessage = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Password_Box = new System.Windows.Forms.TextBox();
            this.Username_Box = new System.Windows.Forms.TextBox();
            this.Password_Label = new System.Windows.Forms.Label();
            this.Username_label = new System.Windows.Forms.Label();
            this.LoginButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.Error_Label = new System.Windows.Forms.Label();
            this.ManageUsersButton = new System.Windows.Forms.Button();
            this.IndexTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverviewTable)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // IndexTab
            // 
            this.IndexTab.Controls.Add(this.ManageUsersButton);
            this.IndexTab.Controls.Add(this.Filelist);
            this.IndexTab.Controls.Add(this.OverviewTable);
            this.IndexTab.Controls.Add(this.Signup_Button);
            this.IndexTab.Controls.Add(this.typeBox);
            this.IndexTab.Controls.Add(this.OverviewLabel);
            this.IndexTab.Controls.Add(this.txtLog);
            this.IndexTab.Controls.Add(this.SendMessage);
            this.IndexTab.Cursor = System.Windows.Forms.Cursors.Default;
            this.IndexTab.Location = new System.Drawing.Point(4, 22);
            this.IndexTab.Name = "IndexTab";
            this.IndexTab.Padding = new System.Windows.Forms.Padding(3);
            this.IndexTab.Size = new System.Drawing.Size(1232, 631);
            this.IndexTab.TabIndex = 0;
            this.IndexTab.Text = "Overview";
            this.IndexTab.UseVisualStyleBackColor = true;
            // 
            // Filelist
            // 
            this.Filelist.FormattingEnabled = true;
            this.Filelist.Location = new System.Drawing.Point(444, 58);
            this.Filelist.Name = "Filelist";
            this.Filelist.Size = new System.Drawing.Size(573, 238);
            this.Filelist.TabIndex = 14;
            this.Filelist.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Filelist_MouseDoubleClick);
            // 
            // OverviewTable
            // 
            this.OverviewTable.AllowUserToAddRows = false;
            this.OverviewTable.AllowUserToDeleteRows = false;
            this.OverviewTable.AllowUserToResizeColumns = false;
            this.OverviewTable.AllowUserToResizeRows = false;
            this.OverviewTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.OverviewTable.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.OverviewTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.OverviewTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OverviewTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClientID});
            this.OverviewTable.Location = new System.Drawing.Point(34, 58);
            this.OverviewTable.MultiSelect = false;
            this.OverviewTable.Name = "OverviewTable";
            this.OverviewTable.ReadOnly = true;
            this.OverviewTable.RowHeadersWidth = 75;
            this.OverviewTable.Size = new System.Drawing.Size(257, 253);
            this.OverviewTable.TabIndex = 13;
            this.OverviewTable.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.OverviewTable.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // ClientID
            // 
            this.ClientID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ClientID.FillWeight = 300F;
            this.ClientID.HeaderText = "Client ID";
            this.ClientID.Name = "ClientID";
            this.ClientID.ReadOnly = true;
            this.ClientID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ClientID.ToolTipText = "The ID of the client";
            // 
            // Signup_Button
            // 
            this.Signup_Button.Location = new System.Drawing.Point(1030, 601);
            this.Signup_Button.Name = "Signup_Button";
            this.Signup_Button.Size = new System.Drawing.Size(95, 23);
            this.Signup_Button.TabIndex = 12;
            this.Signup_Button.Text = "Signup new user";
            this.Signup_Button.UseVisualStyleBackColor = true;
            this.Signup_Button.Click += new System.EventHandler(this.Signup_Button_Click);
            // 
            // typeBox
            // 
            this.typeBox.Location = new System.Drawing.Point(34, 603);
            this.typeBox.Name = "typeBox";
            this.typeBox.Size = new System.Drawing.Size(902, 20);
            this.typeBox.TabIndex = 9;
            this.typeBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLog_KeyDown);
            // 
            // OverviewLabel
            // 
            this.OverviewLabel.AutoSize = true;
            this.OverviewLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverviewLabel.Location = new System.Drawing.Point(28, 13);
            this.OverviewLabel.Name = "OverviewLabel";
            this.OverviewLabel.Size = new System.Drawing.Size(128, 31);
            this.OverviewLabel.TabIndex = 0;
            this.OverviewLabel.Text = "Overview";
            // 
            // txtLog
            // 
            this.txtLog.AllowDrop = true;
            this.txtLog.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtLog.Location = new System.Drawing.Point(34, 317);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(983, 282);
            this.txtLog.TabIndex = 6;
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            // 
            // SendMessage
            // 
            this.SendMessage.Location = new System.Drawing.Point(942, 601);
            this.SendMessage.Name = "SendMessage";
            this.SendMessage.Size = new System.Drawing.Size(75, 23);
            this.SendMessage.TabIndex = 10;
            this.SendMessage.Text = "Send";
            this.SendMessage.UseVisualStyleBackColor = true;
            this.SendMessage.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.IndexTab);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1240, 657);
            this.tabControl1.TabIndex = 0;
            // 
            // Password_Box
            // 
            this.Password_Box.Location = new System.Drawing.Point(612, 292);
            this.Password_Box.Name = "Password_Box";
            this.Password_Box.Size = new System.Drawing.Size(100, 20);
            this.Password_Box.TabIndex = 25;
            this.Password_Box.UseSystemPasswordChar = true;
            this.Password_Box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.on_password_box_enter);
            // 
            // Username_Box
            // 
            this.Username_Box.Location = new System.Drawing.Point(612, 266);
            this.Username_Box.Name = "Username_Box";
            this.Username_Box.Size = new System.Drawing.Size(100, 20);
            this.Username_Box.TabIndex = 24;
            this.Username_Box.Text = "98765432";
            this.Username_Box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.on_username_box_enter);
            // 
            // Password_Label
            // 
            this.Password_Label.AutoSize = true;
            this.Password_Label.Location = new System.Drawing.Point(543, 295);
            this.Password_Label.Name = "Password_Label";
            this.Password_Label.Size = new System.Drawing.Size(53, 13);
            this.Password_Label.TabIndex = 23;
            this.Password_Label.Text = "Password";
            // 
            // Username_label
            // 
            this.Username_label.AutoSize = true;
            this.Username_label.Location = new System.Drawing.Point(543, 266);
            this.Username_label.Name = "Username_label";
            this.Username_label.Size = new System.Drawing.Size(55, 13);
            this.Username_label.TabIndex = 22;
            this.Username_label.Text = "Username";
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(612, 318);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(100, 22);
            this.LoginButton.TabIndex = 21;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.login);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(617, 329);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 35;
            this.textBox1.UseSystemPasswordChar = true;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.on_password_box_enter);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(617, 303);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 34;
            this.textBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.on_username_box_enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(548, 332);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(548, 303);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Username";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(617, 355);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(100, 22);
            this.button4.TabIndex = 31;
            this.button4.Text = "Login";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.login);
            // 
            // Error_Label
            // 
            this.Error_Label.AutoSize = true;
            this.Error_Label.ForeColor = System.Drawing.Color.Red;
            this.Error_Label.Location = new System.Drawing.Point(457, 9);
            this.Error_Label.Name = "Error_Label";
            this.Error_Label.Size = new System.Drawing.Size(0, 13);
            this.Error_Label.TabIndex = 26;
            // 
            // ManageUsersButton
            // 
            this.ManageUsersButton.Location = new System.Drawing.Point(1131, 601);
            this.ManageUsersButton.Name = "ManageUsersButton";
            this.ManageUsersButton.Size = new System.Drawing.Size(95, 23);
            this.ManageUsersButton.TabIndex = 15;
            this.ManageUsersButton.Text = "Manage users";
            this.ManageUsersButton.UseVisualStyleBackColor = true;
            this.ManageUsersButton.Click += new System.EventHandler(this.ManageUsersButton_Click);
            // 
            // DoctorClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.Error_Label);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.Password_Box);
            this.Controls.Add(this.Username_Box);
            this.Controls.Add(this.Password_Label);
            this.Controls.Add(this.Username_label);
            this.Controls.Add(this.LoginButton);
            this.Name = "DoctorClient";
            this.Text = "Doctor Client";
            this.IndexTab.ResumeLayout(false);
            this.IndexTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverviewTable)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage IndexTab;
        private System.Windows.Forms.Label OverviewLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button SendMessage;
        private System.Windows.Forms.TextBox typeBox;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox Password_Box;
        private System.Windows.Forms.TextBox Username_Box;
        private System.Windows.Forms.Label Password_Label;
        private System.Windows.Forms.Label Username_label;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button Signup_Button;
        private System.Windows.Forms.Label Error_Label;
        private System.Windows.Forms.DataGridView OverviewTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientID;
        private System.Windows.Forms.ListBox Filelist;
        private System.Windows.Forms.Button ManageUsersButton;

    }
}

