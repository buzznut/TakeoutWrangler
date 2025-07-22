//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace DecryptDisplay
{
    partial class DisplayForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayForm));
            menuStrip = new MenuStrip();
            saveToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItemPassword = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem1 = new ToolStripMenuItem();
            panelPicture = new Panel();
            richTextInfo = new RichTextBox();
            pictureBoxImage = new PictureBox();
            videoView = new LibVLCSharp.WinForms.VideoView();
            buttonNext = new Button();
            buttonPrevious = new Button();
            timerPasswordCheck = new System.Windows.Forms.Timer(components);
            saveFileDialog = new SaveFileDialog();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)videoView).BeginInit();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            menuStrip.Dock = DockStyle.None;
            menuStrip.Items.AddRange(new ToolStripItem[] { saveToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(97, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "Menu";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, saveAsToolStripMenuItem, toolStripSeparator1, toolStripMenuItemPassword, toolStripSeparator2, exitToolStripMenuItem });
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(37, 20);
            saveToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(182, 22);
            toolStripMenuItem1.Text = "Open...";
            toolStripMenuItem1.Click += fileToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(182, 22);
            saveAsToolStripMenuItem.Text = "Save decrypted file...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(179, 6);
            // 
            // toolStripMenuItemPassword
            // 
            toolStripMenuItemPassword.Name = "toolStripMenuItemPassword";
            toolStripMenuItemPassword.Size = new Size(182, 22);
            toolStripMenuItemPassword.Text = "Enter password...";
            toolStripMenuItemPassword.Click += toolStripMenuItemPassword_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(179, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(182, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem1 });
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            // 
            // aboutToolStripMenuItem1
            // 
            aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            aboutToolStripMenuItem1.Size = new Size(107, 22);
            aboutToolStripMenuItem1.Text = "About";
            aboutToolStripMenuItem1.Click += aboutToolStripMenuItem1_Click;
            // 
            // panelPicture
            // 
            panelPicture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelPicture.AutoScroll = true;
            panelPicture.BorderStyle = BorderStyle.FixedSingle;
            panelPicture.Location = new Point(34, 22);
            panelPicture.Name = "panelPicture";
            panelPicture.Size = new Size(726, 423);
            panelPicture.TabIndex = 1;
            // 
            // richTextInfo
            // 
            richTextInfo.Anchor = AnchorStyles.None;
            richTextInfo.BorderStyle = BorderStyle.FixedSingle;
            richTextInfo.Location = new Point(527, 250);
            richTextInfo.Name = "richTextInfo";
            richTextInfo.ReadOnly = true;
            richTextInfo.Size = new Size(118, 78);
            richTextInfo.TabIndex = 1;
            richTextInfo.TabStop = false;
            richTextInfo.Text = "Loading media file. Please wait...";
            // 
            // pictureBoxImage
            // 
            pictureBoxImage.Anchor = AnchorStyles.None;
            pictureBoxImage.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxImage.Location = new Point(187, 43);
            pictureBoxImage.Name = "pictureBoxImage";
            pictureBoxImage.Size = new Size(298, 83);
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImage.TabIndex = 0;
            pictureBoxImage.TabStop = false;
            // 
            // videoView
            // 
            videoView.BackColor = Color.Black;
            videoView.Location = new Point(531, 74);
            videoView.MediaPlayer = null;
            videoView.Name = "videoView";
            videoView.Size = new Size(196, 127);
            videoView.TabIndex = 2;
            videoView.Text = "videoView";
            // 
            // buttonNext
            // 
            buttonNext.Anchor = AnchorStyles.Right;
            buttonNext.Location = new Point(768, 204);
            buttonNext.Name = "buttonNext";
            buttonNext.Size = new Size(26, 23);
            buttonNext.TabIndex = 3;
            buttonNext.Text = ">";
            buttonNext.UseVisualStyleBackColor = true;
            buttonNext.Click += buttonNext_Click;
            // 
            // buttonPrevious
            // 
            buttonPrevious.Anchor = AnchorStyles.Left;
            buttonPrevious.Location = new Point(2, 204);
            buttonPrevious.Name = "buttonPrevious";
            buttonPrevious.Size = new Size(26, 23);
            buttonPrevious.TabIndex = 4;
            buttonPrevious.Text = "<";
            buttonPrevious.UseVisualStyleBackColor = true;
            buttonPrevious.Click += buttonPrevious_Click;
            // 
            // timerPasswordCheck
            // 
            timerPasswordCheck.Interval = 1000;
            timerPasswordCheck.Tick += timerPasswordCheck_Tick;
            // 
            // DisplayForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonPrevious);
            Controls.Add(buttonNext);
            Controls.Add(videoView);
            Controls.Add(pictureBoxImage);
            Controls.Add(richTextInfo);
            Controls.Add(panelPicture);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "DisplayForm";
            Text = "Takeout Wrangler Unlock";
            FormClosing += DisplayForm_FormClosing;
            Load += DisplayForm_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)videoView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private Panel panelPicture;
        private PictureBox pictureBoxImage;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem1;
        private RichTextBox richTextInfo;
        private LibVLCSharp.WinForms.VideoView videoView;
        private Button buttonNext;
        private Button buttonPrevious;
        private System.Windows.Forms.Timer timerPasswordCheck;
        private SaveFileDialog saveFileDialog;
        private ToolStripMenuItem toolStripMenuItemPassword;
        private ToolStripSeparator toolStripSeparator2;
    }
}
