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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TakeoutWrangler;

public partial class SettingsForm : Form
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public string Filter { get; set; }
    public string Pattern { get; set; }
    public bool Quiet { get; set; }
    public string ActionText { get; set; }

    public SettingsForm()
    {
        InitializeComponent();
    }

    private void radioButtonCopy_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButtonCopy.Checked)
        {
            checkBoxQuiet.Enabled = true;
        }
    }

    private void radioButtonList_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButtonList.Checked)
        {
            checkBoxQuiet.Checked = false;
            checkBoxQuiet.Enabled = false;
        }
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
        textBoxSource.Text = Source;
        textBoxDestination.Text = Destination;
        textBoxFileFilter.Text = Filter;
        textBoxDestinationPattern.Text = Pattern;
        radioButtonCopy.Checked = ActionText.Equals(PhotoCopierActions.Copy.ToString(), StringComparison.OrdinalIgnoreCase);
        radioButtonList.Checked = ActionText.Equals(PhotoCopierActions.List.ToString(), StringComparison.OrdinalIgnoreCase);
        checkBoxQuiet.Checked = Quiet;
    }

    private void buttonOkay_Click(object sender, EventArgs e)
    {
        bool changed = false;
        
        changed |= Source != textBoxSource.Text;
        Source = textBoxSource.Text;

        changed |= Destination != textBoxDestination.Text;
        Destination = textBoxDestination.Text;

        changed |= Filter != textBoxFileFilter.Text;
        Filter = textBoxFileFilter.Text;

        changed |= Pattern != textBoxDestinationPattern.Text;
        Pattern = textBoxDestinationPattern.Text;

        changed |= Quiet != checkBoxQuiet.Checked;
        Quiet = checkBoxQuiet.Checked;

        string actionText = null;
        if (radioButtonCopy.Checked)
            actionText = PhotoCopierActions.Copy.ToString();
        else if (radioButtonList.Checked)
            actionText = PhotoCopierActions.List.ToString();

        changed |= ActionText != actionText;
        ActionText = actionText;

        if (changed)
        {
            DialogResult result = MessageBox.Show("Keep changed values?", "Settings have changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DialogResult = DialogResult.Yes;
            }
        }

        Close();
    }

    private void buttonSourceDialog_Click(object sender, EventArgs e)
    {
        string folder = SelectFolder(textBoxSource.Text);
        if (folder != null)
        {
            textBoxSource.Text = folder;
        }
    }

    private void buttonDestinationDialog_Click(object sender, EventArgs e)
    {
        string folder = SelectFolder(textBoxDestination.Text);
        if (folder != null)
        {
            textBoxDestination.Text = folder;
        }
    }

    private string SelectFolder(string text)
    {
        using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
        {
            if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
            {
                folderDialog.SelectedPath = text;
            }

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                return folderDialog.SelectedPath;
            }
        }

        return null;
    }
}
