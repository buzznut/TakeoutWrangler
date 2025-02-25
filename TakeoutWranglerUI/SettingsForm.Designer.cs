//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:2:25:8:47
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System.Drawing;
using System.Windows.Forms;

namespace TakeoutWranglerUI
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            labelSource = new Label();
            labelDestination = new Label();
            labelPattern = new Label();
            labelFilter = new Label();
            groupBoxSource = new GroupBox();
            labelArchiveFilter = new Label();
            buttonSourceDialog = new Button();
            labelSourceDescription = new Label();
            textBoxFileFilter = new TextBox();
            textBoxSource = new TextBox();
            groupBoxDestination = new GroupBox();
            buttonReorderBackupBrowse = new Button();
            textBoxReorderBackupFolder = new TextBox();
            labelReorderBackup = new Label();
            textBoxMediaFolderDescription = new TextBox();
            textBoxDestinationPattern = new TextBox();
            labelDestinationRootFolderDescription = new Label();
            buttonDestinationDialog = new Button();
            textBoxDestination = new TextBox();
            buttonCancel = new Button();
            buttonOkay = new Button();
            groupBoxAction = new GroupBox();
            labelJunkComma = new Label();
            labelJunkFiles = new Label();
            textBoxJunkFiles = new TextBox();
            checkBoxUseParallel = new CheckBox();
            textBoxActionDescription = new TextBox();
            checkBoxList = new CheckBox();
            comboBoxVerbosity = new ComboBox();
            labelVerbosity = new Label();
            labelActions = new Label();
            comboBoxActions = new ComboBox();
            groupBoxSource.SuspendLayout();
            groupBoxDestination.SuspendLayout();
            groupBoxAction.SuspendLayout();
            SuspendLayout();
            // 
            // labelSource
            // 
            labelSource.AutoSize = true;
            labelSource.Location = new Point(14, 41);
            labelSource.Name = "labelSource";
            labelSource.Size = new Size(58, 15);
            labelSource.TabIndex = 1;
            labelSource.Text = "Directory:";
            // 
            // labelDestination
            // 
            labelDestination.AutoSize = true;
            labelDestination.Location = new Point(14, 36);
            labelDestination.Name = "labelDestination";
            labelDestination.Size = new Size(77, 15);
            labelDestination.TabIndex = 1;
            labelDestination.Text = "Media folder:";
            // 
            // labelPattern
            // 
            labelPattern.AutoSize = true;
            labelPattern.Location = new Point(11, 88);
            labelPattern.Name = "labelPattern";
            labelPattern.Size = new Size(140, 15);
            labelPattern.TabIndex = 5;
            labelPattern.Text = "Sub-folder name pattern:";
            // 
            // labelFilter
            // 
            labelFilter.AutoSize = true;
            labelFilter.Location = new Point(17, 72);
            labelFilter.Name = "labelFilter";
            labelFilter.Size = new Size(55, 15);
            labelFilter.TabIndex = 4;
            labelFilter.Text = "File filter:";
            // 
            // groupBoxSource
            // 
            groupBoxSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxSource.Controls.Add(labelArchiveFilter);
            groupBoxSource.Controls.Add(buttonSourceDialog);
            groupBoxSource.Controls.Add(labelSourceDescription);
            groupBoxSource.Controls.Add(textBoxFileFilter);
            groupBoxSource.Controls.Add(textBoxSource);
            groupBoxSource.Controls.Add(labelSource);
            groupBoxSource.Controls.Add(labelFilter);
            groupBoxSource.Location = new Point(12, 165);
            groupBoxSource.Name = "groupBoxSource";
            groupBoxSource.Size = new Size(596, 116);
            groupBoxSource.TabIndex = 0;
            groupBoxSource.TabStop = false;
            groupBoxSource.Text = "Source files";
            // 
            // labelArchiveFilter
            // 
            labelArchiveFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelArchiveFilter.AutoSize = true;
            labelArchiveFilter.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelArchiveFilter.Location = new Point(245, 72);
            labelArchiveFilter.Name = "labelArchiveFilter";
            labelArchiveFilter.Size = new Size(215, 15);
            labelArchiveFilter.TabIndex = 6;
            labelArchiveFilter.Text = "Archive file filter (default: takeout-*.zip)";
            // 
            // buttonSourceDialog
            // 
            buttonSourceDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSourceDialog.Location = new Point(563, 38);
            buttonSourceDialog.Name = "buttonSourceDialog";
            buttonSourceDialog.Size = new Size(27, 23);
            buttonSourceDialog.TabIndex = 3;
            buttonSourceDialog.Text = "...";
            buttonSourceDialog.UseVisualStyleBackColor = true;
            buttonSourceDialog.Click += buttonSourceDialog_Click;
            // 
            // labelSourceDescription
            // 
            labelSourceDescription.AutoSize = true;
            labelSourceDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelSourceDescription.Location = new Point(78, 21);
            labelSourceDescription.Name = "labelSourceDescription";
            labelSourceDescription.Size = new Size(299, 15);
            labelSourceDescription.TabIndex = 0;
            labelSourceDescription.Text = "Folder that contains the archive zip file(s) 'takeout-*.zip'";
            // 
            // textBoxFileFilter
            // 
            textBoxFileFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxFileFilter.Location = new Point(78, 69);
            textBoxFileFilter.Name = "textBoxFileFilter";
            textBoxFileFilter.Size = new Size(160, 23);
            textBoxFileFilter.TabIndex = 5;
            // 
            // textBoxSource
            // 
            textBoxSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxSource.Location = new Point(78, 38);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(479, 23);
            textBoxSource.TabIndex = 2;
            textBoxSource.TextChanged += textBox_TextChanged;
            // 
            // groupBoxDestination
            // 
            groupBoxDestination.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxDestination.Controls.Add(buttonReorderBackupBrowse);
            groupBoxDestination.Controls.Add(textBoxReorderBackupFolder);
            groupBoxDestination.Controls.Add(labelReorderBackup);
            groupBoxDestination.Controls.Add(textBoxMediaFolderDescription);
            groupBoxDestination.Controls.Add(textBoxDestinationPattern);
            groupBoxDestination.Controls.Add(labelDestinationRootFolderDescription);
            groupBoxDestination.Controls.Add(buttonDestinationDialog);
            groupBoxDestination.Controls.Add(labelDestination);
            groupBoxDestination.Controls.Add(textBoxDestination);
            groupBoxDestination.Controls.Add(labelPattern);
            groupBoxDestination.Location = new Point(12, 288);
            groupBoxDestination.Name = "groupBoxDestination";
            groupBoxDestination.Size = new Size(596, 153);
            groupBoxDestination.TabIndex = 2;
            groupBoxDestination.TabStop = false;
            groupBoxDestination.Text = "Target";
            // 
            // buttonReorderBackupBrowse
            // 
            buttonReorderBackupBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonReorderBackupBrowse.Location = new Point(563, 116);
            buttonReorderBackupBrowse.Name = "buttonReorderBackupBrowse";
            buttonReorderBackupBrowse.Size = new Size(27, 23);
            buttonReorderBackupBrowse.TabIndex = 10;
            buttonReorderBackupBrowse.Text = "...";
            buttonReorderBackupBrowse.UseVisualStyleBackColor = true;
            buttonReorderBackupBrowse.Click += buttonReorderBackupBrowse_Click;
            // 
            // textBoxReorderBackupFolder
            // 
            textBoxReorderBackupFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxReorderBackupFolder.Location = new Point(146, 116);
            textBoxReorderBackupFolder.Name = "textBoxReorderBackupFolder";
            textBoxReorderBackupFolder.Size = new Size(410, 23);
            textBoxReorderBackupFolder.TabIndex = 9;
            // 
            // labelReorderBackup
            // 
            labelReorderBackup.AutoSize = true;
            labelReorderBackup.Location = new Point(11, 119);
            labelReorderBackup.Name = "labelReorderBackup";
            labelReorderBackup.Size = new Size(127, 15);
            labelReorderBackup.TabIndex = 8;
            labelReorderBackup.Text = "Reorder backup folder:";
            // 
            // textBoxMediaFolderDescription
            // 
            textBoxMediaFolderDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxMediaFolderDescription.BorderStyle = BorderStyle.None;
            textBoxMediaFolderDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            textBoxMediaFolderDescription.Location = new Point(173, 66);
            textBoxMediaFolderDescription.Name = "textBoxMediaFolderDescription";
            textBoxMediaFolderDescription.ReadOnly = true;
            textBoxMediaFolderDescription.Size = new Size(383, 16);
            textBoxMediaFolderDescription.TabIndex = 7;
            textBoxMediaFolderDescription.TabStop = false;
            textBoxMediaFolderDescription.Text = " Use: text, $y=year, $m=month, $d=day, $h=hour (media date).  example:  $y_$m";
            // 
            // textBoxDestinationPattern
            // 
            textBoxDestinationPattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDestinationPattern.Location = new Point(158, 85);
            textBoxDestinationPattern.Name = "textBoxDestinationPattern";
            textBoxDestinationPattern.Size = new Size(398, 23);
            textBoxDestinationPattern.TabIndex = 6;
            // 
            // labelDestinationRootFolderDescription
            // 
            labelDestinationRootFolderDescription.AutoSize = true;
            labelDestinationRootFolderDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelDestinationRootFolderDescription.Location = new Point(96, 16);
            labelDestinationRootFolderDescription.Name = "labelDestinationRootFolderDescription";
            labelDestinationRootFolderDescription.Size = new Size(232, 15);
            labelDestinationRootFolderDescription.TabIndex = 0;
            labelDestinationRootFolderDescription.Text = "Root folder for local media files and folders.";
            // 
            // buttonDestinationDialog
            // 
            buttonDestinationDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDestinationDialog.Location = new Point(563, 33);
            buttonDestinationDialog.Name = "buttonDestinationDialog";
            buttonDestinationDialog.Size = new Size(27, 23);
            buttonDestinationDialog.TabIndex = 3;
            buttonDestinationDialog.Text = "...";
            buttonDestinationDialog.UseVisualStyleBackColor = true;
            buttonDestinationDialog.Click += buttonDestinationDialog_Click;
            // 
            // textBoxDestination
            // 
            textBoxDestination.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDestination.Location = new Point(96, 33);
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.Size = new Size(461, 23);
            textBoxDestination.TabIndex = 2;
            textBoxDestination.TextChanged += textBox_TextChanged;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(533, 446);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOkay
            // 
            buttonOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOkay.DialogResult = DialogResult.OK;
            buttonOkay.Location = new Point(454, 446);
            buttonOkay.Name = "buttonOkay";
            buttonOkay.Size = new Size(75, 23);
            buttonOkay.TabIndex = 3;
            buttonOkay.Text = "OK";
            buttonOkay.UseVisualStyleBackColor = true;
            buttonOkay.Click += buttonOkay_Click;
            // 
            // groupBoxAction
            // 
            groupBoxAction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxAction.Controls.Add(labelJunkComma);
            groupBoxAction.Controls.Add(labelJunkFiles);
            groupBoxAction.Controls.Add(textBoxJunkFiles);
            groupBoxAction.Controls.Add(checkBoxUseParallel);
            groupBoxAction.Controls.Add(textBoxActionDescription);
            groupBoxAction.Controls.Add(checkBoxList);
            groupBoxAction.Controls.Add(comboBoxVerbosity);
            groupBoxAction.Controls.Add(labelVerbosity);
            groupBoxAction.Controls.Add(labelActions);
            groupBoxAction.Controls.Add(comboBoxActions);
            groupBoxAction.Location = new Point(12, 10);
            groupBoxAction.Name = "groupBoxAction";
            groupBoxAction.Size = new Size(596, 148);
            groupBoxAction.TabIndex = 5;
            groupBoxAction.TabStop = false;
            groupBoxAction.Text = "Actions";
            // 
            // labelJunkComma
            // 
            labelJunkComma.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelJunkComma.AutoSize = true;
            labelJunkComma.Location = new Point(872, 122);
            labelJunkComma.Name = "labelJunkComma";
            labelJunkComma.Size = new Size(110, 15);
            labelJunkComma.TabIndex = 11;
            labelJunkComma.Text = "(comma separated)";
            // 
            // labelJunkFiles
            // 
            labelJunkFiles.AutoSize = true;
            labelJunkFiles.Location = new Point(10, 122);
            labelJunkFiles.Name = "labelJunkFiles";
            labelJunkFiles.Size = new Size(111, 15);
            labelJunkFiles.TabIndex = 10;
            labelJunkFiles.Text = "Junk file extensions:";
            // 
            // textBoxJunkFiles
            // 
            textBoxJunkFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxJunkFiles.Location = new Point(129, 117);
            textBoxJunkFiles.Name = "textBoxJunkFiles";
            textBoxJunkFiles.Size = new Size(737, 23);
            textBoxJunkFiles.TabIndex = 9;
            // 
            // checkBoxUseParallel
            // 
            checkBoxUseParallel.AutoSize = true;
            checkBoxUseParallel.Location = new Point(82, 13);
            checkBoxUseParallel.Name = "checkBoxUseParallel";
            checkBoxUseParallel.Size = new Size(303, 19);
            checkBoxUseParallel.TabIndex = 6;
            checkBoxUseParallel.Text = "Run in parallel - uses more memory for performance";
            checkBoxUseParallel.UseVisualStyleBackColor = true;
            // 
            // textBoxActionDescription
            // 
            textBoxActionDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxActionDescription.BorderStyle = BorderStyle.FixedSingle;
            textBoxActionDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            textBoxActionDescription.Location = new Point(10, 88);
            textBoxActionDescription.Multiline = true;
            textBoxActionDescription.Name = "textBoxActionDescription";
            textBoxActionDescription.Size = new Size(973, 23);
            textBoxActionDescription.TabIndex = 5;
            textBoxActionDescription.Text = "(action description)";
            // 
            // checkBoxList
            // 
            checkBoxList.AutoSize = true;
            checkBoxList.Checked = true;
            checkBoxList.CheckState = CheckState.Checked;
            checkBoxList.Location = new Point(82, 35);
            checkBoxList.Name = "checkBoxList";
            checkBoxList.Size = new Size(174, 19);
            checkBoxList.TabIndex = 4;
            checkBoxList.Text = "List only - make no changes";
            checkBoxList.UseVisualStyleBackColor = true;
            // 
            // comboBoxVerbosity
            // 
            comboBoxVerbosity.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxVerbosity.FormattingEnabled = true;
            comboBoxVerbosity.Location = new Point(288, 59);
            comboBoxVerbosity.Name = "comboBoxVerbosity";
            comboBoxVerbosity.Size = new Size(121, 23);
            comboBoxVerbosity.TabIndex = 3;
            // 
            // labelVerbosity
            // 
            labelVerbosity.AutoSize = true;
            labelVerbosity.Location = new Point(219, 63);
            labelVerbosity.Name = "labelVerbosity";
            labelVerbosity.Size = new Size(58, 15);
            labelVerbosity.TabIndex = 2;
            labelVerbosity.Text = "Verbosity:";
            // 
            // labelActions
            // 
            labelActions.AutoSize = true;
            labelActions.Location = new Point(23, 63);
            labelActions.Name = "labelActions";
            labelActions.Size = new Size(50, 15);
            labelActions.TabIndex = 0;
            labelActions.Text = "Actions:";
            // 
            // comboBoxActions
            // 
            comboBoxActions.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxActions.FormattingEnabled = true;
            comboBoxActions.Location = new Point(82, 59);
            comboBoxActions.Name = "comboBoxActions";
            comboBoxActions.Size = new Size(121, 23);
            comboBoxActions.TabIndex = 1;
            comboBoxActions.SelectedIndexChanged += comboBoxActions_SelectedIndexChanged;
            // 
            // SettingsForm
            // 
            AcceptButton = buttonOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(620, 475);
            Controls.Add(groupBoxAction);
            Controls.Add(buttonOkay);
            Controls.Add(buttonCancel);
            Controls.Add(groupBoxDestination);
            Controls.Add(groupBoxSource);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(636, 514);
            Name = "SettingsForm";
            Text = "Takeout Wrangler Settings";
            Load += SettingsForm_Load;
            groupBoxSource.ResumeLayout(false);
            groupBoxSource.PerformLayout();
            groupBoxDestination.ResumeLayout(false);
            groupBoxDestination.PerformLayout();
            groupBoxAction.ResumeLayout(false);
            groupBoxAction.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label labelSource;
        private Label labelDestination;
        private Label labelPattern;
        private Label labelFilter;
        private GroupBox groupBoxSource;
        private GroupBox groupBoxDestination;
        private TextBox textBoxSource;
        private TextBox textBoxFileFilter;
        private Label labelArchiveFilter;
        private Button buttonSourceDialog;
        private Label labelSourceDescription;
        private Button buttonDestinationDialog;
        private TextBox textBoxDestination;
        private TextBox textBoxDestinationPattern;
        private Label labelDestinationRootFolderDescription;
        private Button buttonCancel;
        private Button buttonOkay;
        private TextBox textBoxMediaFolderDescription;
        private Button buttonReorderBackupBrowse;
        private TextBox textBoxReorderBackupFolder;
        private Label labelReorderBackup;
        private GroupBox groupBoxAction;
        private Label labelJunkComma;
        private Label labelJunkFiles;
        private TextBox textBoxJunkFiles;
        private CheckBox checkBoxUseParallel;
        private TextBox textBoxActionDescription;
        private CheckBox checkBoxList;
        private ComboBox comboBoxVerbosity;
        private Label labelVerbosity;
        private Label labelActions;
        private ComboBox comboBoxActions;
    }
}
