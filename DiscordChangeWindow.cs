using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Media_Player_SMP
{
    public partial class DiscordChangeWindow : Form
    {
        public DiscordChangeWindow(Form1 form)
        {
            InitializeComponent();
            form1 = form;
        }

        Form1 form1;

        private void DiscordChangeWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                form1.t5flag = true;
                timer1.Enabled = true;
            }
            else
            {
                form1.t5flag = false;
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            form1.str1 = textBox1.Text;
        }
    }
}
