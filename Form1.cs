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
using SharpCaster;
using System.Collections.ObjectModel;
using SharpCaster.Models;
using SharpCaster.Services;
using SharpCaster.Controllers;
using niconicoviewer;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HongliangSoft.Utilities.Gui;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using Microsoft.WindowsAPICodePack.Taskbar;
using DiscordRPC;
using DiscordRPC.Logging;
using SharpPresence;

namespace Media_Player_SMP
{
    public partial class Form1 : Form
    {
        private static string RecoveryFile0 = "C:\\Temp\\RecoveryData0.txt";
        private static string RecoveryFile1 = "C:\\Temp\\RecoveryData1.txt";

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, UInt32 bRevert);

        [DllImport("USER32.DLL")]
        private static extern bool AppendMenu(IntPtr hMenu, UInt32 uFlags, UInt32 uIDNewItem, string lpNewItem);

        private const UInt32 MF_BYCOMMAND = 0x00000000;
        private const UInt32 MF_STRING = 0x00000000;
        private const UInt32 MF_SEPARATOR = 0x00000800;
        private const int WM_SYSCOMMAND = 0x112;
        public DiscordRpcClient DiscordRpcClient = new DiscordRpcClient("495186532903157760", true);
        public string version = "1.30 Pre-Alpha 1";

        public Form1()
        {
            InitializeComponent();
        }

        int recoveryresult = 0;
        string[] cmds = System.Environment.GetCommandLineArgs();

        string NowMedia = "";
        double NowTime = 0;

        public void CloseFast()
        {
            Environment.FailFast("わざと強制終了させた。");
        }

        private int RecoveryProcedure(object state)
        {
            File.WriteAllText(RecoveryFile0, NowMedia);
            File.WriteAllText(RecoveryFile1, NowTime.ToString());
            return 0;
        }

            void link3event(object sender, EventArgs e)
        {
            dialog.Close();
            Application.Exit();
        }

        void link2event(object sender, EventArgs e)
        {
            dialog.Close();
        }

        void link1event(object sender, EventArgs e)
        {
            dialog.Close();
            Thread.Sleep(2000);
            Properties.Settings.Default.Reset();
            TaskDialog dialog1 = new TaskDialog();
            dialog1.Caption = "確認";
            dialog1.InstructionText = "設定ファイルを初期化しました。";
            dialog1.Text = "設定ファイルを初期化しました。この後の動作を選択してください。";
            dialog1.Icon = TaskDialogStandardIcon.Information;

            var link1 = new TaskDialogCommandLink(
                            "link1", "起動を続行する",
                            "コマンドライン引数を引き継いでしまうため不具合が発生することがあります。");
            link1.Click += (sender1, e1) => dialog1.Close();
            dialog1.Controls.Add(link1);

            var link2 = new TaskDialogCommandLink(
                            "link2", "アプリケーションを終了する",
                            "一度アプリケーションを終了してからアプリケーションを再起動することをお勧めします。");
            link2.Click += (sender2, e2) => Application.Exit();
            link2.Default = true;
            dialog1.Controls.Add(link2);

            dialog1.Show();
        }

        DateTime nowtime = new DateTime();
        DateTime duration = new DateTime();


        public void UpgradeSettings(object sender, EventArgs e)
        {
            dialog2.Close();
            Thread.Sleep(5000);
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.IsUpgrade = true;
            Properties.Settings.Default.Save();
            progresswindow.Close();
            Application.Restart();
        }

        public void NoUpgrade(object sender, EventArgs e)
        {
            dialog2.Close();
            Properties.Settings.Default.IsUpgrade = true;
            Properties.Settings.Default.Save();
        }

        private int RecoverData(IntPtr pvParameter)
        {
            // データのリカバリの処理
            System.IO.File.WriteAllText(Environment.CurrentDirectory + @"recovery.log", "退避されたデータ");

            // 回復のキャンセルを確認する処理
            int i = 4;
            while (i != 0)
            {
                bool bCanceled;
                MMFrame.Windows.GlobalHook.VistaRestartRecoveryAPI.ApplicationRecoveryInProgress(out bCanceled);
                if (bCanceled)
                {
                    MessageBox.Show("キャンセルされました");
                    break;
                }
                System.Threading.Thread.Sleep(3000);
                i--;
            }

            MMFrame.Windows.GlobalHook.VistaRestartRecoveryAPI.ApplicationRecoveryFinished(true);
            return 0;
        }

        private MMFrame.Windows.GlobalHook.VistaRestartRecoveryAPI.ApplicationRecoveryCallback mCallback;

