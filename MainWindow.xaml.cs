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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Player> players = new List<Player>();
        public MainWindow()
        {
            InitializeComponent();
            ReadJSON("Datas.json");

            ListPlayers.Click += DisplayPlayers;
            
        }

        public void DisplayPlayers(object sender, RoutedEventArgs e)
        {
            DisplayTitle.Content = "Játékosok sorrendje";

            foreach (var p in players)
            {
                lbList.Items.Add(p.Rank + ": " + p.Name);
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