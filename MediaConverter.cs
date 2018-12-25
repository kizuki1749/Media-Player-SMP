using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Media_Player_SMP
{
    public partial class MediaConverter : Form
    {
        public MediaConverter()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = saveFileDialog1.FileName;
            }
        }

        private void MediaConverter_Load(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.SelectedIndex = 0;
        }

        niconicoviewer.progresswindow progresswindow = new niconicoviewer.progresswindow();

        private void button3_Click(object sender, EventArgs e)
        {
            progresswindow.text = "変換しています。しばらくお待ちください。時間がかかります。";
            progresswindow.progressstyle = ProgressBarStyle.Marquee;
            progresswindow.Show();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var ffMpeg = new FFMpegConverter();
            try
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        return;

                    case 1:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.ac3);
                        break;

                    case 2:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.aiff);
                        break;

                    case 3:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.alaw);
                        break;

                    case 4:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.asf);
                        break;

                    case 5:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.ast);
                        break;

                    case 6:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.au);
                        break;

                    case 7:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.avi);
                        break;

                    case 8:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.caf);
                        break;

                    case 9:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.dts);
                        break;

                    case 10:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.eac3);
                        break;

                    case 11:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.ffm);
                        break;

                    case 12:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.flac);
                        break;

                    case 13:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.flv);
                        break;

                    case 14:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.h261);
                        break;

                    case 15:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.h263);
                        break;

                    case 16:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.h264);
                        break;

                    case 17:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.m4v);
                        break;

                    case 18:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.matroska);
                        break;

                    case 19:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.mjpeg);
                        break;

                    case 20:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.mulaw);
                        break;

                    case 21:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.ogg);
                        break;

                    case 22:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.oma);
                        break;

                    case 23:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.webm);
                        break;

                    case 24:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.wmv);
                        break;

                    case 25:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.raw_video);
                        break;

                    case 26:
                        ffMpeg.ConvertMedia(textBox1.Text, textBox2.Text, Format.raw_data);
                        break;
                }
            }
            catch
            {

            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progresswindow.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
    }
}
