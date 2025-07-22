

//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace DecryptDisplay;

public partial class GetPassword : Form
{
    public string Password
    {
        get
        {
            if (textBoxPassword.Text.Length > 8)
            {
                return textBoxPassword.Text;
            }

            return string.Empty;
        }
    }

    public GetPassword()
    {
        InitializeComponent();
        ValidatePassword();
    }

    private void buttonOkay_Click(object sender, EventArgs e)
    {
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void textBox_TextChanged(object sender, EventArgs e)
    {
        ValidatePassword();
    }

    private void ValidatePassword()
    {
        if (textBoxPassword.Text.Length >= 8)
        {
            buttonOkay.Enabled = true;
        }
        else
        {
            buttonOkay.Enabled = false;
        }
    }

    private void buttonShowPassword_MouseDown(object sender, MouseEventArgs e)
    {
        ButtonShow(textBoxPassword, true);
    }

    private void buttonShowPassword_MouseUp(object sender, MouseEventArgs e)
    {
        ButtonShow(textBoxPassword, false);
    }

    private void ButtonShow(TextBox textBox, bool showPassword)
    {
        if (showPassword && textBox.UseSystemPasswordChar   )
        {
            textBox.UseSystemPasswordChar = false;
        }
        else
        {
            textBox.UseSystemPasswordChar = true;
        }
    }
}
