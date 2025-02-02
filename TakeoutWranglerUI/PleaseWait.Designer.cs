//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System.Drawing;
using System.Windows.Forms;

namespace TakeoutWranglerUI
{
    partial class PleaseWait
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PleaseWait));
            textBoxBody = new TextBox();
            SuspendLayout();
            // 
            // textBoxBody
            // 
            textBoxBody.BorderStyle = BorderStyle.None;
            textBoxBody.Dock = DockStyle.Fill;
            textBoxBody.Location = new Point(0, 0);
            textBoxBody.Multiline = true;
            textBoxBody.Name = "textBoxBody";
            textBoxBody.ReadOnly = true;
            textBoxBody.Size = new Size(338, 22);
            textBoxBody.TabIndex = 0;
            textBoxBody.TextAlign = HorizontalAlignment.Center;
            textBoxBody.UseWaitCursor = true;
            // 
            // PleaseWait
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(338, 22);
            ControlBox = false;
            Controls.Add(textBoxBody);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PleaseWait";
            Text = "Please wait ...";
            TopMost = true;
            UseWaitCursor = true;
            Activated += PleaseWait_Activated;
            Load += PleaseWait_Load;
            Shown += PleaseWait_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxBody;
    }
}
