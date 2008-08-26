using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FreericeBot
{
    public partial class Interface : Form
    {
        public Interface()
        {
            InitializeComponent();
            Random r = new Random();
            
            FreericeBot bot = new FreericeBot();
            bot.statusUpdate += statusUpdate;
            bot.Start();
        }

        private void statusUpdate(string message)
        {
            if (InvokeRequired)//required sense we are going to modify controls
                {
                    Invoke(new FreericeBot.StatusUpdateHandler(statusUpdate), new object[] { message });
                }
            else
                {
                Console.AppendText(message + "\r\n");
                }
        }
    }
}
