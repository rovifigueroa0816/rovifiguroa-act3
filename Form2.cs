using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FigueroaAct_3
{
    public partial class Form2 : Form
    {
        // Database connection string for the inventory system
        private readonly string dbPath = "server=localhost;database=inventorydb;uid=root;pwd=;";

        public Form2()
        {
            InitializeComponent();
        }

        // Triggered when the login form is loaded
        private void Form2_Load(object sender, EventArgs e)
        {
            // Mask password characters for privacy and security
            textBoxPassword.PasswordChar = '•';
        }

        // Login button click event handler
        private void button1_Click(object sender, EventArgs e)
        {
            string enteredUser = textBoxUsername.Text.Trim();
            string enteredPass = textBoxPassword.Text;

            // Validate input fields are not empty
            if (HasEmptyFields(enteredUser, enteredPass))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Check if user credentials exist in the database
                if (VerifyUserInDatabase(enteredUser, enteredPass))
                {
                    MessageBox.Show("Login successful! Welcome to the system.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Clear inputs after successful login
                    textBoxUsername.Clear();
                    textBoxPassword.Clear();

                    // Open the main dashboard (Form3) and hide the login window
                    Form3 inventoryDashboard = new Form3();
                    inventoryDashboard.Show();
                    this.Hide();
                }
                else
                {
                    // Handle invalid login credentials
                    MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxPassword.Clear();
                    textBoxPassword.Focus();
                }
            }
            catch (Exception runtimeError)
            {
                // Handle database connection or query errors
                MessageBox.Show("Database Error: " + runtimeError.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to check for empty input fields
        private bool HasEmptyFields(string user, string pass)
        {
            return string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass);
        }

        // Helper method to query the database and verify user credentials securely
        private bool VerifyUserInDatabase(string user, string pass)
        {
            using (MySqlConnection activeConn = new MySqlConnection(dbPath))
            {
                activeConn.Open();
                string lookupQuery = "SELECT COUNT(1) FROM Users WHERE Username = @inputUser AND Password = @inputPass";
                
                using (MySqlCommand queryCommand = new MySqlCommand(lookupQuery, activeConn))
                {
                    // Bind parameters to prevent SQL injection
                    queryCommand.Parameters.AddWithValue("@inputUser", user);
                    queryCommand.Parameters.AddWithValue("@inputPass", pass);

                    int matchCount = Convert.ToInt32(queryCommand.ExecuteScalar());
                    return matchCount > 0; // Returns true if a matching user record is found
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        // Link to navigate back to the registration screen (Form1)
        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 signUpScreen = new Form1();
            signUpScreen.Show();
            this.Hide();
        }
    }
}
