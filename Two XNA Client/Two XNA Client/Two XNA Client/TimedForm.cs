using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Two_XNA_Client
{
    public partial class TimedForm : Form
    {
        private readonly TwoClient _twoClient;
        private int _timeLeft;
        public TimedForm(TwoClient twoClient, string label, int timeInSeconds)
        {
            _timeLeft = timeInSeconds;
            InitializeComponent();
            _twoClient = twoClient;
            inLabel.Text = label;
            TimeLabel.Text = _timeLeft.ToString() + " seconds.";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while( _timeLeft > 0)
            {
                _twoClient.SendMessage("ANNOUNCE " + _timeLeft + "!");
                button1.Hide();
                Thread.Sleep(1000);
                _timeLeft--;
                TimeLabel.Text = _timeLeft.ToString() + " seconds.";
                this.Refresh();
                
            }
            this.Hide();
        }

    }
}
