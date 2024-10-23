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
            textBoxDestinationPattern = new TextBox();
            labelDestinationFolderNamePatternDescription = new Label();
            labelDestinationRootFolderDescription = new Label();
            buttonDestinationDialog = new Button();
            textBoxDestination = new TextBox();
            radioButtonCopy = new RadioButton();
            radioButtonList = new RadioButton();
            checkBoxQuiet = new CheckBox();
            groupBoxAction = new GroupBox();
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
            labelSource.TabIndex = 0;
            labelSource.Text = "Directory:";
            // 
            // labelDestination
            // 
            labelDestination.AutoSize = true;
            labelDestination.Location = new Point(84, 39);
            labelDestination.Name = "labelDestination";
            labelDestination.Size = new Size(69, 15);
            labelDestination.TabIndex = 1;
            labelDestination.Text = "Root folder:";
            // 
            // labelPattern
            // 
            labelPattern.AutoSize = true;
            labelPattern.Location = new Point(11, 94);
            labelPattern.Name = "labelPattern";
            labelPattern.Size = new Size(142, 15);
            labelPattern.TabIndex = 3;
            labelPattern.Text = "New folder name pattern:";
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
            groupBoxSource.TabIndex = 5;
            groupBoxSource.TabStop = false;
            groupBoxSource.Text = "Archive";
            // 
            // labelArchiveFilter
            // 
            labelArchiveFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelArchiveFilter.AutoSize = true;
            labelArchiveFilter.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelArchiveFilter.Location = new Point(233, 72);
            labelArchiveFilter.Name = "labelArchiveFilter";
            labelArchiveFilter.Size = new Size(215, 15);
            labelArchiveFilter.TabIndex = 9;
            labelArchiveFilter.Text = "Archive file filter (default: takeout-*.zip)";
            // 
            // buttonSourceDialog
            // 
            buttonSourceDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSourceDialog.Location = new Point(589, 41);
            buttonSourceDialog.Name = "buttonSourceDialog";
            buttonSourceDialog.Size = new Size(27, 23);
            buttonSourceDialog.TabIndex = 8;
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
            labelSourceDescription.TabIndex = 7;
            labelSourceDescription.Text = "Folder that contains the archive zip file(s) 'takeout-*.zip'";
            // 
            // textBoxFileFilter
            // 
            textBoxFileFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxFileFilter.BorderStyle = BorderStyle.FixedSingle;
            textBoxFileFilter.Location = new Point(78, 69);
            textBoxFileFilter.Name = "textBoxFileFilter";
            textBoxFileFilter.Size = new Size(150, 23);
            textBoxFileFilter.TabIndex = 6;
            // 
            // textBoxSource
            // 
            textBoxSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxSource.BorderStyle = BorderStyle.FixedSingle;
            textBoxSource.Location = new Point(79, 38);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(504, 23);
            textBoxSource.TabIndex = 5;
            // 
            // groupBoxDestination
            // 
            groupBoxDestination.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxDestination.Controls.Add(textBoxDestinationPattern);
            groupBoxDestination.Controls.Add(labelDestinationFolderNamePatternDescription);
            groupBoxDestination.Controls.Add(labelDestinationRootFolderDescription);
            groupBoxDestination.Controls.Add(buttonDestinationDialog);
            groupBoxDestination.Controls.Add(labelDestination);
            groupBoxDestination.Controls.Add(textBoxDestination);
            groupBoxDestination.Controls.Add(labelPattern);
            groupBoxDestination.Location = new Point(12, 225);
            groupBoxDestination.Name = "groupBoxDestination";
            groupBoxDestination.Size = new Size(680, 133);
            groupBoxDestination.TabIndex = 6;
            groupBoxDestination.TabStop = false;
            groupBoxDestination.Text = "Target";
            // 
            // textBoxDestinationPattern
            // 
            textBoxDestinationPattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDestinationPattern.BorderStyle = BorderStyle.FixedSingle;
            textBoxDestinationPattern.Location = new Point(170, 91);
            textBoxDestinationPattern.Name = "textBoxDestinationPattern";
            textBoxDestinationPattern.Size = new Size(298, 23);
            textBoxDestinationPattern.TabIndex = 14;
            // 
            // labelDestinationFolderNamePatternDescription
            // 
            labelDestinationFolderNamePatternDescription.AutoSize = true;
            labelDestinationFolderNamePatternDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelDestinationFolderNamePatternDescription.Location = new Point(170, 73);
            labelDestinationFolderNamePatternDescription.Name = "labelDestinationFolderNamePatternDescription";
            labelDestinationFolderNamePatternDescription.Size = new Size(384, 15);
            labelDestinationFolderNamePatternDescription.TabIndex = 13;
            labelDestinationFolderNamePatternDescription.Text = " Use: text, $y=year, $m=month, $d=day, $h=hour (media date).  '$y_$m'";
            // 
            // labelDestinationRootFolderDescription
            // 
            labelDestinationRootFolderDescription.AutoSize = true;
            labelDestinationRootFolderDescription.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelDestinationRootFolderDescription.Location = new Point(170, 19);
            labelDestinationRootFolderDescription.Name = "labelDestinationRootFolderDescription";
            labelDestinationRootFolderDescription.Size = new Size(221, 15);
            labelDestinationRootFolderDescription.TabIndex = 12;
            labelDestinationRootFolderDescription.Text = "Folder to for local media files and folders.";
            // 
            // buttonDestinationDialog
            // 
            buttonDestinationDialog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDestinationDialog.Location = new Point(647, 35);
            buttonDestinationDialog.Name = "buttonDestinationDialog";
            buttonDestinationDialog.Size = new Size(27, 23);
            buttonDestinationDialog.TabIndex = 11;
            buttonDestinationDialog.Text = "...";
            buttonDestinationDialog.UseVisualStyleBackColor = true;
            buttonDestinationDialog.Click += buttonDestinationDialog_Click;
            // 
            // textBoxDestination
            // 
            textBoxDestination.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDestination.Location = new Point(170, 36);
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.Size = new Size(471, 23);
            textBoxDestination.TabIndex = 10;
            // 
            // radioButtonCopy
            // 
            radioButtonCopy.AutoSize = true;
            radioButtonCopy.Location = new Point(6, 24);
            radioButtonCopy.Name = "radioButtonCopy";
            radioButtonCopy.Size = new Size(91, 19);
            radioButtonCopy.TabIndex = 7;
            radioButtonCopy.TabStop = true;
            radioButtonCopy.Text = "Always copy";
            radioButtonCopy.UseVisualStyleBackColor = true;
            radioButtonCopy.CheckedChanged += radioButtonCopy_CheckedChanged;
            // 
            // radioButtonList
            // 
            radioButtonList.AutoSize = true;
            radioButtonList.Location = new Point(6, 43);
            radioButtonList.Name = "radioButtonList";
            radioButtonList.Size = new Size(69, 19);
            radioButtonList.TabIndex = 8;
            radioButtonList.TabStop = true;
            radioButtonList.Text = "List only";
            radioButtonList.UseVisualStyleBackColor = true;
            radioButtonList.CheckedChanged += radioButtonList_CheckedChanged;
            // 
            // checkBoxQuiet
            // 
            checkBoxQuiet.AutoSize = true;
            checkBoxQuiet.Location = new Point(103, 24);
            checkBoxQuiet.Name = "checkBoxQuiet";
            checkBoxQuiet.Size = new Size(118, 19);
            checkBoxQuiet.TabIndex = 9;
            checkBoxQuiet.Text = "Quiet (copy only)";
            checkBoxQuiet.UseVisualStyleBackColor = true;
            // 
            // groupBoxAction
            // 
            groupBoxAction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxAction.Controls.Add(radioButtonCopy);
            groupBoxAction.Controls.Add(checkBoxQuiet);
            groupBoxAction.Controls.Add(radioButtonList);
            groupBoxAction.Location = new Point(12, 134);
            groupBoxAction.Name = "groupBoxAction";
            groupBoxAction.Size = new Size(680, 85);
            groupBoxAction.TabIndex = 10;
            groupBoxAction.TabStop = false;
            groupBoxAction.Text = "Actions";
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(617, 368);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 11;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOkay
            // 
            buttonOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOkay.DialogResult = DialogResult.OK;
            buttonOkay.Location = new Point(536, 368);
            buttonOkay.Name = "buttonOkay";
            buttonOkay.Size = new Size(75, 23);
            buttonOkay.TabIndex = 12;
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
            ClientSize = new Size(704, 401);
            Controls.Add(buttonOkay);
            Controls.Add(buttonCancel);
            Controls.Add(groupBoxAction);
            Controls.Add(groupBoxDestination);
            Controls.Add(groupBoxSource);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(720, 440);
            Name = "SettingsForm";
            Text = "Settings";
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
        private Label labelDestinationFolderNamePatternDescription;
        private Label labelDestinationRootFolderDescription;
        private RadioButton radioButtonCopy;
        private RadioButton radioButtonList;
        private CheckBox checkBoxQuiet;
        private GroupBox groupBoxAction;
        private Button buttonCancel;
        private Button buttonOkay;
    }
}
