using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Server;

namespace ChatClientWindows
{
    public partial class LogginForm : Form
    {
        ChatForm parrent;
        public LogginForm(ChatForm parrent)
        {
            InitializeComponent();
            this.parrent = parrent;
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            Tuple<string, int> serverAddress;
            try
            {
                serverAddress = ParseAddress(serverAddresTextBox.Text);
            }
            catch (Exception exc)
            {
                MessageBox.Show(
                    $"Server address has an invalid format:\n{exc.Message}",
                    "Can't parse address",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            string ip = serverAddress.Item1;
            int port = serverAddress.Item2;

            Client client = new Client(ip, port);

            try
            {
                await client.ConnectAsync();
            } 
            catch (Exception exc) 
            {
                MessageBox.Show(
                    $"Can't connect to the remote server:\n{exc.Message}",
                    "Connection error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                client.Disconnect();
                return;
            }

            Dictionary<string, string> credentials = new Dictionary<string, string>()
            {
                ["name"] = nicknameTextBox.Text
            };

            string response = await client.SendRecieveAsync(JsonConvert.SerializeObject(credentials));

            if (response != ChatConsoleVersion.ChatServer.CONNECTION_ESTABLISHED)
            {
                MessageBox.Show(
                    $"Server refused your credentials:\n{response}",
                    "Connection error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                client.Disconnect();
                return;
            }

            parrent.SetConnection(client);
            this.Close();
        }

        private Tuple<string, int> ParseAddress(string address)
        {
            if (address == "")
            {
                return new Tuple<string, int>("127.0.0.1", 8080);
            }
            string[] components = address.Split(":");

            string ip = components[0];
            int port;

            if (components.Length == 1) 
            {
                port = 80;
            }
            else
            {
                port = int.Parse(components[1]);
            }

            return new Tuple<string, int>(ip, port);
        }
    }
}