        TaskDialog dialog;
        TaskDialog dialog2;

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Media Player SMP "+version;
            mCallback = new MMFrame.Windows.GlobalHook.VistaRestartRecoveryAPI.ApplicationRecoveryCallback(this.RecoverData);
            IntPtr del = Marshal.GetFunctionPointerForDelegate(mCallback);
            MMFrame.Windows.GlobalHook.VistaRestartRecoveryAPI.RegisterApplicationRecoveryCallback(del, IntPtr.Zero, 5000, 1 | 4);
            try
            {
                if (cmds[1] == "-reset")
                {
                    if (TaskDialog.IsPlatformSupported == true)
                    {
                        dialog = new TaskDialog();

                        dialog.Caption = "確認";
                        dialog.InstructionText = "確認";
                        dialog.Text = "Media Player SMPの設定ファイルを初期化しますか？";
                        dialog.Icon = TaskDialogStandardIcon.Warning;

                        var link1 = new TaskDialogCommandLink(
                            "link1", "設定ファイルを初期化する",
                            "設定ファイルを初期化します。詳細設定のプロパティ編集などを変更し起動しなくなった場合に実行してください。");
                        link1.Click += new EventHandler(link1event);
                        dialog.Controls.Add(link1);

                        var link2 = new TaskDialogCommandLink(
                            "link2", "設定ファイルを初期化しない (起動を続行)",
                            "設定ファイルを初期化せずにアプリケーションの起動を続行します。");
                        link2.Click += new EventHandler(link2event);
                        dialog.Controls.Add(link2);

                        var link3 = new TaskDialogCommandLink(
                            "link3", "設定ファイルを初期化しない",
                            "設定ファイルを初期化せずにこのままアプリケーションを終了します。");
                        link3.Click += new EventHandler(link3event);
                        dialog.Controls.Add(link3);

                        dialog.Show();
                    }
                    else
                    {
                        if (MessageBox.Show("設定を初期化しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Thread.Sleep(2000);
                            Properties.Settings.Default.Reset();
                            MessageBox.Show("設定を初期化しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Application.Exit();
                        }

                    }
                }
            }
            catch (Exception)
            {

            }

            if (Properties.Settings.Default.IsUpgrade == false)
            {
                dialog2 = new TaskDialog();

                dialog2.Caption = "確認";
                dialog2.InstructionText = "設定ファイルをアップグレードしますか？";
                dialog2.Text = "アプリケーションが更新されたか設定ファイルが初期化された状態で起動しました。設定ファイルをアップグレードしますか？";
                dialog2.DetailsCollapsedLabel = "詳細情報 (上級者向け)";
                dialog2.DetailsExpandedLabel = "詳細情報を非表示";
                dialog2.DetailsExpandedText = "このアプリケーションの設定ファイルのIsUpgradeプロパティが初期値(False)の状態だったため、このダイアログを表示しています。\r\n詳細設定のプロパティ編集タブの設定ファイルタブにあるIsUpgradeプロパティを変更した場合もこのダイアログが表示されます。IsUpgradeプロパティを保持したい場合は、後で確認するを選択してください。\r\n設定ファイルをアップグレードする / 設定ファイルをアップグレードしないを選択した場合、IsUpgradeプロパティがTrueに設定されます。";
                dialog2.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;
                dialog2.Icon = TaskDialogStandardIcon.Information;

                var link1 = new TaskDialogCommandLink(
                            "link1", "設定ファイルをアップグレードする",
                            "前バージョンの設定ファイルを引き継ぎます。アプリケーションを更新した場合はこちらを選択してください。設定ファイルを初期化した状態で起動した場合はこのオプションを選択しないで下さい。");
                link1.Click += new EventHandler(UpgradeSettings);
                dialog2.Controls.Add(link1);

                var link2 = new TaskDialogCommandLink(
                            "link2", "設定ファイルをアップグレードしない",
                            "前バージョンの設定ファイルを引き継ぎません。設定ファイルを初期化した場合や、アプリケーションを初めて起動した場合にはこちらを選択してください。");
                link2.Click += new EventHandler(NoUpgrade);
                dialog2.Controls.Add(link2);

                var link3 = new TaskDialogCommandLink(
                            "link3", "後で確認する",
                            "次回の起動時にもう一度確認します。設定ファイルは初期化された状態で起動します。");
                link3.Click += (sender1, e1) => dialog2.Close();
                dialog2.Controls.Add(link3);

                var link4 = new TaskDialogCommandLink(
                    "link4", "アプリケーションを終了する", "何もせずにアプリケーションを終了します。次回の起動時にもう一度確認します。");
                link4.Click += (sender2, e2) => Application.Exit();
                dialog2.Controls.Add(link4);

                dialog2.Show();
            }

            Font = Properties.Settings.Default.FormFont;
            axWindowsMediaPlayer1.uiMode = "None";
            axWindowsMediaPlayer1.settings.volume = Properties.Settings.Default.firstvol;
            label4.Text = "音量 : " + Properties.Settings.Default.firstvol;
            trackBar2.Value = Properties.Settings.Default.firstvol;
            if (Properties.Settings.Default.autoplay == true)
            {
                axWindowsMediaPlayer1.settings.autoStart = true;
            }
            else
            {
                axWindowsMediaPlayer1.settings.autoStart = false;
            }
            try
            {
                if (cmds[1] != "" && cmds[1] != "-reset")
                {
                    axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(cmds[1]));
                    timer1.Start();
                }
            }
            catch (Exception)
            {

            }

            if (Properties.Settings.Default.betafuture == true)
            {
                chromeCastToolStripMenuItem.Visible = true;
            }

            if (Properties.Settings.Default.EnableAdvancedInfo == true)
            {
                AdvancedInfo a = new AdvancedInfo(this);
                a.Show();
            }

            if (Properties.Settings.Default.EnableErrorDialogs == true)
            {
                axWindowsMediaPlayer1.settings.enableErrorDialogs = true;
            }
            else
            {
                axWindowsMediaPlayer1.settings.enableErrorDialogs = false;
            }

            keyHook = new KeyboardHook();
            keyHook.KeyboardHooked += new KeyboardHookedEventHandler(keyHookProc);
            ServicePointManager.DefaultConnectionLimit = 24;

            DiscordRpcClient.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            DiscordRpcClient.Initialize();
            DiscordRpcClient.SetPresence(new RichPresence()
            {
                Details = "",
                State = "停止中 (00:00 / 00:00)",
                Assets = new Assets()
                {
                    LargeImageKey = "smpicon",
                    LargeImageText = "Media Player SMP " + version
                },
                Timestamps = new Timestamps()
                {
                    Start = nowtime
                }
            });
        }


        private void keyHookProc(object sender, KeyboardHookedEventArgs e)
        {
            
        }

        private static KeyboardHook keyHook;

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
                NowTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
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

        public string ip;
        ObservableCollection<Chromecast> chromecasts;

