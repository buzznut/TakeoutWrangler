//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using AutoUpdaterDotNET;
using PhotoCopyLibrary;
using Spire.Pdf;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using TakeoutWranglerUI;

namespace TakeoutWrangler;

public partial class MainForm : Form
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern uint RegisterWindowMessage(string lpString);

    private PhotoCopier copier;
    private readonly string[] args;
    private AppSettingsJson settings;
    private Configs configs;
    private static uint viewMessage;
    private static uint viewWarning;
    private static uint viewError;
    private static uint viewStatus;
    private static IntPtr myHandle;
    private bool showNoUpdateAvailable;
    private readonly string[] helpFiles = { "Help.pdf", "About.pdf", "HowToGetYourPhotosFromGoogleTakeout.pdf" };
    private readonly Dictionary<string, Stream[]> helpStreams = new Dictionary<string, Stream[]>(StringComparer.OrdinalIgnoreCase)
    {
        { "Help.pdf", null },
        { "About.pdf", null },
        { "HowToGetYourPhotosFromGoogleTakeout.pdf", null }
    };
    private readonly BackgroundWorker worker;
    private readonly string appDir = AppHelpers.GetApplicationDir();
    private bool pagesLoaded;
    private readonly List<ListBoxItem> items = new List<ListBoxItem>();
    private const string updateUrl = "https://raw.githubusercontent.com/buzznut/TakeoutWrangler/refs/heads/master/Installers/TakeoutWrangler/Output/TakeoutWranglerUIupdate.xml";
    private static int totalItems;
    private static int progress;

    private MainForm()
    {
    }

    public MainForm(string[] args)
    {
        SuspendLayout();
        InitializeComponent();

        myHandle = Handle;
        viewMessage = RegisterWindowMessage("UIView_Message");
        viewWarning = RegisterWindowMessage("UIView_Warning");
        viewError = RegisterWindowMessage("UIView_Error");
        viewStatus = RegisterWindowMessage("UIView_Status");
        ResumeLayout(true);

        worker = new BackgroundWorker
        {
            WorkerSupportsCancellation = true,
            WorkerReportsProgress = true
        };

        worker.DoWork += ResolveNextImage;
        worker.RunWorkerCompleted += ResolveNextImageCompleted;

        StartPageResolve(0);

        AutoUpdater.CheckForUpdateEvent += UpdateCheckEvent;
        this.args = args;
    }

    private void StartPageResolve(int index)
    {
        if (index < 0 || index >= helpFiles.Length) return;

        PageLoadData data = new PageLoadData
        {
            PdfFileName = helpFiles[index],
            Index = index
        };

        worker.RunWorkerAsync(data);
    }

    private void ResolveNextImageCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (sender is not BackgroundWorker _) return;

        PageLoadData data = e.Result as PageLoadData;
        if (data == null) return;

        if (e.Cancelled)
        {
            _ = MessageBox.Show("Operation was canceled");
        }
        else if (e.Error != null)
        {
            _ = MessageBox.Show(e.Error.Message);
        }
        else
        {
            helpStreams[data.PdfFileName] = data.Pages;
            data.Pages = null;
        }

        if (data.Index + 1 < helpFiles.Length)
        {
            StartPageResolve(data.Index + 1);
        }
        else
        {
            pagesLoaded = true;
        }
    }

    private void ResolveNextImage(object sender, DoWorkEventArgs e)
    {
        if (sender is not BackgroundWorker bw) return;
        if (appDir == null) return;

        PageLoadData data = e.Argument as PageLoadData;
        if (data == null) return;

        string pdfFile = Path.Combine(appDir, "ResourceFiles", data.PdfFileName);
        using (PdfDocument doc = new PdfDocument(pdfFile))
        {
            data.Pages = new Stream[doc.Pages.Count];

            for (int page = 0; page < doc.Pages.Count; page++)
            {
                bw.ReportProgress(Convert.ToInt32((double)(page + 1) / doc.Pages.Count) * 100);
                FileStream tmpStream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
                doc.SaveToImageStream(page, tmpStream, "bitmap");
                data.Pages[page] = tmpStream;
            }
        }

        e.Result = data;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == viewMessage)
        {
            // Handle the custom message
            string text = Marshal.PtrToStringAuto(m.WParam, Convert.ToInt32(m.LParam));
            items.Add(new ListBoxItem() { ItemColor = listBoxView.ForeColor, Message = text ?? string.Empty });
            timerView.Start();
        }
        else if (m.Msg == viewStatus)
        {
            textBoxStatus.Text = $"{++progress}/{totalItems}";
        }
        else if (m.Msg == viewWarning)
        {
            string text = Marshal.PtrToStringAuto(m.WParam, Convert.ToInt32(m.LParam));
            items.Add(new ListBoxItem() { ItemColor = System.Drawing.Color.DarkOrange, Message = text ?? string.Empty });
            timerView.Start();
        }
        else if (m.Msg == viewError)
        {
            string text = Marshal.PtrToStringAuto(m.WParam, Convert.ToInt32(m.LParam));
            items.Add(new ListBoxItem() { ItemColor = System.Drawing.Color.FromArgb(214, 4, 9), Message = text ?? string.Empty });
            timerView.Start();
        }

        base.WndProc(ref m);
    }

    private PhotoCopier HandleConfiguration(string[] args)
    {
        PhotoCopier photoCopier = new PhotoCopier(OutputHandler, StatusHandler);
        configs = photoCopier.GetConfiguration();

        try
        {
            settings = configs.LoadAppSettings();
            configs.ParseArgs(args);

            configs.GetString("source", out string sourceDir);
            configs.GetString("destination", out string destinationDir);
            configs.GetString("pattern", out string pattern);
            configs.GetString("filter", out string fileFilter);
            configs.GetString("logging", out string loggingString);
            configs.GetString("action", out string actionString);
            configs.GetBool("listonly", out bool listOnly);
            configs.GetBool("parallel", out bool parallel);

            if (!Enum.TryParse(actionString, true, out PhotoCopierActions behavior)) behavior = PhotoCopierActions.Copy;
            if (!Enum.TryParse(loggingString, true, out LoggingVerbosity logging)) logging = LoggingVerbosity.Verbose;

            ReturnCode result = photoCopier.Initialize(nameof(TakeoutWrangler), false, sourceDir, destinationDir, behavior, pattern, fileFilter, logging, listOnly, parallel);
            if (result != ReturnCode.Success)
            {
                buttonRun.Enabled = false;
                return null;
            }
        }
        catch (Exception ex)
        {
            OutputHandler(ex.Message, MessageCode.Error);
        }

        return photoCopier;
    }

    private static void StatusHandler(StatusCode statusCode, int value)
    {
        if (statusCode == StatusCode.Total)
        {
            totalItems = value;
            progress = 0;
            return;
        }
        if (!PostMessage(myHandle, viewStatus, IntPtr.Zero, IntPtr.Zero))
        {
            throw new Exception("Could not post status message.");
        }
    }

    private static void OutputHandler(string output = null, MessageCode errorCode = MessageCode.Success)
    {
        output ??= string.Empty;

        IntPtr lpData = Marshal.StringToHGlobalAuto(output);
        IntPtr lpLength = new IntPtr(output.Length);
        switch (errorCode)
        {
            case MessageCode.Success:
            {
                if (!PostMessage(myHandle, viewMessage, lpData, lpLength))
                {
                    throw new Exception("Could not post view message.");
                }
                break;
            }

            case MessageCode.Warning:
            {
                if (!PostMessage(myHandle, viewWarning, lpData, lpLength))
                {
                    throw new Exception("Could not post view message.");
                }
                break;
            }

            case MessageCode.Error:
            {
                if (!PostMessage(myHandle, viewError, lpData, lpLength))
                {
                    throw new Exception("Could not post view message.");
                }
                break;
            }
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        if (sender is not Form _) return;

        PhotoCopier photoCopier = HandleConfiguration(args);
        if (photoCopier != null)
        {
            copier = photoCopier;
            buttonRun.Enabled = true;
        }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void ShowSettings()
    {
        bool runEnabled = buttonRun.Enabled;
        buttonRun.Enabled = false;

        configs.GetString("action", out string actionString);
        configs.GetString("source", out string source);
        configs.GetString("destination", out string destination);
        configs.GetString("pattern", out string pattern);
        configs.GetString("filter", out string fileFilter);
        configs.GetString("logging", out string loggingString);
        configs.GetBool("listonly", out bool listOnly);
        configs.GetBool("parallel", out bool parallel);

        if (!Enum.TryParse(actionString, true, out PhotoCopierActions behavior)) behavior = PhotoCopierActions.Copy;
        if (!Enum.TryParse(loggingString, true, out LoggingVerbosity logging)) logging = LoggingVerbosity.Verbose;

        SettingsForm settingsForm = new SettingsForm
        {
            Behavior = behavior,
            Source = source,
            Destination = destination,
            Pattern = pattern,
            Filter = fileFilter,
            Logging = logging,
            ListOnly = listOnly,
            Parallel = parallel,
            PhotoCopierSession = copier
        };

        DialogResult result = settingsForm.ShowDialog();
        if (result == DialogResult.Cancel)
        {
            buttonRun.Enabled = runEnabled;
            return;
        }

        if (result != DialogResult.Cancel)
        {
            behavior = settingsForm.Behavior;
            source = settingsForm.Source;
            destination = settingsForm.Destination;
            pattern = settingsForm.Pattern;
            fileFilter = settingsForm.Filter;
            logging = settingsForm.Logging;
            listOnly = settingsForm.ListOnly;
            parallel = settingsForm.Parallel;

            listBoxView.Rows.Clear();
            PhotoCopier photoCopier = new PhotoCopier(OutputHandler, StatusHandler);

            configs.SetString("action", behavior.ToString());
            configs.SetString("source", source);
            configs.SetString("destination", destination);
            configs.SetString("pattern", pattern);
            configs.SetString("logging", logging.ToString());
            configs.SetString("filter", fileFilter);
            configs.SetBool("listonly", listOnly);
            configs.SetBool("parallel", parallel);

            if (result == DialogResult.Yes)
            {
                configs.SaveSettings(settings);
            }

            ReturnCode okay = photoCopier.Initialize(nameof(TakeoutWrangler), false, source, destination, behavior, pattern, fileFilter, logging, listOnly, parallel);
            if (okay == ReturnCode.Success)
            {
                copier = photoCopier;
                buttonRun.Enabled = true;
            }
            else
            {
                copier = null;
                buttonRun.Enabled = false;
            }
        }
    }

    private void timerView_Tick(object sender, EventArgs e)
    {
        if (sender is not System.Windows.Forms.Timer timer) return;

        try
        {
            timer.Stop();
            if (items.Count > 0)
            {
                foreach (ListBoxItem item in items)
                {
                    int index = listBoxView.Rows.Add();
                    DataGridViewCell cell = listBoxView.Rows[index].Cells[0];
                    if (cell == null) { continue; }

                    cell.Value = item.Message;
                    cell.Tag = item.ItemColor;
                }
                listBoxView.FirstDisplayedScrollingRowIndex = listBoxView.RowCount - 1;
                items.Clear();
            }
        }
        finally
        {
            if (copier == null)
            {
                ShowSettings();
            }
        }
    }

    private async void buttonRun_ClickAsync(object sender, EventArgs e)
    {
        if (copier == null) return;
        timerView.Start();

        if ((buttonRun.Tag as bool?).GetValueOrDefault(false) is bool isRunning && isRunning)
        {
            copier.Stop();
        }
        else
        {
            try
            {
                ReturnCode code = await copier.RunAsync().ConfigureAwait(false);
                OutputHandler();
                OutputHandler($"Results: return code={code}", ErrorCodeFromReturnCode(code));
                OutputHandler();
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                OutputHandler(ex.Message, MessageCode.Error);
            }
        }
    }

    public static MessageCode ErrorCodeFromReturnCode(ReturnCode code)
    {
        switch (code)
        {
            case ReturnCode.Success:
                return MessageCode.Success;
            case ReturnCode.HadIssues:
                return MessageCode.Warning;
            case ReturnCode.DirectoryError:
                return MessageCode.Error;
            case ReturnCode.Canceled:
                return MessageCode.Warning;
            case ReturnCode.Error:
            default:
                return MessageCode.Error;
        }
    }

    private void toolStripMenuItemPrint_Click(object sender, EventArgs e)
    {
        PrintConsole(false);
    }

    private void PrintConsole(bool showPreview)
    {
        try
        {
            NewPrintDialog newPrintDialog = new NewPrintDialog(CreateText);
            newPrintDialog.ShowDialog(this);
        }
        catch (Exception)
        {
        }
    }

    private StringBuilder CreateText(bool useSelection)
    {
        StringBuilder sb = new StringBuilder();

        foreach (DataGridViewRow row in listBoxView.Rows)
        {
            if (useSelection && !row.Selected) continue;

            int cellCount = 0;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cellCount > 0) sb.Append(' ');
                sb.Append(cell.Value?.ToString() ?? string.Empty);
                cellCount++;
            }

            sb.AppendLine(string.Empty);
        }

        return sb;
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowSettings();
    }

    private void clearToolStripMenuItem_Click(object sender, EventArgs e)
    {
        listBoxView.Rows.Clear();
    }

    private void TimerIsRunning_Tick(object sender, EventArgs e)
    {
        if (!(copier?.IsRunning).GetValueOrDefault())
        {
            buttonRun.Tag = false;
            buttonRun.Text = Constants.Execute;
        }
        else
        {
            buttonRun.Tag = true;
            buttonRun.Text = Constants.Cancel;
        }
    }

    private void UpdateCheckEvent(UpdateInfoEventArgs args)
    {
        switch (args.Error)
        {
            case null:
                if (args.IsUpdateAvailable)
                {
                    // Uncomment the following line if you want to show standard update dialog instead.
                    AutoUpdater.ShowUpdateForm(args);
                }
                else if (showNoUpdateAvailable)
                {
                    MessageBox.Show(
                        "No update available please try again later.", "Updates",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                break;

            case WebException:
                MessageBox.Show(
                            "There is a problem reaching update server. Please check your internet connection and try again later.",
                            "Update Check Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                break;
            
            default:
#if DEBUG
                if (Debugger.IsAttached) Debugger.Break();
#endif
                MessageBox.Show(args.Error.Message,
                    args.Error.GetType().ToString(), MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                break;
        }

        showNoUpdateAvailable = true;
    }

    private void ToolStripSaveConsole_Click(object sender, EventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Text Files|*.txt", Title = "Save Console Output" };

        DialogResult dr = saveFileDialog.ShowDialog();
        if (dr == DialogResult.OK)
        {
            string filename = saveFileDialog.FileName;

            FileStreamOptions fileStreamOptions = new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create };
            using (TextWriter tw = new StreamWriter(filename, fileStreamOptions))
            {
                foreach (object item in listBoxView.Rows)
                {
                    tw.WriteLine(item.ToString());
                }
            }
        }
    }

    private void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    private void listBoxView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.C && e.Control)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataGridViewRow item in listBoxView.SelectedRows)
            {
                if (item == null)
                    sb.AppendLine();
                else
                {
                    int count = 0;
                    foreach (DataGridViewCell cell in item.Cells)
                    {
                        if (count > 0) sb.Append(", ");
                        sb.Append((cell.Value as string) ?? "");
                        count++;
                    }
                    sb.AppendLine();
                }
            }

            if (sb.Length > 0)
            {
                Clipboard.SetText(sb.ToString());
            }
        }
        else if (e.KeyCode == Keys.A && e.Control)
        {
            foreach (int index in Enumerable.Range(0, listBoxView.Rows.Count))
            {
                listBoxView.Rows[index].Selected = true;
                foreach (DataGridViewCell cell in listBoxView.Rows[index].Cells)
                {
                    cell.Selected = true;
                }
            }
        }
    }

    private void toolStripMenuTakeout_Click(object sender, EventArgs e)
    {
        OpenUrl("https://takeout.google.com/");
    }

    private void howToUseGoogleTakeoutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (appDir == null) return;

        HelpViewer viewer = new HelpViewer();
        if (pagesLoaded)
            viewer.Initialize("How to get your photos from Google Takeout", helpStreams["HowToGetYourPhotosFromGoogleTakeout.pdf"]);
        else
            viewer.Initialize(appDir, "How to get your photos from Google Takeout", "HowToGetYourPhotosFromGoogleTakeout.pdf");

        viewer.ShowDialog();
    }

    private void helpToolStripMenuItemHelp_Click(object sender, EventArgs e)
    {
        if (appDir == null) return;

        HelpViewer viewer = new HelpViewer();
        if (pagesLoaded)
            viewer.Initialize("How to get your photos from Google Takeout", helpStreams["Help.pdf"]);
        else
            viewer.Initialize(appDir, "TakeoutWrangler help", "Help.pdf");
        viewer.ShowDialog();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (appDir == null) return;

        HelpViewer viewer = new HelpViewer();
        if (pagesLoaded)
            viewer.Initialize("How to get your photos from Google Takeout", helpStreams["About.pdf"]);
        else
            viewer.Initialize(appDir, "About TakeoutWrangler", "About.pdf");
        viewer.ShowDialog();
    }

    private void listBoxView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
        if (sender is not DataGridView lbv) return;

        if (e.RowIndex < 0 || e.Graphics == null) return;
        DataGridViewRow row = lbv.Rows[e.RowIndex];
        using (SolidBrush backBrush = row.Selected ? new SolidBrush(SystemColors.Highlight) : new SolidBrush(BackColor))
        {
            Font cellFont = e.CellStyle?.Font ?? lbv.Font;
            lbv.RowTemplate.Height = cellFont.Height;
            row.Height = cellFont.Height;

            var cell = lbv.Rows[e.RowIndex].Cells[0];
            if (cell == null) return;

            System.Drawing.Color itemColor = cell.Tag as System.Drawing.Color? ?? ForeColor;

            using (SolidBrush foreBrush = row.Selected ? new SolidBrush(BackColor) : new SolidBrush(itemColor))
            {
                e.Graphics.FillRectangle(backBrush, e.CellBounds);
                e.Graphics.DrawString((cell.Value as string) ?? "", cellFont, foreBrush, e.CellBounds);
            }
            e.Handled = true;
        }
    }

    private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(updateUrl)) AutoUpdater.Start(updateUrl);
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
        PrintConsole(true);
    }

    private void printPreviewDialog_Click(object sender, EventArgs e)
    {

    }
}

public class PageLoadData
{
    public string PdfFileName { get; set; }
    public Stream[] Pages { get; set; }
    public int Index { get; set; }
}

public class ListBoxItem
{
    public string Message { get; set; }
    public Color ItemColor { get; set; }
}
