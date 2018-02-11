using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Shell32;
using Tsukikage.WinMM.MidiIO;
using System.Net;
using System.Text.RegularExpressions;


namespace Media_Player_SMP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string[] cmds = System.Environment.GetCommandLineArgs();

        private void Form1_Load(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.uiMode = "None";
            axWindowsMediaPlayer1.settings.volume = 50;
            axWindowsMediaPlayer1.settings.autoStart = false;
            try
            {
                if (cmds[1] != "")
                {
                    axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(cmds[1]));
                    timer1.Start();
                }
            }
            catch (Exception)
            {

            }
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
            try
            {
                label2.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
                trackBar1.Value = (int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            }
            catch (Exception)
            {

            }
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
#if ID3取得
                    /*
                    Mp3File mp3 = new Mp3File(axWindowsMediaPlayer1.currentMedia.sourceURL);
                    label9.Visible = true;
                    try
                    {
                        StatusChange("ID3 v2 タグの取得を試みています...");
                        Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2x);
                        label9.Text = "タイトル: " + tag.Title + "\nアーティスト: " + tag.Artists + "\nアルバム: " + tag.Album + "\n収録日: " + tag.RecordingDate + "\n著作権: " + tag.CopyrightUrl + "\n指揮者: " + tag.Conductor + "\nトラック: " + tag.Track + "\nリリース年: " + tag.Year;
                    }
                    catch (Exception)
                    {
                        StatusChange("ID3 v1 タグの取得を試みています...");
                        Id3Tag tag = mp3.GetTag(Id3TagFamily.Version1x);
                        label9.Text = "タイトル: " + tag.Title + "\nアーティスト: " + tag.Artists + "\nアルバム: " + tag.Album + "\n収録日: " + tag.RecordingDate + "\n著作権: " + tag.CopyrightUrl + "\n指揮者: " + tag.Conductor + "\nトラック: " + tag.Track + "\nリリース年: " + tag.Year;
                    }
                    */
#endif
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

        private void axWindowsMediaPlayer1_StatusChange(object sender, EventArgs e)
        {
            StatusChange(axWindowsMediaPlayer1.status);
        }
        public enum Language { Japanese , English }
        public async void StatusChange(string status)
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
            label5.Text = status;
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
            if (panel3.Visible == true)
            {
                StatusChange("テキストファイル セッション を閉じました。");
            }
            panel3.Visible = false;
            label1.Text = axWindowsMediaPlayer1.currentMedia.name;
            label7.Text = axWindowsMediaPlayer1.currentMedia.sourceURL;
            label8.Text = axWindowsMediaPlayer1.currentMedia.name;
            string Extension = Path.GetExtension(axWindowsMediaPlayer1.currentMedia.sourceURL);
            if ( Extension == ".mp3" || Extension == "MP3" || Extension == ".wma" || Extension == ".WMA" || Extension == ".m4a" || Extension == ".M4A" || Extension == ".wav" || Extension == ".WAV" )
            {
                panel2.Visible = true;
                checkBox1.Enabled = true;
                trackBar2.Enabled = true;
                axWindowsMediaPlayer1.settings.mute = checkBox1.Checked;
                axWindowsMediaPlayer1.settings.volume = trackBar2.Value;
                label9.Visible = false;
            }
            else if ( Extension == ".mp3" || Extension == "MP3" )
            {
            }
            else if ( Extension == ".mid" || Extension == ".MID" )
            {
                panel2.Visible = true;
                checkBox1.Enabled = false;
                trackBar2.Enabled = false;
                MidiSequence midi = new MidiSequence(axWindowsMediaPlayer1.currentMedia.sourceURL);
                label9.Text = "タイトル: " + midi.Title + "\n\n著作権: " + midi.Copyright + "\n\nトラック: "+midi.Tracks;
                label9.Visible = true;
            }
            else
            {
                panel2.Visible = false;
                checkBox1.Enabled = true;
                trackBar2.Enabled = true;
                axWindowsMediaPlayer1.settings.mute = checkBox1.Checked;
                axWindowsMediaPlayer1.settings.volume = trackBar2.Value;
                label9.Visible = false;
            }
            label3.Text = axWindowsMediaPlayer1.currentMedia.durationString;
            int duration = (int)axWindowsMediaPlayer1.currentMedia.duration;
            trackBar1.Maximum = duration;
        }

        private string ByteToStr(byte[] b)
        {
            Encoding Enc = System.Text.Encoding.UTF8;
            return Enc.GetString(b).TrimEnd();
        }

        private int ByteToInt(byte b)
        {
            return Convert.ToInt32(b);
        }

        private void uRLを開くUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(toolStripTextBox1.Text));
            timer1.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (label2.Text == "")
            {
                label2.Text = "00:00";
                timer1.Stop();
            }
            else if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString != "")
            {
                timer1.Start();
            }
        }

        private void リストの削除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("リストを削除しますか？この操作はもとに戻せません。","確認",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                axWindowsMediaPlayer1.currentPlaylist.clear();
                panel2.Visible = false;
                label1.Text = "";
                label3.Text = "00:00";
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

        private void axWindowsMediaPlayer1_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e)
        {
            DialogResult result = MessageBox.Show("再生時にエラーが発生しました。\n\n対象ファイルパス: " + axWindowsMediaPlayer1.currentMedia.sourceURL, "エラー", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
            if (result == DialogResult.Abort)
            {
                axWindowsMediaPlayer1.Ctlcontrols.stop();
            }
            else if (result == DialogResult.Retry)
            {
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void 開くOToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (Properties.Settings.Default.textfileflag == false)
                {
                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                    axWindowsMediaPlayer1.currentPlaylist.clear();
                    label1.Text = "";
                    label3.Text = "00:00";
                }
                StreamReader sr = new StreamReader(openFileDialog2.FileName, Encoding.GetEncoding("Shift_JIS"));
                string text = sr.ReadToEnd();
                textBox1.Text = text;
                panel3.Visible = true;
                StatusChange(openFileDialog2.FileName+" を読み込みました。");
                sr.Close();
            }
        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                if (panel2.Visible == false)
                {
                    panel2.Visible = false;
                }
                else
                {
                    panel2.Visible = true;
                }
            }
            else
            {
                panel2.Visible = false;
            }
            StatusChange("テキストファイル セッション を閉じました。");
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void 環境設定OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Option f = new Option();
            if (this.TopMost == true)
            {
                f.TopMost = true;
            }
            else
            {
                f.TopMost = false;
            }
            f.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void フルスクリーンFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            フルスクリーンFToolStripMenuItem.Checked = !フルスクリーンFToolStripMenuItem.Checked;
        }
        bool maxflag = false;
        private void フルスクリーンFToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (フルスクリーンFToolStripMenuItem.Checked == true)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                    maxflag = true;
                }
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                timer3.Enabled = true;
                StatusChange("フルスクリーン表示に切り替えました。Ctrl(Control)キー+Altキー+Fキーで元に戻ります。またマウスを一番上や一番下に合わせるとメニューやシークバーが表示されます。");
                menuStrip1.Visible = false;
                panel1.Visible = false;
            }
            else
            {
                if (maxflag == true)
                {
                    WindowState = FormWindowState.Normal;
                }
                FormBorderStyle = FormBorderStyle.Sizable;
                if (maxflag == true)
                {
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    WindowState = FormWindowState.Normal;
                }
                timer3.Enabled = false;
                StatusChange("ウィンドウ表示に切り替えました。");
                menuStrip1.Visible = true;
                panel1.Visible = true;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (MousePosition.Y == 0)
            {
                menuStrip1.Visible = true;
            }
            else
            {
                if (MousePosition.Y >= 25)
                {
                    menuStrip1.Visible = false;
                }
            }
            if ((Size.Height - 1) <= MousePosition.Y)
            {
                panel1.Visible = true;
            }
            if ((Size.Height - 98) >= MousePosition.Y)
            {
                panel1.Visible = false;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        bool errflag = false;
        string errtext = "";

        private async void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            Array.Sort(fileName);
            p.Show();
            p.progressstyle = ProgressBarStyle.Continuous;
            p.progressvalue = 0;
            p.text = "ファイルの読み込み中です。( " + p.progressvalue + " / " + p.progressmax + " )";
            p.progressmax = 0;
            try
            {
                p.progressstyle = ProgressBarStyle.Marquee;
                foreach (string file in fileName)
                {
                    p.text = "しばらくお待ちください。ファイル数が多いと時間がかかる場合があります。";
                    FileAttributes file1 = File.GetAttributes(file);
                    if ((file1 & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        p.progressmax += Directory.GetFiles(file, "*", SearchOption.TopDirectoryOnly).Length;
                    }
                    else
                    {
                        p.progressmax++;
                    }
                }
                p.progressstyle = ProgressBarStyle.Continuous;
                Add(fileName);
            }
            catch
            {
                errflag = true;
                errtext += "読み込みに不明なエラーが発生しました。\n";

            }
        }

        niconicoviewer.progresswindow p = new niconicoviewer.progresswindow();

        public async Task Add(string[] filelist)
        {
            bool flag = false;
            p.progressstyle = ProgressBarStyle.Continuous;
            foreach (string file1 in filelist)
            {
                Array.Sort(filelist);
                await Task.Delay(20);
                FileAttributes file = File.GetAttributes(file1);
                p.text = "ファイルの読み込み中です。( " + p.progressvalue + " / " + p.progressmax + " )\n"+file1;
                if ((file & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    p.Visible = true;
                    flag = true;
                    string[] files = Directory.GetFiles(file1, "*", SearchOption.AllDirectories);
                    p.progressmax += files.Length;
                    await Task.Delay(20);
                    await Add(files);
                }
                else
                {
                    foreach (string fileext in Properties.Settings.Default.fileext)
                    {
                        if (fileext == Path.GetExtension(file1))
                        {
                            axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(file1));
                        }
                    }
                }
                try
                {
                    p.progressvalue++;
                }
                catch
                {

                }
                if (p.cancel == true)
                {
                    break;
                }
            }
            if (errflag == true)
            {
                //MessageBox.Show(errtext, "エラー一覧", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            errflag = false;
            errtext = "";

            flag = false;
            p.Visible = false;
        }

        private static niconicoviewer.progresswindow _Instance;

        public static niconicoviewer.progresswindow Instance
        {
            get
            {
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }

        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                p.Show();
                p.progressstyle = ProgressBarStyle.Continuous;
                p.progressvalue = 0;
                p.text = "ファイルの読み込み中です。( " + p.progressvalue + " / " + p.progressmax + " )";

                try
                {
                    p.progressmax = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*", SearchOption.TopDirectoryOnly).Length;
                    string[] a = { "" };
                    a[0] = folderBrowserDialog1.SelectedPath;
                    Add(a);
                }
                catch
                {
                    errflag = true;
                    errtext += "読み込みに不明なエラーが発生しました。" + folderBrowserDialog1.SelectedPath + "\n";
                }
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            Form2.Form2Instance = f;
            f.Show();
            f.url = toolStripTextBox1.Text;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.setMode("loop", checkBox2.Checked);
        }

        private void このウィンドウを常に最前面に表示するTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            このウィンドウを常に最前面に表示するTToolStripMenuItem.Checked = !このウィンドウを常に最前面に表示するTToolStripMenuItem.Checked;
        }

        private void このウィンドウを常に最前面に表示するTToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            switch (このウィンドウを常に最前面に表示するTToolStripMenuItem.Checked)
            {
                case true:
                    this.TopMost = true;
                    break;

                case false:
                    this.TopMost = false;
                    break;
            }
        }
    }
}
