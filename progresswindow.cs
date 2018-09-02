using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace niconicoviewer
{
    public partial class progresswindow : Form
    {
        public progresswindow()
        {
            InitializeComponent();
        }

        private void progresswindow_Load(object sender, EventArgs e)
        {
            
        }

        public bool progressvisible
        {
            set
            {
                progressBar1.Visible = value;
            }
            get
            {
                return progressBar1.Visible;
            }
        }

        public string text
        {
            set
            {
                label1.Text = value;
            }
            get
            {
                return label1.Text;
            }
        }
        public int progressvalue
        {
            set
            {
                progressBar1.Value = value;
            }
            get
            {
                return progressBar1.Value;
            }
        }
        public ProgressBarStyle progressstyle
        {
            set
            {
                progressBar1.Style = value;
            }
            get
            {
                return progressBar1.Style;
            }
        }

        public int progressmax
        {
            set
            {
                progressBar1.Maximum = value;
            }
            get
            {
                return progressBar1.Maximum;
            }
        }

        TaskbarProgressBarState taskbarProgressBarState;

        public TaskbarProgressBarState TaskbarprogressBarState
        {
            set
            {
                taskbarProgressBarState = TaskbarprogressBarState;
                TaskbarManager.Instance.SetProgressState(TaskbarprogressBarState);
            }
            get
            {
                return taskbarProgressBarState;
            }
        }

        public void SetTProgressValue(int val, int max)
        {
            TaskbarManager.Instance.SetProgressValue(val, max);
        }

        public bool cancel
        {
            set
            {
                cancel = value;
            }
            get
            {
                return cancelflag;
            }
        }

        bool cancelflag = false;

        private void button1_Click(object sender, EventArgs e)
        {
            cancelflag = true;

        }
    }
}
