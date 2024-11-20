//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:11:16:13:40
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using PhotoCopyLibrary;
using System.Text;

namespace TakeoutWrangler;

public partial class SettingsForm : Form
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public string Filter { get; set; }
    public string Pattern { get; set; }
    public bool ListOnly { get; set; }
    public LoggingVerbosity Logging { get; set; }
    public PhotoCopierActions Behavior { get; set; }

    public SettingsForm()
    {
        SuspendLayout();
        InitializeComponent();

        foreach (PhotoCopierActions action in Enum.GetValues(typeof(PhotoCopierActions)))
        {
            comboBoxActions.Items.Add(action);
        }
        comboBoxActions.SelectedItem = PhotoCopierActions.Copy;

        foreach (LoggingVerbosity verbosity in Enum.GetValues(typeof(LoggingVerbosity)))
        {
            comboBoxVerbosity.Items.Add(verbosity);
        }
        comboBoxVerbosity.SelectedItem = LoggingVerbosity.Verbose;

        checkBoxList.Checked = ListOnly;

        SetActionDescription();

        ResumeLayout(true);
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
        SuspendLayout();

        textBoxSource.Text = Source;
        textBoxDestination.Text = Destination;
        textBoxFileFilter.Text = Filter;
        textBoxDestinationPattern.Text = Pattern;

        comboBoxActions.SelectedItem = Behavior;
        comboBoxActions.SelectedIndex = comboBoxActions.FindStringExact(Behavior.ToString());

        comboBoxVerbosity.SelectedItem = Logging;
        comboBoxVerbosity.SelectedIndex = comboBoxVerbosity.FindStringExact(Logging.ToString());

        checkBoxList.Checked = ListOnly;

        SetActionDescription();

        ResumeLayout(true);
    }

    private void buttonOkay_Click(object sender, EventArgs e)
    {
        bool changed = false;

        changed |= Source != textBoxSource.Text;
        Source = textBoxSource.Text;

        changed |= Destination != textBoxDestination.Text;
        Destination = textBoxDestination.Text;

        changed |= Filter != textBoxFileFilter.Text;
        Filter = textBoxFileFilter.Text;

        changed |= Pattern != textBoxDestinationPattern.Text;
        Pattern = textBoxDestinationPattern.Text;

        PhotoCopierActions behavior = (comboBoxActions.SelectedItem as PhotoCopierActions?).GetValueOrDefault(PhotoCopierActions.Copy);
        changed |= Behavior != behavior;
        Behavior = behavior;

        LoggingVerbosity logging = (comboBoxVerbosity.SelectedItem as LoggingVerbosity?).GetValueOrDefault(LoggingVerbosity.Verbose);
        changed |= Logging != logging;
        Logging = logging;

        changed |= ListOnly != checkBoxList.Checked;
        ListOnly = checkBoxList.Checked;

        if (changed)
        {
            DialogResult result = MessageBox.Show("Keep changed values?", "Settings have changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DialogResult = DialogResult.Yes;
            }
        }

        Close();
    }

    private void buttonSourceDialog_Click(object sender, EventArgs e)
    {
        string folder = SelectFolder(textBoxSource.Text);
        if (folder != null)
        {
            textBoxSource.Text = folder;
        }
    }

    private void buttonDestinationDialog_Click(object sender, EventArgs e)
    {
        string folder = SelectFolder(textBoxDestination.Text);
        if (folder != null)
        {
            textBoxDestination.Text = folder;
        }
    }

    private string SelectFolder(string text)
    {
        using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
        {
            if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
            {
                folderDialog.SelectedPath = text;
            }

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                return folderDialog.SelectedPath;
            }
        }

        return null;
    }

    private void SetActionDescription()
    {
        StringBuilder sb = new StringBuilder();

        if (ListOnly) sb.Append("(List only - no changes) ");

        switch (((PhotoCopierActions?)comboBoxActions.SelectedItem).GetValueOrDefault(PhotoCopierActions.Copy))
        {
            case PhotoCopierActions.Copy:
                sb.Append("Copy media files from takeout zip archives to folder");
                break;
            case PhotoCopierActions.Reorder:
                sb.Append("Reorder media files to match new file pattern");
                break;
            case PhotoCopierActions.Overwrite:
                sb.Append("Overwrite all media files to folder");
                break;
        }

        textBoxActionDescription.Text = sb.ToString();
    }

    private void comboBoxActions_SelectedIndexChanged(object sender, EventArgs e)
    {
        PhotoCopierActions newBehavior = (comboBoxActions.SelectedItem as PhotoCopierActions?).GetValueOrDefault(PhotoCopierActions.Copy);
        if (newBehavior == PhotoCopierActions.Reorder)
        {
            groupBoxSource.Enabled = false;
        }
        else
        {
            groupBoxSource.Enabled = true;
        }

        SetActionDescription();
    }
}
