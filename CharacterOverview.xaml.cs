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
    /// Interaction logic for CharacterOverview.xaml
    /// </summary>
    public partial class CharacterOverview : Window
    {
        public CharacterOverview()
        {
            InitializeComponent();

            SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            if (sqlCon.State==ConnectionState.Closed)
                sqlCon.Open();

            /*queries to join character name with the name of their weapon and its damage*/
            string weaponQuery = "select thing.WeaponName\r\nfrom (select RealWeapons.CharID, Weapons.WeaponName from RealWeapons\r\nleft join Weapons on Realweapons.WeaponID=Weapons.WeaponID) as thing\r\nwhere CharID=(select * from CurrChar)";
            SqlCommand weaponName = new SqlCommand(weaponQuery,sqlCon);
            weaponName.CommandType = CommandType.Text;

            string damageQuary = "select thing.Damage\r\nfrom (select RealWeapons.CharID, Weapons.Damage from RealWeapons\r\nleft join Weapons on Realweapons.WeaponID=Weapons.WeaponID) as thing\r\nwhere CharID=(select * from CurrChar)";
            SqlCommand weaponDamage = new SqlCommand(damageQuary, sqlCon);
            weaponDamage.CommandType = CommandType.Text;

            /*Displays weapon info*/
            weaponInfo.Text = " " + Convert.ToString(weaponName.ExecuteScalar()) + ": " + Convert.ToString(weaponDamage.ExecuteScalar())+" damage";

            /*Displays character name on top of window*/
            string charNameQuery = "select charname from Characters where CharID = (select * from CurrChar)";
            SqlCommand findCharName = new SqlCommand(charNameQuery,sqlCon);
            findCharName.CommandType = CommandType.Text;
            string thing = Convert.ToString(findCharName.ExecuteScalar());
            charName.Text = char.ToUpper(thing[0])+thing.Substring(1);

            sqlCon.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CombatScreen battle = new CombatScreen();
            battle.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CharacterScreen chars = new CharacterScreen();
            chars.Show();
            this.Close();
        }
    }
}
