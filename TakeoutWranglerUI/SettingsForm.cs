//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:2:25:8:47
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using PhotoCopyLibrary;
using System.Text;

namespace TakeoutWranglerUI;

public partial class SettingsForm : Form
{
    public string Source;
    public string Destination;
    public string Backup;
    public string Filter;
    public string Pattern;
    public bool ListOnly;
    public bool Parallel;
    public string Junk;
    public LoggingVerbosity Logging;
    public PhotoCopierActions Behavior;
    public PhotoCopier PhotoCopierSession;

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
        checkBoxUseParallel.Checked = Parallel;

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
        textBoxJunkFiles.Text = Junk;
        textBoxReorderBackupFolder.Text = Backup;

        comboBoxActions.SelectedItem = Behavior;
        comboBoxActions.SelectedIndex = comboBoxActions.FindStringExact(Behavior.ToString());

        comboBoxVerbosity.SelectedItem = Logging;
        comboBoxVerbosity.SelectedIndex = comboBoxVerbosity.FindStringExact(Logging.ToString());

        checkBoxList.Checked = ListOnly;
        checkBoxUseParallel.Checked = Parallel;

        SetActionDescription();

        ResumeLayout(true);
    }

    private void buttonOkay_Click(object sender, EventArgs e)
    {
        bool changed = false;
        string reason;

        changed |= string.Compare(Source, textBoxSource.Text) != 0;
        Source = textBoxSource.Text.Trim();

        changed |= string.Compare(Filter, textBoxFileFilter.Text) != 0;
        Filter = textBoxFileFilter.Text.Trim();

        changed |= string.Compare(Destination, textBoxDestination.Text) != 0;
        Destination = textBoxDestination.Text.Trim();

        changed |= string.Compare(Backup, textBoxReorderBackupFolder.Text) != 0;
        Backup = textBoxReorderBackupFolder.Text.Trim();

        changed |= string.Compare(Pattern, textBoxDestinationPattern.Text) != 0;
        Pattern = textBoxDestinationPattern.Text;

        PhotoCopierActions behavior = (comboBoxActions.SelectedItem as PhotoCopierActions?).GetValueOrDefault(PhotoCopierActions.Copy);
        changed |= Behavior != behavior;
        Behavior = behavior;

        LoggingVerbosity logging = (comboBoxVerbosity.SelectedItem as LoggingVerbosity?).GetValueOrDefault(LoggingVerbosity.Verbose);
        changed |= Logging != logging;
        Logging = logging;

        changed |= ListOnly != checkBoxList.Checked;
        ListOnly = checkBoxList.Checked;

        changed |= Parallel != checkBoxUseParallel.Checked;
        Parallel = checkBoxUseParallel.Checked;

        changed |= string.Compare(Junk, textBoxJunkFiles.Text) != 0;
        Junk = textBoxJunkFiles.Text.Trim();

        if (changed)
        {
            PhotoCopier copier = new PhotoCopier(null, null);
            if (Behavior != PhotoCopierActions.Reorder && !copier.ValidateSource(textBoxSource.Text, Filter, behavior, null, out reason))
            {
                DialogResult validateResult = MessageBox.Show($"{reason}. Continue?", "Validate Source", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (validateResult == DialogResult.No)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            if (!copier.ValidateDestination(textBoxDestination.Text, null, out reason))
            {
                MessageBox.Show(reason, "Validate Target", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            if (!copier.ValidatePattern(textBoxDestinationPattern.Text, null, out reason))
            {
                MessageBox.Show(reason, "Validate Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            if (!copier.ValidateJunk(textBoxJunkFiles.Text, null, out reason))
            {
                MessageBox.Show(reason, "Validate Junk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            bool inPlace = Source.Equals(Destination, StringComparison.OrdinalIgnoreCase);
            if (inPlace && !copier.ValidateReorderBackupFolder(textBoxReorderBackupFolder.Text, null, out reason))
            {
                MessageBox.Show(reason, "Validate Reorder backup folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            if (changed)
            {
                DialogResult result = MessageBox.Show("Keep changed values?", "Settings have changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DialogResult = DialogResult.Yes;
                }
            }
        }
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
                sb.Append("Copy media files from takeout zip archives to folder.");
                break;
            case PhotoCopierActions.Reorder:
                sb.Append("Reorder media files to match new file pattern.");
                break;
        }

        textBoxActionDescription.Text = sb.ToString();
    }

    private void comboBoxActions_SelectedIndexChanged(object sender, EventArgs e)
    {
        PhotoCopierActions newBehavior = (comboBoxActions.SelectedItem as PhotoCopierActions?).GetValueOrDefault(PhotoCopierActions.Copy);
        SettingsChanged(newBehavior == PhotoCopierActions.Reorder);
    }

    private void SettingsChanged(bool isReorder)
    {
        bool inPlace = false;
        if (isReorder)
        {
            inPlace = textBoxSource.Text.Equals(textBoxDestination.Text, StringComparison.OrdinalIgnoreCase);
            labelSourceDescription.Text = inPlace ? TWContstants.SourceDescriptionInPlace : TWContstants.SourceDescriptionReorder;
        }
        else
        {
            labelSourceDescription.Text = TWContstants.SourceDescriptionCopy;
        }

        labelJunkFiles.Enabled = isReorder;
        textBoxJunkFiles.Enabled = isReorder;
        labelJunkComma.Enabled = isReorder;

        labelFilter.Enabled = !isReorder;
        textBoxFileFilter.Enabled = !isReorder;
        labelArchiveFilter.Enabled = !isReorder;

        labelReorderBackup.Enabled = inPlace;
        textBoxReorderBackupFolder.Enabled = inPlace;
        buttonReorderBackupBrowse.Enabled = inPlace;

        SetActionDescription();
    }

    private void buttonReorderBackupBrowse_Click(object sender, EventArgs e)
    {
        string folder = SelectFolder(textBoxReorderBackupFolder.Text);
        if (folder != null)
        {
            textBoxReorderBackupFolder.Text = folder;
        }
    }

    private void checkBoxReorderInPlace_CheckedChanged(object sender, EventArgs e)
    {
        PhotoCopierActions newBehavior = (comboBoxActions.SelectedItem as PhotoCopierActions?).GetValueOrDefault(PhotoCopierActions.Copy);
        SettingsChanged(newBehavior == PhotoCopierActions.Reorder);
    }

    private void textBox_TextChanged(object sender, EventArgs e)
    {
        SettingsChanged(Behavior == PhotoCopierActions.Reorder);
    }
}
