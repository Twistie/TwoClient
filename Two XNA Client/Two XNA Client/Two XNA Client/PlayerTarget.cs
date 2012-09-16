using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Two_XNA_Client
{
    public partial class PlayerTarget : Form
    {
        private TwoClient _twoClient;

        public PlayerTarget(TwoClient twoClient, List<Player> playerList)
        {
            _twoClient = twoClient;
            InitializeComponent();
            foreach (Player p in playerList)
            {
                PlayerBox.Items.Add(p.Name);
            }
        }
        private void TargetButtonClick(object sender, EventArgs e)
        {
            if (PlayerBox.SelectedIndex == -1)
                return;
            _twoClient.SendMessage("CARDARGS " + PlayerBox.SelectedIndex);
            this.Hide();
        }
    }
}