using PhotoCopyLibrary;
using Spire.Pdf;
using System.ComponentModel;
using System.Reflection;

namespace TakeoutWranglerUI;

public partial class HelpViewer : Form
{
    private int currentPage;
    private int totalPages;
    private Dictionary<int, Image> images = new Dictionary<int, Image>();
    private PdfDocument doc = new PdfDocument();
    private BackgroundWorker worker = new BackgroundWorker();
    private Image wait;

    public HelpViewer()
    {
        InitializeComponent();
    }

    private void buttonEnd_Click(object sender, EventArgs e)
    {
        currentPage = totalPages - 1;

        EnableButtons();
        DrawPage();
    }

    private void buttonRight_Click(object sender, EventArgs e)
    {
        currentPage = Math.Min(currentPage + 1, totalPages - 1);

        EnableButtons();
        DrawPage();
    }

    private void buttonLeft_Click(object sender, EventArgs e)
    {
        currentPage = Math.Max(currentPage - 1, 0);

        EnableButtons();
        DrawPage();
    }

    private void buttonBegin_Click(object sender, EventArgs e)
    {
        currentPage = 0;

        EnableButtons();
        DrawPage();
    }

    internal void Initialize(string appDir, string title, string pdfName)
    {
        SuspendLayout();

        InitializeBackgroundWorker();

        if (appDir != null && Directory.Exists(appDir))
        {
            string pdfFile = Path.Combine(appDir, "ResourceFiles", pdfName);
            if (!File.Exists(pdfFile)) return;

            Stream waitStream = LoadBitmapFromResources(appDir);
            doc.LoadFromFile(pdfFile);
            Text = title;

            totalPages = doc.Pages.Count;
            currentPage = 0;

            if (waitStream != null)
            {
                pictureBoxView.Image = Image.FromStream(waitStream);
            }
        }

        ResumeLayout(true);
    }

    private void InitializeBackgroundWorker()
    {
        worker = new BackgroundWorker
        {
            WorkerSupportsCancellation = true
        };

        worker.DoWork += ResolveImage;
        worker.RunWorkerCompleted += ResolveImageCompleted;
    }

    private void DrawPage()
    {
        if (images.TryGetValue(currentPage, out Image image))
        {
            pictureBoxView.Image = image;
            pictureBoxView.Invalidate();
            splitContainerView.Panel1.VerticalScroll.Value = 0;
            return;
        }

        if (worker.IsBusy) return;

        ImageResolveData data = new ImageResolveData
        {
            Document = doc,
            CurrentPage = currentPage
        };

        worker.RunWorkerAsync(data);
    }

    private void EnableButtons()
    {
        buttonEnd.Enabled = totalPages > 1;
        buttonRight.Enabled = totalPages > 1;
        buttonBegin.Enabled = currentPage > 0 && totalPages > 0;
        buttonLeft.Enabled = currentPage > 0 && totalPages > 0;
        textBoxPageInfo.Text = $"  {currentPage + 1}/{totalPages}  ";
    }

    private Stream LoadBitmapFromResources(string appDir)
    {
        string bitmapFile = Path.Combine(appDir, "ResourceFiles", "LoadingBitmap.bmp");
        if (!File.Exists(bitmapFile)) return null;

        return new FileStream(bitmapFile, FileMode.Open, FileAccess.Read);
    }

    private void ResolveImage(object sender, DoWorkEventArgs e)
    {
        BackgroundWorker bw = sender as BackgroundWorker;
        if (bw == null) return;

        ImageResolveData data = e.Argument as ImageResolveData;
        if (data == null) return;

        data.Image = Bitmap.FromStream(data.Document.SaveAsImage(data.CurrentPage));
        e.Result = data;
    }

    private void ResolveImageCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        ImageResolveData data = e.Result as ImageResolveData;
        if (data == null) return;

        if (e.Cancelled)
        {
            MessageBox.Show("Operation was canceled");
        }
        else if (e.Error != null)
        {
            MessageBox.Show(e.Error.Message);
        }
        else
        {
            images[data.CurrentPage] = data.Image;
            pictureBoxView.Image = data.Image;
            pictureBoxView.Invalidate();
            splitContainerView.Panel1.VerticalScroll.Value = 0;
        }
    }

    private void HelpViewer_Load(object sender, EventArgs e)
    {
        DrawPage();
        EnableButtons();
    }
}

public class ImageResolveData
{
    public int CurrentPage { get; internal set; }
    public Image Image { get; internal set; }
    public PdfDocument Document { get; internal set; }
}