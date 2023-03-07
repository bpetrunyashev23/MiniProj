using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
using System.Windows.Shapes;

namespace MiniProj
{
    /// <summary>
    /// Interaction logic for CharacterScreen.xaml
    /// </summary>
    public partial class CharacterScreen : Window
    {
        public CharacterScreen()
        {
            InitializeComponent();
        }

        private void Button_Click_Create(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();

            /*counts existing characters with the name of the character the user wants to create*/
            string checkUniqueQuery = "select count (*) from Characters where charname=@name";
            SqlCommand checkUnique = new SqlCommand(checkUniqueQuery, sqlCon);
            checkUnique.CommandType = CommandType.Text;
            checkUnique.Parameters.AddWithValue("@name", txtCharname.Text);
            int count = Convert.ToInt32(checkUnique.ExecuteScalar());

            /*creates the new character if no other character has that name*/
            if (count == 0)
            {
                /*generate the character id of the new character*/
                string query1 = "Select count(*) from Characters";
                SqlCommand sqlCmd1 = new SqlCommand(query1, sqlCon);
                sqlCmd1.CommandType = CommandType.Text;
                int charID = Convert.ToInt32(sqlCmd1.ExecuteScalar()) + 1;

                /*looks at which account is using the program to bind the created character to their id*/
                string currUserQuery = "select * from CurrentUser";
                SqlCommand sqlCmd = new SqlCommand(currUserQuery, sqlCon);
                sqlCmd.CommandType = CommandType.Text;
                int id = Convert.ToInt32(sqlCmd.ExecuteScalar());

                string creationQuery = "insert into Characters values (@charId, @id, @Charname)";
                SqlCommand createComm = new SqlCommand(creationQuery, sqlCon);
                createComm.CommandType = CommandType.Text;
                createComm.Parameters.AddWithValue("@charId", charID);
                createComm.Parameters.AddWithValue("@id", id);
                createComm.Parameters.AddWithValue("@Charname", txtCharname.Text);
                createComm.ExecuteNonQuery();

                MessageBox.Show("User character belongs to: " + Convert.ToString(id));
                MessageBox.Show("CharacterID: " + Convert.ToString(charID));

                /*generates the id of the character's weapon instance (all existing weapons for all characters are kept in the separate table RealWeapons whereas the 3 weapon types exist in the Weapons table)*/
                string RealWeaponIdQuery = "select count(*) from RealWeapons";
                SqlCommand getRealWId = new SqlCommand(RealWeaponIdQuery, sqlCon);
                getRealWId.CommandType = CommandType.Text;
                int RealWId = Convert.ToInt32(getRealWId.ExecuteScalar()) + 1;

                string AddWeaponQuery = "insert into RealWeapons values (@RealId,1,@charID)";
                SqlCommand AddWeapon = new SqlCommand(AddWeaponQuery, sqlCon);
                AddWeapon.CommandType = CommandType.Text;
                AddWeapon.Parameters.AddWithValue("@RealId", RealWId);
                AddWeapon.Parameters.AddWithValue("@charID", charID);
                AddWeapon.ExecuteNonQuery();
            }
            else
                MessageBox.Show("Character with this name already exists!");


            /*-------------------*/
            /*makes the data grid show character name together with their weapon type and its damage*/
            string currUserProfQuery = "select CharId, Charname into CurrentCharProfile\r\nfrom Characters\r\nwhere ID=(select * from CurrentUser)";
            SqlCommand popCurrProf = new SqlCommand(currUserProfQuery, sqlCon);
            popCurrProf.CommandType = CommandType.Text;
            popCurrProf.ExecuteNonQuery();


            string query = "select CurrentCharProfile.Charname, Weapons.WeaponName, Weapons.Damage\r\nfrom CurrentCharProfile\r\nleft join Weapons on (select WeaponID from RealWeapons where RealWeapons.CharID=CurrentCharProfile.CharID)=Weapons.WeaponID";
            SqlCommand cmd = new SqlCommand(query, sqlCon);
            cmd.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            DataGrid_.ItemsSource = dt.DefaultView;
            adapter.Update(dt);

            string dropCurrProfQuery = "drop table CurrentCharProfile";
            SqlCommand dropCurrProf = new SqlCommand(dropCurrProfQuery, sqlCon);
            dropCurrProf.CommandType = CommandType.Text;
            dropCurrProf.ExecuteNonQuery();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            try
            {
                sqlCon.Open();

                /*makes the data grid show character name together with their weapon type and its damage*/
                string currUserQuery = "select id from CurrentUser";
                SqlCommand sqlCmd = new SqlCommand(currUserQuery, sqlCon);
                sqlCmd.CommandType = CommandType.Text;
                int id = Convert.ToInt32(sqlCmd.ExecuteScalar());

                string currUserProfQuery = "select CharId, Charname into CurrentCharProfile\r\nfrom Characters\r\nwhere ID=@id";
                SqlCommand popCurrProf = new SqlCommand(currUserProfQuery, sqlCon);
                popCurrProf.CommandType = CommandType.Text;
                popCurrProf.Parameters.AddWithValue("@id", id);
                popCurrProf.ExecuteNonQuery();


                string query = "select CurrentCharProfile.Charname, Weapons.WeaponName, Weapons.Damage\r\nfrom CurrentCharProfile\r\nleft join Weapons on (select WeaponID from RealWeapons where RealWeapons.CharID=CurrentCharProfile.CharID)=Weapons.WeaponID";
                SqlCommand cmd = new SqlCommand(query, sqlCon);
                cmd.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DataGrid_.ItemsSource = dt.DefaultView;
                adapter.Update(dt);

                string dropCurrProfQuery = "drop table CurrentCharProfile";
                SqlCommand dropCurrProf = new SqlCommand(dropCurrProfQuery, sqlCon);
                dropCurrProf.CommandType = CommandType.Text;
                dropCurrProf.ExecuteNonQuery();

                MessageBox.Show("Successful loading");
                sqlCon.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_Launch(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();

            /*deletes the record of the last played character so that it may be replaced with the one the user is trying to launch*/
            string emptyCurrCharQuery = "delete from CurrChar";
            SqlCommand emptyCurrChar = new SqlCommand(emptyCurrCharQuery, sqlCon);
            emptyCurrChar.CommandType = CommandType.Text;
            emptyCurrChar.ExecuteNonQuery();

            /*checks if character with that name exists and belongs to the user*/
            string currUserQuery = "select * from CurrentUser";
            SqlCommand pullCurrUser = new SqlCommand(currUserQuery, sqlCon);
            pullCurrUser.CommandType = CommandType.Text;
            int currUserID = Convert.ToInt32(pullCurrUser.ExecuteScalar());

            string testOwnershipQuery = "select count(*) from Characters where Charname=@Charname and ID=@id";
            SqlCommand testOwnership = new SqlCommand(testOwnershipQuery, sqlCon);
            testOwnership.CommandType = CommandType.Text;
            testOwnership.Parameters.AddWithValue("@Charname", txtLaunch.Text);
            testOwnership.Parameters.AddWithValue("@id", currUserID);
            int OwnershipValue = Convert.ToInt32(testOwnership.ExecuteScalar());


            /*changes the current character to the one the user is trying to launch*/
            if (OwnershipValue == 1)
            {
                string findCharIDQuery = "select CharID from Characters where Charname=@Charname";
                SqlCommand findCharID = new SqlCommand(findCharIDQuery, sqlCon);
                findCharID.CommandType = CommandType.Text;
                findCharID.Parameters.AddWithValue("@Charname", txtLaunch.Text);
                int CharID = Convert.ToInt32(findCharID.ExecuteScalar());

                string popCurrCharQuery = "insert into CurrChar values(@CharID)";
                SqlCommand popCurrChar = new SqlCommand(popCurrCharQuery, sqlCon);
                popCurrChar.CommandType = CommandType.Text;
                popCurrChar.Parameters.AddWithValue("@CharID", CharID);
                popCurrChar.ExecuteNonQuery();

                CharacterOverview currCharScreen = new CharacterOverview();
                currCharScreen.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Enter a valid character name or delete repeating characters!");
            }
        }

        private void Delete_Char(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();

            /*deletes the character's weapon instance and then the character themself*/
            string findCharIDQuery = "select CharID from Characters where Charname=@Charname";
            SqlCommand findCharID = new SqlCommand(findCharIDQuery, sqlCon);
            findCharID.CommandType = CommandType.Text;
            findCharID.Parameters.AddWithValue("@Charname", txtLaunch.Text);
            int CharID = Convert.ToInt32(findCharID.ExecuteScalar());

            string deleteWeaponQuery = "delete from RealWeapons where CharID=@CharID";
            SqlCommand deleteWeapon = new SqlCommand(deleteWeaponQuery, sqlCon);
            deleteWeapon.CommandType = CommandType.Text;
            deleteWeapon.Parameters.AddWithValue("@CharID", CharID);
            deleteWeapon.ExecuteNonQuery();

            string deleteCharQuery = "delete from Characters where charname=@name";
            SqlCommand deleteChar = new SqlCommand(deleteCharQuery,sqlCon);
            deleteChar.CommandType = CommandType.Text;
            deleteChar.Parameters.AddWithValue("@name", txtLaunch.Text);
            deleteChar.ExecuteNonQuery();

            /*-----------*/
            /*makes the data grid show character name together with their weapon type and its damage*/
            string currUserQuery = "select id from CurrentUser";
            SqlCommand sqlCmd = new SqlCommand(currUserQuery, sqlCon);
            sqlCmd.CommandType = CommandType.Text;
            int id = Convert.ToInt32(sqlCmd.ExecuteScalar());

            string currUserProfQuery = "select CharId, Charname into CurrentCharProfile\r\nfrom Characters\r\nwhere ID=@id";
            SqlCommand popCurrProf = new SqlCommand(currUserProfQuery, sqlCon);
            popCurrProf.CommandType = CommandType.Text;
            popCurrProf.Parameters.AddWithValue("@id", id);
            popCurrProf.ExecuteNonQuery();


            string query = "select CurrentCharProfile.Charname, Weapons.WeaponName, Weapons.Damage\r\nfrom CurrentCharProfile\r\nleft join Weapons on (select WeaponID from RealWeapons where RealWeapons.CharID=CurrentCharProfile.CharID)=Weapons.WeaponID";
            SqlCommand cmd = new SqlCommand(query, sqlCon);
            cmd.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            DataGrid_.ItemsSource = dt.DefaultView;
            adapter.Update(dt);

            string dropCurrProfQuery = "drop table CurrentCharProfile";
            SqlCommand dropCurrProf = new SqlCommand(dropCurrProfQuery, sqlCon);
            dropCurrProf.CommandType = CommandType.Text;
            dropCurrProf.ExecuteNonQuery();

            sqlCon.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow LoginWin = new MainWindow();
            LoginWin.Show();
            this.Close();
        }
    }
}
