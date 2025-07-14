//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System.Drawing.Printing;
using System.Text;
using System.ComponentModel;

namespace TakeoutWranglerUI;

public partial class NewPrintDialog : Form
{
    private bool initd;
    private NewPrintDocument document;

    public delegate StringBuilder GetContent(bool useSelection);
    public GetContent Content;
    public StringBuilder StringToPrint;
    public Font PrintFont;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int PageCount { private set; get; }

    public bool HadOutput;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public NewPrintDocument Document
    {
        get => document;
        private set
        {
            value.QueryPageSettings += printDocument_QueryPageSettings;
            value.PrintPage += printDocument_PrintPage;
            value.BeginPrint += printDocument_BeginPrint;
            value.EndPrint += printDocument_EndPrint;
            if (value.IsPreview)
            {
                printPreviewControl.Document = value;
                document = value;
            }
            else
            {
                printPreviewControl.Document = null;
                document = value;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool UseAntiAlias
    {
        get => Document.IsPreview ? printPreviewControl.UseAntiAlias : false;
        set
        {
            StringToPrint = Content(Document.PrinterSettings.PrintRange == PrintRange.Selection);
            if (Document.IsPreview)
            {
                printPreviewControl.UseAntiAlias = value;
            }
        }
    }

    public NewPrintDialog(GetContent content) : base()
    {
        Content = content;
        PrintFont = new Font("Arial", 12);

        SuspendLayout();
        InitializeComponent();

        Document = new NewPrintDocument();
        printPreviewControl.StartPageChanged += previewControl_StartPageChanged;

        numericUpDownCopies.Value = 1;

        PrinterSettings ps = new PrinterSettings();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            comboBoxPrinters.Items.Add(printer);
        }

        if (!string.IsNullOrEmpty(ps.PrinterName))
        {
            int index = comboBoxPrinters.FindString(ps.PrinterName);
            if (index >= 0)
            {
                comboBoxPrinters.SelectedIndex = index;
            }

            ShowSettings(ps);
        }

        initd = true;
        ResumeLayout(true);
    }

    private void ShowSettings(PrinterSettings ps)
    {
        SuspendLayout();

        try
        {
            comboBoxBothSides.Items.Clear();

            if (ps.CanDuplex)
            {
                comboBoxBothSides.Enabled = true;

                foreach (Duplex duplex in Enum.GetValues<Duplex>())
                {
                    comboBoxBothSides.Items.Add(duplex.ToString());
                }

                int index = comboBoxBothSides.FindStringExact(ps.Duplex.ToString());
                comboBoxBothSides.SelectedIndex = index;
            }
            else
            {
                comboBoxBothSides.Enabled = false;
            }

            if (ps.SupportsColor)
            {
                comboBoxColorMode.Enabled = true;
            }
            else
            {
                comboBoxColorMode.Enabled = false;
            }

            switch (ps.PrintRange)
            {
                case PrintRange.AllPages:
                {
                    radioButtonAll.Checked = true;
                    textBoxFromPage.Enabled = false;
                    textBoxToPage.Enabled = false;
                    textBoxFromPage.Text = string.Empty;
                    textBoxToPage.Text = string.Empty;
                    break;
                }

                case PrintRange.SomePages:
                {
                    radioButtonPages.Checked = true;
                    textBoxFromPage.Enabled = true;
                    textBoxToPage.Enabled = true;
                    textBoxFromPage.Text = Document.PrinterSettings.FromPage.ToString();
                    textBoxToPage.Text = Document.PrinterSettings.ToPage.ToString();
                    break;
                }

                case PrintRange.Selection:
                {
                    radioButtonSelection.Checked = true;
                    textBoxFromPage.Enabled = false;
                    textBoxToPage.Enabled = false;
                    textBoxFromPage.Text = string.Empty;
                    textBoxToPage.Text = string.Empty;
                    break;
                }

                default:
                    break;
            }

        }
        finally
        {
            ResumeLayout(true);
        }
    }

    private void previewControl_StartPageChanged(object sender, EventArgs e)
    {
    }

    private void printDocument_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
    {
    }

    private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
        if (sender is not PrintDocument printInfo) return;

        if (e.Graphics == null)
        {
            e.Cancel = true;
            return;
        }

        if (PrintFont == null)
        {
            PrintFont = new Font("Arial", 12);
        }

        PrinterSettings ps = e.PageSettings.PrinterSettings;
        string toPrint = StringToPrint.ToString();

        // Sets the value of charactersOnPage to the number of characters
        // of stringToPrint that will fit within the bounds of the page.
        e.Graphics.MeasureString(toPrint, PrintFont,
            e.MarginBounds.Size, StringFormat.GenericTypographic,
            out int charactersOnPage, out int _);

        string textOnThisPage = toPrint.Substring(0, charactersOnPage);
        PageCount++;

        if (ps.PrintRange != PrintRange.SomePages || PageCount >= ps.FromPage && PageCount <= ps.ToPage)
        {
            HadOutput = true;

            // Draws the string within the bounds of the page
            e.Graphics.DrawString(textOnThisPage, PrintFont, Brushes.Black,
                e.MarginBounds, StringFormat.GenericTypographic);
        }

        // Remove the portion of the string that has been printed.
        _ = StringToPrint.Remove(0, charactersOnPage);

        // Check to see if more pages are to be printed.
        e.HasMorePages = (StringToPrint.Length > 0);
        if (!e.HasMorePages)
        {
            Done();

            if (!HadOutput)
            {
                e.Cancel = true;
            }

            return;
        }
    }

    private void Done()
    {
    }

    private void buttonPrev_Click(object sender, EventArgs e)
    {
        if (printPreviewControl.StartPage > 0)
        {
            printPreviewControl.StartPage--;
        }

        SetPageDetail();
    }

    private void buttonNext_Click(object sender, EventArgs e)
    {
        if (printPreviewControl.StartPage < PageCount - 1)
        {
            printPreviewControl.StartPage++;
        }

        SetPageDetail();
    }

    private void SetPageDetail()
    {
        if (PageCount > 1)
        {
            buttonPrev.Enabled = printPreviewControl.StartPage > 0;
            buttonNext.Enabled = printPreviewControl.StartPage < PageCount - (printPreviewControl.Columns * printPreviewControl.Rows);
        }
        else
        {
            buttonNext.Enabled = false;
            buttonPrev.Enabled = false;
        }

        textBoxPage.Text = $"{printPreviewControl.StartPage}/{PageCount}";
    }

    private void numericUpDownDisplayPageCount_ValueChanged(object sender, EventArgs e)
    {
        if (numericUpDownDisplayPageCount.Value > 1)
        {
            printPreviewControl.Columns = 2;

            if (numericUpDownDisplayPageCount.Value > 2)
            {
                printPreviewControl.Rows = 2;
            }
            else
            {
                printPreviewControl.Rows = 1;
            }
        }
        else
        {
            printPreviewControl.Rows = 1;
            printPreviewControl.Columns = 1;
        }
    }

    private void printDocument_EndPrint(object sender, PrintEventArgs e)
    {
        if (e.PrintAction == PrintAction.PrintToPreview)
        {
            SetPageDetail();
        }
    }

    private void printDocument_BeginPrint(object sender, PrintEventArgs e)
    {
    }

    private void comboBoxPrinters_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!initd) return;

