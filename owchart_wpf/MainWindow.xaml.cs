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
using System.Windows.Forms;

namespace owchart_wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //注册消息
            SecurityService.Load();
            chartRender = new BaseRender();
            ChartExtend chartExtend = new ChartExtend();
            chartRender.eventBase = chartExtend;
            chartExtend.OnLoadEx();
            chartExtend.PaintCore = new WPFPaint();
            (chartExtend.PaintCore as WPFPaint).render = chartRender;
            chartRender.Height = 300;
            //AddChild(chartRender);
            Grid1.Children.Add(chartRender);
            gridRender = new BaseRender();
            GridExtend gridExtend = new GridExtend();
            gridRender.eventBase = gridExtend;
            gridExtend.PaintCore = new WPFPaint();
            (gridExtend.PaintCore as WPFPaint).render = gridRender;
            gridRender.Height = 300;
            gridExtend.chartExtend = chartExtend;
            Grid1.Children.Add(gridRender);
            chartExtend.ChangeSecurity("600000.SH");
        }

        /// <summary>
        /// K线控件
        /// </summary>
        private BaseRender chartRender;

        /// <summary>
        /// 表格控件
        /// </summary>
        private BaseRender gridRender;

        /// <summary>
        /// 窗体关闭方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 键盘按下方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (chartRender != null)
            {
                chartRender.OnKeyDownEx(e);
            }
            if (gridRender != null)
            {
                gridRender.OnKeyDownEx(e);
            }
        }

        /// <summary>
        /// 键盘抬起方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (chartRender != null)
            {
                chartRender.OnKeyUpEx(e);
            }
            if (gridRender != null)
            {
                gridRender.OnKeyUpEx(e);
            }
        }

        /// <summary>
        /// 大小改变方法
        /// </summary>
        /// <param name="sizeInfo">参数</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            System.Windows.Size newSize = sizeInfo.NewSize;
            chartRender.Height = newSize.Height / 2;
            gridRender.Height = newSize.Height / 2;
        }
    }
}
