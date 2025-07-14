//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using CommonLibrary;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace DecryptDisplay;

public partial class DisplayForm : Form
{
    private LibVLC libvlc = new LibVLC();
    private Stream displayStream;
    private string originalExtension = string.Empty;
    private string originalFileName = string.Empty;
    private Control displayedControl;
    private int currentFileIndex = 0;
    private int lastCurrentFileIndex = -1;
    private List<string> allFiles;
    private readonly Stopwatch passwordVisible = new Stopwatch();
    private string password;

    private readonly HashSet<string> pictureFormats =
        new HashSet<string>(
        [
            ".bmp", ".jpg", ".jpeg",
            ".png", ".gif", ".tiff",
            ".tif", ".ico", ".wmf",
            ".emf"
        ],
        StringComparer.OrdinalIgnoreCase);

    private readonly HashSet<string> movieFormats =
        new HashSet<string>(
        [
            ".VLC",   ".3G2",   ".3GA",
            ".3GP",   ".AAC",   ".ADT",
            ".ANX",   ".AVCHD", ".AVI",
            ".BIK",   ".DV",    ".F4V",
            ".FLV",   ".GXF",   ".H264",
            ".HDMOV", ".ISO",   ".M1V",
            ".M2V",   ".M4A",   ".M4B",
            ".M4V",   ".MJPG",  ".MKA",
            ".MKS",   ".MOV",   ".MP3",
            ".MP4",   ".MPEG1", ".MPEG4",
            ".MPG2",  ".MPV",   ".MTS",
            ".NSV",   ".NUV",   ".OGG",
            ".OGV",   ".OMA",   ".OPUS",
            ".PSS",   ".RA",    ".RBS",
            ".REC",   ".RM",    ".RMI",
            ".RT",    ".SPX",   ".SVI",
            ".TOD",   ".TRP",   ".TS",
            ".TTA",   ".VLT",   ".VOC",
            ".VP6",   ".VQF",   ".VRO",
            ".WAV",   ".WEBM",  ".WMA",
            ".WV",    ".XESC",  ".ZAB"
        ],
        StringComparer.OrdinalIgnoreCase);

