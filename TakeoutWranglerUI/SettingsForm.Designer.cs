//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
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
            buttonShowConfirm = new Button();
            buttonShowPassword = new Button();
            labelLast = new Label();
            labelConfirm = new Label();
            textBoxConfirm = new TextBox();
            labelPassword = new Label();
            textBoxPassword = new TextBox();
            checkBoxKeepLocked = new CheckBox();
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
            tabControlSettings = new TabControl();
            tabActions = new TabPage();
            tabPageContent = new TabPage();
            groupBoxContent = new GroupBox();
            checkBoxDoOther = new CheckBox();
            checkBoxMail = new CheckBox();
            checkBoxMedia = new CheckBox();
            tabSource = new TabPage();
            tabTarget = new TabPage();
            tabMail = new TabPage();
            groupBoxMailFolders = new GroupBox();
            checkBoxInbox = new CheckBox();
            checkBoxArchived = new CheckBox();
            checkBoxSent = new CheckBox();
            checkBoxSpam = new CheckBox();
            checkBoxTrash = new CheckBox();
            checkBoxOther = new CheckBox();
            groupBoxSource.SuspendLayout();
            groupBoxDestination.SuspendLayout();
            groupBoxAction.SuspendLayout();
            tabControlSettings.SuspendLayout();
            tabActions.SuspendLayout();
            tabPageContent.SuspendLayout();
            groupBoxContent.SuspendLayout();
            tabSource.SuspendLayout();
            tabTarget.SuspendLayout();
            tabMail.SuspendLayout();
            groupBoxMailFolders.SuspendLayout();
            SuspendLayout();
            // 
            // labelSource
            // 
            labelSource.AutoSize = true;
            labelSource.Location = new Point(14, 35);
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
            labelDestination.Size = new Size(97, 15);
            labelDestination.TabIndex = 1;
            labelDestination.Text = "Local root folder:";
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
            labelFilter.Location = new Point(17, 63);
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
            groupBoxSource.Location = new Point(8, 6);
            groupBoxSource.Name = "groupBoxSource";
            groupBoxSource.Size = new Size(658, 92);
            groupBoxSource.TabIndex = 0;
            groupBoxSource.TabStop = false;
            groupBoxSource.Text = "Source files";
            // 
            // labelArchiveFilter
            // 
            labelArchiveFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelArchiveFilter.AutoSize = true;
            labelArchiveFilter.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelArchiveFilter.Location = new Point(307, 63);
            labelArchiveFilter.Name = "labelArchiveFilter";
            labelArchiveFilter.Size = new Size(215, 15);
            labelArchiveFilter.TabIndex = 6;
            labelArchiveFilter.Text = "Archive file filter (default: takeout-*.zip)";
            // 
            // buttonSourceDialog
            // 
            buttonSourceDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSourceDialog.Location = new Point(625, 32);
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
            labelSourceDescription.Location = new Point(78, 17);
            labelSourceDescription.Name = "labelSourceDescription";
            labelSourceDescription.Size = new Size(299, 15);
            labelSourceDescription.TabIndex = 0;
            labelSourceDescription.Text = "Folder that contains the archive zip file(s) 'takeout-*.zip'";
            // 
            // textBoxFileFilter
            // 
            textBoxFileFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxFileFilter.Location = new Point(78, 60);
            textBoxFileFilter.Name = "textBoxFileFilter";
            textBoxFileFilter.Size = new Size(222, 23);
            textBoxFileFilter.TabIndex = 5;
            // 
            // textBoxSource
            // 
            textBoxSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxSource.Location = new Point(78, 32);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(541, 23);
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
            groupBoxDestination.Location = new Point(8, 13);
            groupBoxDestination.Name = "groupBoxDestination";
            groupBoxDestination.Size = new Size(656, 153);
            groupBoxDestination.TabIndex = 2;
            groupBoxDestination.TabStop = false;
            groupBoxDestination.Text = "Target";
            // 
            // buttonReorderBackupBrowse
            // 
            buttonReorderBackupBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonReorderBackupBrowse.Location = new Point(623, 116);
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
            textBoxReorderBackupFolder.Size = new Size(470, 23);
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
            textBoxMediaFolderDescription.Location = new Point(158, 66);
            textBoxMediaFolderDescription.Name = "textBoxMediaFolderDescription";
            textBoxMediaFolderDescription.ReadOnly = true;
            textBoxMediaFolderDescription.Size = new Size(443, 16);
            textBoxMediaFolderDescription.TabIndex = 7;
            textBoxMediaFolderDescription.TabStop = false;
            textBoxMediaFolderDescription.Text = " Use: text, $y=year, $m=month, $d=day, $h=hour (media date).  example:  $y_$m";
            // 
            // textBoxDestinationPattern
            // 
            textBoxDestinationPattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDestinationPattern.Location = new Point(158, 85);
            textBoxDestinationPattern.Name = "textBoxDestinationPattern";
            textBoxDestinationPattern.Size = new Size(458, 23);
            textBoxDestinationPattern.TabIndex = 6;
            // 
            // labelDestinationRootFolderDescription
            // 
            labelDestinationRootFolderDescription.AutoSize = true;
            labelDestinationRootFolderDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelDestinationRootFolderDescription.Location = new Point(117, 16);
            labelDestinationRootFolderDescription.Name = "labelDestinationRootFolderDescription";
            labelDestinationRootFolderDescription.Size = new Size(196, 15);
            labelDestinationRootFolderDescription.TabIndex = 0;
            labelDestinationRootFolderDescription.Text = "Root folder for local files and folders.";
            // 
            // buttonDestinationDialog
            // 
            buttonDestinationDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDestinationDialog.Location = new Point(623, 33);
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
            textBoxDestination.Location = new Point(117, 33);
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.Size = new Size(497, 23);
            textBoxDestination.TabIndex = 2;
            textBoxDestination.TextChanged += textBox_TextChanged;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(617, 287);
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
            buttonOkay.Location = new Point(538, 287);
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
            groupBoxAction.Controls.Add(buttonShowConfirm);
            groupBoxAction.Controls.Add(buttonShowPassword);
            groupBoxAction.Controls.Add(labelLast);
            groupBoxAction.Controls.Add(labelConfirm);
            groupBoxAction.Controls.Add(textBoxConfirm);
            groupBoxAction.Controls.Add(labelPassword);
            groupBoxAction.Controls.Add(textBoxPassword);
            groupBoxAction.Controls.Add(checkBoxKeepLocked);
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
            groupBoxAction.Location = new Point(6, 6);
            groupBoxAction.Name = "groupBoxAction";
            groupBoxAction.Size = new Size(660, 193);
            groupBoxAction.TabIndex = 5;
            groupBoxAction.TabStop = false;
            groupBoxAction.Text = "Actions";
            // 
            // buttonShowConfirm
            // 
            buttonShowConfirm.Location = new Point(590, 49);
            buttonShowConfirm.Name = "buttonShowConfirm";
            buttonShowConfirm.Size = new Size(16, 18);
            buttonShowConfirm.TabIndex = 7;
            buttonShowConfirm.Text = "*";
            buttonShowConfirm.UseVisualStyleBackColor = true;
            buttonShowConfirm.MouseDown += buttonShowConfirm_MouseDown;
            buttonShowConfirm.MouseUp += buttonShowConfirm_MouseUp;
            // 
            // buttonShowPassword
            // 
            buttonShowPassword.Location = new Point(359, 49);
            buttonShowPassword.Name = "buttonShowPassword";
            buttonShowPassword.Size = new Size(16, 18);
            buttonShowPassword.TabIndex = 18;
            buttonShowPassword.Text = "*";
            buttonShowPassword.UseVisualStyleBackColor = true;
            buttonShowPassword.MouseDown += buttonShowPassword_MouseDown;
            buttonShowPassword.MouseUp += buttonShowPassword_MouseUp;
            // 
            // labelLast
            // 
            labelLast.AutoSize = true;
            labelLast.Enabled = false;
            labelLast.Location = new Point(608, 51);
            labelLast.Name = "labelLast";
            labelLast.Size = new Size(11, 15);
            labelLast.TabIndex = 17;
            labelLast.Text = ")";
            // 
            // labelConfirm
            // 
            labelConfirm.AutoSize = true;
            labelConfirm.Enabled = false;
            labelConfirm.Location = new Point(387, 51);
            labelConfirm.Name = "labelConfirm";
            labelConfirm.Size = new Size(52, 15);
            labelConfirm.TabIndex = 16;
            labelConfirm.Text = "confirm:";
            // 
            // textBoxConfirm
            // 
            textBoxConfirm.Enabled = false;
            textBoxConfirm.Location = new Point(443, 47);
            textBoxConfirm.Name = "textBoxConfirm";
            textBoxConfirm.Size = new Size(144, 23);
            textBoxConfirm.TabIndex = 15;
            textBoxConfirm.UseSystemPasswordChar = true;
            textBoxConfirm.TextChanged += password_TextChanged;
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Enabled = false;
            labelPassword.Location = new Point(143, 51);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(67, 15);
            labelPassword.TabIndex = 14;
            labelPassword.Text = "( password:";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Enabled = false;
            textBoxPassword.Location = new Point(211, 47);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(144, 23);
            textBoxPassword.TabIndex = 13;
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.TextChanged += password_TextChanged;
            // 
            // checkBoxKeepLocked
            // 
            checkBoxKeepLocked.AutoSize = true;
            checkBoxKeepLocked.Location = new Point(10, 49);
            checkBoxKeepLocked.Name = "checkBoxKeepLocked";
            checkBoxKeepLocked.Size = new Size(129, 19);
            checkBoxKeepLocked.TabIndex = 12;
            checkBoxKeepLocked.Text = "Keep locked folders";
            checkBoxKeepLocked.UseVisualStyleBackColor = true;
            checkBoxKeepLocked.CheckedChanged += checkBoxKeepLocked_CheckedChanged;
            // 
            // labelJunkComma
            // 
            labelJunkComma.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelJunkComma.AutoSize = true;
            labelJunkComma.Location = new Point(131, 137);
            labelJunkComma.Name = "labelJunkComma";
            labelJunkComma.Size = new Size(110, 15);
            labelJunkComma.TabIndex = 11;
            labelJunkComma.Text = "(comma separated)";
            // 
            // labelJunkFiles
            // 
            labelJunkFiles.AutoSize = true;
            labelJunkFiles.Location = new Point(10, 163);
            labelJunkFiles.Name = "labelJunkFiles";
            labelJunkFiles.Size = new Size(111, 15);
            labelJunkFiles.TabIndex = 10;
            labelJunkFiles.Text = "Junk file extensions:";
            // 
            // textBoxJunkFiles
            // 
            textBoxJunkFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxJunkFiles.Location = new Point(129, 158);
            textBoxJunkFiles.Name = "textBoxJunkFiles";
            textBoxJunkFiles.Size = new Size(525, 23);
            textBoxJunkFiles.TabIndex = 9;
            // 
            // checkBoxUseParallel
            // 
            checkBoxUseParallel.AutoSize = true;
            checkBoxUseParallel.Location = new Point(10, 13);
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
            textBoxActionDescription.Location = new Point(10, 108);
            textBoxActionDescription.Multiline = true;
            textBoxActionDescription.Name = "textBoxActionDescription";
            textBoxActionDescription.Size = new Size(644, 23);
            textBoxActionDescription.TabIndex = 5;
            textBoxActionDescription.Text = "(action description)";
            // 
            // checkBoxList
            // 
            checkBoxList.AutoSize = true;
            checkBoxList.Checked = true;
            checkBoxList.CheckState = CheckState.Checked;
            checkBoxList.Location = new Point(10, 30);
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
            comboBoxVerbosity.Location = new Point(278, 79);
            comboBoxVerbosity.Name = "comboBoxVerbosity";
            comboBoxVerbosity.Size = new Size(121, 23);
            comboBoxVerbosity.TabIndex = 3;
            // 
            // labelVerbosity
            // 
            labelVerbosity.AutoSize = true;
            labelVerbosity.Location = new Point(209, 83);
            labelVerbosity.Name = "labelVerbosity";
            labelVerbosity.Size = new Size(58, 15);
            labelVerbosity.TabIndex = 2;
            labelVerbosity.Text = "Verbosity:";
            // 
            // labelActions
            // 
            labelActions.AutoSize = true;
            labelActions.Location = new Point(13, 83);
            labelActions.Name = "labelActions";
            labelActions.Size = new Size(50, 15);
            labelActions.TabIndex = 0;
            labelActions.Text = "Actions:";
            // 
            // comboBoxActions
            // 
            comboBoxActions.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxActions.FormattingEnabled = true;
            comboBoxActions.Location = new Point(72, 79);
            comboBoxActions.Name = "comboBoxActions";
            comboBoxActions.Size = new Size(121, 23);
            comboBoxActions.TabIndex = 1;
            comboBoxActions.SelectedIndexChanged += comboBoxActions_SelectedIndexChanged;
            // 
            // tabControlSettings
            // 
            tabControlSettings.Appearance = TabAppearance.FlatButtons;
            tabControlSettings.Controls.Add(tabActions);
            tabControlSettings.Controls.Add(tabPageContent);
            tabControlSettings.Controls.Add(tabSource);
            tabControlSettings.Controls.Add(tabTarget);
            tabControlSettings.Controls.Add(tabMail);
            tabControlSettings.Location = new Point(12, 12);
            tabControlSettings.Name = "tabControlSettings";
            tabControlSettings.SelectedIndex = 0;
            tabControlSettings.Size = new Size(680, 267);
            tabControlSettings.TabIndex = 6;
            // 
            // tabActions
            // 
            tabActions.Controls.Add(groupBoxAction);
            tabActions.Location = new Point(4, 27);
            tabActions.Name = "tabActions";
            tabActions.Padding = new Padding(3);
            tabActions.Size = new Size(672, 236);
            tabActions.TabIndex = 0;
            tabActions.Text = "Actions";
            tabActions.UseVisualStyleBackColor = true;
            // 
            // tabPageContent
            // 
            tabPageContent.Controls.Add(groupBoxContent);
            tabPageContent.Location = new Point(4, 27);
            tabPageContent.Name = "tabPageContent";
            tabPageContent.Padding = new Padding(3);
            tabPageContent.Size = new Size(672, 236);
            tabPageContent.TabIndex = 4;
            tabPageContent.Text = "Content";
            tabPageContent.UseVisualStyleBackColor = true;
            // 
            // groupBoxContent
            // 
            groupBoxContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxContent.Controls.Add(checkBoxDoOther);
            groupBoxContent.Controls.Add(checkBoxMail);
            groupBoxContent.Controls.Add(checkBoxMedia);
            groupBoxContent.Location = new Point(6, 6);
            groupBoxContent.Name = "groupBoxContent";
            groupBoxContent.Size = new Size(661, 88);
            groupBoxContent.TabIndex = 1;
            groupBoxContent.TabStop = false;
            groupBoxContent.Text = "Content handing";
            // 
            // checkBoxDoOther
            // 
            checkBoxDoOther.AutoSize = true;
            checkBoxDoOther.Location = new Point(7, 58);
            checkBoxDoOther.Name = "checkBoxDoOther";
            checkBoxDoOther.Size = new Size(127, 19);
            checkBoxDoOther.TabIndex = 2;
            checkBoxDoOther.Text = "Keep other content";
            checkBoxDoOther.UseVisualStyleBackColor = true;
            // 
            // checkBoxMail
            // 
            checkBoxMail.AutoSize = true;
            checkBoxMail.Location = new Point(7, 39);
            checkBoxMail.Name = "checkBoxMail";
            checkBoxMail.Size = new Size(128, 19);
            checkBoxMail.TabIndex = 1;
            checkBoxMail.Text = "Keep email content";
            checkBoxMail.UseVisualStyleBackColor = true;
            // 
            // checkBoxMedia
            // 
            checkBoxMedia.AutoSize = true;
            checkBoxMedia.Location = new Point(7, 20);
            checkBoxMedia.Name = "checkBoxMedia";
            checkBoxMedia.Size = new Size(169, 19);
            checkBoxMedia.TabIndex = 0;
            checkBoxMedia.Text = "Keep photo/media content";
            checkBoxMedia.UseVisualStyleBackColor = true;
            // 
            // tabSource
            // 
            tabSource.Controls.Add(groupBoxSource);
            tabSource.Location = new Point(4, 27);
            tabSource.Name = "tabSource";
            tabSource.Padding = new Padding(3);
            tabSource.Size = new Size(672, 236);
            tabSource.TabIndex = 1;
            tabSource.Text = "Source";
            tabSource.UseVisualStyleBackColor = true;
            // 
            // tabTarget
            // 
            tabTarget.Controls.Add(groupBoxDestination);
            tabTarget.Location = new Point(4, 27);
            tabTarget.Name = "tabTarget";
            tabTarget.Size = new Size(672, 236);
            tabTarget.TabIndex = 2;
            tabTarget.Text = "Target";
            tabTarget.UseVisualStyleBackColor = true;
            // 
            // tabMail
            // 
            tabMail.Controls.Add(groupBoxMailFolders);
            tabMail.Location = new Point(4, 27);
            tabMail.Name = "tabMail";
            tabMail.Padding = new Padding(3);
            tabMail.Size = new Size(672, 236);
            tabMail.TabIndex = 3;
            tabMail.Text = "Mail";
            tabMail.UseVisualStyleBackColor = true;
            // 
            // groupBoxMailFolders
            // 
            groupBoxMailFolders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxMailFolders.Controls.Add(checkBoxOther);
            groupBoxMailFolders.Controls.Add(checkBoxInbox);
            groupBoxMailFolders.Controls.Add(checkBoxArchived);
            groupBoxMailFolders.Controls.Add(checkBoxSent);
            groupBoxMailFolders.Controls.Add(checkBoxSpam);
            groupBoxMailFolders.Controls.Add(checkBoxTrash);
            groupBoxMailFolders.Location = new Point(5, 5);
            groupBoxMailFolders.Name = "groupBoxMailFolders";
            groupBoxMailFolders.Size = new Size(661, 139);
            groupBoxMailFolders.TabIndex = 0;
            groupBoxMailFolders.TabStop = false;
            groupBoxMailFolders.Text = "Folder handing";
            // 
            // checkBoxInbox
            // 
            checkBoxInbox.AutoSize = true;
            checkBoxInbox.Location = new Point(7, 96);
            checkBoxInbox.Name = "checkBoxInbox";
            checkBoxInbox.Size = new Size(133, 19);
            checkBoxInbox.TabIndex = 4;
            checkBoxInbox.Text = "Keep inbox contents";
            checkBoxInbox.UseVisualStyleBackColor = true;
            // 
            // checkBoxArchived
            // 
            checkBoxArchived.AutoSize = true;
            checkBoxArchived.Location = new Point(7, 77);
            checkBoxArchived.Name = "checkBoxArchived";
            checkBoxArchived.Size = new Size(149, 19);
            checkBoxArchived.TabIndex = 3;
            checkBoxArchived.Text = "Keep archived contents";
            checkBoxArchived.UseVisualStyleBackColor = true;
            // 
            // checkBoxSent
            // 
            checkBoxSent.AutoSize = true;
            checkBoxSent.Location = new Point(7, 58);
            checkBoxSent.Name = "checkBoxSent";
            checkBoxSent.Size = new Size(126, 19);
            checkBoxSent.TabIndex = 2;
            checkBoxSent.Text = "Keep sent contents";
            checkBoxSent.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpam
            // 
            checkBoxSpam.AutoSize = true;
            checkBoxSpam.Location = new Point(7, 39);
            checkBoxSpam.Name = "checkBoxSpam";
            checkBoxSpam.Size = new Size(133, 19);
            checkBoxSpam.TabIndex = 1;
            checkBoxSpam.Text = "Keep spam contents";
            checkBoxSpam.UseVisualStyleBackColor = true;
            // 
            // checkBoxTrash
            // 
            checkBoxTrash.AutoSize = true;
            checkBoxTrash.Location = new Point(7, 20);
            checkBoxTrash.Name = "checkBoxTrash";
            checkBoxTrash.Size = new Size(130, 19);
            checkBoxTrash.TabIndex = 0;
            checkBoxTrash.Text = "Keep trash contents";
            checkBoxTrash.UseVisualStyleBackColor = true;
            // 
            // checkBoxOther
            // 
            checkBoxOther.AutoSize = true;
            checkBoxOther.Location = new Point(7, 115);
            checkBoxOther.Name = "checkBoxOther";
            checkBoxOther.Size = new Size(161, 19);
            checkBoxOther.TabIndex = 5;
            checkBoxOther.Text = "Keep other folder content";
            checkBoxOther.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AcceptButton = buttonOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(704, 316);
            Controls.Add(tabControlSettings);
            Controls.Add(buttonOkay);
            Controls.Add(buttonCancel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(720, 355);
            Name = "SettingsForm";
            Text = "Takeout Wrangler Settings";
            Load += SettingsForm_Load;
            groupBoxSource.ResumeLayout(false);
            groupBoxSource.PerformLayout();
            groupBoxDestination.ResumeLayout(false);
            groupBoxDestination.PerformLayout();
            groupBoxAction.ResumeLayout(false);
            groupBoxAction.PerformLayout();
            tabControlSettings.ResumeLayout(false);
            tabActions.ResumeLayout(false);
            tabPageContent.ResumeLayout(false);
            groupBoxContent.ResumeLayout(false);
            groupBoxContent.PerformLayout();
            tabSource.ResumeLayout(false);
            tabTarget.ResumeLayout(false);
            tabMail.ResumeLayout(false);
            groupBoxMailFolders.ResumeLayout(false);
            groupBoxMailFolders.PerformLayout();
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
        private CheckBox checkBoxKeepLocked;
        private Label labelLast;
        private Label labelConfirm;
        private TextBox textBoxConfirm;
        private Label labelPassword;
        private TextBox textBoxPassword;
        private TabControl tabControlSettings;
        private TabPage tabActions;
        private TabPage tabSource;
        private TabPage tabTarget;
        private TabPage tabMail;
        private Button buttonShowPassword;
        private Button buttonShowConfirm;
        private GroupBox groupBoxMailFolders;
        private CheckBox checkBoxSpam;
        private CheckBox checkBoxTrash;
        private CheckBox checkBoxSent;
        private CheckBox checkBoxArchived;
        private TabPage tabPageContent;
        private GroupBox groupBoxContent;
        private CheckBox checkBoxDoOther;
        private CheckBox checkBoxMail;
        private CheckBox checkBoxMedia;
        private CheckBox checkBoxInbox;
        private CheckBox checkBoxOther;
    }
}
