/*
 * OWCHART证券图形控件
 * 著作权编号：2012SR088937
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.IO;
using System.Runtime.InteropServices;
using System.Data;
using owchart;
using System.Windows.Threading;
using owchart_net;

namespace owchart_wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Gaia 2016/11/26
        /// <summary>
        /// 创建窗体
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SecurityService.Load();
            mainForm = new MainForm();
            Background = Brushes.Black;
            //设置Winform交互控件
            mainForm.TopLevel = false;
            mainForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            windowsFormsHost1.Child = mainForm;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000);
            timer.Tick += new EventHandler(timer_Tick);
            WindowState = WindowState.Maximized;
            Closed += new EventHandler(MainWindow_Closed);
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        /// <summary>
        /// 秒表
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// 主窗体
        /// </summary>
        private MainForm mainForm;

        /// <summary>
        /// 秒表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e) {
 
        }

        /// <summary>
        /// K线控件
        /// </summary>
        private Chart chart = new Chart();

        /// <summary>
        /// 当前被选中的画线工具
        /// </summary>
        private string curPaintLine = string.Empty;

        /// <summary>
        /// 坐标结构
        /// </summary>
        public struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// 系统函数，用于获取坐标
        /// </summary>
        /// <param name="pt">输出坐标</param>
        /// <returns>是否成功</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <returns>坐标</returns>
        public POINT GetMousePoint()
        {
            POINT mp = new POINT();
            GetCursorPos(out mp);
            Point clientPoint = PointFromScreen(new Point(mp.x, mp.y));
            mp.x = (int)clientPoint.X;
            mp.y = (int)clientPoint.Y;
            return mp;
        }
        #endregion
    }
}
