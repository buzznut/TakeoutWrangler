//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:11:16:13:40
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace TakeoutWrangler
{
    partial class MainForm
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
            if (disposing)
            {
                if ((components != null))
                {
                    components.Dispose();
                }

                foreach (Stream[] streams in helpStreams.Values)
                {
                    if (streams == null) continue;

                    foreach (Stream stream in streams)
                    {
                        stream?.Dispose();
                    }
                }

                helpStreams.Clear();
                worker?.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStripMainMenu = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuTakeout = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            clearToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemPrint = new ToolStripMenuItem();
            toolStripSaveConsole = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItemHelp = new ToolStripMenuItem();
            howToUseGoogleTakeoutToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            listBoxView = new ListBox();
            timerView = new System.Windows.Forms.Timer(components);
            buttonRun = new Button();
            printDialog = new PrintDialog();
            printDocument = new System.Drawing.Printing.PrintDocument();
            textBoxStatus = new TextBox();
            labelProgress = new Label();
            timerIsRunning = new System.Windows.Forms.Timer(components);
            menuStripMainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripMainMenu
            // 
            menuStripMainMenu.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, helpToolStripMenuItem });
            menuStripMainMenu.Location = new Point(0, 0);
            menuStripMainMenu.Name = "menuStripMainMenu";
            menuStripMainMenu.Size = new Size(642, 24);
            menuStripMainMenu.TabIndex = 0;
            menuStripMainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuTakeout, toolStripSeparator2, clearToolStripMenuItem, toolStripMenuItemPrint, toolStripSaveConsole, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuTakeout
            // 
            toolStripMenuTakeout.Name = "toolStripMenuTakeout";
            toolStripMenuTakeout.Size = new Size(201, 22);
            toolStripMenuTakeout.Text = "Google Takeout...";
            toolStripMenuTakeout.Click += toolStripMenuTakeout_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(198, 6);
            // 
            // clearToolStripMenuItem
            // 
            clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            clearToolStripMenuItem.Size = new Size(201, 22);
            clearToolStripMenuItem.Text = "Clear console";
            clearToolStripMenuItem.Click += clearToolStripMenuItem_Click;
            // 
            // toolStripMenuItemPrint
            // 
            toolStripMenuItemPrint.Name = "toolStripMenuItemPrint";
            toolStripMenuItemPrint.Size = new Size(201, 22);
            toolStripMenuItemPrint.Text = "Print console contents...";
            toolStripMenuItemPrint.Click += toolStripMenuItemPrint_Click;
            // 
            // toolStripSaveConsole
            // 
            toolStripSaveConsole.Name = "toolStripSaveConsole";
            toolStripSaveConsole.Size = new Size(201, 22);
            toolStripSaveConsole.Text = "Save console contents...";
            toolStripSaveConsole.Click += toolStripSaveConsole_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(198, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(201, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { settingsToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(93, 20);
            viewToolStripMenuItem.Text = "Configuration";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(125, 22);
            settingsToolStripMenuItem.Text = "Settings...";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { helpToolStripMenuItemHelp, howToUseGoogleTakeoutToolStripMenuItem, toolStripSeparator3, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItemHelp
            // 
            helpToolStripMenuItemHelp.Name = "helpToolStripMenuItemHelp";
            helpToolStripMenuItemHelp.Size = new Size(229, 22);
            helpToolStripMenuItemHelp.Text = "Help...";
            helpToolStripMenuItemHelp.Click += helpToolStripMenuItemHelp_Click;
            // 
            // howToUseGoogleTakeoutToolStripMenuItem
            // 
            howToUseGoogleTakeoutToolStripMenuItem.Name = "howToUseGoogleTakeoutToolStripMenuItem";
            howToUseGoogleTakeoutToolStripMenuItem.Size = new Size(229, 22);
            howToUseGoogleTakeoutToolStripMenuItem.Text = "How to use Google Takeout...";
            howToUseGoogleTakeoutToolStripMenuItem.Click += howToUseGoogleTakeoutToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(226, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(229, 22);
            aboutToolStripMenuItem.Text = "About...";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // listBoxView
            // 
            listBoxView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxView.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBoxView.FormattingEnabled = true;
            listBoxView.HorizontalScrollbar = true;
            listBoxView.ItemHeight = 14;
            listBoxView.Location = new Point(5, 24);
            listBoxView.Name = "listBoxView";
            listBoxView.SelectionMode = SelectionMode.None;
            listBoxView.Size = new Size(632, 284);
            listBoxView.TabIndex = 1;
            listBoxView.SizeChanged += listBoxView_SizeChanged;
            listBoxView.KeyDown += listBoxView_KeyDown;
            // 
            // timerView
            // 
            timerView.Interval = 200;
            timerView.Tick += timerView_Tick;
            // 
            // buttonRun
            // 
            buttonRun.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonRun.Enabled = false;
            buttonRun.Location = new Point(525, 321);
            buttonRun.Name = "buttonRun";
            buttonRun.Size = new Size(112, 23);
            buttonRun.TabIndex = 2;
            buttonRun.Text = "Execute";
            buttonRun.UseVisualStyleBackColor = true;
            buttonRun.Click += buttonRun_Click;
            // 
            // printDialog
            // 
            printDialog.Document = printDocument;
            // 
            // printDocument
            // 
            printDocument.PrintPage += printDocument_PrintPage;
            // 
            // textBoxStatus
            // 
            textBoxStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxStatus.BorderStyle = BorderStyle.FixedSingle;
            textBoxStatus.Location = new Point(69, 322);
            textBoxStatus.Name = "textBoxStatus";
            textBoxStatus.ReadOnly = true;
            textBoxStatus.Size = new Size(101, 23);
            textBoxStatus.TabIndex = 3;
            textBoxStatus.TextAlign = HorizontalAlignment.Center;
            // 
            // labelProgress
            // 
            labelProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelProgress.AutoSize = true;
            labelProgress.Location = new Point(5, 324);
            labelProgress.Name = "labelProgress";
            labelProgress.Size = new Size(55, 15);
            labelProgress.TabIndex = 4;
            labelProgress.Text = "Progress:";
            // 
            // timerIsRunning
            // 
            timerIsRunning.Enabled = true;
            timerIsRunning.Interval = 250;
            timerIsRunning.Tick += timerIsRunning_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(642, 353);
            Controls.Add(labelProgress);
            Controls.Add(textBoxStatus);
            Controls.Add(buttonRun);
            Controls.Add(listBoxView);
            Controls.Add(menuStripMainMenu);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStripMainMenu;
            MinimumSize = new Size(658, 392);
            Name = "MainForm";
            Text = "Takeout Wrangler";
            Load += MainForm_Load;
            menuStripMainMenu.ResumeLayout(false);
            menuStripMainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStripMainMenu;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ListBox listBoxView;
        private System.Windows.Forms.Timer timerView;
        private Button buttonRun;
        private PrintDialog printDialog;
        private System.Drawing.Printing.PrintDocument printDocument;
        private ToolStripMenuItem toolStripMenuItemPrint;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem clearToolStripMenuItem;
        private TextBox textBoxStatus;
        private Label labelProgress;
        private System.Windows.Forms.Timer timerIsRunning;
        private ToolStripMenuItem toolStripSaveConsole;
        private ToolStripMenuItem toolStripMenuTakeout;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItemHelp;
        private ToolStripMenuItem howToUseGoogleTakeoutToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem aboutToolStripMenuItem;
    }
}
