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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();

                /*check if username/password combination is in the database*/
                string query = "SELECT COUNT(1) FROM Login_Details Where username=@Username and password_=@Password";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                sqlCmd.Parameters.AddWithValue("@Password", txtPassword.Password);

                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count == 1)
                {
                    /*updates the record of the current user of the program*/

                    string emptyCurrQuery = "delete from CurrentUser";
                    SqlCommand emptyCurr = new SqlCommand(emptyCurrQuery, sqlCon);
                    emptyCurr.CommandType = CommandType.Text;
                    emptyCurr.ExecuteNonQuery();

                    string query1 = "select id from Login_Details where username=@Username";
                    SqlCommand sqlCmd1 = new SqlCommand(query1, sqlCon);
                    sqlCmd1.CommandType = CommandType.Text;
                    sqlCmd1.Parameters.AddWithValue("@Username", txtUsername.Text);
                    int id = Convert.ToInt32(sqlCmd1.ExecuteScalar());

                    string insertCurr = "insert into CurrentUser values (@id)";
                    SqlCommand currCom = new SqlCommand(insertCurr, sqlCon);
                    currCom.CommandType = CommandType.Text;
                    currCom.Parameters.AddWithValue("@id", id);
                    currCom.ExecuteNonQuery();

                    CharacterScreen chars = new CharacterScreen();
                    chars.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username or password are not correct!");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }

        }

        private void Button_CLick_2(object sender, RoutedEventArgs e)
        {
            SignUp signup = new SignUp();
            signup.Show();
            this.Close();
        }
    }
}
