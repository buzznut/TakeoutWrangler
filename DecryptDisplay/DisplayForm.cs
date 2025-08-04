//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:24:7:12
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
    private double zoomFactor = 1.0;
    private Image originalImage = null;
    private int p_left;
    private int p_top;
    private int p_right;
    private int p_bottom;

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

        p_left = 32;
        p_top = 22;
        p_right = 42;
        p_bottom = 40;

        panelPicture.Controls.Add(videoView);
        panelPicture.Controls.Add(pictureBoxImage);
        panelPicture.Controls.Add(richTextInfo);

        pictureBoxImage.BorderStyle = BorderStyle.None;
        panelPicture.BorderStyle = BorderStyle.None;
        richTextInfo.BorderStyle = BorderStyle.None;

        // Enable MouseWheel event
        MouseWheel += PictureBox_MouseWheel;

        SetControl(richTextInfo);
        ResizePanel();

        ResumeLayout(true);
    }

    private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
    {
        // Adjust zoom factor based on scroll direction
        bool adjusted = false;
        if (e.Delta > 0)
        {
            zoomFactor *= 1.1f; // Zoom in
            adjusted = true;
        }
        else if (e.Delta < 0 && zoomFactor > 0.1f)
        {
            zoomFactor /= 1.1f; // Zoom out
            adjusted = true;
        }

        if (adjusted)
        {
            SetZoom();
        }
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
            Title = "Select a file to open",
            Filter = "All Files (*.*)|*.*",
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

                originalExtension = string.Empty;
                bool isEncrypted = originalFileName.EndsWith(".twl", StringComparison.OrdinalIgnoreCase);

                using (FileStream stream = new FileStream(originalFileName, FileMode.Open, FileAccess.Read))
                {
                    if (isEncrypted)
                    {
                        tmpPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(originalFileName) + ".tmp");
                        displayStream = new FileStream(tmpPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose | FileOptions.Encrypted);
                        FileEncryption.DecryptFile(stream, displayStream, password, out originalExtension, out DateTime originalDate);
                    }
                    else
                    {
                        originalExtension = Path.GetExtension(originalFileName);
                        displayStream = stream;
                    }

                    if (pictureFormats.Contains(originalExtension))
                    {
                        wasShown = DisplayImage(displayStream);
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
                    else if (IsTextFile(displayStream))
                    {
                        SetControl(richTextInfo);
                        string[] lines;

                        displayStream.Seek(0, SeekOrigin.Begin);

                        using (StreamReader reader = new StreamReader(displayStream, true))
                        {
                            lines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.TrimEntries);
                            if (lines.Length > 0)
                            {
                                SetRichTextBoxLines(lines, false);
                                wasShown = true;
                            }
                        }
                    }
                }

                if (!wasShown)
                {
                    // If the decrypted file is neither an image, text file, nor a movie, show a warning
                    SetControl(richTextInfo);
                    SetRichTextBoxLines(["The file is not a supported format."]);
                }
            }
            catch
            {
                SetControl(richTextInfo);
                SetRichTextBoxLine($"Could not open: \"{originalFileName}\"");
            }
        }
    }

    public static bool IsTextFile(Stream stream)
    {
        const int sampleSize = 1024; // Read the first 1024 bytes
        byte[] buffer = new byte[sampleSize];

        // set stream position to the beginning
        stream.Seek(0, SeekOrigin.Begin);

        int bytesRead = stream.Read(buffer, 0, sampleSize);

        // Reset stream position after reading
        stream.Seek(0, SeekOrigin.Begin);

        // Check for non-text characters
        for (int i = 0; i < bytesRead; i++)
        {
            if (buffer[i] == 0) // Null byte indicates binary
                return false;

            // Check for valid ASCII range (optional: extend for UTF-8)
            if (buffer[i] < 32 && buffer[i] != 9 && buffer[i] != 10 && buffer[i] != 13)
                return false;
        }

        return true;
    }

    private bool DisplayImage(Stream imageStream)
    {
        try
        {
            // If the file is an image, display it in the PictureBox
            SetControl(pictureBoxImage);
            passwordVisible.Restart();
            timerPasswordCheck.Start();

            displayStream.Seek(0, SeekOrigin.Begin);
            originalImage = Image.FromStream(displayStream);

            zoomFactor = Math.Min(1.0, Math.Min(pictureBoxImage.Width / (double)originalImage.Width, pictureBoxImage.Height / (double)originalImage.Height)); // Reset the zoom factor to 1.0
            SetImage(pictureBoxImage);

            return true;
        }
        catch (Exception ex)
        {
            SetControl(richTextInfo);
            SetRichTextBoxLine($"Error displaying image: {ex.Message}");
            return false;
        }
    }

    private void SetRichTextBoxLine(string line, bool center = true)
    {
        string[] lines = line.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        SetRichTextBoxLines(lines, center);
    }

    private void SetRichTextBoxLines(string[] lines, bool center = true)
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
        richTextInfo.SelectionAlignment = center ? HorizontalAlignment.Center : HorizontalAlignment.Left;
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
        originalImage?.Dispose(); // Dispose of the original image if it exists
        originalImage = null;

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
            buttonZoomIn.Visible = false;
            buttonZoomOut.Visible = false;
        }
        else if (obj is PictureBox pbi && pbi.Name == "pictureBoxImage")
        {
            pbi.Visible = true;
            pbi.Dock = DockStyle.None;
            pbi.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pbi.SizeMode = PictureBoxSizeMode.AutoSize;
            displayedControl = pbi;
            buttonZoomIn.Visible = true;
            buttonZoomOut.Visible = true;
        }
        else if (obj is VideoView vvi && vvi.Name == "videoView")
        {
            vvi.MediaPlayer = new MediaPlayer(libvlc);
            vvi.Visible = true;
            vvi.Dock = DockStyle.Fill;
            displayedControl = vvi;
            buttonZoomIn.Visible = false;
            buttonZoomOut.Visible = false;
        }
        else
        {
            displayedControl = null;
        }

        ResizeControl();
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
            "This application is used to decrypt and display files encrypted with the .twl format.",
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
                if (args[ii].Contains(".twl", StringComparison.OrdinalIgnoreCase))
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
            SetRichTextBoxLine("No files selected.");
            return;
        }

        allFiles.Sort();
        currentFileIndex = 0;
        lastCurrentFileIndex = -1;
        bool haveEncryptedFiles = allFiles.Any(x => x.EndsWith(".twl", StringComparison.OrdinalIgnoreCase));

        if (haveEncryptedFiles && string.IsNullOrEmpty(password))
        {
            RequestPassword();
        }

        DoDisplay();
    }

    private void RequestPassword()
    {
        password = string.Empty;

        SetControl(richTextInfo);
        SetRichTextBoxLine("Please enter the password to decrypt the file.");

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

    private void toolStripMenuItemPassword_Click(object sender, EventArgs e)
    {
        RequestPassword();
    }

    private void buttonZoomIn_Click(object sender, EventArgs e)
    {
        zoomFactor *= 1.2; // Increase zoom factor by 20%
        if (displayedControl is PictureBox pictureBox && originalImage != null)
        {
            SetImage(pictureBox);
        }
        else if (displayedControl is VideoView videoView && videoView.MediaPlayer != null)
        {
            videoView.MediaPlayer.Scale = (float)zoomFactor;
        }
    }

    private void buttonZoomOut_Click(object sender, EventArgs e)
    {
        zoomFactor /= 1.2; // Decrease zoom factor by 20%
        if (displayedControl is PictureBox pictureBox && originalImage != null)
        {
            SetImage(pictureBox);
        }
        else if (displayedControl is VideoView videoView && videoView.MediaPlayer != null)
        {
            videoView.MediaPlayer.Scale = (float)zoomFactor;
        }
    }

    private void SetImage(PictureBox pictureBox)
    {
        pictureBox.SuspendLayout();
        Size newSize = new Size((int)(originalImage.Width * zoomFactor), (int)(originalImage.Height * zoomFactor));
        pictureBox.Image?.Dispose(); // Dispose of any previous image to free resources
        pictureBox.Image = new Bitmap(originalImage, newSize);
        pictureBox.Invalidate();
        ResizeControl();
        pictureBox.ResumeLayout(true);
    }

    private void buttonReset_Click(object sender, EventArgs e)
    {
        if (displayedControl is PictureBox pictureBox && originalImage != null)
        {
            zoomFactor = Math.Min(1.0, Math.Min(pictureBoxImage.Width / (double)originalImage.Width, pictureBoxImage.Height / (double)originalImage.Height)); // Reset the zoom factor to 1.0
            SetImage(pictureBox);
        }
        else if (displayedControl is VideoView videoView && videoView.MediaPlayer != null)
        {
            zoomFactor = 1.0; // Reset zoom factor to 1.0
            videoView.MediaPlayer.Scale = 1.0f; // Reset video scale to 1.0
        }
    }

    private void SetZoom()
    {
        if (displayedControl is PictureBox pictureBox && originalImage != null)
        {
            SetImage(pictureBox);
        }
        else if (displayedControl is VideoView videoView && videoView.MediaPlayer != null)
        {
            videoView.MediaPlayer.Scale = (float)zoomFactor; // Reset video scale to 1.0
        }
    }

    private void DisplayForm_ResizeEnd(object sender, EventArgs e)
    {
        //ResizePanel();
    }

    private void ResizePanel()
    {
        panelPicture.Location = new Point(p_left, p_top);
        panelPicture.Size = new Size(
            Math.Max(Width - p_left - p_right, 1),
            Math.Max(Height - p_top - p_bottom, 1));

        panelPicture.ResumeLayout(true);

        ResizeControl();
    }

    private void ResizeControl()
    {
        if (displayedControl == null) return;

        displayedControl.SuspendLayout();

        if (displayedControl == null) return;
        displayedControl.Location = new Point(0, 0);
        displayedControl.Size = new Size(
            Math.Max(panelPicture.Width - 1, 1),
            Math.Max(panelPicture.Height - 1, 1));

        displayedControl.ResumeLayout(true);
    }
}
