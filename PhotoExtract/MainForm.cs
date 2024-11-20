//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:11:16:13:40
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using PhotoCopyLibrary;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;

namespace TakeoutWrangler;

public partial class MainForm : Form
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern uint RegisterWindowMessage(string lpString);

    private int visibleItems;
    private PhotoCopier copier;
    private string stringToPrint;
    private DateTime timerPause = DateTime.MinValue;
    private readonly string[] args;
    private AppSettingsJson settings;
    private Configs configs;
    private static uint viewMessage;
    private static uint viewStatus;
    private static IntPtr myHandle;

    private MainForm()
    {
    }

    public MainForm(string[] args)
    {
        SuspendLayout();
        InitializeComponent();

        myHandle = Handle;
        viewMessage = RegisterWindowMessage("UIView_Message");
        viewStatus = RegisterWindowMessage("UIView_Status");
        visibleItems = listBoxView.ClientSize.Height / listBoxView.ItemHeight;
        ResumeLayout(true);

        this.args = args;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == viewMessage)
        {
            // Handle the custom message
            listBoxView.SuspendLayout();
            string text = Marshal.PtrToStringAuto(m.WParam, Convert.ToInt32(m.LParam));
            listBoxView.Items.Add(text ?? string.Empty);
            timerView.Start();
        }
        else if (m.Msg == viewStatus)
        {
            string text = Marshal.PtrToStringAuto(m.WParam, Convert.ToInt32(m.LParam));
            textBoxStatus.Text = text?.PadRight(13) ?? string.Empty;
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

            if (!Enum.TryParse(actionString, true, out PhotoCopierActions behavior)) behavior = PhotoCopierActions.Copy;
            if (!Enum.TryParse(loggingString, true, out LoggingVerbosity logging)) logging = LoggingVerbosity.Verbose;

            int result = photoCopier.Initialize(nameof(TakeoutWrangler), false, sourceDir, destinationDir, behavior, pattern, fileFilter, logging, listOnly);
            if (result != (int)ReturnCode.Success)
            {
                photoCopier = null;
            }
        }
        catch (Exception ex)
        {
            OutputHandler(ex.Message);
        }

        return photoCopier;
    }

    private static void StatusHandler(string status)
    {
        status ??= string.Empty;

        IntPtr lpData = Marshal.StringToHGlobalAuto(status);
        IntPtr lpLength = new IntPtr(status.Length);
        if (!PostMessage(myHandle, viewStatus, lpData, lpLength))
        {
            throw new Exception("Could not post status message.");
        }
    }

    private static void OutputHandler(string output = null)
    {
        output ??= string.Empty;

        IntPtr lpData = Marshal.StringToHGlobalAuto(output);
        IntPtr lpLength = new IntPtr(output.Length);
        if (!PostMessage(myHandle, viewMessage, lpData, lpLength))
        {
            throw new Exception("Could not post view message.");
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
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
            ListOnly = listOnly
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

            listBoxView.Items.Clear();
            PhotoCopier photoCopier = new PhotoCopier(OutputHandler, StatusHandler);

            configs.SetString("action", behavior.ToString());
            configs.SetString("source", source);
            configs.SetString("destination", destination);
            configs.SetString("pattern", pattern);
            configs.SetString("logging", logging.ToString());
            configs.SetString("filter", fileFilter);
            configs.SetBool("listonly", listOnly);

            if (result == DialogResult.Yes)
            {
                configs.SaveSettings(settings);
            }

            int okay = photoCopier.Initialize(nameof(TakeoutWrangler), false, source, destination, behavior, pattern, fileFilter, logging, listOnly);
            if (okay == (int)ReturnCode.Success)
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

    private void listBoxView_SizeChanged(object sender, EventArgs e)
    {
        visibleItems = listBoxView.ClientSize.Height / listBoxView.ItemHeight;
    }

    private void timerView_Tick(object sender, EventArgs e)
    {
        try
        {
            timerView.Stop();
            listBoxView.TopIndex = Math.Max(listBoxView.Items.Count - visibleItems + 1, 0);
            listBoxView.ResumeLayout(false);
        }
        finally
        {
            if (DateTime.Now - timerPause < TimeSpan.FromMilliseconds(1000))
            {
                timerView.Start();
            }
            else if (copier == null)
            {
                ShowSettings();
            }
        }
    }

    private async void buttonRun_Click(object sender, EventArgs e)
    {
        if (copier == null) return;
        timerView.Start();

        if ((buttonRun.Tag as bool?).GetValueOrDefault(false) is bool isRunning && isRunning)
        {
            copier.Stop();
        }
        else
        {
            ReturnCode code = await copier.RunAsync();
            OutputHandler();
            OutputHandler($"Results: {code}");
        }
    }

    private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
        if (e.Graphics == null) return;

        // Sets the value of charactersOnPage to the number of characters
        // of stringToPrint that will fit within the bounds of the page.
        e.Graphics.MeasureString(stringToPrint, Font,
            e.MarginBounds.Size, StringFormat.GenericTypographic,
            out int charactersOnPage, out int _);

        // Draws the string within the bounds of the page
        e.Graphics.DrawString(stringToPrint, Font, Brushes.Black,
            e.MarginBounds, StringFormat.GenericTypographic);

        // Remove the portion of the string that has been printed.
        stringToPrint = stringToPrint.Substring(charactersOnPage);

        // Check to see if more pages are to be printed.
        e.HasMorePages = (stringToPrint.Length > 0);
    }

    private void toolStripMenuItemPrint_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            foreach (object item in listBoxView.Items)
            {
                sb.AppendLine(item?.ToString() ?? string.Empty);
            }

            stringToPrint = sb.ToString();

            if (DialogResult.OK == printDialog.ShowDialog())
            {
                printDocument.Print();
            }
        }
        catch (Exception ex)
        {
            var foo = ex;
        }
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowSettings();
    }

    private void clearToolStripMenuItem_Click(object sender, EventArgs e)
    {
        listBoxView.Items.Clear();
    }

    private void timerIsRunning_Tick(object sender, EventArgs e)
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
}
