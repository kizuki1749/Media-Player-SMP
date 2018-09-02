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
    public partial class AdvancedInfo : Form
    {
        public AdvancedInfo(Form1 f)
        {
            InitializeComponent();
            form1 = f;
        }

        public static Form1 form1;

        private void AdvancedInfo_Load(object sender, EventArgs e)
        {
            info info = new info();
            propertyGrid1.SelectedObject = info;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            propertyGrid1.Refresh();
        }
    }

    public class info
    {
        [Category("Ctlcontrols")]
        [ReadOnly(true)]
        public double currentPosition
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.Ctlcontrols.currentPosition; }
        }
        [Category("Ctlcontrols")]
        [ReadOnly(true)]
        public string currentPositionString
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.Ctlcontrols.currentPositionString; }
        }
        [Category("Settings")]
        [ReadOnly(true)]
        public bool mute
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.settings.mute; }
        }
        [Category("Settings")]
        [ReadOnly(true)]
        public int playCount
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.settings.playCount; }
        }
        [Category("Settings")]
        [ReadOnly(true)]
        public double rate
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.settings.rate; }
        }
        [Category("Settings")]
        [ReadOnly(true)]
        public int volume
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.settings.volume; }
        }
        [Category("Status")]
        [ReadOnly(true)]
        public string status
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.status; }
        }
        [Category("Status")]
        [ReadOnly(true)]
        public WMPLib.WMPOpenState OpenState
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.openState ; }
        }
        [Category("Status")]
        [ReadOnly(true)]
        public WMPLib.WMPPlayState PlayState
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.playState; }
        }
        [Category("URL")]
        [ReadOnly(true)]
        public string URL
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.URL; }
        }
        [Category("Settings")]
        [ReadOnly(true)]
        public Size imageSourceHeight
        {
            get { return new Size(AdvancedInfo.form1.axWindowsMediaPlayer1.currentMedia.imageSourceWidth,AdvancedInfo.form1.axWindowsMediaPlayer1.currentMedia.imageSourceHeight); }
        }
        [Category("currentMedia")]
        [ReadOnly(true)]
        public string sourceURL
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.currentMedia.sourceURL; }
        }
        [Category("currentMedia")]
        [ReadOnly(true)]
        public double duration
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.currentMedia.duration; }
        }
        [Category("currentMedia")]
        [ReadOnly(true)]
        public string durationString
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.currentMedia.durationString; }
        }
        [Category("enableContextMenu")]
        public bool enableContextMenu
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.enableContextMenu; }
            set { AdvancedInfo.form1.axWindowsMediaPlayer1.enableContextMenu = value; }
        }
        [Category("currentPlaylist")]
        [ReadOnly(true)]
        public int count
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.currentPlaylist.count; }
        }
        [Category("Flags")]
        [ReadOnly(true)]
        public bool isOnline
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.isOnline; }
        }
        [Category("Flags")]
        [ReadOnly(true)]
        public bool isRemote
        {
            get { return AdvancedInfo.form1.axWindowsMediaPlayer1.isRemote; }
        }
    }
}
