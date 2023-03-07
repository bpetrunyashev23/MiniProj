using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for CombatScreen.xaml
    /// </summary>
    public partial class CombatScreen : Window
    {
        /*variables used for generating enemy health and keeping track of current health*/
        public int totalhealth;
        public int health;
        public CombatScreen()
        {
            /*generating enemy starting health & displaying it*/
            InitializeComponent();
            Random rnd = new Random();
            totalhealth = rnd.Next(80, 121);
            health = totalhealth;
            enemyHP.Text = "Enemy HP: " + Convert.ToString(totalhealth) + "/" + Convert.ToString(totalhealth);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-HD9RKJ8;Initial Catalog = MiniProj; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();

            /*pulling damage of character's weapon that is subtracted from the enemy's current health on each click*/
            string damageQuary = "select thing.Damage\r\nfrom (select RealWeapons.CharID, Weapons.Damage from RealWeapons\r\nleft join Weapons on Realweapons.WeaponID=Weapons.WeaponID) as thing\r\nwhere CharID=(select * from CurrChar)";
            SqlCommand weaponDamage = new SqlCommand(damageQuary, sqlCon);
            weaponDamage.CommandType = CommandType.Text;
            health -= Convert.ToInt32(weaponDamage.ExecuteScalar());

            if (health <= 0)
            {
                MessageBox.Show("You have defeated the enemy!");

                /*generating loot & higher tier drop chance*/
                int dropLvl = 1;

                Random rnd = new Random();
                int tier2chance = rnd.Next(1, 11);
                if (tier2chance == 10)
                {
                    dropLvl = 2;
                    int tier3chance = rnd.Next(1, 11);
                    if (tier3chance == 10)
                        dropLvl = 3;
                }


                /*pulling weapon lvl (WeaponID in RealWeapons table) to compare with lvl of weapon dropped by monster*/
                string weaponLvlCurrentQuery = "select WeaponID from RealWeapons where CharID=(select * from CurrChar)";
                SqlCommand weaponLvlCurrent = new SqlCommand(weaponLvlCurrentQuery, sqlCon);
                weaponLvlCurrent.CommandType = CommandType.Text;
                int currLevel = Convert.ToInt32(weaponLvlCurrent.ExecuteScalar());

                /*checking for upgrade & replacing weaker weapon in database*/
                if (dropLvl > currLevel)
                {
                    string updateWeaponQuery = "update RealWeapons\r\nset WeaponID = @weaponId\r\nwhere CharID = (select * from CurrChar)";
                    SqlCommand updateWeapon = new SqlCommand(updateWeaponQuery, sqlCon);
                    updateWeapon.CommandType = CommandType.Text;
                    updateWeapon.Parameters.AddWithValue("@weaponId", dropLvl);
                    updateWeapon.ExecuteNonQuery();

                    MessageBox.Show("You have received a better weapon!");
                }

                sqlCon.Close();

                CharacterOverview currCharScreen = new CharacterOverview();
                currCharScreen.Show();
                this.Close();

            }

            /*updates enemy HP on each click*/
            enemyHP.Text = "Enemy HP: " + Convert.ToString(health) + "/" + Convert.ToString(totalhealth);
        }
    }
}