        public async void CastdeviceAsync(string ip2)
        {
            retry:
            String hostName = Dns.GetHostName();    // 自身のホスト名を取得
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            foreach (IPAddress address in addresses)
            {
                // IPv4 のみを追加する
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    // こんな感じで，コンボボックスに突っ込んだり
                    //MyIps.Items.Add(address);
                    ip = address.ToString();
                }
            }
            button7.Enabled = false;
            panel4.Visible = true;
            try
            {
                chromecasts.Clear();
            }
            catch
            {

            }
            chromecasts = new ObservableCollection<Chromecast>();
            if (ip == "")
            {
                StatusChange("Chromecast: キャストデバイスを探索しています... (IPアドレス:" + ip + ")");
            }
            else
            {
                StatusChange("Chromecast: キャストデバイスを探索しています... (IPアドレス:" + ip2 + ")");
            }
            listBox1.Items.Clear();
            listBox1.Items.Add("探索中...");
            if (ip == "")
            {
                chromecasts = await ChromecastService.Current.StartLocatingDevices(ip);
            }
            else
            {
                try
                {
                    chromecasts = await ChromecastService.Current.StartLocatingDevices(ip2);
                }
                catch
                {
                    if (MessageBox.Show("指定したIPアドレスは使用できません。", "エラー", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        goto retry;
                    }
                    else
                    {
                        panel4.Visible = false;
                        return;
                    }
                }
            }
            ip = ip2;
            listBox1.Items.Clear();
            foreach (var chromecast in chromecasts)
            {
                listBox1.Items.Add(chromecast.FriendlyName);
            }
            button7.Enabled = true;
        }
        progresswindow progresswindow = new progresswindow();
        private void axWindowsMediaPlayer1_OpenStateChange(object sender, AxWMPLib._WMPOCXEvents_OpenStateChangeEvent e)
        {
            progresswindow = new progresswindow();
            progresswindow.progressstyle = ProgressBarStyle.Marquee;
            progresswindow.text = "しばらくお待ちください";
            switch (e.newState)
            {
                case 12:
                    if (Properties.Settings.Default.LoadingDialogEnable == true)
                    {
                        progresswindow.text = "メディアを開いています";
                        if (progresswindow.Visible == false)
                        {
                            progresswindow.Show();
                        }
                    }
                    break;

                case 13:
                    if (progresswindow.Visible == true)
                    {
                        progresswindow.Close();
                    }
                    label3.Text = axWindowsMediaPlayer1.currentMedia.durationString;
                    int duration = (int)axWindowsMediaPlayer1.currentMedia.duration;
                    trackBar1.Maximum = duration;
                    break;

                case 14:
                    if (Properties.Settings.Default.LoadingDialogEnable == true)
                    {
                        progresswindow.text = "コーデックを取得しています";
                        if (progresswindow.Visible == false)
                        {
                            progresswindow.Show();
                        }
                    }
                    break;

                case 15:
                    if (Properties.Settings.Default.LoadingDialogEnable == true)
                    {
                        progresswindow.text = "コーデックを取得しました";
                        if (progresswindow.Visible == false)
                        {
                            progresswindow.Show();
                        }
                    }
                    break;

                case 20:
                    if (Properties.Settings.Default.LoadingDialogEnable == true)
                    {
                        progresswindow.text = "メディアを待機中です";
                        if (progresswindow.Visible == false)
                        {
                            progresswindow.Show();
                        }
                    }
                    break;

                case 21:
                    if (Properties.Settings.Default.LoadingDialogEnable == true)
                    {
                        progresswindow.text = axWindowsMediaPlayer1.currentMedia.sourceURL+"\r\nURLに接続しています";
                        if (progresswindow.Visible == false)
                        {
                            progresswindow.Show();
                        }
                    }
                    break;

                default:
                    if (progresswindow.Visible == true)
                    {
                        progresswindow.Close();
                    }
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
        public enum Language { Japanese, English }
        public async void StatusChange(string status)
        {
            if (label5.Visible == true)
            {
                flag1++;
            }
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

            if (flag1 == 0)
            {
                label5.Visible = false;
            }
            else
            {
                flag1--;
            }
        }

        int flag1 = 0;

        void hookKeyboardTest(ref MMFrame.Windows.GlobalHook.KeyboardHook.StateKeyboard s)
        {
            switch (s.Key)
            {
                case Keys.Play:
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    break;
            }
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
            NowMedia = axWindowsMediaPlayer1.currentMedia.sourceURL;
            if (panel3.Visible == true)
            {
                StatusChange("テキストファイル セッション を閉じました。");
            }
            panel3.Visible = false;
            label1.Text = axWindowsMediaPlayer1.currentMedia.name;
            label7.Text = axWindowsMediaPlayer1.currentMedia.sourceURL;
            label8.Text = axWindowsMediaPlayer1.currentMedia.name;
            string Extension = Path.GetExtension(axWindowsMediaPlayer1.currentMedia.sourceURL);
            if (Extension == ".mp3" || Extension == ".MP3" || Extension == ".wma" || Extension == ".WMA" || Extension == ".m4a" || Extension == ".M4A" || Extension == ".wav" || Extension == ".WAV")
            {
                panel2.Visible = true;
                checkBox1.Enabled = true;
                trackBar2.Enabled = true;
                axWindowsMediaPlayer1.settings.mute = checkBox1.Checked;
                axWindowsMediaPlayer1.settings.volume = trackBar2.Value;
                label9.Visible = false;
            }
            else if (Extension == ".mp3" || Extension == "MP3")
            {
            }
            else if (Extension == ".mid" || Extension == ".MID")
            {
                panel2.Visible = true;
                checkBox1.Enabled = false;
                trackBar2.Enabled = false;
                MidiSequence midi = new MidiSequence(axWindowsMediaPlayer1.currentMedia.sourceURL);
                label9.Text = "タイトル: " + midi.Title + "\n\n著作権: " + midi.Copyright + "\n\nトラック: " + midi.Tracks;
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


            if (Path.GetExtension(axWindowsMediaPlayer1.currentMedia.sourceURL) == ".mp3" || Path.GetExtension(axWindowsMediaPlayer1.currentMedia.sourceURL) == ".MP3")
            {
                TagLib.File id3 = TagLib.File.Create(axWindowsMediaPlayer1.currentMedia.sourceURL);
                string artists;
                artists = "";
                foreach (string artist in id3.Tag.Artists)
                {
                    artists = artist + " ";
                }

                string albumartists;
                albumartists = "";
                foreach (string albumartist in id3.Tag.AlbumArtists)
                {
                    albumartists = albumartist + " ";
                }

                string genres;
                genres = "";
                foreach (string genre in id3.Tag.Genres)
                {
                    genres = genre + " ";
                }

                string track;
                track = "";
                if (id3.Tag.TrackCount == 0)
                {
                    track = id3.Tag.Track.ToString();
                }
                else
                {
                    track = id3.Tag.Track + " / " + id3.Tag.TrackCount;
                }
                if (id3.Tag.DiscCount == 0)
                {
                    track = id3.Tag.Disc.ToString();
                }
                else
                {
                    track = id3.Tag.Disc + " / " + id3.Tag.DiscCount;
                }

                label9.Text = "タイトル: " + id3.Tag.Title.ToString() + "\nトラック番号: " + id3.Tag.Track + "\nディスク番号: " + id3.Tag.Disc + "\nアーティスト: " + artists + "\nアルバム: " + id3.Tag.Album + "\nアルバムアーティスト: " + albumartists + "\nジャンル: " + genres + "\nリリース年: " + id3.Tag.Year + "\n著作権: " + id3.Tag.Copyright;
                label9.Visible = true;
            }
            else
            {
                label9.Visible = false;
            }
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
            if (MessageBox.Show("リストを削除しますか？この操作はもとに戻せません。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
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
            label6.Text = "バッファ中... (" + axWindowsMediaPlayer1.network.bufferingProgress + "%)";
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
                StatusChange(openFileDialog2.FileName + " を読み込みました。");
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
            Option f = new Option(this);
            if (this.TopMost == true)
            {
                f.TopMost = true;
            }
            else
            {
                f.TopMost = false;
            }
            f.ShowDialog(this);
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
                else
                {
                    maxflag = false;
                }
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                timer3.Enabled = true;
                StatusChange("フルスクリーン表示に切り替えました。Ctrl(Control)キー+Altキー+Fキーで元に戻ります。またマウスを一番上や一番下に合わせるとメニューやシークバーが表示されます。");
                menuStrip1.Visible = false;
                panel1.Visible = false;
                フルウィンドウWToolStripMenuItem.Enabled = false;
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
                フルウィンドウWToolStripMenuItem.Enabled = true;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            Point formClientCurPos = this.PointToClient(Cursor.Position);

            if (formClientCurPos.Y == 0)
            {
                menuStrip1.Visible = true;
            }
            else
            {
                if (formClientCurPos.Y >= 25)
                {
                    menuStrip1.Visible = false;
                }
            }
            if ((Size.Height - 1) <= formClientCurPos.Y)
            {
                panel1.Visible = true;
            }
            if ((Size.Height - 98) >= formClientCurPos.Y)
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
                p.text = "ファイルの読み込み中です。( " + p.progressvalue + " / " + p.progressmax + " )\n" + file1;
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
            /*
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
            */
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                var directoryPath = dialog.FileName;
                p.Show();
                p.progressstyle = ProgressBarStyle.Continuous;
                p.progressvalue = 0;
                p.text = "ファイルの読み込み中です。( " + p.progressvalue + " / " + p.progressmax + " )";

                try
                {
                    p.progressmax = Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).Length;
                    string[] a = { "" };
                    a[0] = directoryPath;
                    Add(a);
                }
                catch
                {
                    errflag = true;
                    errtext += "読み込みに不明なエラーが発生しました。" + folderBrowserDialog1.SelectedPath + "\n";
                }

            }
        }

        public void error()
        {
            throw new Exception("テスト");
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

        private void button6_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
        }

        private void キャストCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CastdeviceAsync("");
        }

        private async void button7_ClickAsync(object sender, EventArgs e)
        {
            button6.Enabled = false;
            button7.Enabled = false;
            StatusChange("Chromecast: " + listBox1.SelectedItem + "に接続中...");
            ObservableCollection<Chromecast> chromecasts = await ChromecastService.Current.StartLocatingDevices(ip);
            if (listBox1.SelectedIndex != -1)
            {
                return;
            }
            var chromecast = chromecasts[listBox1.SelectedIndex];
            SharpCasterDemoController _controller = null;
            ChromecastService.Current.ChromeCastClient.ConnectedChanged += async delegate { if (_controller == null) _controller = await ChromecastService.Current.ChromeCastClient.LaunchSharpCaster(); };
            ChromecastService.Current.ChromeCastClient.ApplicationStarted +=
            async delegate
            {
                while (_controller == null)
                {
                    await Task.Delay(500);
                }

                await _controller.LoadMedia(axWindowsMediaPlayer1.currentMedia.sourceURL, "video/mp4", null, SharpCaster.Models.ChromecastRequests.StreamType.BUFFERED);
            };
            await ChromecastService.Current.ConnectToChromecast(chromecast);
            panel4.Visible = false;
        }

        private void iPアドレスを指定してキャストToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CastdeviceAsync(toolStripTextBox2.Text);
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (progresswindow.Visible == true)
            {
                if (axWindowsMediaPlayer1.openState == WMPLib.WMPOpenState.wmposMediaOpen || axWindowsMediaPlayer1.openState == WMPLib.WMPOpenState.wmposPlaylistOpenNoMedia)
                    progresswindow.Close();
            }
        }

        private void Form1_MouseHover(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Show();
        }

        private void マウスカーソルを非表示HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (マウスカーソルを非表示HToolStripMenuItem.Checked == true)
            {
                Cursor.Show();
                マウスカーソルを非表示HToolStripMenuItem.Checked = false;
            }
            else
            {
                Cursor.Hide();
                マウスカーソルを非表示HToolStripMenuItem.Checked = true;
            }
        }

