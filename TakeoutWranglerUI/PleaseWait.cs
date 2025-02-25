//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using TakeoutWranglerUI;
using System.Drawing;
using System.Windows.Forms;

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
