using System;
using System.Collections.Generic;
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
using MiniProj;
using Microsoft.Data.SqlClient;
using System.Data;


namespace MiniProj
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (txtPassword.Password == txtPassword_Check.Password)
            {
                SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();

                /*checks if username is already taken*/
                string uniqunessCheck = "SELECT COUNT(1) FROM Login_Details Where username=@Username";
                SqlCommand sqlCmdUnique = new SqlCommand(uniqunessCheck, sqlCon);
                sqlCmdUnique.CommandType = CommandType.Text;
                sqlCmdUnique.Parameters.AddWithValue("@Username", txtUsername.Text);
                int count = Convert.ToInt32(sqlCmdUnique.ExecuteScalar());

                if (count != 0)
                    MessageBox.Show("Username already taken!");
                else
                {
                    try
                    {
                        /*creates new account in database*/
                        string query1 = "Select count(*) from Login_Details";
                        SqlCommand sqlCmd1 = new SqlCommand(query1, sqlCon);
                        sqlCmd1.CommandType = CommandType.Text;
                        int id = Convert.ToInt32(sqlCmd1.ExecuteScalar()) + 1;


                        string query = "insert into Login_Details values (@id,@Username,@Password_)";
                        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                        sqlCmd.CommandType = CommandType.Text;
                        sqlCmd.Parameters.AddWithValue("@id", id);
                        sqlCmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                        sqlCmd.Parameters.AddWithValue("@Password_", txtPassword.Password);
                        sqlCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        sqlCon.Close();
                    }

                    MainWindow Login = new MainWindow();
                    Login.Show();
                    this.Close();
                }                

            }
            else
            {
                MessageBox.Show("Password does not match!");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow LoginWin = new MainWindow();
            LoginWin.Show();
            this.Close();
        }
    }
}
