using Spire.Pdf;
using System.ComponentModel;

namespace TakeoutWranglerUI;

public partial class HelpViewer : Form
{
    private int currentPage;
    private int totalPages;
    private Stream[] pages;
    private readonly PdfDocument doc = new PdfDocument();
    private BackgroundWorker worker = new BackgroundWorker();
    private readonly List<IDisposable> disposables = new List<IDisposable>();

    public HelpViewer()
    {
        InitializeComponent();
        disposables.Add(worker);
        disposables.Add(doc);
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

    internal void Initialize(string title, Stream[] pages)
    {
        // all the pages are already loaded
        SuspendLayout();

        Text = title;
        this.pages = pages;
        totalPages = pages.Length;
        currentPage = 0;
        DrawPage();

        ResumeLayout(true);
    }

    internal void Initialize(string appDir, string title, string pdfName)
    {
        // must load pages from PDF in the background
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
            pages = new Stream[totalPages];

            currentPage = 0;

            if (waitStream != null)
            {
                pictureBoxView.Image = Image.FromStream(waitStream);
                disposables.Add(waitStream);
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
        if (pages[currentPage] != null)
        {
            pictureBoxView.Image?.Dispose();
            pictureBoxView.Image = Bitmap.FromStream(pages[currentPage]);
            disposables.Add(pictureBoxView.Image);

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

        FileStream tmpStream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
        doc.SaveToImageStream(data.CurrentPage, tmpStream, "bitmap");
        data.Image = tmpStream;

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
            pages[data.CurrentPage] = data.Image;

            pictureBoxView.Image?.Dispose();
            pictureBoxView.Image = Bitmap.FromStream(pages[currentPage]);

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
    public Stream Image { get; internal set; }
    public PdfDocument Document { get; internal set; }
}