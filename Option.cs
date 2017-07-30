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
    public partial class Option : Form
    {
        public Option()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    panel2.Visible = true;
                    panel3.Visible = false;
                    break;

                case 1:
                    panel3.Visible = true;
                    panel2.Visible = false;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        public void SaveSettings()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.Launguage = "Japanese";
                    break;

                case 1:
                    Properties.Settings.Default.Launguage = "English";
                    break;
            }
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
