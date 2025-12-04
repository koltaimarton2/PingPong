using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Numerics;
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
            AddMatch.Click += DisplayPlanGame;
            DisplayTitle.Content = "";
        }

        public void DisplayPlayers(object sender, RoutedEventArgs e)
        {
            gridAdd.Visibility = Visibility.Hidden;
            gridEdit.Visibility = Visibility.Hidden;

            gridList.Visibility = Visibility.Visible;
            gridPlan.Visibility = Visibility.Hidden;

            gridDelete.Visibility = Visibility.Hidden;
            lbList.Items.Clear();

            players.Sort((a, b) => a.Rank.CompareTo(b.Rank));
            DisplayTitle.Content = "Játékosok sorrendje";

            foreach (var p in players)
            {
                lbList.Items.Add(p.Rank + ": " + p.Name);
            }

            lblErrorMessage.Content = "";
        }

        void DisplayAddPlayer(object sender, RoutedEventArgs e)
        {
            gridEdit.Visibility = Visibility.Hidden;
            gridAdd.Visibility = Visibility.Visible;
            gridList.Visibility = Visibility.Hidden;
            gridDelete.Visibility = Visibility.Hidden;
            gridPlan.Visibility = Visibility.Hidden;


            DisplayTitle.Content = "Játékos felvétele";

            btnSubmit.Click += AddPlayer;

            btnSubmit.Content = "Felvétel";

            lblErrorMessage.Content = "";


        }

        public void AddPlayer(object sender, RoutedEventArgs e)
        {

            string errorMessage = "";

            if (tbName.Text == "")
            {
                errorMessage += "Adjon meg nevet! ";
            }
            if (tbSkill.Text != "" && int.TryParse(tbSkill.Text, out int skill))
            {
                if (skill > 100 || skill < 0)
                {
                    errorMessage += "Érvénytelen értékelés!";
                }
            }
            else if (tbSkill.Text == "")
            {
                errorMessage += "Érvénytelen értékelés! ";

            }

            if ((tbRank.Text != "" && int.TryParse(tbRank.Text, out int rank)) )
            {
                if (rank <= 0)
                {
                    errorMessage += "Érvénytelen rank!";
                }
            }
            else if (tbRank.Text == "")
            {
                errorMessage += "Érvénytelen rank!";

            }
            if (errorMessage == "")
            {
                Player player = new Player();
                player.Name = tbName.Text;
                player.Rank = int.Parse(tbRank.Text);
                if(player.Rank > players.Count+1)
                {
                    player.Rank = players.Count+1;
                }
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

                lblErrorMessage.Content = "Sikeres játékosfelvétel!";
            }
            else
            {
                lblErrorMessage.Content = errorMessage;
            }
            tbName.Text = "";
            tbRank.Text = "";
            tbSkill.Text = "";
            tbDesc.Text = "";

            
        }

        void DisplayChangePlayer(object sender, RoutedEventArgs e)
        {
            gridEdit.Visibility = Visibility.Visible;
            gridAdd.Visibility = Visibility.Hidden;
            gridList.Visibility = Visibility.Hidden;
            gridDelete.Visibility = Visibility.Hidden;
            gridPlan.Visibility = Visibility.Hidden;


            DisplayTitle.Content = "Játékos szerkesztése";

            btnEditSubmit.Click += ChangePlayer;

            cbEditName.Items.Clear();

            foreach (var player in players)
            {
                cbEditName.Items.Add(player.Name);
            }

            lblErrorMessage.Content = "";


        }

        public void ChangePlayer(object sender, RoutedEventArgs e)
        {
            lblErrorMessage.Content = "";
            if (string.IsNullOrWhiteSpace(cbEditName.Text))
            {
                lblErrorMessage.Content = "Válasszon játékost!";
                return;
            }

            Player player = players.FirstOrDefault(p => p.Name == cbEditName.Text);
            if (player == null)
            {
                lblErrorMessage.Content = "Játékos nem található!";
                return;
            }

            
            string errorMessage = "";

            if (tbEditSkill.Text != "" && int.TryParse(tbEditSkill.Text, out int skill))
            {
                if (skill > 100 || skill < 0)
                {
                    errorMessage += "Érvénytelen értékelés!";
                }
            }
            else if (tbEditSkill.Text == "")
            {
                errorMessage += "Érvénytelen értékelés! ";

            }

            if (tbEditRank.Text != "" && int.TryParse(tbEditRank.Text, out int rank))
            {
                if (rank <= 0)
                {
                    errorMessage += "Érvénytelen rank!";
                }
            }
            else if (tbEditRank.Text == "")
            {
                errorMessage += "Érvénytelen rank! ";

            }
            if (errorMessage == "")
            {
                int originalRank = player.Rank;
                players.Remove(player);
                player.Name = cbEditName.Text;
                player.Rank = int.Parse(tbEditRank.Text);
                if (player.Rank > players.Count + 1)
                {
                    player.Rank = players.Count + 1;
                }
                player.SkillPoints = int.Parse(tbEditSkill.Text);
                player.Description = tbEditDesc.Text;


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
                    if (l == player.Rank)
                    {

                        foreach (var p in players)
                        {
                            if (p.Rank < r && p.Rank >= l)
                            {
                                p.Rank++;
                            }
                        }
                    }
                    else
                    {
                        foreach (var p in players)
                        {
                            if (p.Rank <= r && p.Rank > l)
                            {
                                p.Rank--;
                            }
                        }
                    }
                }

                players.Add(player);

                UpdateJSON("Datas.json");
                ReadJSON("Datas.json");
            }
            else
            {
                lblErrorMessage.Content = errorMessage;
            }
            cbEditName.Text = "";
            tbEditRank.Text = "";
            tbEditSkill.Text = "";
            tbEditDesc.Text = "";
        }



        void DisplayDeletePlayer(object sender, RoutedEventArgs e)
        {
            gridAdd.Visibility = Visibility.Hidden;
            gridList.Visibility = Visibility.Hidden;
            gridPlan.Visibility = Visibility.Hidden;
            gridDelete.Visibility = Visibility.Visible;
            gridEdit.Visibility = Visibility.Hidden;


            DisplayTitle.Content = "Játékos törlése";

            btnDeleteSubmit.Click += DeletePlayer;

            cbDeleteName.Items.Clear();

            foreach (var p in players)
            {
                cbDeleteName.Items.Add(p.Name);
            }

            lblErrorMessage.Content = "";

            cbDeleteName.Text = "Válassz...";

        }

        void DeletePlayer(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";


            Player player = null;

            foreach (var p in players)
            {
                if (p.Name == cbDeleteName.Text)
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
                
                lblErrorMessage.Content = "Sikeres törlés!";
            }
            else
            {
                lblErrorMessage.Content = errorMessage;
            }
            cbDeleteName.Text = "";
        }


        void DisplayPlanGame(object sender, RoutedEventArgs e)
        {
            gridPlan.Visibility = Visibility.Visible;
            gridAdd.Visibility = Visibility.Hidden;
            gridList.Visibility = Visibility.Hidden;
            gridEdit.Visibility = Visibility.Hidden;

            gridDelete.Visibility = Visibility.Hidden;

            DisplayTitle.Content = "Meccs tervezése";

            btnPlanMatch.Click += PlanGame;

            lblErrorMessage.Content = "";

            cbPlayer1.Items.Clear();
            cbPlayer2.Items.Clear();

            foreach (var p in players)
            {
                cbPlayer1.Items.Add(p.Name);
                cbPlayer2.Items.Add(p.Name);
            }

            cbPlayer1.Text = "Válassz...";
            cbPlayer2.Text = "Válassz...";


        }

        void PlanGame(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";
            Player player1 = null;
            Player player2 = null;

            foreach (var p in players)
            {
                if (p.Name == cbPlayer1.Text) player1 = p;
                if (p.Name == cbPlayer2.Text) player2 = p;
            }

            if (player1 == null || player2 == null)
            {
                errorMessage += "Nincsen kiválasztva játékos!";
            } 

            if (errorMessage == "")
            {
                var checkedValue = spRadios.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked.HasValue && r.IsChecked.Value).Content;


                string message;
                Player winner = null;

                switch (checkedValue)
                {
                    case "1":
                        var result = SimulateGame(player1, player2);
                        message = result.Item1;
                        winner = result.Item2;
                        MessageBox.Show(message, "Meccs eredménye");
                        break;
                    case "3":
                        int win1 = 0;
                        int win2 = 0;
                        while (true)
                        {
                            if (win1 > win2 && win1 == 2)
                            {
                                MessageBox.Show($"{player1.Name} nyerte a teljes mérkőzést.");
                                break;
                            }
                            if (win2 > win1 && win2 == 2)
                            {
                                MessageBox.Show($"{player2.Name} nyerte a teljes mérkőzést.");
                                break;
                            }

                            result = SimulateGame(player1, player2);
                            message = result.Item1;
                            winner = result.Item2;
                            MessageBox.Show(message, "Meccs eredménye");


                            if (winner.Name == player1.Name) win1++;
                            else if (winner.Name == player2.Name) win2++;

                        }
                        break;
                    case "5":
                        win1 = 0;
                        win2 = 0;
                        while (true)
                        {
                            if (win1 > win2 && win1 == 3)
                            {
                                MessageBox.Show($"{player1.Name} nyerte a teljes mérkőzést.");
                                break;
                            }
                            if (win2 > win1 && win2 == 3)
                            {
                                MessageBox.Show($"{player2.Name} nyerte a teljes mérkőzést.");
                                break;
                            }

                            result = SimulateGame(player1, player2);
                            message = result.Item1;
                            winner = result.Item2;
                            MessageBox.Show(message, "Meccs eredménye");


                            if (winner.Name == player1.Name) win1++;
                            else if (winner.Name == player2.Name) win2++;

                        }
                        break;
                }

                if(player1.Name == winner.Name)
                {
                    if (player1.Rank != 1)
                    {
                        for (int i = 0; i < players.Count; i++)
                        {
                            var p = players[i];
                            if (p.Name == winner.Name && winner.SkillPoints <= player2.SkillPoints)
                            {
                                players[i].SkillPoints += (player2.SkillPoints - winner.SkillPoints) / 10;

                                players[i].Rank--;
                                foreach (var pl in players)
                                {
                                    if (pl.Rank == winner.Rank && pl != winner)
                                    {
                                        pl.Rank++;
                                        break;
                                    }
                                }
                                break;

                            }
                        }
                    }
                }
                else 
                {
                    if (player2.Rank != 1)
                    {
                        for (int i = 0; i < players.Count; i++)
                        {
                            var p = players[i];
                            if (p.Name == winner.Name && winner.SkillPoints <= player1.SkillPoints)
                            {
                                players[i].SkillPoints += (player1.SkillPoints - winner.SkillPoints) / 10;

                                players[i].Rank--;
                                foreach (var pl in players)
                                {
                                    if (pl.Rank == winner.Rank - 1)
                                    {
                                        pl.Rank++;
                                        break;
                                    }
                                }
                                break;

                            }
                        }
                    }
                }

            }
        }


        public (string, Player) SimulateGame(Player p1, Player p2)
        {
            Random random = new Random();

            int p1Points = 0;
            int p2Points = 0;

            double chance = (double)p1.SkillPoints / (p1.SkillPoints + p2.SkillPoints);

            StringBuilder log = new StringBuilder();
            log.AppendLine($"{p1.Name} ({p1.SkillPoints}) vs {p2.Name} ({p2.SkillPoints})");
            log.AppendLine("--------------------------------------------------");

            while (true)
            {
                double roll = random.NextDouble();
                
                if (roll < chance)
                {
                    p1Points++;
                    log.AppendLine($"{p1.Name} szerzi a pontot! {p1Points} - {p2Points}");
                }
                else
                {
                    p2Points++;
                    log.AppendLine($"{p2.Name} szerzi a pontot! {p1Points} - {p2Points}");
                }

                if (p1Points >= 11 && (p1Points - p2Points) >=2 )
                {
                    log.AppendLine("--------------------------------------------------");

                    log.AppendLine($"{p1.Name} megnyerte a szettet!");
                    return (log.ToString(),p1);
                }
                if (p2Points >= 11 && (p2Points - p1Points) >= 2)
                {
                    log.AppendLine("--------------------------------------------------");

                    log.AppendLine($"{p2.Name} megnyerte a szettet!");
                    return (log.ToString(), p2);
                }
            }


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

        private void cbEditName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

        }

        private void LoadEditData(object sender, EventArgs e)
        {
            Player player = null;
            foreach (var p in players)
            {
                if (p.Name == cbEditName.Text)
                {
                    player = p;
                    break;
                }
            }
            if (player != null)
            { 
                tbEditRank.Text = player.Rank.ToString();
                tbEditSkill.Text = player.SkillPoints.ToString();
                tbEditDesc.Text = player.Description;
            }
        }
    }
}