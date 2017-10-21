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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        private static Form2 _form2Instance;

        public static Form2 Form2Instance
        {
            get
            {
                return _form2Instance;
            }
            set
            {
                _form2Instance = value;
            }
        }

        public string url
        {
            get
            {
                return axShockwaveFlash1.Movie;
            }
            set
            {
                axShockwaveFlash1.Movie = value;
            }
        }
    }
}
