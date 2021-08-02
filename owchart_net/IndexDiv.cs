/*
 * OWCHART证券图形控件
 * 著作权编号：2012SR088937
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using owchart;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace owchart_net {
    /// <summary>
    /// 指标层
    /// </summary>
    public partial class IndexDiv : Control {
        public IndexDiv() {
            InitializeComponent();
            timer.Enabled = true;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 2000;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// 秒表
        /// </summary>
        private System.Windows.Forms.Timer timer = new Timer();

        /// <summary>
        /// 秒表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e) {
            Invalidate();
        }

        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="text">文字</param>
        /// <param name="dwPenColor">颜色</param>
        /// <param name="font">字体</param>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        public static Size DrawText(CPaint paint, String text, Color dwPenColor, Font font, int x, int y) {
            Size tSize = paint.MeasureString(text, font).ToSize();
            Rectangle tRect = new Rectangle(x, y, tSize.Width + 1, tSize.Height);
            paint.DrawString(text, font, dwPenColor, tRect);
            return tSize;
        }

        /// <summary>
        /// 根据价格获取颜色
        /// </summary>
        /// <param name="price">价格</param>
        /// <param name="comparePrice">比较价格</param>
        /// <returns>颜色</returns>
        public Color GetPriceColor(double price, double comparePrice) {
            if (Program.BlackOrWhite)
            {
                if (price != 0)
                {
                    if (price > comparePrice)
                    {
                        return Color.FromArgb(255, 80, 80);
                    }
                    else if (price < comparePrice)
                    {
                        return Color.FromArgb(80, 255, 80);
                    }
                }
                return Color.FromArgb(255, 255, 255);
            }
            else
            {
                return Color.Black;
            }
        }

        /// <summary>
        /// 绘制有下划线的数字
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="value">值</param>
        /// <param name="digit">保留小数位数</param>
        /// <param name="font">字体</param>
        /// <param name="fontColor">文字颜色</param>
        /// <param name="zeroAsEmpty">0是否为空</param>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <returns>绘制的横坐标</returns>
        public static int DrawUnderLineNum(CPaint paint, double value, int digit, Font font, Color fontColor, bool zeroAsEmpty, int x, int y) {
            if (zeroAsEmpty && value == 0) {
                String text = "-";
                Size size = paint.MeasureString(text, font).ToSize();
                DrawText(paint, text, fontColor, font, x, y);
                return size.Width;
            } else {
                String[] nbs = LbCommon.GetValueByDigit(value, digit, true).Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (nbs.Length == 1) {
                    Size size = paint.MeasureString(nbs[0], font).ToSize();
                    DrawText(paint, nbs[0], fontColor, font, x, y);
                    return size.Width;
                } else {
                    Size decimalSize = paint.MeasureString(nbs[0], font).ToSize();
                    Size size = paint.MeasureString(nbs[1], font).ToSize();
                    DrawText(paint, nbs[0], fontColor, font, x, y);
                    DrawText(paint, nbs[1], fontColor, font, x
                        + decimalSize.Width - 4, y);
                    paint.DrawLine(fontColor, 1, DashStyle.Solid, x
                        + decimalSize.Width, y + decimalSize.Height - 3,
                        x + decimalSize.Width + size.Width - 7, y + decimalSize.Height - 3);
                    return decimalSize.Width + size.Width - 7;
                }
            }
        }

        public CPaint paint = new CPaint();

        /// <summary>
        /// 重绘
        /// </summary>
        /// <param name="paint"></param>
        public virtual void DoPaint(CPaint paint)
        {
            if (!Program.BlackOrWhite)
            {
                paint.Clear(Color.White);
            }

            int width = Width;
            int height = Height;
            if (width > 0 && height > 0)
            {
                SecurityLatestData ssLatestData = new SecurityLatestData();
                SecurityLatestData szLatestData = new SecurityLatestData();
                SecurityLatestData cyLatestData = new SecurityLatestData();
                if (SecurityService.GetLatestData("000001.SH", ref ssLatestData) > 0
                    && SecurityService.GetLatestData("399001.SZ", ref szLatestData) > 0
                    && SecurityService.GetLatestData("399006.SZ", ref cyLatestData) > 0)
                {

                }
                Color titleColor = Color.FromArgb(255, 255, 80);
                Font font = new Font("微软雅黑", 12);
                Font indexFont = new Font("微软雅黑", 12);
                Color grayColor = Color.FromArgb(200, 200, 200);
                if (!Program.BlackOrWhite)
                {
                    titleColor = Color.Black;
                }
                //上证指数
                Color indexColor = GetPriceColor(ssLatestData.m_close, ssLatestData.m_lastClose);
                int left = 1;
                DrawText(paint, "上证", titleColor, font, left, 3);
                left += 40;
                paint.DrawLine(grayColor, 1, DashStyle.Solid, left, 0, left, height);
                String amount = (ssLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                Size amountSize = paint.MeasureString(amount, indexFont).ToSize();
                DrawText(paint, amount, titleColor, indexFont, width / 3 - amountSize.Width, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4;
                int length = DrawUnderLineNum(paint, ssLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                left += length + (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(paint, ssLatestData.m_close - ssLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                //深证指数
                left = width / 3;
                paint.DrawLine(grayColor, 1, DashStyle.Solid, left, 0, left, height);
                indexColor = GetPriceColor(szLatestData.m_close, szLatestData.m_lastClose);
                DrawText(paint, "深证", titleColor, font, left, 3);
                left += 40;
                paint.DrawLine(grayColor, 1, DashStyle.Solid, left, 0, left, height);
                amount = (szLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                amountSize = paint.MeasureString(amount, indexFont).ToSize();
                DrawText(paint, amount, titleColor, indexFont, width * 2 / 3 - amountSize.Width, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(paint, szLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                left += length + (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(paint, szLatestData.m_close - szLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                //创业指数
                left = width * 2 / 3;
                paint.DrawLine(grayColor, 1, DashStyle.Solid, left, 0, left, height);
                indexColor = GetPriceColor(cyLatestData.m_close, cyLatestData.m_lastClose);
                DrawText(paint, "创业", titleColor, font, left, 3);
                left += 40;
                paint.DrawLine(grayColor, 1, DashStyle.Solid, left, 0, left, height);
                amount = (cyLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                amountSize = paint.MeasureString(amount, indexFont).ToSize();
                DrawText(paint, amount, titleColor, indexFont, width - amountSize.Width, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(paint, cyLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4 + length;
                length = DrawUnderLineNum(paint, cyLatestData.m_close - cyLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                paint.DrawRectangle(grayColor, 1, DashStyle.Solid, new Rectangle(0, 0, width - 1, height - 1));
            }
        }

        /// <summary>
        /// 重绘方法
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            paint.gInput = pe.Graphics;
            paint.BeginPaint(DisplayRectangle);
            DoPaint(paint);
            paint.EndPaint();
        }

        /// <summary>
        /// 大小改变事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            Invalidate();
        }
    }
}
