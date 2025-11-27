using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace PingPong
{
    public partial class MainWindow : Window
    {
        public List<Player> players = new List<Player>();
        public MainWindow()
        {
            InitializeComponent();
            ReadJSON("Datas.json");

            ListPlayers.Click += DisplayPlayers;
            AddPlayers.Click += DisplayAddPlayer;
            btnSubmit.Click += AddPlayer;
            
        }

        public void DisplayPlayers(object sender, RoutedEventArgs e)
        {
            gridAdd.Visibility = Visibility.Hidden;
            gridList.Visibility = Visibility.Visible;
            lbList.Items.Clear();

            players.Sort((a, b) => a.Rank.CompareTo(b.Rank));
            DisplayTitle.Content = "Játékosok sorrendje";
            
            foreach (var p in players)
            {
                lbList.Items.Add(p.Rank + ": " + p.Name);
            }
        }

        void DisplayAddPlayer(object sender, RoutedEventArgs e)
        {
            gridAdd.Visibility = Visibility.Visible;
            gridList.Visibility = Visibility.Hidden;

            DisplayTitle.Content = "Játékos felvétele";
        }

        public void AddPlayer(object sender, RoutedEventArgs e)
        {

            string errorMessage = "";

            if (tbName.Text.Length == 0) 
            {
                    errorMessage += "Adjon meg nevet! ";
            }
            if (int.Parse(tbSkill.Text) > 100 || int.Parse(tbSkill.Text) < 0)
            {
                errorMessage += "Érvénytelen értékelés!";
            }
            if(errorMessage == "")
            {
                Player player = new Player();
                player.Name = tbName.Text;
                player.Rank = int.Parse(tbRank.Text);
                player.SkillPoints = int.Parse(tbSkill.Text);
                player.Description = tbDesc.Text;

                bool changeNeeded = false;

                foreach(var p in players)
                {
                    if(p.Rank == player.Rank)
                    {
                        changeNeeded = true;
                        break;
                    }
                }

                if (changeNeeded)
                {
                    foreach (var p in players)
                    {
                        if(p.Rank >= player.Rank)
                        {
                            p.Rank++;
                        }
                    }
                }

                players.Add(player);

                //lbErrorMessage.Content = "Sikeres játékosfelvétel!";
                tbName.Text = "";
                tbRank.Text = "";
                tbSkill.Text = "";
                tbDesc.Text = "";
            }
            else
            {
                //lblErrorMessage.Content = errorMessage;
            }
        }

        public void ReadJSON(string file)
        {
            using(StreamReader sr = new StreamReader(file))
            {
                players = JsonConvert.DeserializeObject<List<Player>>(sr.ReadToEnd())!;
            }
        }
    }
}