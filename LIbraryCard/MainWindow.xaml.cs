using LibraryCard.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string connectionString = @"Data Source=library.sqlite;Version=3;";
        readonly SQLiteConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            txt_CardID.Focus();
            btn_Register.Visibility = Visibility.Hidden;

            InitConnection();
        }

        private void InitConnection()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Check if the Users table exists
                string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
                using (SQLiteCommand checkTableCommand = new SQLiteCommand(checkTableQuery, connection))
                {
                    using (SQLiteDataReader reader = checkTableCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            // The Users table does not exist, so create it
                            string createTableQuery = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Name TEXT, Phone TEXT)";
                            using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                            {
                                createTableCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                connection.Close();
            }
        }


        private async void GetUser(string code)
        {
            txb_Message.Foreground = Brushes.Black;
            txb_Message.Text = "Searching...";

            // Delay for 1 second (1000 milliseconds)
            await Task.Delay(1000);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string queryString = "SELECT * FROM Users WHERE Id = @code";

                using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@code", code);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            // Access data using reader, e.g., reader["ColumnName"]
                            int id = Convert.ToInt32(reader["Id"]);
                            string name = reader["Name"].ToString();
                            string phone = reader["Phone"].ToString();

                            // Process data or store it as needed
                            txt_Name.Text = name;
                            txt_Phone.Text = phone;

                            txb_Message.Foreground = Brushes.Black;
                            txb_Message.Text = "User found!";
                            btn_Reset.Focus();
                        }
                        else
                        {
                            txb_Message.Foreground = Brushes.Red;
                            txb_Message.Text = "User not found!";
                            txt_Name.Focus();
                            btn_Register.Visibility = Visibility.Visible;
                        }
                    }
                }

                connection.Close();
            }
        }

        private void Txt_CardID_TextChanged(object sender, TextChangedEventArgs e)
        {
            var code = txt_CardID.Text;

            if(code != "") GetUser(code);
        }

        private void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            btn_Register.Visibility = Visibility.Hidden;
            txb_Message.Text = string.Empty;
            txt_Name.Clear();
            txt_Phone.Clear();
            txt_CardID.Clear();
            txt_CardID.Focus();
        }

        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if(txt_Name.Text.Length > 0 && txt_Phone.Text.Length > 0)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Users (Id, Name, Phone) VALUES (@Id, @Name, @Phone)";

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        // Replace @Name and @Phone with the actual values you want to insert
                        command.Parameters.AddWithValue("@Id", txt_CardID.Text);
                        command.Parameters.AddWithValue("@Name", txt_Name.Text);
                        command.Parameters.AddWithValue("@Phone", txt_Phone.Text);

                        // Execute the INSERT query
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
        }
    }
}
