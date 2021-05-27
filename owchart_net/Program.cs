using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace owchart_net
{
    public static class Program
    {
        private static bool blackOrWhite = true;

        /// <summary>
        /// 黑色或白色
        /// </summary>
        public static bool BlackOrWhite
        {
            get { return blackOrWhite; }
            set { blackOrWhite = value; }
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(){
            SecurityService.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}