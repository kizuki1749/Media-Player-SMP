using niconicoviewer;
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
                    panel4.Visible = false;
                    break;

                case 1:
                    panel3.Visible = false;
                    panel2.Visible = false;
                    panel4.Visible = true;
                    break;

                case 2:
                    panel3.Visible = true;
                    panel2.Visible = false;
                    panel4.Visible = false;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        public async void SaveSettings()
        {
            progresswindow p = new progresswindow();
            p.progressvisible = false;
            p.text = "設定ファイルを書き換えています。";
            p.Show();
            await Task.Delay(2000);
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.Launguage = "Japanese";
                    break;

                case 1:
                    Properties.Settings.Default.Launguage = "English";
                    break;
            }
            if (checkBox1.Checked == true)
            {
                Properties.Settings.Default.textfileflag = true;
            }
            else
            {
                Properties.Settings.Default.textfileflag = false;
            }

            Properties.Settings.Default.Save();
            p.Close();
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

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
        }

        private void Option_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Launguage == "Japanease")
            {
                comboBox1.SelectedIndex = 0;
            }
            else if (Properties.Settings.Default.Launguage == "English")
            {
                comboBox1.SelectedIndex = 1;
            }
            if (Properties.Settings.Default.textfileflag == true)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            exteditwindow exteditwindow = new exteditwindow();
            exteditwindow.ShowDialog();
        }
    }
}