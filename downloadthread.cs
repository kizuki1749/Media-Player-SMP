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
    public partial class downloadthread : Form
    {
        public downloadthread(Form1 form)
        {
            InitializeComponent();
            form1 = form;
        }

        Form1 form1;

        private void downloadthread_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = form1.GetThread();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            form1.SetThread((int)numericUpDown1.Value);
        }
    }
}
