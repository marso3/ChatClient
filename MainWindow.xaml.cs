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
        
        public MainWindow()
        {
            InitializeComponent();
            MessageManager manager = MessageManager.Instance;
            tbUser.Text = manager.CreaUsuari(tbUser.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EnviaMissatge(tbMsg.Text);
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnviaMissatge(tbMsg.Text);
            }
        }

        private void EnviaMissatge(string missatge)
        {
            tbMsg.Text = "";
            MessageManager manager = MessageManager.Instance;
            manager.EnviaMissatge(missatge);
        }

        
        public void ActualitzaChat(string chat)
        {
            tbChat.Text = chat;
        }

        private void btnConecta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageManager manager = MessageManager.Instance;
                manager.IniciaConexio(this, tbServer.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }
    }
}
