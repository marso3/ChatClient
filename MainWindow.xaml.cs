using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient httpclient = new HttpClient();
        string http = "http://";
        public MainWindow()
        {
            InitializeComponent();
            CreaUsuari();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EnviaMissatge(tbMsg.Text);
        }
        private void IniciaConexio(string server)
        {
            var timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(100).TotalMilliseconds);
            timer.Elapsed += async (sender, e) =>
            {
                var resposta = await httpclient.GetAsync($"{http}{server}/o");
                var chat = await resposta.Content.ReadAsStringAsync();

                // Use Dispatcher to update UI on the UI thread
                Dispatcher.Invoke(() =>
                {
                    if (chat != "")
                    {
                        ActualitzaChat(chat);
                    }
                });
            };

            timer.Start();
        }
        private void EnviaMissatge(string missatge)
        {
            tbMsg.Text = "";
            var msg = $"{tbUser.Text}: {missatge}";
            HttpUtility.UrlEncode(msg);
            httpclient.GetAsync($"{http}{tbServer.Text}/a?m={msg}");
        }

        private void CreaUsuari()
        {
            if(tbUser.Text == "")
            {
                tbUser.Text = $"usuari{new Random().Next(1000)}";
            }
        }
        private void ActualitzaChat(string chat)
        {
            tbChat.Text = chat;
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnviaMissatge(tbMsg.Text);
            }
        }

        private void btnConecta_Click(object sender, RoutedEventArgs e)
        {
            IniciaConexio(tbServer.Text);
        }
    }
}