        string oldName = Document.PrinterSettings.PrinterName;
        string newName = comboBoxPrinters.Text;
        if (oldName != newName)
        {
            Document.PrinterSettings.PrinterName = newName;
            ShowSettings(Document.PrinterSettings);
        }
    }

    private void radioButtonAll_CheckedChanged(object sender, EventArgs e)
    {
        if (!initd) return;

        if (radioButtonAll.Checked)
        {
            Document.PrinterSettings.PrintRange = PrintRange.AllPages;
            ShowSettings(Document.PrinterSettings);
            if (Document.IsPreview)
            {
                StringToPrint = Content(Document.PrinterSettings.PrintRange == PrintRange.Selection);
                printPreviewControl.InvalidatePreview();
            }
        }
    }

    private void radioButtonSelection_CheckedChanged(object sender, EventArgs e)
    {
        if (!initd) return;

        if (radioButtonSelection.Checked)
        {
            Document.PrinterSettings.PrintRange = PrintRange.Selection;
            if (Document.IsPreview)
            {
                StringToPrint = Content(Document.PrinterSettings.PrintRange == PrintRange.Selection);
                printPreviewControl.InvalidatePreview();
            }
        }
    }

    private void radioButtonPages_CheckedChanged(object sender, EventArgs e)
    {
        if (!initd) return;

        if (radioButtonPages.Checked)
        {
            Document.PrinterSettings.PrintRange = PrintRange.SomePages;

            int.TryParse(textBoxFromPage.Text, out int fromValue);
            Document.PrinterSettings.FromPage = fromValue;

            int.TryParse(textBoxToPage.Text, out int toValue);
            Document.PrinterSettings.ToPage = toValue;

            ShowSettings(Document.PrinterSettings);
            if (Document.IsPreview)
            {
                StringToPrint = Content(Document.PrinterSettings.PrintRange == PrintRange.Selection);
                printPreviewControl.InvalidatePreview();
            }
        }
    }

    private void textBoxFromPage_Validating(object sender, CancelEventArgs e)
    {
        if (sender is TextBox tb && int.TryParse(tb.Text, out int value)) return;
        e.Cancel = true;
    }

    private void textBoxToPage_Validating(object sender, CancelEventArgs e)
    {
        if (sender is TextBox tb && int.TryParse(tb.Text, out int value)) return;
        e.Cancel = true;
    }

    private void buttonPreview_Click(object sender, EventArgs e)
    {
        if (!Document.IsPreview)
        {
            NewPrintDocument oldValue = Document;
            Document = new NewPrintDocument() { IsPreview = true };
            if (oldValue != null)
            {
                Document.PrinterSettings.Copies = oldValue.PrinterSettings.Copies;
                Document.PrinterSettings.PrintRange = oldValue.PrinterSettings.PrintRange;
                oldValue.Dispose();
            }
        }

        StringToPrint = Content(Document.PrinterSettings.PrintRange == PrintRange.Selection);
        buttonPreview.Enabled = false;
    }

    private void buttonPrint_Click(object sender, EventArgs e)
    {
        StringToPrint = Content(Document.PrinterSettings.PrintRange == PrintRange.Selection);
        Document.PrinterSettings.Copies = (short)Math.Max(1, Math.Min(numericUpDownCopies.Maximum, Convert.ToInt32(numericUpDownCopies.Value)));

        Document.Print();
    }
}

public class NewPrintDocument : PrintDocument
{
    public bool IsPreview;
}
