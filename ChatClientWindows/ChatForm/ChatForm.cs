using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server;

namespace ChatClientWindows
{
    public partial class ChatForm : Form
    {
        Client client;
        CancellationTokenSource token;
        public ChatForm()
        {
            InitializeComponent();

            connectMenu.Click += connectMenu_Click;
            disconnectMenu.Click += disconnectMenu_Click;
        }

        public void ConnectToServer()
        {
            LogginForm loggin = new LogginForm(this);
            loggin.ShowDialog();
            if (client == null || !client.IsConnected())
            {
                this.Close();
                return;
            }
            token = new CancellationTokenSource();

            chatBox.Text = "========= Welcome to the server =========\n";
            client.StartReceiving(OnReceive, token);
        }

        void OnReceive(string message)
        {
            try
            {
                message = ChatConsoleVersion.ChatClientUI.ParseResponse(message);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    e.Message,
                    "Invalid response",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            chatBox.Text += $"{message}\n";
            chatBox.SelectionStart = chatBox.Text.Length;
            chatBox.ScrollToCaret();
        }

        public void SetConnection(Client client)
        {
            if (this.client != null && this.client.IsConnected())
            {
                this.client.Disconnect();
            }
            this.client = client;
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            if (!client.IsConnected())
            {
                MessageBox.Show(
                    "You are not connected to any chat server.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ConnectToServer();
                return;
            }
            string message = messageBox.Text;
            messageBox.Text = "";
            if (message == "")
            {
                return;
            }
            client.SendAsync(message);
        }

        private void ChatForm_Shown(object sender, EventArgs e)
        {
            ConnectToServer();
        }

        private void connectMenu_Click(object sender, EventArgs e)
        {
            ConnectToServer();
        }

        private void disconnectMenu_Click(object sender, EventArgs e)
        {
            this.client.StopReceiving();
            this.client.Disconnect();
            ConnectToServer();
        }
    }
}
