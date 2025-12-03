using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
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
            EditPlayers.Click += DisplayChangePlayer;
            DeletePlayers.Click += DisplayDeletePlayer;

        }

        public void DisplayPlayers(object sender, RoutedEventArgs e)
        {
            gridAdd.Visibility = Visibility.Hidden;
            gridList.Visibility = Visibility.Visible;
            gridDelete.Visibility = Visibility.Hidden;
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
            gridDelete.Visibility = Visibility.Hidden;

            DisplayTitle.Content = "Játékos felvétele";

            btnSubmit.Click -= ChangePlayer;
            btnSubmit.Click -= AddPlayer;
            btnSubmit.Click += AddPlayer;

            btnSubmit.Content = "Felvétel";

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
            if (errorMessage == "")
            {
                Player player = new Player();
                player.Name = tbName.Text;
                player.Rank = int.Parse(tbRank.Text);
                player.SkillPoints = int.Parse(tbSkill.Text);
                player.Description = tbDesc.Text;

                bool changeNeeded = false;

                foreach (var p in players)
                {
                    if (p.Rank == player.Rank)
                    {
                        changeNeeded = true;
                        break;
                    }
                }

                if (changeNeeded)
                {
                    foreach (var p in players)
                    {
                        if (p.Rank >= player.Rank)
                        {
                            p.Rank++;
                        }
                    }
                }

                players.Add(player);

                UpdateJSON("Datas.json");
                ReadJSON("Datas.json");

                //lbErrorMessage.Content = "Sikeres játékosfelvétel!";
            }
            else
            {
                //lblErrorMessage.Content = errorMessage;
            }
            tbName.Text = "";
            tbRank.Text = "";
            tbSkill.Text = "";
            tbDesc.Text = "";

        }

        void DisplayChangePlayer(object sender, RoutedEventArgs e)
        {
            gridAdd.Visibility = Visibility.Visible;
            gridList.Visibility = Visibility.Hidden;
            gridDelete.Visibility = Visibility.Hidden;

            DisplayTitle.Content = "Játékos szerkesztése";

            btnSubmit.Click -= ChangePlayer;
            btnSubmit.Click -= AddPlayer;
            btnSubmit.Click += ChangePlayer;

            btnSubmit.Content = "Szerkesztés";

        }

        public void ChangePlayer(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (tbName.Text.Length == 0)
            {
                errorMessage += "Adjon meg nevet! ";
            }
            if (int.Parse(tbSkill.Text) > 100 || int.Parse(tbSkill.Text) < 0)
            {
                errorMessage += "Érvénytelen értékelés! ";
            }

            Player player = null;

            foreach (var p in players)
            {
                if (p.Name == tbName.Text)
                {
                    player = p;
                    break;
                }
            }

            if (player == null)
            {
                errorMessage += "Nincs iyen nevű játékos!";
            }

            if (errorMessage == "")
            {
                int originalRank = player.Rank;
                players.Remove(player);
                player.Name = tbName.Text;
                player.Rank = int.Parse(tbRank.Text);
                player.SkillPoints = int.Parse(tbSkill.Text);
                player.Description = tbDesc.Text;


                bool changeNeeded = false;

                foreach (var p in players)
                {
                    if (p.Rank == player.Rank)
                    {
                        changeNeeded = true;
                        break;
                    }
                }

                int l = 0;
                int r = 0;

                if (originalRank < player.Rank)
                {
                    l = originalRank;
                    r = player.Rank;
                }
                else
                {
                    l = player.Rank;
                    r = originalRank;
                }

                if (changeNeeded)
                {
                    foreach (var p in players)
                    {
                        if (p.Rank < r && p.Rank >= l)
                        {
                            p.Rank++;
                        }
                    }
                }

                players.Add(player);

                UpdateJSON("Datas.json");
                ReadJSON("Datas.json");
            }
            else
            {
                //lblErrorMessage.Content = errorMessage;
            }
            tbName.Text = "";
            tbRank.Text = "";
            tbSkill.Text = "";
            tbDesc.Text = "";
        }



        void DisplayDeletePlayer(object sender, RoutedEventArgs e)
        {
            gridAdd.Visibility = Visibility.Hidden;
            gridList.Visibility = Visibility.Hidden;
            gridDelete.Visibility = Visibility.Visible;

            DisplayTitle.Content = "Játékos törlése";

            btnDeleteSubmit.Click += DeletePlayer;

            btnSubmit.Content = "Szerkesztés";

        }

        void DeletePlayer(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";


            Player player = null;

            foreach (var p in players)
            {
                if (p.Name == tbDeleteName.Text)
                {
                    player = p;
                    break;
                }
            }

            if (player == null)
            {
                errorMessage += "Nincs iyen nevű játékos!";
            }

            if (errorMessage == "")
            {

                foreach (var p in players)
                {
                    if (p.Rank > player.Rank)
                    {
                        p.Rank--;
                    }
                }

                players.Remove(player);

                UpdateJSON("Datas.json");
                ReadJSON("Datas.json");
            }
            else
            {
                //lblErrorMessage.Content = errorMessage;
            }
            tbDeleteName.Text = "";
        }




        public void ReadJSON(string file)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                players = JsonConvert.DeserializeObject<List<Player>>(sr.ReadToEnd())!;
            }
        }

        public void UpdateJSON(string file)
        {
            var newJson = new StringBuilder();
            newJson.AppendLine("[");

            for (int i = 0; i < players.Count; i++)
            {
                var p = players[i];

                newJson.AppendLine($@"  {{
                            ""Name"": ""{p.Name}"",
                            ""Rank"": {p.Rank},
                            ""SkillPoints"": {p.SkillPoints},
                            ""Description"": ""{p.Description}""
                          }}{(i < players.Count - 1 ? "," : "")}");
            }

            newJson.AppendLine("]");

            File.WriteAllText(file, newJson.ToString(), new UTF8Encoding(false));
        }
    }
}