        private void フルウィンドウWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuStrip1.Visible = false;
            panel1.Visible = false;
            StatusChange("フルウィンドウモードに切り替えました。Escキーで戻ります。");
            フルウィンドウWToolStripMenuItem.Checked = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (フルウィンドウWToolStripMenuItem.Checked == true && e.KeyCode == Keys.Escape)
            {
                menuStrip1.Visible = true;
                panel1.Visible = true;
                フルウィンドウWToolStripMenuItem.Checked = false;
            }
        }

        public static double ToRoundDown(double dValue, int iDigits)
        {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? System.Math.Floor(dValue * dCoef) / dCoef :
                                System.Math.Ceiling(dValue * dCoef) / dCoef;
        }

        private void downloadClient_DownloadProgressChanged(object sender,
            DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("{0}% ({1}byte 中 {2}byte) ダウンロードが終了しました。",
                e.ProgressPercentage, e.TotalBytesToReceive, e.BytesReceived);

            long totalbyte = 0;
            string totalbyte1 = "";
            long flag02 = 0;

            if (e.BytesReceived >= 1000)
            {
                totalbyte = e.BytesReceived / 1024;
                totalbyte1 = totalbyte.ToString() + " KB";
            }
            if (e.BytesReceived >= 1048576)
            {
                totalbyte = e.BytesReceived / 1048576;
                totalbyte1 = totalbyte.ToString() + " MB";
            }
            flag02 = e.BytesReceived / 1048576;
            if (e.BytesReceived <= 1000)
            {
                totalbyte1 = e.BytesReceived + " B";
            }

            long now = 0;
            string nowbyte = "";
            long flag01 = 0;

            if (e.TotalBytesToReceive >= 1024)
            {
                now = (e.TotalBytesToReceive / 1024);
                nowbyte = now + " KB";
            }
            if (e.TotalBytesToReceive >= 1048576)
            {
                now = e.TotalBytesToReceive / 1048576;
                nowbyte = now + " MB";
            }
            flag01 = e.TotalBytesToReceive / 1048576;
            if (e.TotalBytesToReceive <= 1000)
            {
                now = e.TotalBytesToReceive;
                nowbyte = now + " B";
            }
            progresswindow1.progressstyle = ProgressBarStyle.Continuous;
            if (flag == false)
            {
                try
                {
                    progresswindow1.progressmax = (int)e.TotalBytesToReceive;
                    progresswindow1.progressvalue = (int)e.BytesReceived;
                    progresswindow1.SetTProgressValue((int)e.BytesReceived, (int)e.TotalBytesToReceive);
                }
                catch
                {
                    flag = true;
                }
            }
            else
            {
                progresswindow1.progressmax = 100;
                progresswindow1.progressvalue = e.ProgressPercentage;
                progresswindow1.SetTProgressValue(e.ProgressPercentage, 100);
            }
            if (progresswindow1.progressmax != e.TotalBytesToReceive)
            {
                flag = true;
            }

            string str1 = "ダウンロード中: " + e.ProgressPercentage + "% " + totalbyte1 + " / " + nowbyte + " (" + e.BytesReceived + "Bytes / " + e.TotalBytesToReceive + "Bytes)";
            string str2 = "";
            if (str1.Length > 128)
            {
                str2 = str1.Remove(125) + "...";
            }
            else
            {
                str2 = str1;
            }

            string str3 = downloaduri.ToString();
            string str4 = "";
            if (str3.Length > 128)
            {
                str4 = str3.Remove(125) + "...";
            }
            else
            {
                str4 = str3;
            }

            DiscordRpcClient.SetPresence(new RichPresence()
                {
                    Details = str4,
                    State = str2,
                    Assets = new Assets()
                    {
                        LargeImageKey = "smpicon",
                        LargeImageText = "Media Player SMP " + version
                    }
                });

            progresswindow1.text = "ダウンロード中: " + downloaduri.ToString() + "\r\n"+e.ProgressPercentage+"% "+totalbyte1+" / "+nowbyte+" ("+e.BytesReceived+" Bytes / "+e.TotalBytesToReceive+" Bytes)";
        }

        private void downloadClient_DownloadFileCompleted(object sender,
             AsyncCompletedEventArgs e)
        {
            timer5.Enabled = true;
            progresswindow1.Close();
            progresswindow1.TaskbarprogressBarState = TaskbarProgressBarState.NoProgress;
            if (e.Cancelled == false)
            {
                axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia("Temp" + Path.GetExtension(downloaduri.ToString())));
                StatusChange("ダウンロード終了: " + downloaduri);
            }
            else
            {
                StatusChange("ダウンロードキャンセル: " + downloaduri);
            }
            flag = false;
            ダウンロードをキャンセルCToolStripMenuItem.Enabled = false;
            toolStripMenuItem6.Enabled = true;
        }

        bool flag = false;
        progresswindow progresswindow1 = new progresswindow();
        Uri downloaduri = null;
        WebClient downloadClient = null;

        public void filedownloadtemp(Uri uri)
        {
            progresswindow1 = new progresswindow();
            StatusChange("ダウンロード開始: "+uri);
            downloaduri = uri;

            toolStripMenuItem6.Enabled = false;
            ダウンロードをキャンセルCToolStripMenuItem.Enabled = true;

            progresswindow1.text = "ダウンロード中: "+uri.ToString()+"";
            progresswindow1.progressstyle = ProgressBarStyle.Marquee;
            progresswindow1.progressvalue = 0;
            progresswindow1.progressmax = 0;
            progresswindow1.progressvisible = true;
            progresswindow1.SetTProgressValue(0, 0);
            progresswindow1.TaskbarprogressBarState = TaskbarProgressBarState.Normal;
            progresswindow1.Show();

            timer5.Enabled = false;

            downloadClient = null;

            Uri u = uri;

            //WebClientの作成
            if (downloadClient == null)
            {
                downloadClient = new WebClient();
                //イベントハンドラの作成
                downloadClient.DownloadProgressChanged +=
                    new DownloadProgressChangedEventHandler(
                        downloadClient_DownloadProgressChanged);
                downloadClient.DownloadFileCompleted +=
                    new AsyncCompletedEventHandler(
                        downloadClient_DownloadFileCompleted);
            }
            //非同期ダウンロードを開始する
            downloadClient.DownloadFileAsync(u, "Temp"+Path.GetExtension(uri.ToString()));
        }

        public void SetThread(int thread)
        {
            ServicePointManager.DefaultConnectionLimit = thread;
        }

        public int GetThread()
        {
            return ServicePointManager.DefaultConnectionLimit;
        }

        [System.Security.Permissions.UIPermission(System.Security.Permissions.SecurityAction.Demand,Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //左キーが押されているか調べる
            if ((keyData & Keys.KeyCode) == Keys.Escape)
            {
                if (フルウィンドウWToolStripMenuItem.Checked == true)
                {
                    menuStrip1.Visible = true;
                    panel1.Visible = true;
                    フルウィンドウWToolStripMenuItem.Checked = false;
                }
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void 再生速度の変更SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpeedForm speedForm = new SpeedForm(this);
            speedForm.Show();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            OpenURLWindow openURLWindow = new OpenURLWindow(this);
            openURLWindow.ShowDialog();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // 再起動用コードの登録
            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(
                    new RestartSettings("-restart",
                    RestartRestrictions.NotOnReboot | RestartRestrictions.NotOnPatch));

            // 修復用メソッドの登録
            RecoveryData data = new RecoveryData(new RecoveryCallback(RecoveryProcedure), null);
            RecoverySettings settings = new RecoverySettings(data, 0);
            ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(settings);

            // 起動時の再起動かどうかの判断
            // 再起動時にデータを修復するコードの作成
            if (System.Environment.GetCommandLineArgs().Length > 1 &&
                System.Environment.GetCommandLineArgs()[1] == "-restart")
            {
                if (File.Exists(RecoveryFile0) == true && File.Exists(RecoveryFile1) == true)
                {
                    try
                    {
                        axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(File.ReadAllText(RecoveryFile0)));
                        axWindowsMediaPlayer1.Ctlcontrols.currentPosition = double.Parse(File.ReadAllText(RecoveryFile1));
                        recoveryresult = 1;
                    }
                    catch
                    {
                        recoveryresult = 2;
                    }
                }
            }
            switch (recoveryresult)
            {
                case 0:
                    break;

                case 1:
                    StatusChange("強制終了時の状態を自動的に復元しました。");
                    break;

                case 2:
                    StatusChange("強制終了時の状態の一部または全てを復元することができませんでした。");
                    break;
            }
        }

        private void ダウンロードをキャンセルCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            downloadClient.CancelAsync();
        }

        private void ダウンロードスレッド数の変更SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            downloadthread downloadthread = new downloadthread(this);
            downloadthread.Show();
        }

        private void バージョン情報AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Media Player SMP "+version+"\r\n"+ "By kizuki1749\r\n\r\nCopyright © 2017-2018 kizuki1749 All Rights Reserved.","バージョン情報",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (string file in Directory.GetFiles(".", "Temp.*"))
                {
                    DeleteFile(file);
                }
                StatusChange("成功: テンポラリファイルを削除しました。");
            }
            catch
            {
                StatusChange("失敗: テンポラリファイルを削除できませんでした。");
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     指定したファイルを削除します。</summary>
        /// <param name="stFilePath">
        ///     削除するファイルまでのパス。</param>
        /// -----------------------------------------------------------------------------
        public static void DeleteFile(string stFilePath)
        {
            System.IO.FileInfo cFileInfo = new System.IO.FileInfo(stFilePath);

            // ファイルが存在しているか判断する
            if (cFileInfo.Exists)
            {
                // 読み取り専用属性がある場合は、読み取り専用属性を解除する
                if ((cFileInfo.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
                {
                    cFileInfo.Attributes = System.IO.FileAttributes.Normal;
                }

                // ファイルを削除する
                cFileInfo.Delete();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string file in Directory.GetFiles(".", "Temp.*"))
                {
                    DeleteFile(file);
                }
                StatusChange("成功: テンポラリファイルを削除しました。");
            }
            catch
            {
                StatusChange("失敗: テンポラリファイルを削除できませんでした。");
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            string str1 = "";
            try
            {
                str1 = axWindowsMediaPlayer1.currentMedia.name;

                if (Path.GetExtension(axWindowsMediaPlayer1.currentMedia.sourceURL) == ".mp3" || Path.GetExtension(axWindowsMediaPlayer1.currentMedia.sourceURL) == ".MP3")
                {
                    TagLib.File id3 = TagLib.File.Create(axWindowsMediaPlayer1.currentMedia.sourceURL);
                    string artists;
                    artists = "";
                    foreach (string artist in id3.Tag.Artists)
                    {
                        artists = artist + " ";
                    }

                    string albumartists;
                    albumartists = "";
                    foreach (string albumartist in id3.Tag.AlbumArtists)
                    {
                        albumartists = albumartist + " ";
                    }

                    string genres;
                    genres = "";
                    foreach (string genre in id3.Tag.Genres)
                    {
                        genres = genre + " ";
                    }

                    string track;
                    track = "";
                    if (id3.Tag.TrackCount == 0)
                    {
                        track = id3.Tag.Track.ToString();
                    }
                    else
                    {
                        track = id3.Tag.Track + " / " + id3.Tag.TrackCount;
                    }
                    if (id3.Tag.DiscCount == 0)
                    {
                        track = id3.Tag.Disc.ToString();
                    }
                    else
                    {
                        track = id3.Tag.Disc + " / " + id3.Tag.DiscCount;
                    }
                    str1 += " (" + artists + " / " + id3.Tag.Album + " / トラック番号: " + id3.Tag.Track + " / " + genres + " / " + id3.Tag.Year + ")";
                }

            }
            catch
            {
                str1 = "";
            }
            string str2 = "";
            if (str1.Length > 128)
            {
                str2 = str1.Remove(125) + "...";
            }
            else
            {
                str2 = str1;
            }
            try
            {
                switch (axWindowsMediaPlayer1.playState)
                {
                    case WMPLib.WMPPlayState.wmppsPlaying:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                        }
                        else
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsScanForward:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                if (axWindowsMediaPlayer1.settings.rate == 1.0)
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                                else
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsScanReverse:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                if (axWindowsMediaPlayer1.settings.rate == 1.0)
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                                else
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;


                    case WMPLib.WMPPlayState.wmppsStopped:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsReady:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsPaused:
                        DiscordRpcClient.SetPresence(new RichPresence()
                        {
                            Details = str2,
                            State = "一時停止中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                            Assets = new Assets()
                            {
                                LargeImageKey = "smpicon",
                                LargeImageText = "Media Player SMP " + version
                            }
                        });
                        break;

                    case WMPLib.WMPPlayState.wmppsBuffering:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "バッファ中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "バッファ中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsUndefined:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = "",
                                State = "停止中 (00:00 / 00:00)",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = "",
                                State = "停止中 (00:00 / 00:00)",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;
                }
            }
            catch
            {
                string str3 = "";
                string str4 = "";
                str3 = axWindowsMediaPlayer1.currentMedia.name;
                if (str1.Length > 128)
                {
                    str4 = str1.Remove(125) + "...";
                }
                else
                {
                    str4 = str3;
                }
                str2 = str4;

                switch (axWindowsMediaPlayer1.playState)
                {
                    case WMPLib.WMPPlayState.wmppsPlaying:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                        }
                        else
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsScanForward:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                if (axWindowsMediaPlayer1.settings.rate == 1.0)
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                                else
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsScanReverse:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            if (axWindowsMediaPlayer1.settings.rate == 1.0)
                            {
                                DiscordRpcClient.SetPresence(new RichPresence()
                                {
                                    Details = str2,
                                    State = "再生中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = "smpicon",
                                        LargeImageText = "Media Player SMP " + version
                                    }
                                });
                            }
                            else
                            {
                                if (axWindowsMediaPlayer1.settings.rate == 1.0)
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                                else
                                {
                                    DiscordRpcClient.SetPresence(new RichPresence()
                                    {
                                        Details = str2,
                                        State = "再生中(" + axWindowsMediaPlayer1.settings.rate + "倍速) (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "smpicon",
                                            LargeImageText = "Media Player SMP " + version
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "再生中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;


                    case WMPLib.WMPPlayState.wmppsStopped:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsReady:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "停止中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsPaused:
                        DiscordRpcClient.SetPresence(new RichPresence()
                        {
                            Details = str2,
                            State = "一時停止中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                            Assets = new Assets()
                            {
                                LargeImageKey = "smpicon",
                                LargeImageText = "Media Player SMP " + version
                            }
                        });
                        break;

                    case WMPLib.WMPPlayState.wmppsBuffering:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "バッファ中 (00:00 / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = str2,
                                State = "バッファ中 (" + axWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + axWindowsMediaPlayer1.currentMedia.durationString + ")",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;

                    case WMPLib.WMPPlayState.wmppsUndefined:
                        if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString == "")
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = "",
                                State = "停止中 (00:00 / 00:00)",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        else
                        {
                            DiscordRpcClient.SetPresence(new RichPresence()
                            {
                                Details = "",
                                State = "停止中 (00:00 / 00:00)",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "smpicon",
                                    LargeImageText = "Media Player SMP " + version
                                }
                            });
                        }
                        break;
                }
            }
        }
    }
}

