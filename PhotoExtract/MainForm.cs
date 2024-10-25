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
    private static uint customMessage;
    private static IntPtr myHandle;

    private MainForm()
    {

    }

    public MainForm(string[] args)
    {
        SuspendLayout();
        InitializeComponent();

        myHandle = Handle;
        customMessage = RegisterWindowMessage("UIView_Message");
        visibleItems = listBoxView.ClientSize.Height / listBoxView.ItemHeight;
        ResumeLayout(true);

        this.args = args;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == customMessage)
        {
            // Handle the custom message
            listBoxView.SuspendLayout();
            string text = Marshal.PtrToStringAuto(m.WParam, Convert.ToInt32(m.LParam));
            listBoxView.Items.Add(text ?? string.Empty);
            timerView.Start();
        }

        base.WndProc(ref m);
    }

    private PhotoCopier HandleConfiguration(string[] args)
    {
        PhotoCopier photoCopier = new PhotoCopier(OutputHandler);
        configs = photoCopier.GetConfiguration();

        try
        {
            settings = configs.LoadAppSettings();
            configs.ParseArgs(args);

            configs.GetString("source", out string sourceDir);
            configs.GetString("destination", out string destinationDir);
            configs.GetString("pattern", out string pattern);
            configs.GetString("filter", out string fileFilter);
            configs.GetBool("quiet", out bool quiet);
            configs.GetString("action", out string actionText);

            int result = photoCopier.Initialize(nameof(TakeoutWrangler), false, sourceDir, destinationDir, actionText, pattern, fileFilter, quiet);
            if (result != 0)
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

    private static void OutputHandler(string output)
    {
        output ??= string.Empty;

        IntPtr lpData = Marshal.StringToHGlobalAuto(output);
        IntPtr lpLength = new IntPtr(output.Length);
        if (!PostMessage(myHandle, customMessage, lpData, lpLength))
        {
            throw new Exception("Could not post message.");
        }
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
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

        configs.GetString("action", out string actionText);
        configs.GetString("source", out string source);
        configs.GetString("destination", out string destination);
        configs.GetString("pattern", out string pattern);
        configs.GetString("filter", out string fileFilter);
        configs.GetBool("quiet", out bool quiet);

        SettingsForm settingsForm = new SettingsForm
        {
            ActionText = actionText,
            Source = source,
            Destination = destination,
            Pattern = pattern,
            Filter = fileFilter,
            Quiet = quiet
        };

        DialogResult result = settingsForm.ShowDialog();
        if (result == DialogResult.Cancel)
        {
            buttonRun.Enabled = runEnabled;
            return;
        }

        if (result != DialogResult.Cancel)
        {
            actionText = settingsForm.ActionText;
            source = settingsForm.Source;
            destination = settingsForm.Destination;
            pattern = settingsForm.Pattern;
            fileFilter = settingsForm.Filter;
            quiet = settingsForm.Quiet;

            listBoxView.Items.Clear();
            PhotoCopier photoCopier = new PhotoCopier(OutputHandler);

            //Configs configs = photoCopier.GetConfiguration();

            configs.SetString("action", actionText);
            configs.SetString("source", source);
            configs.SetString("destination", destination);
            configs.SetString("pattern", pattern);
            configs.SetBool("quiet", quiet);
            configs.SetString("filter", fileFilter);

            if (result == DialogResult.Yes)
            {
                configs.SaveSettings(settings);
            }

            int okay = photoCopier.Initialize(nameof(TakeoutWrangler), false, source, destination, actionText, pattern, fileFilter, quiet);
            if (okay == 0)
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

        await copier.RunAsync();
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
        if (DialogResult.OK == printDialog.ShowDialog())
        {
            StringBuilder sb = new StringBuilder();
            foreach (object line in listBoxView.Items)
            {
                string text = line as string;
                sb.AppendLine(text);
            }

            printDocument.Print();
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
}
