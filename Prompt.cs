using System;
using System.Windows.Forms;

public static class Prompt
{
    public static string ShowDialog(string text, string caption)
    {
        Form prompt = new Form()
        {
            Width = 300,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = caption,
            StartPosition = FormStartPosition.CenterScreen,
            MaximizeBox = false,
            MinimizeBox = false
        };

        Label textLabel = new Label() { Left = 20, Top = 20, Text = text, AutoSize = true };
        TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 240 };
        Button confirmation = new Button() { Text = "Ok", Left = 180, Width = 80, Top = 80, DialogResult = DialogResult.OK };
        Button cancel = new Button() { Text = "Cancel", Left = 90, Width = 80, Top = 80, DialogResult = DialogResult.Cancel };

        // Set default button action
        prompt.AcceptButton = confirmation;
        prompt.CancelButton = cancel;

        prompt.Controls.Add(textLabel);
        prompt.Controls.Add(inputBox);
        prompt.Controls.Add(confirmation);
        prompt.Controls.Add(cancel);
        prompt.FormClosing += (sender, e) =>
        {
            // Ensure non-empty input
            if (prompt.DialogResult == DialogResult.OK && string.IsNullOrWhiteSpace(inputBox.Text))
            {
                MessageBox.Show("Name cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        };

        return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : string.Empty;
    }
}