namespace VincaNote.MouseKeyBoadHook
{

    /// <summary>
    /// マウスの操作
    /// </summary>
    public enum MouseMessage
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_MBUTTONDOWN = 0x207,
        WM_MBUTTONUP = 0x208,
        WM_MBUTTONDBLCLK = 0x209,
        WM_MOUSEMOVE = 0x200,
        WM_MOUSEWHEEL = 0x20A,
        WM_XBUTTONDOWN = 0x20B,
        WM_XBUTTONUP = 0x20C
    }

    /// <summary>
    /// キーボードの操作
    /// </summary>
    public enum KeyBoadMessage
    {
        WM_KEYDOWN = 0x100,
        WM_KEYUP = 0x0101,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105
    }

    /// <summary>
    /// フックタイプ
    /// </summary>
    [Flags]
    public enum HookType
    {
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    /// <summary>
    /// 点のx座標とy軸座標定義
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// 低レベルのマウス入力イベントに関する情報
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    /// <summary>
    /// 低レベルのキーボード入力イベントに関する情報
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    public class MouseKeyBoadHook : IDisposable
    {
        /// <summary>
        /// フック用デリゲート
        private delegate int HookDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        private IntPtr mouseHook = IntPtr.Zero;
        private IntPtr keyboadHook = IntPtr.Zero;
        private GCHandle mouseHookHandle;
        private GCHandle keyboadHookHandle;

        /// <summary>
        /// キーボードフックイベント
        /// </summary>
        public event EventHandler<KeyBoadHookEventArgs> KeyBoadHooked;
        protected virtual void OnKeyBoadHookEvent(KeyBoadHookEventArgs e)
        {
            EventHandler<KeyBoadHookEventArgs> handler = KeyBoadHooked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// マウスフックイベント
        /// </summary>
        public event EventHandler<MouseHookEventArgs> MouseHooked;
        protected virtual void OnMouseHookEvent(MouseHookEventArgs e)
        {
            EventHandler<MouseHookEventArgs> handler = MouseHooked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// マウスフック状態
        /// </summary>
        public bool IsMouseHook { get; set; }

        /// <summary>
        /// キーボードフック状態
        /// </summary>
        public bool IsKeyboadHook { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MouseKeyBoadHook()
        {
            this.IsMouseHook = false;
            this.IsKeyboadHook = false;
        }

        /// <summary>
        /// フック登録
        /// </summary>
        /// <param name="type"></param>
        public void SetHook(HookType type)
        {

            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule module = process.MainModule)
            {
                // マウスフック登録
                if (type.HasFlag(HookType.WH_MOUSE_LL))
                {
                    HookDelegate mouseHook = new HookDelegate(MouseHookProc);
                    this.mouseHookHandle = GCHandle.Alloc(mouseHook);
                    this.mouseHook = NativeMethods.SetWindowsHookEx((int)HookType.WH_MOUSE_LL,
                                                                    Marshal.GetFunctionPointerForDelegate(mouseHook),
                                                                    NativeMethods.GetModuleHandle(module.ModuleName),
                                                                    0);

                    if (this.mouseHook.Equals(IntPtr.Zero))
                    {
                        int hResult = Marshal.GetHRForLastWin32Error();
                        Marshal.ThrowExceptionForHR(hResult);
                    }

                    this.IsMouseHook = true;
                }

                // キーボードフック登録
                if (type.HasFlag(HookType.WH_KEYBOARD_LL))
                {
                    HookDelegate keyboadHook = new HookDelegate(KeyBoadHookProc);
                    this.keyboadHookHandle = GCHandle.Alloc(keyboadHook);
                    this.keyboadHook = NativeMethods.SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL,
                                                                    Marshal.GetFunctionPointerForDelegate(keyboadHook),
                                                                    NativeMethods.GetModuleHandle(module.ModuleName),
                                                                    0);
                    if (this.keyboadHook.Equals(IntPtr.Zero))
                    {
                        int hResult = Marshal.GetHRForLastWin32Error();
                        Marshal.ThrowExceptionForHR(hResult);
                    }

                    this.IsKeyboadHook = true;
                }
            }
        }

        /// <summary>
        /// フック解除
        /// </summary>
        /// <param name="type"></param>
        public void UnSetHook(HookType type)
        {
            if (type.HasFlag(HookType.WH_MOUSE_LL) && this.mouseHook != IntPtr.Zero)
            {
                this.IsMouseHook = false;
                NativeMethods.UnhookWindowsHookEx(this.mouseHook);
                this.mouseHook = IntPtr.Zero;
                if (this.mouseHookHandle.IsAllocated)
                {
                    this.mouseHookHandle.Free();
                }
            }

            if (type.HasFlag(HookType.WH_KEYBOARD_LL) && this.keyboadHook != IntPtr.Zero)
            {
                this.IsKeyboadHook = false;
                NativeMethods.UnhookWindowsHookEx(this.keyboadHook);
                this.keyboadHook = IntPtr.Zero;
                if (this.keyboadHookHandle.IsAllocated)
                {
                    this.keyboadHookHandle.Free();
                }
            }
        }
        /// <summary>
        /// デストラクタ
        /// </summary>
        ~MouseKeyBoadHook()
        {
            Dispose(false);
        }

        /// <summary>
        /// マウスフック
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT msllhook;
                MouseHookEventArgs e;
                msllhook = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                e = new MouseHookEventArgs((MouseMessage)wParam.ToInt32(), msllhook);
                OnMouseHookEvent(e);
                if (e.Cancel)
                {
                    return -1;
                }
            }
            return NativeMethods.CallNextHookEx(this.mouseHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// キーボードフック
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyBoadHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT kbllhook;
                KeyBoadHookEventArgs e;
                kbllhook = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                e = new KeyBoadHookEventArgs((KeyBoadMessage)wParam.ToInt32(), kbllhook);
                OnKeyBoadHookEvent(e);
                if (e.Cancel)
                {
                    return -1;
                }
            }
            return NativeMethods.CallNextHookEx(this.keyboadHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// Disposeの実装
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //マネージオブジェクト開放
            }

            //アンマネージオブジェクト開放
            UnSetHook(HookType.WH_KEYBOARD_LL | HookType.WH_MOUSE_LL);
            disposed = true;
        }

        internal static class NativeMethods
        {
            /// <summary>
            /// アプリケーション定義のフックプロシージャをフックチェーン内にインストールします。
            /// フックプロシージャをインストールすると、特定のイベントタイプを監視できます。
            /// 監視の対象になるイベントは、特定のスレッド、または呼び出し側スレッドと同じデスクトップ内のすべてのスレッドに関連付けられているものです。
            /// </summary>
            /// <param name="idHook"></param>
            /// <param name="lpfn"></param>
            /// <param name="hInstance"></param>
            /// <param name="threadId"></param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hInstance, int threadId);

            /// <summary>
            /// SetWindowsHookEx 関数を使ってフックチェーン内にインストールされたフックプロシージャを削除します
            /// </summary>
            /// <param name="hhk"></param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            /// <summary>
            /// 現在のフックチェーン内の次のフックプロシージャに、フック情報を渡します。
            /// フックプロシージャは、フック情報を処理する前でも、フック情報を処理した後でも、この関数を呼び出せます。
            /// </summary>
            /// <param name="idHook"></param>
            /// <param name="nCode"></param>
            /// <param name="wParam"></param>
            /// <param name="lParam"></param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// 呼び出し側プロセスのアドレス空間に該当ファイルがマップされている場合、指定されたモジュール名のモジュールハンドルを返します。
            /// </summary>
            /// <param name="lpModuleName"></param>
            /// <returns></returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(String lpModuleName);
        }
    }

    /// <summary>
    /// MouseHookに関するイベント情報
    /// </summary>
    public class MouseHookEventArgs : CancelEventArgs
    {
        private MouseMessage message;
        private int posX;
        private int posY;
        private uint flags;
        private int delta;
        private int xButton;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="keyhook"></param>
        internal MouseHookEventArgs(MouseMessage msg, MSLLHOOKSTRUCT mousuHook)
        {
            this.message = msg;
            this.flags = mousuHook.flags;
            this.posX = mousuHook.pt.x;
            this.posY = mousuHook.pt.y;
            this.delta = 0;

            switch (msg)
            {
                case MouseMessage.WM_MOUSEWHEEL:
                    this.delta = (short)(mousuHook.mouseData >> 16);
                    break;
                case MouseMessage.WM_XBUTTONDOWN:
                case MouseMessage.WM_XBUTTONUP:
                    xButton = (short)(mousuHook.mouseData >> 16);
                    break;
            }
        }

        /// <summary>
        /// マウス操作
        /// </summary>
        public MouseMessage Message { get { return this.message; } }
        /// <summary>
        /// X座標
        /// </summary>
        public int X { get { return this.posX; } }
        /// <summary>
        /// Y座標
        /// </summary>
        public int Y { get { return this.posY; } }
        /// <summary>
        /// Xボタン1
        /// </summary>
        public bool IsXButton1 { get { return xButton.Equals(0x0001); } }
        /// <summary>
        /// Xボタン2
        /// </summary>
        public bool IsXButton2 { get { return xButton.Equals(0x0002); } }
        /// <summary>
        /// ホイール回転量
        /// </summary>
        public int WheelDelta { get { return this.delta; } }

        /// <summary>
        /// イベントがインジェクトされたかどうか
        /// </summary>
        public bool IsInjected { get { return ((this.flags & 0x0001) != 0); } }
        /// <summary>
        /// 低い整合性レベルで実行中のプロセスからイベントがインジェクトされたかどうか
        /// </summary>
        public bool IsLowerInjected { get { return ((this.flags & 0x0002) != 0); } }

    }

    /// <summary>
    /// KeyboadHookに関するイベント情報
    /// </summary>
    public class KeyBoadHookEventArgs : CancelEventArgs
    {
        private KeyBoadMessage message;
        private uint flags;
        private uint vkCode;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="keyhook"></param>
        internal KeyBoadHookEventArgs(KeyBoadMessage msg, KBDLLHOOKSTRUCT keyhook)
        {
            this.message = msg;
            this.flags = keyhook.flags;
            this.vkCode = keyhook.vkCode;
        }

        /// <summary>
        /// 仮想キーコード
        /// </summary>
        public uint VirtualKeyCode { get { return this.vkCode; } }
        /// <summary>
        /// キーボード操作
        /// </summary>
        public KeyBoadMessage Message { get { return this.message; } }
        /// <summary>
        /// キーがファンクションキーや数値キーパッド上のキーなどの拡張キーかどうか
        /// </summary>
        public bool IsExtended { get { return ((this.flags & 0x0001) != 0); } }
        /// <summary>
        /// イベントがインジェクトされたかどうか
        /// </summary>
        public bool IsInjected { get { return ((this.flags & 0x0010) != 0); } }
        /// <summary>
        /// 低い整合性レベルで実行中のプロセスからイベントがインジェクトされたかどうか
        /// </summary>
        public bool IsLowerInjected { get { return ((this.flags & 0x0002) != 0); } }
        /// <summary>
        /// ALTキーが押されているかどうか
        /// </summary>
        public bool IsAltKeyPressed { get { return ((this.flags & 0x0020) != 0); } }
        /// <summary>
        /// キーが押されているかどうか
        /// </summary>
        public bool IsKeyPressed { get { return ((this.flags & 0x0080) == 0); } }

    }
}

