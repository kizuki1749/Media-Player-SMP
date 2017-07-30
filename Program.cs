using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

namespace Media_Player_SMP
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            switch (Properties.Settings.Default.Launguage)
            {
                case "English":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;

                case "Japanese":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja");
                    break;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
