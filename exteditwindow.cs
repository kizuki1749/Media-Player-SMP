using niconicoviewer;
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

namespace Media_Player_SMP
{
    public partial class exteditwindow : Form
    {
        public exteditwindow()
        {
            InitializeComponent();
        }

        private void exteditwindow_Load(object sender, EventArgs e)
        {
            foreach (string addtext in Properties.Settings.Default.fileext)
            {
                listBox1.Items.Add(addtext);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int a = listBox1.SelectedIndex;
            if (a == -1) return;
            textBox1.Text = listBox1.Items[a].ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int a = listBox1.SelectedIndex;
            if (a == -1) return;
            listBox1.Items[a] = string.Format(textBox1.Text,a);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("新しい拡張子");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Insert(listBox1.SelectedIndex,"新しい拡張子");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            apply();
        }

        public async void apply()
        {
            progresswindow p = new progresswindow();
            p.progressvisible = false;
            p.text = "設定ファイルを書き換えています。";
            p.Show();
            await Task.Delay(2000);
            int b = 0;
            b = 0;
            Properties.Settings.Default.fileext.Clear();
            foreach (string a in listBox1.Items)
            {
                Properties.Settings.Default.fileext.Add(a);
                b = b + 1;
            }
            p.Close();
            p.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            apply();
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
