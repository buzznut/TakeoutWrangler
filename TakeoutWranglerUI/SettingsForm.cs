//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
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
    private bool isLoaded;
    private bool passwordTextHasChanged;

    public Settings Settings { get; private set; }
    public string Password
    {
        get => textBoxPassword.Text.Trim();
    }
    public PhotoCopier PhotoCopierSession;

    public SettingsForm(Settings settings)
    {
        SuspendLayout();
        InitializeComponent();
        buttonOkay.Enabled = false;
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));

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

        checkBoxList.Checked = Settings.ListOnly;
        checkBoxUseParallel.Checked = Settings.Parallel;

        // photos
        checkBoxKeepLocked.Checked = Settings.KeepLocked;

        // mail
        checkBoxTrash.Checked = Settings.KeepTrash;
        checkBoxSpam.Checked = Settings.KeepSpam;

        SetActionDescription();

        ResumeLayout(true);

        EnableOkayButton();
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
        SuspendLayout();

        textBoxSource.Text = Settings.Source;
        textBoxDestination.Text = Settings.Destination;
        textBoxFileFilter.Text = Settings.Filter;
        textBoxDestinationPattern.Text = Settings.Pattern;
        textBoxJunkFiles.Text = Settings.JunkExtensions;
        textBoxReorderBackupFolder.Text = Settings.Backup;

        comboBoxActions.SelectedItem = Settings.Behavior;
        comboBoxActions.SelectedIndex = comboBoxActions.FindStringExact(Settings.Behavior.ToString());

        comboBoxVerbosity.SelectedItem = Settings.Logging;
        comboBoxVerbosity.SelectedIndex = comboBoxVerbosity.FindStringExact(Settings.Logging.ToString());

        checkBoxList.Checked = Settings.ListOnly;
        checkBoxUseParallel.Checked = Settings.Parallel;

        // photos
        checkBoxKeepLocked.Checked = Settings.KeepLocked;

        // mail
        checkBoxTrash.Checked = Settings.KeepTrash;
        checkBoxSpam.Checked = Settings.KeepSpam;
        checkBoxSent.Checked = Settings.KeepSent;
        checkBoxArchived.Checked = Settings.KeepArchived;

        SetActionDescription();
        isLoaded = true;
        SettingsChanged(Settings.Behavior == PhotoCopierActions.Reorder);

        ResumeLayout(true);
        EnableOkayButton();
    }

    private void buttonOkay_Click(object sender, EventArgs e)
    {
        bool changed = false;
        string reason;

        changed |= string.Compare(Settings.Source, textBoxSource.Text) != 0;
        Settings.Source = textBoxSource.Text.Trim();

        changed |= string.Compare(Settings.Filter, textBoxFileFilter.Text) != 0;
        Settings.Filter = textBoxFileFilter.Text.Trim();

        changed |= string.Compare(Settings.Destination, textBoxDestination.Text) != 0;
        Settings.Destination = textBoxDestination.Text.Trim();

        changed |= string.Compare(Settings.Backup, textBoxReorderBackupFolder.Text) != 0;
        Settings.Backup = textBoxReorderBackupFolder.Text.Trim();

        changed |= string.Compare(Settings.Pattern, textBoxDestinationPattern.Text) != 0;
        Settings.Pattern = textBoxDestinationPattern.Text;

        PhotoCopierActions behavior = (comboBoxActions.SelectedItem as PhotoCopierActions?).GetValueOrDefault(PhotoCopierActions.Copy);
        changed |= Settings.Behavior != behavior;
        Settings.Behavior = behavior;

        LoggingVerbosity logging = (comboBoxVerbosity.SelectedItem as LoggingVerbosity?).GetValueOrDefault(LoggingVerbosity.Verbose);
        changed |= Settings.Logging != logging;
        Settings.Logging = logging;

        changed |= Settings.ListOnly != checkBoxList.Checked;
        Settings.ListOnly = checkBoxList.Checked;

        changed |= Settings.Parallel != checkBoxUseParallel.Checked;
        Settings.Parallel = checkBoxUseParallel.Checked;

        changed |= Settings.KeepLocked != checkBoxKeepLocked.Checked;
        Settings.KeepLocked = checkBoxKeepLocked.Checked;

        changed |= string.Compare(Settings.JunkExtensions, textBoxJunkFiles.Text) != 0;
        Settings.JunkExtensions = textBoxJunkFiles.Text.Trim();

        changed |= passwordTextHasChanged;
        passwordTextHasChanged = false;

        changed |= Settings.KeepTrash != checkBoxTrash.Checked;
        Settings.KeepTrash = checkBoxTrash.Checked;

        changed |= Settings.KeepSpam != checkBoxSpam.Checked;
        Settings.KeepSpam = checkBoxSpam.Checked;

        changed |= Settings.KeepSent != checkBoxSent.Checked;
        Settings.KeepSent = checkBoxSent.Checked;

        changed |= Settings.KeepArchived != checkBoxArchived.Checked;
        Settings.KeepArchived = checkBoxArchived.Checked;

        if (changed)
        {
            PhotoCopier copier = new PhotoCopier(null, null);
            if (Settings.Behavior != PhotoCopierActions.Reorder && !copier.ValidateSource(textBoxSource.Text, Settings.Filter, behavior, null, out reason))
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

            bool inPlace = Settings.Source.Equals(Settings.Destination, StringComparison.OrdinalIgnoreCase);
            if (inPlace && !copier.ValidateReorderBackupDirectory(textBoxReorderBackupFolder.Text, null, out reason))
            {
                MessageBox.Show(reason, "Validate Reorder backup folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            if (Settings.KeepLocked)
            {
                if (textBoxPassword.Text != textBoxConfirm.Text || textBoxPassword.Text.Length < 6)
                {
                    MessageBox.Show(reason, "Password does not match or is too short", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            if (changed)
            {
                DialogResult result = MessageBox.Show("Keep changed values?", "Settings have changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DialogResult = DialogResult.Yes;

                    if (!Settings.KeepLocked)
                    {
                        textBoxPassword.Clear();
                        textBoxConfirm.Clear();
                    }
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

        if (Settings.ListOnly) sb.Append("(List only - no changes) ");

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
        if (!isLoaded) return;

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

        labelFilter.Enabled = !isReorder;
        textBoxFileFilter.Enabled = !isReorder;
        labelArchiveFilter.Enabled = !isReorder;

        labelReorderBackup.Enabled = inPlace;
        textBoxReorderBackupFolder.Enabled = inPlace;
        buttonReorderBackupBrowse.Enabled = inPlace;

        textBoxPassword.Enabled = checkBoxKeepLocked.Checked;
        textBoxConfirm.Enabled = checkBoxKeepLocked.Checked;
        labelPassword.Enabled = checkBoxKeepLocked.Checked;
        labelConfirm.Enabled = checkBoxKeepLocked.Checked;
        labelLast.Enabled = checkBoxKeepLocked.Checked;

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

    private void textBox_TextChanged(object sender, EventArgs e)
    {
        SettingsChanged(Settings.Behavior == PhotoCopierActions.Reorder);
    }

    private void password_TextChanged(object sender, EventArgs e)
    {
        EnableOkayButton();
        passwordTextHasChanged = true;
    }

    private void checkBoxKeepLocked_CheckedChanged(object sender, EventArgs e)
    {
        if (isLoaded)
        {
            SettingsChanged(Settings.Behavior == PhotoCopierActions.Reorder);
            EnableOkayButton();
        }
    }

    private void EnableOkayButton()
    {
        if (isLoaded)
        {
            buttonOkay.Enabled = !string.IsNullOrWhiteSpace(textBoxSource.Text) &&
                                 !string.IsNullOrWhiteSpace(textBoxDestination.Text) &&
                                 !string.IsNullOrWhiteSpace(textBoxFileFilter.Text) &&
                                 !string.IsNullOrWhiteSpace(textBoxDestinationPattern.Text) &&
                                 (!checkBoxKeepLocked.Checked || (!string.IsNullOrWhiteSpace(textBoxPassword.Text) && textBoxPassword.Text == textBoxConfirm.Text && textBoxPassword.Text.Length >= 8));
        }
    }

    private void buttonShowPassword_MouseDown(object sender, MouseEventArgs e)
    {
        ButtonShow(textBoxPassword, true);
    }

    private void buttonShowPassword_MouseUp(object sender, MouseEventArgs e)
    {
        ButtonShow(textBoxPassword, false);
    }

    private void buttonShowConfirm_MouseDown(object sender, MouseEventArgs e)
    {
        ButtonShow(textBoxConfirm, true);
    }

    private void buttonShowConfirm_MouseUp(object sender, MouseEventArgs e)
    {
        ButtonShow(textBoxConfirm, false);
    }

    private void ButtonShow(TextBox textBox, bool showPassword)
    {
        if (showPassword && textBox.UseSystemPasswordChar)
        {
            textBox.UseSystemPasswordChar = false;
        }
        else
        {
            textBox.UseSystemPasswordChar = true;
        }
    }

}
