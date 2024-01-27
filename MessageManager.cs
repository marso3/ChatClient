using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Web;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ChatClient
{
    public sealed class MessageManager
    {
        private static MessageManager instance = null;
        private static HttpClient httpclient;

        private static string http = "http://";
        private string server = "";
        private string usuari = "";

        public static MessageManager Instance
        {
            get{
                if (instance == null)
                {
                    instance = new MessageManager();
                    httpclient = new HttpClient();
                }
                return instance;
            }
        }

        public void IniciaConexio(MainWindow parent, string server)
        {
            this.server = server;
            var timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(100).TotalMilliseconds);
            timer.Elapsed += async (sender, e) =>
            {
                try
                {
                    var resposta = await httpclient.GetAsync($"{http}{this.server}/o");
                    var encryptedChat = await resposta.Content.ReadAsStringAsync();

                    // Use Dispatcher to update UI on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (encryptedChat != "")
                        {
                            StringBuilder sb = new StringBuilder();
                            var chatList = encryptedChat.Split("\r\n").ToList();
                            foreach (var line in chatList)
                            {
                                if (line != "")
                                {
                                    sb.AppendLine(EncryptionManager.Decrypt(line));
                                }
                            }
                            
                            parent.ActualitzaChat(sb.ToString());
                        }
                    });
                }
                catch
                {
                    throw new Exception("No s'ha pogut establir connexió amb el servidor");
                }
            };

            timer.Start();
        }
        public void EnviaMissatge(string missatge)
        {
            var msg = $"{this.usuari}: {missatge}";
            var encryptedMessage = EncryptionManager.Encrypt(msg);
            httpclient.GetAsync($"{http}{this.server}/a?m={Convert.ToBase64String(encryptedMessage)}");
        }
        public string CreaUsuari(string usuari)
        {
            if (usuari == "")
            {
                this.usuari = $"usuari{new Random().Next(1000)}";
            }
            return this.usuari;
        }
    }
}
