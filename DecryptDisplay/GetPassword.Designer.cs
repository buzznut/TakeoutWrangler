//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace DecryptDisplay
{
    partial class GetPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetPassword));
            groupBoxPassword = new GroupBox();
            buttonShowConfirm = new Button();
            buttonShowPassword = new Button();
            textBoxConfirm = new TextBox();
            textBoxPassword = new TextBox();
            labelConfirm = new Label();
            labelPassword = new Label();
            buttonCancel = new Button();
            buttonOkay = new Button();
            groupBoxPassword.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxPassword
            // 
            groupBoxPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxPassword.Controls.Add(buttonShowConfirm);
            groupBoxPassword.Controls.Add(buttonShowPassword);
            groupBoxPassword.Controls.Add(textBoxConfirm);
            groupBoxPassword.Controls.Add(textBoxPassword);
            groupBoxPassword.Controls.Add(labelConfirm);
            groupBoxPassword.Controls.Add(labelPassword);
            groupBoxPassword.Location = new Point(6, 8);
            groupBoxPassword.Name = "groupBoxPassword";
            groupBoxPassword.Size = new Size(330, 87);
            groupBoxPassword.TabIndex = 0;
            groupBoxPassword.TabStop = false;
            groupBoxPassword.Text = "Enter data";
            // 
            // buttonShowConfirm
            // 
            buttonShowConfirm.Location = new Point(307, 55);
            buttonShowConfirm.Name = "buttonShowConfirm";
            buttonShowConfirm.Size = new Size(16, 18);
            buttonShowConfirm.TabIndex = 5;
            buttonShowConfirm.Text = "*";
            buttonShowConfirm.UseVisualStyleBackColor = true;
            buttonShowConfirm.MouseDown += buttonShowConfirm_MouseDown;
            buttonShowConfirm.MouseUp += buttonShowConfirm_MouseUp;
            // 
            // buttonShowPassword
            // 
            buttonShowPassword.Location = new Point(307, 26);
            buttonShowPassword.Name = "buttonShowPassword";
            buttonShowPassword.Size = new Size(16, 18);
            buttonShowPassword.TabIndex = 4;
            buttonShowPassword.Text = "*";
            buttonShowPassword.UseVisualStyleBackColor = true;
            buttonShowPassword.MouseDown += buttonShowPassword_MouseDown;
            buttonShowPassword.MouseUp += buttonShowPassword_MouseUp;
            // 
            // textBoxConfirm
            // 
            textBoxConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxConfirm.Location = new Point(90, 48);
            textBoxConfirm.Name = "textBoxConfirm";
            textBoxConfirm.Size = new Size(215, 23);
            textBoxConfirm.TabIndex = 3;
            textBoxConfirm.UseSystemPasswordChar = true;
            textBoxConfirm.TextChanged += textBox_TextChanged;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPassword.Location = new Point(90, 20);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(215, 23);
            textBoxPassword.TabIndex = 2;
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.TextChanged += textBox_TextChanged;
            // 
            // labelConfirm
            // 
            labelConfirm.AutoSize = true;
            labelConfirm.Location = new Point(30, 51);
            labelConfirm.Name = "labelConfirm";
            labelConfirm.Size = new Size(54, 15);
            labelConfirm.TabIndex = 1;
            labelConfirm.Text = "Confirm:";
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(24, 23);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(60, 15);
            labelPassword.TabIndex = 0;
            labelPassword.Text = "Password:";
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(261, 100);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonOkay
            // 
            buttonOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOkay.DialogResult = DialogResult.OK;
            buttonOkay.Location = new Point(181, 100);
            buttonOkay.Name = "buttonOkay";
            buttonOkay.Size = new Size(75, 23);
            buttonOkay.TabIndex = 2;
            buttonOkay.Text = "OK";
            buttonOkay.UseVisualStyleBackColor = true;
            buttonOkay.Click += buttonOkay_Click;
            // 
            // GetPassword
            // 
            AcceptButton = buttonOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(341, 127);
            Controls.Add(buttonOkay);
            Controls.Add(buttonCancel);
            Controls.Add(groupBoxPassword);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(357, 166);
            Name = "GetPassword";
            Text = "GetPassword";
            groupBoxPassword.ResumeLayout(false);
            groupBoxPassword.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxPassword;
        private TextBox textBoxConfirm;
        private TextBox textBoxPassword;
        private Label labelConfirm;
        private Label labelPassword;
        private Button buttonCancel;
        private Button buttonOkay;
        private Button buttonShowPassword;
        private Button buttonShowConfirm;
    }
}