namespace MMFrame.Windows.GlobalHook
{
    /// <summary>
    /// キーボードのグローバルフックに関するクラス
    /// </summary>
    public static class KeyboardHook
    {
        /// <summary>
        /// P/Invoke
        /// </summary>
        private static class NativeMethods
        {
            /// <summary>
            /// フックプロシージャのデリゲート
            /// </summary>
            /// <param name="nCode">フックプロシージャに渡すフックコード</param>
            /// <param name="msg">フックプロシージャに渡す値</param>
            /// <param name="msllhookstruct">フックプロシージャに渡す値</param>
            /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
            public delegate System.IntPtr KeyboardHookCallback(int nCode, uint msg, ref KBDLLHOOKSTRUCT kbdllhookstruct);

            /// <summary>
            /// アプリケーション定義のフックプロシージャをフックチェーン内にインストールします。
            /// フックプロシージャをインストールすると、特定のイベントタイプを監視できます。
            /// 監視の対象になるイベントは、特定のスレッド、または呼び出し側スレッドと同じデスクトップ内のすべてのスレッドに関連付けられているものです。
            /// </summary>
            /// <param name="idHook">フックタイプ</param>
            /// <param name="lpfn">フックプロシージャ</param>
            /// <param name="hMod">アプリケーションインスタンスのハンドル</param>
            /// <param name="dwThreadId">スレッドの識別子</param>
            /// <returns>関数が成功すると、フックプロシージャのハンドルが返ります。関数が失敗すると、NULL が返ります。</returns>
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr SetWindowsHookEx(int idHook, KeyboardHookCallback lpfn, System.IntPtr hMod, uint dwThreadId);

