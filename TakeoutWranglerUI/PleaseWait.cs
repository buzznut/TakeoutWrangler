using TakeoutWrangler;
using System.Drawing;

namespace TakeoutWranglerUI;

public partial class PleaseWait : Form
{
    private readonly Form owner;

    public PleaseWait(string body, Form owner)
    {
        SuspendLayout();
        InitializeComponent();
        textBoxBody.Text = body;
        this.owner = owner;
        ResumeLayout(true);
    }

    private void PleaseWait_Activated(object sender, EventArgs e)
    {
        if (owner == null) return;

        Rectangle frmRect = new Rectangle(owner.Location, owner.Size);
        Rectangle dlgRect = new Rectangle(Location, Size);

        int x = frmRect.Left + (frmRect.Width - dlgRect.Right + dlgRect.Left) / 2;
        int y = frmRect.Top + (frmRect.Height - dlgRect.Bottom + dlgRect.Top) / 2;

        Location = new Point(x, y);
        TopMost = true;
    }

    private void PleaseWait_Load(object sender, EventArgs e)
    {
        PleaseWait_Activated(sender, e);
    }

    private void PleaseWait_Shown(object sender, EventArgs e)
    {
        PleaseWait_Activated(sender, e);
    }
}
