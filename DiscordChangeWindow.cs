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
            checkBox1.Checked = form1.t5flag;
            textBox1.Text = form1.str1;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                form1.t5flag = false;
                timer1.Enabled = false;
            }
            else
            {
                form1.t5flag = true;
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            form1.str1 = textBox1.Text;
        }
    }
}