            /// <summary>
            /// 現在のフックチェーン内の次のフックプロシージャに、フック情報を渡します。
            /// フックプロシージャは、フック情報を処理する前でも、フック情報を処理した後でも、この関数を呼び出せます。
            /// </summary>
            /// <param name="hhk">現在のフックのハンドル</param>
            /// <param name="nCode">フックプロシージャに渡すフックコード</param>
            /// <param name="msg">フックプロシージャに渡す値</param>
            /// <param name="msllhookstruct">フックプロシージャに渡す値</param>
            /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr CallNextHookEx(System.IntPtr hhk, int nCode, uint msg, ref KBDLLHOOKSTRUCT kbdllhookstruct);

            /// <summary>
            /// SetWindowsHookEx 関数を使ってフックチェーン内にインストールされたフックプロシージャを削除します。
            /// </summary>
            /// <param name="hhk">削除対象のフックプロシージャのハンドル</param>
            /// <returns>関数が成功すると、0 以外の値が返ります。関数が失敗すると、0 が返ります。</returns>
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(System.IntPtr hhk);
        }

        /// <summary>
        /// キーボードの状態の構造体
        /// </summary>
        public struct StateKeyboard
        {
            public Stroke Stroke;
            public System.Windows.Forms.Keys Key;
            public uint ScanCode;
            public uint Flags;
            public uint Time;
            public System.IntPtr ExtraInfo;
        }