    public DisplayForm()
    {
        SuspendLayout();

        InitializeComponent();

        videoView.MediaPlayer = new MediaPlayer(libvlc);

        panelPicture.Controls.Add(videoView);
        panelPicture.Controls.Add(pictureBoxImage);
        panelPicture.Controls.Add(richTextInfo);

        pictureBoxImage.BorderStyle = BorderStyle.None;
        panelPicture.BorderStyle = BorderStyle.None;
        richTextInfo.BorderStyle = BorderStyle.None;

        SetControl(richTextInfo);

        ResumeLayout(true);
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CloseImage();
        Environment.Exit(0);
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (displayStream == null)
        {
            MessageBox.Show("There is no file to save. Please open a file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            saveFileDialog.Filter = $"Media Files (*{originalExtension})|*{originalExtension}";
            saveFileDialog.Title = "Save As";
            saveFileDialog.DefaultExt = originalExtension.TrimStart('.');
            saveFileDialog.AddExtension = true;
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(originalFileName) ?? ".\\";
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(originalFileName) + originalExtension;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    displayStream.Position = 0;
                    using (FileStream outputStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        displayStream.CopyTo(outputStream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void fileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CloseImage();

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select a file to decrypt",
            Filter = "Encrypted Files (*.twlock)|*.twlock|All Files (*.*)|*.*",
            Multiselect = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        DialogResult result = openFileDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            SetControl(richTextInfo);
            SetRichTextBoxLine($"Opening file...");

            string[] selectedFiles = openFileDialog.FileNames;
            ShowFiles(selectedFiles);
        }
    }

    private void DoDisplay()
    {
        if (currentFileIndex != lastCurrentFileIndex)
        {
            lastCurrentFileIndex = currentFileIndex;
            string tmpPath = null;

            CloseImage();
            originalFileName = allFiles[currentFileIndex];

            try
            {
                bool wasShown = false;
                tmpPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(originalFileName) + ".tmp");
                displayStream = new FileStream(tmpPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

                originalExtension = string.Empty;

                using (FileStream stream = new FileStream(originalFileName, FileMode.Open, FileAccess.Read))
                {
                    FileEncryption.DecryptFile(stream, displayStream, password, out originalExtension, out DateTime originalDate);
                    if (pictureFormats.Contains(originalExtension))
                    {
                        // If the decrypted file is an image, display it in the PictureBox
                        SetControl(pictureBoxImage);
                        passwordVisible.Restart();
                        timerPasswordCheck.Start();

                        displayStream.Seek(0, SeekOrigin.Begin);
                        pictureBoxImage.Image = System.Drawing.Image.FromStream(displayStream);
                        panelPicture.Invalidate();

                        wasShown = true;
                    }
                    else if (movieFormats.Contains(originalExtension))
                    {
                        // If the decrypted file is a movie, play it in the VideoView
                        SetControl(videoView);
                        passwordVisible.Restart();
                        timerPasswordCheck.Start();

                        displayStream.Seek(0, SeekOrigin.Begin);
                        Media media = new Media(libvlc, new StreamMediaInput(displayStream));
                        videoView.MediaPlayer?.Play(media);
                        wasShown = true;
                    }
                }

                if (!wasShown)
                {
                    // If the decrypted file is neither an image nor a movie, show a warning
                    SetControl(richTextInfo);
                    SetRichTextBoxLines(["The file is not a supported image or video format."]);
                }
            }
            catch
            {
                SetControl(richTextInfo);
                SetRichTextBoxLine($"Could not open: \"{originalFileName}\"");
            }
        }
    }

    private void SetRichTextBoxLine(string line)
    {
        string[] lines = line.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        SetRichTextBoxLines(lines);
    }

    private void SetRichTextBoxLines(string[] lines)
    {
        richTextInfo.Clear();
        int visibleLineCount = richTextInfo.ClientSize.Height / richTextInfo.Font.Height;

        SuspendLayout();

        if (visibleLineCount > lines.Length)
        {
            // If there are fewer lines than can fit, add empty lines to fill the space at the top
            int emptyLinesNeeded = (visibleLineCount - lines.Length) / 2;
            for (int i = 0; i < emptyLinesNeeded; i++)
            {
                richTextInfo.AppendText(Environment.NewLine);
            }
        }

        foreach (string line in lines)
        {
            richTextInfo.AppendText(line.Trim() + Environment.NewLine);
        }

        richTextInfo.SelectAll();
        richTextInfo.SelectionAlignment = HorizontalAlignment.Center;
        richTextInfo.ScrollToCaret();

        ResumeLayout(true);
    }

    private void AdjustRichTextBox()
    {
        int visibleLineCount = richTextInfo.ClientSize.Height / richTextInfo.Font.Height;
        List<string> allLines = richTextInfo.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

        int indexStartLine = 0;
        foreach (string line in allLines)
        {
            if (!string.IsNullOrWhiteSpace(line.Trim()))
            {
                break;
            }

            indexStartLine++;
        }

        List<string> lines = allLines.GetRange(indexStartLine, allLines.Count - indexStartLine);


        SuspendLayout();

        if (visibleLineCount > lines.Count)
        {
            // If there are fewer lines than can fit, add empty lines to fill the space at the top
            int emptyLinesNeeded = (visibleLineCount - lines.Count) / 2;
            for (int i = 0; i < emptyLinesNeeded; i++)
            {
                richTextInfo.AppendText(Environment.NewLine);
            }
        }

        foreach (string line in lines)
        {
            richTextInfo.AppendText(line.Trim() + Environment.NewLine);
        }

        richTextInfo.SelectAll();
        richTextInfo.SelectionAlignment = HorizontalAlignment.Center;
        richTextInfo.ScrollToCaret();

        ResumeLayout(true);
    }

    private void SetControl(Control obj)
    {
        SuspendLayout();

        pictureBoxImage.Visible = false;
        pictureBoxImage.Image?.Dispose(); // Dispose of any previous image to free resources
        pictureBoxImage.Image = null;
        pictureBoxImage.Dock = DockStyle.None;

        videoView.Visible = false;
        videoView.Dock = DockStyle.None;
        if (videoView.MediaPlayer?.Media != null)
        {
            videoView.MediaPlayer.Media.Dispose(); // Clear any previous image
            videoView.MediaPlayer.Media = null;
        }
        videoView.Dock = DockStyle.None;

        richTextInfo.Visible = false;
        richTextInfo.Text = string.Empty;
        richTextInfo.Dock = DockStyle.None;

        if (obj is RichTextBox tbi && tbi.Name == "richTextInfo")
        {
            tbi.Visible = true;
            tbi.Clear();
            tbi.Dock = DockStyle.Fill;
            displayedControl = tbi;
        }
        else if (obj is PictureBox pbi && pbi.Name == "pictureBoxImage")
        {
            pbi.Visible = true;
            pbi.Dock = DockStyle.Fill;
            displayedControl = pbi;
        }
        else if (obj is VideoView vvi && vvi.Name == "videoView")
        {
            vvi.MediaPlayer = new MediaPlayer(libvlc);
            vvi.Visible = true;
            vvi.Dock = DockStyle.Fill;
            displayedControl = vvi;
        }
        else
        {
            displayedControl = null;
        }

        ResumeLayout(false);
    }

    private void DisplayForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        CloseImage();
    }

    private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        Version appVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        SetControl(richTextInfo);
        SetRichTextBoxLines(
        [
            $"DecryptDisplay v{appVersion.ToString()}",
            "This application is used to decrypt and display files encrypted with the .twlock format.",
            "Developed by Stewart Nutter",
            "For more information, visit: https://github.com/buzznut/TakeoutWrangler"
        ]);
    }

    private void DisplayForm_SizeChanged(object sender, EventArgs e)
    {
        if (displayedControl == richTextInfo)
        {
            AdjustRichTextBox();
        }
    }

    private void buttonPrevious_Click(object sender, EventArgs e)
    {
        if (allFiles == null || allFiles.Count == 0) return;

        currentFileIndex--;
        if (currentFileIndex < 0)
        {
            currentFileIndex = allFiles.Count - 1; // Loop back to the last file
        }

        DoDisplay();
    }

    private void buttonNext_Click(object sender, EventArgs e)
    {
        if (allFiles == null || allFiles.Count == 0) return;

        currentFileIndex++;
        if (currentFileIndex >= allFiles.Count)
        {
            currentFileIndex = 0; // Loop back to the first file
        }

        DoDisplay();
    }

    private void timerPasswordCheck_Tick(object sender, EventArgs e)
    {
        if (passwordVisible.Elapsed >= TimeSpan.FromMinutes(5))
        {
            timerPasswordCheck.Stop();
            passwordVisible.Stop();
            password = string.Empty;
            currentFileIndex = -1;
            lastCurrentFileIndex = -1;
            allFiles.Clear();
            SetControl(null);
            CloseImage();
        }
    }

    private void CloseImage()
    {
        pictureBoxImage.Image?.Dispose();
        pictureBoxImage.Image = null;

        if (videoView.MediaPlayer != null)
        {
            videoView.MediaPlayer.Media?.Dispose();
            videoView.MediaPlayer.Media = null;
        }

        displayStream?.Dispose();
        displayStream = null;

        originalExtension = string.Empty;
        originalFileName = string.Empty;
    }

    private void DisplayForm_Load(object sender, EventArgs e)
    {
        List<string> args = Environment.GetCommandLineArgs().ToList();
        args.RemoveAt(0); // Remove the first argument which is the executable path

        string all = string.Join(',', args);
        SetRichTextBoxLine(all);

        List<string> argFiles = new List<string>();
        if (args.Count > 0)
        {
            for (int ii = 0; ii < args.Count; ii++)
            {
                if (args[ii].Contains(".twlock", StringComparison.OrdinalIgnoreCase))
                {
                    argFiles.Add(args[ii]);
                }
            }
        }

        if (argFiles.Count > 0)
        {
            ShowFiles(argFiles);
        }
    }

    private void ShowFiles(ICollection<string> selectedFiles)
    {
        allFiles = selectedFiles.ToList();
        if (allFiles.Count == 0)
        {
            SetRichTextBoxLine("No .twlock files selected.");
            return;
        }

        allFiles.Sort();
        currentFileIndex = 0;
        lastCurrentFileIndex = -1;

        GetPassword getPasswordForm = new GetPassword();
        if (getPasswordForm.ShowDialog() == DialogResult.OK)
        {
            password = getPasswordForm.Password;

            if (!string.IsNullOrEmpty(password))
            {
                DoDisplay();
            }
            else
            {
                CloseImage();
                SetRichTextBoxLine("Password is required to decrypt the file.");
            }
        }
    }
}
