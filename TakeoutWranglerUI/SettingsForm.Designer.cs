//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace TakeoutWrangler
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
            textBoxMediaFolderDescription = new TextBox();
            textBoxDestinationPattern = new TextBox();
            labelDestinationRootFolderDescription = new Label();
            buttonDestinationDialog = new Button();
            textBoxDestination = new TextBox();
            groupBoxAction = new GroupBox();
            textBoxActionDescription = new TextBox();
            checkBoxList = new CheckBox();
            comboBoxVerbosity = new ComboBox();
            labelVerbosity = new Label();
            labelActions = new Label();
            comboBoxActions = new ComboBox();
            buttonCancel = new Button();
            buttonOkay = new Button();
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
            labelDestination.Location = new Point(14, 39);
            labelDestination.Name = "labelDestination";
            labelDestination.Size = new Size(77, 15);
            labelDestination.TabIndex = 1;
            labelDestination.Text = "Media folder:";
            // 
            // labelPattern
            // 
            labelPattern.AutoSize = true;
            labelPattern.Location = new Point(11, 94);
            labelPattern.Name = "labelPattern";
            labelPattern.Size = new Size(151, 15);
            labelPattern.TabIndex = 5;
            labelPattern.Text = "Media folder name pattern:";
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
            groupBoxSource.Location = new Point(12, 12);
            groupBoxSource.Name = "groupBoxSource";
            groupBoxSource.Size = new Size(680, 116);
            groupBoxSource.TabIndex = 0;
            groupBoxSource.TabStop = false;
            groupBoxSource.Text = "Takeout files";
            // 
            // labelArchiveFilter
            // 
            labelArchiveFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelArchiveFilter.AutoSize = true;
            labelArchiveFilter.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelArchiveFilter.Location = new Point(233, 72);
            labelArchiveFilter.Name = "labelArchiveFilter";
            labelArchiveFilter.Size = new Size(215, 15);
            labelArchiveFilter.TabIndex = 6;
            labelArchiveFilter.Text = "Archive file filter (default: takeout-*.zip)";
            // 
            // buttonSourceDialog
            // 
            buttonSourceDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSourceDialog.Location = new Point(647, 38);
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
            textBoxFileFilter.Size = new Size(150, 23);
            textBoxFileFilter.TabIndex = 5;
            // 
            // textBoxSource
            // 
            textBoxSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxSource.Location = new Point(78, 38);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(563, 23);
            textBoxSource.TabIndex = 2;
            // 
            // groupBoxDestination
            // 
            groupBoxDestination.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxDestination.Controls.Add(textBoxMediaFolderDescription);
            groupBoxDestination.Controls.Add(textBoxDestinationPattern);
            groupBoxDestination.Controls.Add(labelDestinationRootFolderDescription);
            groupBoxDestination.Controls.Add(buttonDestinationDialog);
            groupBoxDestination.Controls.Add(labelDestination);
            groupBoxDestination.Controls.Add(textBoxDestination);
            groupBoxDestination.Controls.Add(labelPattern);
            groupBoxDestination.Location = new Point(12, 271);
            groupBoxDestination.Name = "groupBoxDestination";
            groupBoxDestination.Size = new Size(680, 133);
            groupBoxDestination.TabIndex = 2;
            groupBoxDestination.TabStop = false;
            groupBoxDestination.Text = "Target";
            // 
            // textBoxMediaFolderDescription
            // 
            textBoxMediaFolderDescription.BorderStyle = BorderStyle.None;
            textBoxMediaFolderDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            textBoxMediaFolderDescription.Location = new Point(173, 72);
            textBoxMediaFolderDescription.Name = "textBoxMediaFolderDescription";
            textBoxMediaFolderDescription.ReadOnly = true;
            textBoxMediaFolderDescription.Size = new Size(485, 16);
            textBoxMediaFolderDescription.TabIndex = 7;
            textBoxMediaFolderDescription.TabStop = false;
            textBoxMediaFolderDescription.Text = " Use: text, $y=year, $m=month, $d=day, $h=hour (media date).  example:  $y_$m";
            // 
            // textBoxDestinationPattern
            // 
            textBoxDestinationPattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDestinationPattern.Location = new Point(170, 91);
            textBoxDestinationPattern.Name = "textBoxDestinationPattern";
            textBoxDestinationPattern.Size = new Size(298, 23);
            textBoxDestinationPattern.TabIndex = 6;
            // 
            // labelDestinationRootFolderDescription
            // 
            labelDestinationRootFolderDescription.AutoSize = true;
            labelDestinationRootFolderDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelDestinationRootFolderDescription.Location = new Point(96, 19);
            labelDestinationRootFolderDescription.Name = "labelDestinationRootFolderDescription";
            labelDestinationRootFolderDescription.Size = new Size(221, 15);
            labelDestinationRootFolderDescription.TabIndex = 0;
            labelDestinationRootFolderDescription.Text = "Folder to for local media files and folders.";
            // 
            // buttonDestinationDialog
            // 
            buttonDestinationDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDestinationDialog.Location = new Point(647, 36);
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
            textBoxDestination.Location = new Point(96, 36);
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.Size = new Size(545, 23);
            textBoxDestination.TabIndex = 2;
            // 
            // groupBoxAction
            // 
            groupBoxAction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxAction.Controls.Add(textBoxActionDescription);
            groupBoxAction.Controls.Add(checkBoxList);
            groupBoxAction.Controls.Add(comboBoxVerbosity);
            groupBoxAction.Controls.Add(labelVerbosity);
            groupBoxAction.Controls.Add(labelActions);
            groupBoxAction.Controls.Add(comboBoxActions);
            groupBoxAction.Location = new Point(12, 134);
            groupBoxAction.Name = "groupBoxAction";
            groupBoxAction.Size = new Size(680, 131);
            groupBoxAction.TabIndex = 1;
            groupBoxAction.TabStop = false;
            groupBoxAction.Text = "Actions";
            // 
            // textBoxActionDescription
            // 
            textBoxActionDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxActionDescription.BorderStyle = BorderStyle.None;
            textBoxActionDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            textBoxActionDescription.Location = new Point(84, 77);
            textBoxActionDescription.Multiline = true;
            textBoxActionDescription.Name = "textBoxActionDescription";
            textBoxActionDescription.Size = new Size(587, 44);
            textBoxActionDescription.TabIndex = 5;
            textBoxActionDescription.Text = "(action description)";
            // 
            // checkBoxList
            // 
            checkBoxList.AutoSize = true;
            checkBoxList.Checked = true;
            checkBoxList.CheckState = CheckState.Checked;
            checkBoxList.Location = new Point(82, 22);
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
            comboBoxVerbosity.Location = new Point(323, 46);
            comboBoxVerbosity.Name = "comboBoxVerbosity";
            comboBoxVerbosity.Size = new Size(121, 23);
            comboBoxVerbosity.TabIndex = 3;
            // 
            // labelVerbosity
            // 
            labelVerbosity.AutoSize = true;
            labelVerbosity.Location = new Point(254, 50);
            labelVerbosity.Name = "labelVerbosity";
            labelVerbosity.Size = new Size(58, 15);
            labelVerbosity.TabIndex = 2;
            labelVerbosity.Text = "Verbosity:";
            // 
            // labelActions
            // 
            labelActions.AutoSize = true;
            labelActions.Location = new Point(23, 50);
            labelActions.Name = "labelActions";
            labelActions.Size = new Size(50, 15);
            labelActions.TabIndex = 0;
            labelActions.Text = "Actions:";
            // 
            // comboBoxActions
            // 
            comboBoxActions.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxActions.FormattingEnabled = true;
            comboBoxActions.Location = new Point(82, 46);
            comboBoxActions.Name = "comboBoxActions";
            comboBoxActions.Size = new Size(121, 23);
            comboBoxActions.TabIndex = 1;
            comboBoxActions.SelectedIndexChanged += comboBoxActions_SelectedIndexChanged;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(617, 412);
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
            buttonOkay.Location = new Point(536, 412);
            buttonOkay.Name = "buttonOkay";
            buttonOkay.Size = new Size(75, 23);
            buttonOkay.TabIndex = 3;
            buttonOkay.Text = "OK";
            buttonOkay.UseVisualStyleBackColor = true;
            buttonOkay.Click += buttonOkay_Click;
            // 
            // SettingsForm
            // 
            AcceptButton = buttonOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(704, 441);
            Controls.Add(buttonOkay);
            Controls.Add(buttonCancel);
            Controls.Add(groupBoxAction);
            Controls.Add(groupBoxDestination);
            Controls.Add(groupBoxSource);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(720, 480);
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
        private GroupBox groupBoxAction;
        private Button buttonCancel;
        private Button buttonOkay;
        private Label labelActions;
        private ComboBox comboBoxActions;
        private ComboBox comboBoxVerbosity;
        private Label labelVerbosity;
        private TextBox textBoxActionDescription;
        private CheckBox checkBoxList;
        private TextBox textBoxMediaFolderDescription;
    }
}