        /// <summary>
        /// 挙動の列挙型
        /// </summary>
        public enum Stroke
        {
            KEY_DOWN,
            KEY_UP,
            SYSKEY_DOWN,
            SYSKEY_UP,
            UNKNOWN
        }

        /// <summary>
        /// キーボードのグローバルフックを実行しているかどうかを取得、設定します。
        /// </summary>
        public static bool IsHooking
        {
            get;
            private set;
        }

        /// <summary>
        /// キーボードのグローバルフックを中断しているかどうかを取得、設定します。
        /// </summary>
        public static bool IsPaused
        {
            get;
            private set;
        }

        /// <summary>
        /// キーボードの状態を取得、設定します。
        /// </summary>
        public static StateKeyboard State;

        /// <summary>
        /// フックプロシージャ内でのイベント用のデリゲート
        /// </summary>
        /// <param name="msg">キーボードに関するウィンドウメッセージ</param>
        /// <param name="msllhookstruct">低レベルのキーボードの入力イベントの構造体</param>
        public delegate void HookHandler(ref StateKeyboard state);

        /// <summary>
        /// 低レベルのキーボードの入力イベントの構造体
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public System.IntPtr dwExtraInfo;
        }

        /// <summary>
        /// フックプロシージャのハンドル
        /// </summary>
        private static System.IntPtr Handle;

        /// <summary>
        /// 入力をキャンセルするかどうかを取得、設定します。
        /// </summary>
        private static bool IsCancel;

        /// <summary>
        /// 登録イベントのリストを取得、設定します。
        /// </summary>
        private static System.Collections.Generic.List<HookHandler> Events;

        /// <summary>
        /// フックプロシージャ内でのイベント
        /// </summary>
        private static event HookHandler HookEvent;

        /// <summary>
        /// フックチェーンにインストールするフックプロシージャのイベント
        /// </summary>
        private static event NativeMethods.KeyboardHookCallback hookCallback;

        /// <summary>
        /// フックプロシージャをフックチェーン内にインストールし、キーボードのグローバルフックを開始します。
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public static void Start()
        {
            if (IsHooking)
            {
                return;
            }

            IsHooking = true;
            IsPaused = false;

            hookCallback = HookProcedure;
            System.IntPtr h = System.Runtime.InteropServices.Marshal.GetHINSTANCE(typeof(KeyboardHook).Assembly.GetModules()[0]);

            // WH_KEYBOARD_LL = 13;
            Handle = NativeMethods.SetWindowsHookEx(13, hookCallback, h, 0);

            if (Handle == System.IntPtr.Zero)
            {
                IsHooking = false;
                IsPaused = true;

                throw new System.ComponentModel.Win32Exception();
            }
        }

        /// <summary>
        /// キーボードのグローバルフックを停止し、フックプロシージャをフックチェーン内から削除します。
        /// </summary>
        public static void Stop()
        {
            if (!IsHooking)
            {
                return;
            }

            if (Handle != System.IntPtr.Zero)
            {
                IsHooking = false;
                IsPaused = true;

                ClearEvent();

                NativeMethods.UnhookWindowsHookEx(Handle);
                Handle = System.IntPtr.Zero;
                hookCallback -= HookProcedure;
            }
        }

        /// <summary>
        /// 次のフックプロシージャにフック情報を渡すのをキャンセルします。
        /// </summary>
        public static void Cancel()
        {
            IsCancel = true;
        }

        /// <summary>
        /// キーボードのグローバルフックを中断します。
        /// </summary>
        public static void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// キーボード操作時のイベントを追加します。
        /// </summary>
        /// <param name="hookHandler"></param>
        public static void AddEvent(HookHandler hookHandler)
        {
            if (Events == null)
            {
                Events = new System.Collections.Generic.List<HookHandler>();
            }

            Events.Add(hookHandler);
            HookEvent += hookHandler;
        }

        /// <summary>
        /// キーボード操作時のイベントを削除します。
        /// </summary>
        /// <param name="hookHandler"></param>
        public static void RemoveEvent(HookHandler hookHandler)
        {
            if (Events == null)
            {
                return;
            }

            HookEvent -= hookHandler;
            Events.Remove(hookHandler);
        }

        /// <summary>
        /// キーボード操作時のイベントを全て削除します。
        /// </summary>
        public static void ClearEvent()
        {
            if (Events == null)
            {
                return;
            }

            foreach (HookHandler e in Events)
            {
                HookEvent -= e;
            }

            Events.Clear();
        }

        /// <summary>
        /// フックチェーンにインストールするフックプロシージャ
        /// </summary>
        /// <param name="nCode">フックプロシージャに渡すフックコード</param>
        /// <param name="msg">フックプロシージャに渡す値</param>
        /// <param name="msllhookstruct">フックプロシージャに渡す値</param>
        /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
        private static System.IntPtr HookProcedure(int nCode, uint msg, ref KBDLLHOOKSTRUCT s)
        {
            if (nCode >= 0 && HookEvent != null && !IsPaused)
            {
                State.Stroke = GetStroke(msg);
                State.Key = (System.Windows.Forms.Keys)s.vkCode;
                State.ScanCode = s.scanCode;
                State.Flags = s.flags;
                State.Time = s.time;
                State.ExtraInfo = s.dwExtraInfo;

                HookEvent(ref State);

                if (IsCancel)
                {
                    IsCancel = false;

                    return (System.IntPtr)1;
                }
            }

            return NativeMethods.CallNextHookEx(Handle, nCode, msg, ref s);
        }

        /// <summary>
        /// キーボードキーの挙動を取得します。
        /// </summary>
        /// <param name="msg">キーボードに関するウィンドウメッセージ</param>
        /// <returns>キーボードボタンの挙動</returns>
        private static Stroke GetStroke(uint msg)
        {
            switch (msg)
            {
                case 0x100:
                    // WM_KEYDOWN
                    return Stroke.KEY_DOWN;
                case 0x101:
                    // WM_KEYUP
                    return Stroke.KEY_UP;
                case 0x104:
                    // WM_SYSKEYDOWN
                    return Stroke.SYSKEY_DOWN;
                case 0x105:
                    // WM_SYSKEYUP
                    return Stroke.SYSKEY_UP;
                default:
                    return Stroke.UNKNOWN;
            }
        }
    }

    static class VistaRestartRecoveryAPI
    {
        [DllImport("kernel32.dll")]
        internal static extern
            uint RegisterApplicationRecoveryCallback(IntPtr pRecoveryCallback, IntPtr pvParameter, int dwPingInterval, int dwFlags);
        [DllImport("kernel32.dll")]
        internal static extern
            uint ApplicationRecoveryInProgress(out bool pbCanceled);
        [DllImport("kernel32.dll")]
        internal static extern
            uint ApplicationRecoveryFinished(bool pSuccess);

        internal delegate int ApplicationRecoveryCallback(IntPtr pvParameter);
    }
}