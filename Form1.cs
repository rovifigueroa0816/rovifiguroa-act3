using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FigueroaAct_3
{
    public partial class Form1 : Form
    {
        private readonly string _sqlConn = "server=localhost;database=InventoryDB;uid=root;pwd=;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Apply security masking to password fields using a compact array action
            Array.ForEach(new[] { textBox2, textBox4 }, field => field.PasswordChar = '•');
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Package form inputs into an anonymous structure for clean evaluation
            var inputData = new
            {
                Name = textBox1.Text.Trim(),
                User = textBox3.Text.Trim(),
                Pass = textBox2.Text,
                Confirm = textBox4.Text
            };

            // Guard clause for empty fields
            if (string.IsNullOrEmpty(inputData.Name) || string.IsNullOrEmpty(inputData.User) ||
                string.IsNullOrEmpty(inputData.Pass) || string.IsNullOrEmpty(inputData.Confirm))
            {
                MessageBox.Show("Please fill out all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Guard clause for mismatched passwords
            if (inputData.Pass != inputData.Confirm)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Clear();
                textBox4.Clear();
                textBox2.Focus();
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_sqlConn))
                {
                    connection.Open();

                    // Check if the username already exists in the database
                    string checkQuery = "SELECT COUNT(1) FROM Users WHERE Username = @uName";
                    using (MySqlCommand verificationCmd = new MySqlCommand(checkQuery, connection))
                    {
                        verificationCmd.Parameters.AddWithValue("@uName", inputData.User);
                        int matchesFound = Convert.ToInt32(verificationCmd.ExecuteScalar());

                        if (matchesFound > 0)
                        {
                            MessageBox.Show("Username is already taken. Please choose another one.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insert the new user record
                    string insertQuery = "INSERT INTO Users (FullName, Username, Password) VALUES (@fName, @uName, @pWord)";
                    using (MySqlCommand creationCmd = new MySqlCommand(insertQuery, connection))
                    {
                        creationCmd.Parameters.AddWithValue("@fName", inputData.Name);
                        creationCmd.Parameters.AddWithValue("@uName", inputData.User);
                        creationCmd.Parameters.AddWithValue("@pWord", inputData.Pass);

                        creationCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Account successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearAllInputs();
                }
            }
            catch (MySqlException sqlEx)
            {
                MessageBox.Show("Database Error: " + sqlEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception generalEx)
            {
                MessageBox.Show("Unexpected Error: " + generalEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearAllInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox1.Focus();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Form2().Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
    }
}
