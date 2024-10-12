using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace QuizApplication
{
    public partial class SignupForm : Form
    {
        private CustomTextBox? txtUsername;
        private CustomTextBox? txtPassword;
        private CustomTextBox? txtConfirmPassword;
        private AnimatedButton? btnSignup;
        private Label? lblLogin;
        private Label? lblMessage;

        public SignupForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sign Up";
            this.Size = new Size(400, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(18, 18, 18); // Almost black background

            // Create form title
            Label lblTitle = new Label
            {
                Text = "JOIN US",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(300, 50),
                Location = new Point(50, 50),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Create input fields
            txtUsername = new CustomTextBox("USERNAME", 50, 130);
            txtPassword = new CustomTextBox("PASSWORD", 50, 210) { IsPassword = true };
            txtConfirmPassword = new CustomTextBox("CONFIRM PASSWORD", 50, 290) { IsPassword = true };

            // Create signup button
            btnSignup = new AnimatedButton
            {
                Text = "SIGN UP",
                Size = new Size(300, 50),
                Location = new Point(50, 370),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 185, 84), // Spotify green
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            // Create login link
            lblLogin = new Label
            {
                Text = "Already have an account? Log in",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(29, 185, 84), // Spotify green
                Size = new Size(300, 30),
                Location = new Point(50, 430),
                Cursor = Cursors.Hand
            };

            // Create message label
            lblMessage = new Label
            {
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(229, 9, 20), // Netflix red
                Size = new Size(300, 30),
                Location = new Point(50, 460)
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { lblTitle, txtUsername, txtPassword, txtConfirmPassword, btnSignup, lblLogin, lblMessage });

            // Add event handlers
            btnSignup.Click += BtnSignup_Click;
            lblLogin.Click += LblLogin_Click;

            // Add close button
            Label btnClose = new Label
            {
                Text = "×",
                Size = new Size(30, 30),
                Location = new Point(360, 10),
                ForeColor = Color.White,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            // Enable form drag
            this.MouseDown += SignupForm_MouseDown;
            this.MouseMove += SignupForm_MouseMove;
        }

        private void BtnSignup_Click(object? sender, EventArgs e)
        {
            if (txtUsername == null || txtPassword == null || txtConfirmPassword == null || lblMessage == null)
            {
                MessageBox.Show("An error occurred. Please try again.");
                return;
            }

            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                lblMessage.Text = "Please fill in all fields.";
                return;
            }

            if (password != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match.";
                return;
            }

            if (CreateUser(username, password))
            {
                MessageBox.Show("User created successfully. You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                new LoginForm().ShowDialog();
                this.Close();
            }
            else
            {
                lblMessage.Text = "Username already exists.";
            }
        }

        private void LblLogin_Click(object? sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().ShowDialog();
            this.Close();
        }

        private bool CreateUser(string username, string password)
        {
            string connectionString = "server=localhost;user=root;password=root;database=vp21;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO users (username, password) VALUES (@username, @password)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    command.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException ex) when (ex.Number == 1062)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        // Allow form to be moved by dragging
        private Point lastPoint;

        private void SignupForm_MouseDown(object? sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void SignupForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }
    }

    public class CustomTextBox : TextBox
    {
        private string _placeholder;

        public CustomTextBox(string placeholder, int left, int top)
        {
            _placeholder = placeholder;
            this.Text = placeholder;
            this.Font = new Font("Segoe UI", 12);
            this.ForeColor = Color.Gray;
            this.BackColor = Color.FromArgb(24, 24, 24);
            this.BorderStyle = BorderStyle.None;
            this.Size = new Size(300, 30);
            this.Location = new Point(left, top);

            this.Enter += CustomTextBox_Enter;
            this.Leave += CustomTextBox_Leave;
            this.TextChanged += CustomTextBox_TextChanged;

            this.Paint += CustomTextBox_Paint;
        }

        private void CustomTextBox_Paint(object? sender, PaintEventArgs e)
        {
            // Draw a line under the textbox
            e.Graphics.DrawLine(new Pen(Color.FromArgb(40, 40, 40), 2), new Point(0, this.Height - 1), new Point(this.Width, this.Height - 1));
        }

        private void CustomTextBox_Enter(object? sender, EventArgs e)
        {
            if (this.Text == _placeholder)
            {
                this.Text = "";
                this.ForeColor = Color.White;
            }
        }

        private void CustomTextBox_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = _placeholder;
                this.ForeColor = Color.Gray;
            }
        }

        private void CustomTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (this.Text != _placeholder && this.Text.Length > 0)
            {
                this.ForeColor = Color.White;
            }
        }

        public bool IsPassword
        {
            get { return this.UseSystemPasswordChar; }
            set
            {
                this.UseSystemPasswordChar = value;
                this.PasswordChar = value ? '●' : '\0';
            }
        }
    }

    public class AnimatedButton : Button
    {
        private Color _originalColor;
        private System.Windows.Forms.Timer _animationTimer;

        public AnimatedButton()
        {
            _originalColor = this.BackColor;
            _animationTimer = new System.Windows.Forms.Timer { Interval = 50 };
            _animationTimer.Tick += AnimationTimer_Tick;

            this.FlatAppearance.BorderSize = 0;
            this.FlatStyle = FlatStyle.Flat;

            this.MouseEnter += AnimatedButton_MouseEnter;
            this.MouseLeave += AnimatedButton_MouseLeave;
        }

        private void AnimatedButton_MouseEnter(object? sender, EventArgs e)
        {
            _animationTimer.Start();
        }

        private void AnimatedButton_MouseLeave(object? sender, EventArgs e)
        {
            _animationTimer.Stop();
            this.BackColor = _originalColor;
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(
                (int)(this.BackColor.R * 0.9),
                (int)(this.BackColor.G * 0.9),
                (int)(this.BackColor.B * 0.9)
            );

            if (this.BackColor.R <= 20 && this.BackColor.G <= 20 && this.BackColor.B <= 20)
            {
                _animationTimer.Stop();
            }
        }
    }
}