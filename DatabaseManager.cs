using System;
using MySql.Data.MySqlClient;

namespace QuizApplication
{
    public static class DatabaseManager
    {
        private static string connectionString = "server=localhost;user=root;password=root;database=vp21;";

        public static Quiz RetrieveQuizFromDatabase()
        {
            Quiz quiz = new Quiz();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM questions";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string text = reader.GetString("text");
                            string optionsString = reader.GetString("options");
                            int correctAnswerIndex = reader.GetInt32("correct_answer_index");

                            List<string> options = new List<string>(optionsString.Split('|'));

                            quiz.AddQuestion(new Question(id, text, options, correctAnswerIndex));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return quiz;
        }

        public static void SaveQuizResult(string userName, int score, int totalQuestions)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO quiz_results (user_name, score, total_questions, completion_time) " +
                                   "VALUES (@userName, @score, @totalQuestions, @completionTime)";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userName", userName);
                    command.Parameters.AddWithValue("@score", score);
                    command.Parameters.AddWithValue("@totalQuestions", totalQuestions);
                    command.Parameters.AddWithValue("@completionTime", DateTime.Now);

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the quiz result: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
