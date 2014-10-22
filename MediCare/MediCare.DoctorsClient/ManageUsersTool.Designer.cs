namespace MediCare
{
    partial class ManageUsersTool
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ClientID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClientPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.Error_Label = new System.Windows.Forms.Label();
            this.DeleteUserButton = new System.Windows.Forms.Button();
            this.DeleteAllUsersButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClientID,
            this.ClientPassword});
            this.dataGridView1.Location = new System.Drawing.Point(12, 56);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 100;
            this.dataGridView1.Size = new System.Drawing.Size(365, 287);
            this.dataGridView1.TabIndex = 0;
            // 
            // ClientID
            // 
            this.ClientID.HeaderText = "Client ID";
            this.ClientID.Name = "ClientID";
            this.ClientID.ReadOnly = true;
            // 
            // ClientPassword
            // 
            this.ClientPassword.HeaderText = "Password";
            this.ClientPassword.Name = "ClientPassword";
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(12, 12);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(137, 25);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "Manage users";
            // 
            // Error_Label
            // 
            this.Error_Label.AutoSize = true;
            this.Error_Label.ForeColor = System.Drawing.Color.Red;
            this.Error_Label.Location = new System.Drawing.Point(248, 24);
            this.Error_Label.Name = "Error_Label";
            this.Error_Label.Size = new System.Drawing.Size(0, 13);
            this.Error_Label.TabIndex = 2;
            // 
            // DeleteUserButton
            // 
            this.DeleteUserButton.BackColor = System.Drawing.SystemColors.Control;
            this.DeleteUserButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DeleteUserButton.Location = new System.Drawing.Point(12, 350);
            this.DeleteUserButton.Name = "DeleteUserButton";
            this.DeleteUserButton.Size = new System.Drawing.Size(173, 30);
            this.DeleteUserButton.TabIndex = 3;
            this.DeleteUserButton.Text = "Delete user";
            this.DeleteUserButton.UseVisualStyleBackColor = false;
            this.DeleteUserButton.Click += new System.EventHandler(this.DeleteUserButton_Click);
            // 
            // DeleteAllUsersButton
            // 
            this.DeleteAllUsersButton.BackColor = System.Drawing.SystemColors.Control;
            this.DeleteAllUsersButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DeleteAllUsersButton.Location = new System.Drawing.Point(204, 350);
            this.DeleteAllUsersButton.Name = "DeleteAllUsersButton";
            this.DeleteAllUsersButton.Size = new System.Drawing.Size(173, 30);
            this.DeleteAllUsersButton.TabIndex = 4;
            this.DeleteAllUsersButton.Text = "Delete all users";
            this.DeleteAllUsersButton.UseVisualStyleBackColor = false;
            this.DeleteAllUsersButton.Click += DeleteAllUsersButton_Click;
            // 
            // ManageUsersTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 392);
            this.Controls.Add(this.DeleteAllUsersButton);
            this.Controls.Add(this.DeleteUserButton);
            this.Controls.Add(this.Error_Label);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ManageUsersTool";
            this.Text = "Manage Users Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label Error_Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientPassword;
        private System.Windows.Forms.Button DeleteUserButton;
        private System.Windows.Forms.Button DeleteAllUsersButton;
    }
}