using niconicoviewer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Media_Player_SMP
{
    public partial class Option : Form
    {
        public Option(Form1 f)
        {
            form1 = f;
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
            if (checkBox2.Checked == true)
            {
                Properties.Settings.Default.betafuture = true;
            }
            else
            {
                Properties.Settings.Default.betafuture = false;
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

        public static Form1 form1;

        private void Option_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            propertyGrid1.SelectedObject = (Form1)Owner;
            propertyGrid7.SelectedObject = (MenuStrip)form1.Controls["menuStrip1"];
            propertyGrid2.SelectedObject = (AxWMPLib.AxWindowsMediaPlayer)form1.Controls["axWindowsMediaPlayer1"];
            propertyGrid3.SelectedObject = (Panel)form1.Controls["Panel1"];
            propertyGrid4.SelectedObject = (Panel)form1.Controls["Panel2"];
            propertyGrid5.SelectedObject = (Panel)form1.Controls["Panel3"];
            propertyGrid6.SelectedObject = (Panel)form1.Controls["Panel4"];
            Property prop = new Property();
            propertyGrid8.SelectedObject = prop;
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
            if (Properties.Settings.Default.betafuture == true)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            exteditwindow exteditwindow = new exteditwindow();
            exteditwindow.ShowDialog();
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            button7.Enabled = false;
            DateTime dNow = System.DateTime.Now;
            textBox1.Text = dNow.ToString("[HH:mm:ss]") + " 更新プログラムを確認しています";
            progressBar1.Style = ProgressBarStyle.Marquee;
            await Task.Delay(1000);
            try
            {
                //if (!ApplicationDeployment.IsNetworkDeployed) return;
                ApplicationDeployment currentDeploy = ApplicationDeployment.CurrentDeployment;
                if (currentDeploy.CheckForUpdate())
                {
                    button7.Enabled = true;
                    dNow = System.DateTime.Now;
                    textBox1.Text += "\r\n"+dNow.ToString("[HH:mm:ss]") + " 更新プログラムがあります";
                    dNow = System.DateTime.Now;
                    textBox1.Text += "\r\n"+dNow.ToString("[HH:mm:ss]") + " 現在のバージョン: "+currentDeploy.CurrentVersion;
                    dNow = System.DateTime.Now;
                    textBox1.Text += "\r\n"+dNow.ToString("[HH:mm:ss]") + " ダウンロードURL: "+currentDeploy.UpdateLocation;
                }
                else
                {
                    dNow = System.DateTime.Now;
                    textBox1.Text += "\r\n"+dNow.ToString("[HH:mm:ss]") + " このプログラムは最新です。 バージョン: "+currentDeploy.CurrentVersion;
                }

            }
            catch (DeploymentException exp)
            {
                dNow = System.DateTime.Now;
                textBox1.Text += "\r\n"+dNow.ToString("\n" + "[HH:mm:ss]") + " エラーが発生しました。\n"+exp.Message;
            }
            finally
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                button6.Enabled = true;
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            button7.Enabled = false;
            DateTime dNow = System.DateTime.Now;
            textBox1.Text = "" + dNow.ToString("[HH:mm:ss]") + " 更新プログラムを確認しています";
            progressBar1.Style = ProgressBarStyle.Marquee;
            await Task.Delay(1000);
            ApplicationDeployment currentDeploy = ApplicationDeployment.CurrentDeployment;
            currentDeploy.UpdateCompleted += new AsyncCompletedEventHandler(completed);
            dNow = System.DateTime.Now;
            textBox1.Text += "\r\n" + dNow.ToString("[HH:mm:ss]") + " 更新プログラムが見つかりました";
            dNow = System.DateTime.Now;
            textBox1.Text += "\r\n" + dNow.ToString("[HH:mm:ss]") + " 更新プログラムをインストールしています";
            dNow = System.DateTime.Now;
            textBox1.Text += "\r\n" + dNow.ToString("[HH:mm:ss]") + " この画面を閉じないでください";
            await Task.Delay(1000);
            try
            {
                currentDeploy.UpdateAsync();
            }
            catch (DeploymentException exp)
            {
                dNow = System.DateTime.Now;
                textBox1.Text += "\r\n" + dNow.ToString("\n" + "[HH:mm:ss]") + " エラーが発生しました。\n" + exp.Message;
            }
            finally
            {
                
            }
        }

        private void completed(object sender, AsyncCompletedEventArgs e)
        {
            DateTime dNow = System.DateTime.Now;
            textBox1.Text += "\r\n" + dNow.ToString("[HH:mm:ss]") + " 更新プログラムをインストールしました。再起動してください。";
            button8.Enabled = true;
            button7.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Blocks;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ApplicationDeployment currentDeploy = ApplicationDeployment.CurrentDeployment;
        }
        bool endflag1 = false;
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            endflag1 = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            LoadSettings();
        }

        private void propertyGrid8_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            form1.CloseFast();
        }

        private void commandLink1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void commandLink3_Click(object sender, EventArgs e)
        {
            form1.CloseFast();
        }

        private void commandLink2_Click(object sender, EventArgs e)
        {
            form1.error();
        }

        private void commandLink4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ExecutablePath, "-reset");
            Application.Exit();
        }
    }

    public class Property
    {
        [Category("確認事項")]
        [ReadOnly(true)]
        [Description("使い方のタブを確認してから使用してください。")]
        public string 確認事項
        {
            get { return ""; }
        }

        [Category("一般")]
        [Description("このプログラムの言語を設定します。GUIの設定の言語設定の選択肢以外の言語を使用する場合に設定してください。(言語ファイルは別に必要になります)")]
        [DefaultValue("Japanese")]
        public string language
        {
            get { return Properties.Settings.Default.Launguage; }
            set { Properties.Settings.Default.Launguage = value; }
        }

        [Category("一般")]
        [DefaultValue(false)]
        public bool textfileflag
        {
            get { return Properties.Settings.Default.textfileflag; }
            set { Properties.Settings.Default.textfileflag = value; }
        }

        [Category("一般")]
        public string[] fileext
        {
            get
            {
                StringCollection a = Properties.Settings.Default.fileext;
                return a.Cast<string>().ToArray();
            }
            set
            {
                StringCollection a = new StringCollection();
                a.AddRange(value);
                Properties.Settings.Default.fileext = a;
            }
        }

        [Category("一般")]
        [DefaultValue(false)]
        public bool betafuture
        {
            get { return Properties.Settings.Default.betafuture; }
            set { Properties.Settings.Default.betafuture = value; }
        }

        [Category("Windows Media Player ActiveX コントロール")]
        [Description("値の有効範囲: 0 ～ 100")]
        [DefaultValue(100)]
        public int firstvol
        {
            get { return Properties.Settings.Default.firstvol; }
            set { Properties.Settings.Default.firstvol = value; }
        }

        [Category("Windows Media Player ActiveX コントロール")]
        [DefaultValue(false)]
        public bool autoplay
        {
            get { return Properties.Settings.Default.autoplay; }
            set { Properties.Settings.Default.autoplay = value; }
        }

        [Category("詳細")]
        [Description("別ウィンドウで統計情報を表示します。表示/非表示の切り替えは再起動後に有効になります。")]
        [DefaultValue(false)]
        public bool EnableAdvancedInfo
        {
            get { return Properties.Settings.Default.EnableAdvancedInfo; }
            set { Properties.Settings.Default.EnableAdvancedInfo = value; }
        }

        [Category("描画 / 表示")]
        [Description("このプロパティを変更すると各種コントロールのサイズが変更される場合があります。この設定は再起動は不要です。")]
        public Font FormFont
        {
            get { return Properties.Settings.Default.FormFont; }
            set
            {
                Properties.Settings.Default.FormFont = value;
                Option.form1.Font = value;
            }
        }

        [Category("一般")]
        [Description("Windows Media Player ActiveX からのエラーメッセージを表示する")]
        [DefaultValue(false)]
        public bool EnableErrorDialogs
        {
            get { return Properties.Settings.Default.EnableErrorDialogs; }
            set { Properties.Settings.Default.EnableErrorDialogs = value; }
        }

        [Category("詳細")]
        [Description("不具合あり。False推奨。")]
        [DefaultValue(false)]
        public bool LoadingDialogEnable
        {
            get { return Properties.Settings.Default.LoadingDialogEnable; }
            set { Properties.Settings.Default.LoadingDialogEnable = value; }
        }

        [Category("一般")]
        [Description("設定ファイルのアップグレードが必要かを示すフラグ。True = 不要 False = 必要")]
        [DefaultValue(true)]
        public bool IsUpgrade
        {
            get { return Properties.Settings.Default.IsUpgrade; }
            set { Properties.Settings.Default.IsUpgrade = value; }
        }
    }
}