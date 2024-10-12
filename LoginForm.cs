using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace QuizApplication
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnGoToSignup;
        private Label lblMessage;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Login";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(18, 18, 18); // Dark background (similar to signup)

            // Create a panel to hold the controls
            Panel panel = new Panel
            {
                Size = new Size(300, 200),
                Location = new Point(50, 50),
                BackColor = Color.FromArgb(24, 24, 24), // Darker panel background
                BorderStyle = BorderStyle.None
            };

            // Username and password textboxes
            txtUsername = new TextBox
            {
                Left = 100,
                Top = 30,
                Width = 180,
                Font = new Font("Arial", 12),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPassword = new TextBox
            {
                Left = 100,
                Top = 70,
                Width = 180,
                PasswordChar = '•',
                Font = new Font("Arial", 12),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Login button
            btnLogin = new Button
            {
                Text = "Login",
                Left = 100,
                Top = 110,
                Width = 180,
                Height = 35,
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(29, 185, 84), // Spotify green (similar to signup)
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };

            // Go to signup button
            btnGoToSignup = new Button
            {
                Text = "New user? Sign up",
                Left = 100,
                Top = 155,
                Width = 180,
                Font = new Font("Arial", 10),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(29, 185, 84), // Same green color for consistency
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };

            // Error message label
            lblMessage = new Label
            {
                Left = 100,
                Top = 200,
                Width = 180,
                ForeColor = Color.Red,
                Font = new Font("Arial", 10)
            };

            // Labels for Username and Password
            Label lblUsername = new Label
            {
                Text = "Username:",
                Left = 10,
                Top = 33,
                Font = new Font("Arial", 12),
                ForeColor = Color.White
            };
            Label lblPassword = new Label
            {
                Text = "Password:",
                Left = 10,
                Top = 73,
                Font = new Font("Arial", 12),
                ForeColor = Color.White
            };

            // Add event handlers
            btnLogin.Click += BtnLogin_Click;
            btnGoToSignup.Click += BtnGoToSignup_Click;

            // Add controls to panel and form
            panel.Controls.AddRange(new Control[] { lblUsername, lblPassword, txtUsername, txtPassword, btnLogin, btnGoToSignup, lblMessage });
            this.Controls.Add(panel);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Please fill in all fields.";
                return;
            }

            if (AuthenticateUser(username, password))
            {
                this.Hide();
                new Form1(username).ShowDialog();
                this.Close();
            }
            else
            {
                txtPassword.Clear();  // Clear the password field after failed attempt
            }
        }

        private void BtnGoToSignup_Click(object sender, EventArgs e)
        {
            this.Hide();
            new SignupForm().ShowDialog();
            this.Close();
        }

        private bool AuthenticateUser(string username, string password)
        {
            string connectionString = "server=localhost;user=root;password=root;database=vp21;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the username exists
                    string checkUsernameQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                    MySqlCommand checkUsernameCommand = new MySqlCommand(checkUsernameQuery, connection);
                    checkUsernameCommand.Parameters.AddWithValue("@username", username);

                    int userCount = Convert.ToInt32(checkUsernameCommand.ExecuteScalar());

                    if (userCount == 0)
                    {
                        lblMessage.Text = "No account exists with this username.";
                        return false; // Exit if no such username exists
                    }

                    // Check if the username and password match
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        // Valid credentials
                        return true;
                    }
                    else
                    {
                        // Incorrect password
                        lblMessage.Text = "Incorrect password. Please try again.";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }
}
