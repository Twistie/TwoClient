using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Two_XNA_Client
{
    public partial class FindServer : Form
    {
        readonly Dictionary<int, IPEndPoint> _serverDictionary = new Dictionary<int, IPEndPoint>();
        public IPEndPoint ServerAddr;
        public string InName;
        private TwoClient _twoClient;
        public FindServer(TwoClient twoClient)
        {
            _twoClient = twoClient;
            InitializeComponent();
        }

        private void JoinButtonClick(object sender, EventArgs e)
        {
            InName = NameBox.Text;
            if (serverBox.SelectedIndex == -1)
                return;
            ServerAddr = _serverDictionary[serverBox.SelectedIndex];
            ServerAddr.Port = 48484;
            //_twoClient.SendJoin();
            
            this.Hide();
        }
        public void AddServer(string name, IPEndPoint inAddr)
        {
            _serverDictionary.Add(serverBox.Items.Count, inAddr);
            serverBox.Items.Add(name);
        }
        public void AddServerFromThread(string name, IPEndPoint inAddr)
        {
            if (this.serverBox.InvokeRequired)
            {
                this.serverBox.BeginInvoke(new MethodInvoker(delegate() { AddServerFromThread(name, inAddr); }));
                
            }
            else
                AddServer(name, inAddr);
        }
    }
}
