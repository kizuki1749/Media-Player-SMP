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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.uiMode = "None";
            axWindowsMediaPlayer1.settings.volume = 50;
        }

        private void 開くOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] files = openFileDialog1.FileNames;
                foreach (string a in files)
                {
                    axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(a));
                }
                timer1.Start();
            }
        }

        public static void MediaOpen(string path)
        {
            Form1 Form1 = new Form1();
            Form1.axWindowsMediaPlayer1.URL = path;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
            trackBar1.Value = (int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = trackBar1.Value;
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            timer1.Stop();
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Start();
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void axWindowsMediaPlayer1_OpenStateChange(object sender, AxWMPLib._WMPOCXEvents_OpenStateChangeEvent e)
        {
            switch (e.newState)
            {
                case 13:
                    label3.Text = axWindowsMediaPlayer1.currentMedia.durationString;
                    int duration = (int)axWindowsMediaPlayer1.currentMedia.duration;
                    trackBar1.Maximum = duration;
                    break;

                default:
                    break;
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = trackBar2.Value;
            label4.Text = "音量 : " + trackBar2.Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.mute = checkBox1.Checked;
        }

        private async void axWindowsMediaPlayer1_StatusChange(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.status == "停止" || axWindowsMediaPlayer1.status == "準備完了")
            {
                timer1.Stop();
                label2.Text = "00:00";
                trackBar1.Value = 0;
            }
            if (axWindowsMediaPlayer1.status == "バッファー中")
            {
                progressBar1.Visible = true;
                label6.Visible = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
                label6.Text = "バッファ中...";
            }
            else
            {
                progressBar1.Visible = false;
                label6.Visible = false;
            }
            label5.Text = axWindowsMediaPlayer1.status;
            label5.Visible = true;
            await Task.Delay(5000);
            label5.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.previous();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.next();
        }

        private void axWindowsMediaPlayer1_CurrentItemChange(object sender, AxWMPLib._WMPOCXEvents_CurrentItemChangeEvent e)
        {
            label1.Text = axWindowsMediaPlayer1.currentMedia.name;
        }

        private void uRLを開くUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(toolStripTextBox1.Text));
            timer1.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
        }

        private void リストの削除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("リストを削除しますか？この操作はもとに戻せません。","確認",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                axWindowsMediaPlayer1.currentPlaylist.clear();
            }
        }

        private void axWindowsMediaPlayer1_Buffering(object sender, AxWMPLib._WMPOCXEvents_BufferingEvent e)
        {
            progressBar1.Visible = true;
            label6.Visible = true;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = axWindowsMediaPlayer1.network.bufferingProgress;
            label6.Text = "バッファ中... ("+axWindowsMediaPlayer1.network.bufferingProgress+"%)";
            if (axWindowsMediaPlayer1.network.bufferingProgress == 100)
            {
                progressBar1.Visible = false;
                label6.Visible = false;
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
