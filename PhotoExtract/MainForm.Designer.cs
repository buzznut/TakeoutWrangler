//  <@$&< copyright begin >&$@> 8EF3F3608034F1A9CC6F945BA1A2053665BCA4FFC65BF31743F47CE665FDB0FB:20241017.A:2024:10:17:18:28
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// 
// This software application and source code is copyrighted and is licensed
// for use by you only. Only this product's installation files may be shared.
// 
// This license does not allow the removal or code changes that cause the
// ignoring, or modifying the copyright in any form.
// 
// This software is licensed "as is" and no warranty is implied or given.
// 
// Stewart A. Nutter
// 711 Indigo Ln
// Waunakee, WI  53597
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStripMainMenu = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemPrint = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            clearToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            listBoxView = new ListBox();
            timerView = new System.Windows.Forms.Timer(components);
            buttonRun = new Button();
            printDialog = new PrintDialog();
            printDocument = new System.Drawing.Printing.PrintDocument();
            menuStripMainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripMainMenu
            // 
            menuStripMainMenu.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem });
            menuStripMainMenu.Location = new Point(0, 0);
            menuStripMainMenu.Name = "menuStripMainMenu";
            menuStripMainMenu.Size = new Size(642, 24);
            menuStripMainMenu.TabIndex = 0;
            menuStripMainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemPrint, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItemPrint
            // 
            toolStripMenuItemPrint.Name = "toolStripMenuItemPrint";
            toolStripMenuItemPrint.Size = new Size(108, 22);
            toolStripMenuItemPrint.Text = "Print...";
            toolStripMenuItemPrint.Click += toolStripMenuItemPrint_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(105, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(108, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { clearToolStripMenuItem, toolStripSeparator2, settingsToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // clearToolStripMenuItem
            // 
            clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            clearToolStripMenuItem.Size = new Size(125, 22);
            clearToolStripMenuItem.Text = "Clear";
            clearToolStripMenuItem.Click += clearToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(122, 6);
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(125, 22);
            settingsToolStripMenuItem.Text = "Settings...";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
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
            listBoxView.Size = new Size(632, 298);
            listBoxView.TabIndex = 1;
            listBoxView.SizeChanged += listBoxView_SizeChanged;
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
            buttonRun.Location = new Point(525, 326);
            buttonRun.Name = "buttonRun";
            buttonRun.Size = new Size(112, 23);
            buttonRun.TabIndex = 2;
            buttonRun.Text = "Extract";
            buttonRun.UseVisualStyleBackColor = true;
            buttonRun.Click += buttonRun_Click;
            // 
            // printDialog
            // 
            printDialog.Document = printDocument;
            printDialog.UseEXDialog = true;
            // 
            // printDocument
            // 
            printDocument.PrintPage += printDocument_PrintPage;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(642, 353);
            Controls.Add(buttonRun);
            Controls.Add(listBoxView);
            Controls.Add(menuStripMainMenu);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStripMainMenu;
            MinimumSize = new Size(658, 392);
            Name = "MainForm";
            Text = "Takeout Wrangler";
            Load += MainForm_Load;
            Shown += MainForm_Shown;
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
        private ToolStripMenuItem clearToolStripMenuItem;
        private ListBox listBoxView;
        private System.Windows.Forms.Timer timerView;
        private Button buttonRun;
        private PrintDialog printDialog;
        private System.Drawing.Printing.PrintDocument printDocument;
        private ToolStripMenuItem toolStripMenuItemPrint;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem settingsToolStripMenuItem;
    }
}
