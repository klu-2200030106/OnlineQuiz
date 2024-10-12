using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuizApplication
{
    public partial class Form1 : Form
    {
        private Quiz? currentQuiz;  // Marked nullable to fix CS8618
        private int currentQuestionIndex;
        private int score;
        private string username;
        // Declare controls, using null-forgiving operator to prevent CS8602 warnings
        private Label lblQuestion = null!;
        private RadioButton rbOption1 = null!;
        private RadioButton rbOption2 = null!;
        private RadioButton rbOption3 = null!;
        private RadioButton rbOption4 = null!;
        private Button btnSubmit = null!;
        private Button btnRestart = null!;

        public Form1(string username)
        {
            this.username = username;
            InitializeComponent();
            InitializeQuiz();
        }

        private void InitializeComponent()
        {
            // Form-level background color
            this.BackColor = Color.FromArgb(240, 240, 240);  // Light gray

            // Initialize and set up form controls with colors and styles
            lblQuestion = new Label()
            {
                Text = "Question",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),  // Blue
            };

            rbOption1 = new RadioButton()
            {
                Location = new System.Drawing.Point(20, 60),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(0, 51, 102),  // Darker Blue
            };
            rbOption2 = new RadioButton()
            {
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(0, 51, 102),  // Darker Blue
            };
            rbOption3 = new RadioButton()
            {
                Location = new System.Drawing.Point(20, 120),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(0, 51, 102),  // Darker Blue
            };
            rbOption4 = new RadioButton()
            {
                Location = new System.Drawing.Point(20, 150),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(0, 51, 102),  // Darker Blue
            };

            btnSubmit = new Button()
            {
                Text = "Submit",
                Location = new System.Drawing.Point(20, 200),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(34, 139, 34),  // Green
                FlatStyle = FlatStyle.Flat,
            };
            btnSubmit.FlatAppearance.BorderColor = Color.DarkGreen;

            btnRestart = new Button()
            {
                Text = "Restart",
                Location = new System.Drawing.Point(100, 200),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 20, 60),  // Crimson
                FlatStyle = FlatStyle.Flat,
                Visible = false,
            };
            btnRestart.FlatAppearance.BorderColor = Color.DarkRed;

            // Add event handlers
            btnSubmit.Click += btnSubmit_Click!;
            btnRestart.Click += btnRestart_Click!;

            // Add controls to the form
            Controls.Add(lblQuestion);
            Controls.Add(rbOption1);
            Controls.Add(rbOption2);
            Controls.Add(rbOption3);
            Controls.Add(rbOption4);
            Controls.Add(btnSubmit);
            Controls.Add(btnRestart);

            // Set form properties
            this.Text = "Quiz Application";
            this.ClientSize = new System.Drawing.Size(350, 300);  // Adjusted size
        }

        private void InitializeQuiz()
        {
            currentQuiz = DatabaseManager.RetrieveQuizFromDatabase();
            currentQuestionIndex = 0;
            score = 0;
            DisplayQuestion();
        }

        private void DisplayQuestion()
        {
            if (currentQuestionIndex < currentQuiz!.Questions.Count)
            {
                Question question = currentQuiz.Questions[currentQuestionIndex];
                lblQuestion.Text = $"Question {currentQuestionIndex + 1}: {question.Text}";

                rbOption1.Text = question.Options[0];
                rbOption2.Text = question.Options[1];
                rbOption3.Text = question.Options[2];
                rbOption4.Text = question.Options[3];

                rbOption1.Checked = false;
                rbOption2.Checked = false;
                rbOption3.Checked = false;
                rbOption4.Checked = false;
            }
            else
            {
                DisplayResult();
            }
        }

        private void btnSubmit_Click(object? sender, EventArgs e)
        {
            if (currentQuestionIndex < currentQuiz!.Questions.Count)
            {
                Question question = currentQuiz.Questions[currentQuestionIndex];
                int selectedAnswer = -1;

                if (rbOption1.Checked) selectedAnswer = 0;
                else if (rbOption2.Checked) selectedAnswer = 1;
                else if (rbOption3.Checked) selectedAnswer = 2;
                else if (rbOption4.Checked) selectedAnswer = 3;

                if (selectedAnswer == -1)
                {
                    MessageBox.Show("Please select an answer before submitting.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (selectedAnswer == question.CorrectAnswerIndex)
                {
                    score++;
                    MessageBox.Show("Correct!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Incorrect. The correct answer was: {question.Options[question.CorrectAnswerIndex]}", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                currentQuestionIndex++;
                DisplayQuestion();
            }
        }

        private void DisplayResult()
        {
            lblQuestion.Text = $"Quiz completed! Your score: {score}/{currentQuiz!.Questions.Count}";
            rbOption1.Visible = false;
            rbOption2.Visible = false;
            rbOption3.Visible = false;
            rbOption4.Visible = false;
            btnSubmit.Visible = false;
            btnRestart.Visible = true;

            // Save the result to the database using the stored username
            DatabaseManager.SaveQuizResult(username, score, currentQuiz.Questions.Count);
        }

        private void btnRestart_Click(object? sender, EventArgs e)
        {
            rbOption1.Visible = true;
            rbOption2.Visible = true;
            rbOption3.Visible = true;
            rbOption4.Visible = true;
            btnSubmit.Visible = true;
            btnRestart.Visible = false;
            InitializeQuiz();
        }
    }
}



public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }

        public Question(int id, string text, List<string> options, int correctAnswerIndex)
        {
            Id = id;
            Text = text;
            Options = options;
            CorrectAnswerIndex = correctAnswerIndex;
        }
    }

    public class Quiz
    {
        public List<Question> Questions { get; set; }

        public Quiz()
        {
            Questions = new List<Question>();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }
    }


// Program.cs
