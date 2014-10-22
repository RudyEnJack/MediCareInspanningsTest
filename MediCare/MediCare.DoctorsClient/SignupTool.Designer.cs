namespace MediCare
{
    partial class SignupTool
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
            this.Username_TextBox = new System.Windows.Forms.TextBox();
            this.Name_Label = new System.Windows.Forms.Label();
            this.Password_Label = new System.Windows.Forms.Label();
            this.Password_TextBox = new System.Windows.Forms.TextBox();
            this.Submit_Button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Password_Verify_TextBox = new System.Windows.Forms.TextBox();
            this.Error_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Username_TextBox
            // 
            this.Username_TextBox.Location = new System.Drawing.Point(106, 37);
            this.Username_TextBox.Name = "Username_TextBox";
            this.Username_TextBox.Size = new System.Drawing.Size(100, 20);
            this.Username_TextBox.TabIndex = 0;
            this.Username_TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.on_username_box_enter);
            // 
            // Name_Label
            // 
            this.Name_Label.AutoSize = true;
            this.Name_Label.Location = new System.Drawing.Point(14, 40);
            this.Name_Label.Name = "Name_Label";
            this.Name_Label.Size = new System.Drawing.Size(58, 13);
            this.Name_Label.TabIndex = 1;
            this.Name_Label.Text = "Username:";
            // 
            // Password_Label
            // 
            this.Password_Label.AutoSize = true;
            this.Password_Label.Location = new System.Drawing.Point(14, 66);
            this.Password_Label.Name = "Password_Label";
            this.Password_Label.Size = new System.Drawing.Size(56, 13);
            this.Password_Label.TabIndex = 3;
            this.Password_Label.Text = "Password:";
            // 
            // Password_TextBox
            // 
            this.Password_TextBox.Location = new System.Drawing.Point(106, 63);
            this.Password_TextBox.Name = "Password_TextBox";
            this.Password_TextBox.Size = new System.Drawing.Size(100, 20);
            this.Password_TextBox.TabIndex = 1;
            this.Password_TextBox.UseSystemPasswordChar = true;
            this.Password_TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.on_password_box_enter);
            // 
            // Submit_Button
            // 
            this.Submit_Button.Location = new System.Drawing.Point(106, 115);
            this.Submit_Button.Name = "Submit_Button";
            this.Submit_Button.Size = new System.Drawing.Size(100, 23);
            this.Submit_Button.TabIndex = 3;
            this.Submit_Button.Text = "Submit";
            this.Submit_Button.UseVisualStyleBackColor = true;
            this.Submit_Button.Click += new System.EventHandler(this.login);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Password Again:";
            // 
            // Password_Verify_TextBox
            // 
            this.Password_Verify_TextBox.Location = new System.Drawing.Point(106, 89);
            this.Password_Verify_TextBox.Name = "Password_Verify_TextBox";
            this.Password_Verify_TextBox.Size = new System.Drawing.Size(100, 20);
            this.Password_Verify_TextBox.TabIndex = 2;
            this.Password_Verify_TextBox.UseSystemPasswordChar = true;
            this.Password_Verify_TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.on_verify_password_box_enter);
            // 
            // Error_Label
            // 
            this.Error_Label.AutoSize = true;
            this.Error_Label.ForeColor = System.Drawing.Color.Red;
            this.Error_Label.Location = new System.Drawing.Point(65, 9);
            this.Error_Label.Name = "Error_Label";
            this.Error_Label.Size = new System.Drawing.Size(0, 13);
            this.Error_Label.TabIndex = 7;
            // 
            // SignupTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 174);
            this.Controls.Add(this.Error_Label);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Password_Verify_TextBox);
            this.Controls.Add(this.Submit_Button);
            this.Controls.Add(this.Password_Label);
            this.Controls.Add(this.Password_TextBox);
            this.Controls.Add(this.Name_Label);
            this.Controls.Add(this.Username_TextBox);
            this.Name = "SignupTool";
            this.Text = "Add new user";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Username_TextBox;
        private System.Windows.Forms.Label Name_Label;
        private System.Windows.Forms.Label Password_Label;
        private System.Windows.Forms.TextBox Password_TextBox;
        private System.Windows.Forms.Button Submit_Button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Password_Verify_TextBox;
        private System.Windows.Forms.Label Error_Label;
    }
}