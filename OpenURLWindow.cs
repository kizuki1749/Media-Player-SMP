﻿using System;
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
    public partial class OpenURLWindow : Form
    {
        public OpenURLWindow(Form1 form)
        {
            form1 = form;
            InitializeComponent();
        }

        Form1 form1;

        private void button1_Click(object sender, EventArgs e)
        {
            string[] url = textBox1.Text.Split(new [] { "\r\n" },StringSplitOptions.None);
            foreach (string a in url)
            {
                form1.axWindowsMediaPlayer1.currentPlaylist.appendItem(form1.axWindowsMediaPlayer1.newMedia(a));
            }
            form1.timer1.Start();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                return;
            }
            string[] url = textBox1.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            form1.filedownloadtemp(new Uri(url[0]));
            Close();
        }

        private void OpenURLWindow_Load(object sender, EventArgs e)
        {
        }
    }
}
