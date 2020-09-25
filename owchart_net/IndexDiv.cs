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
        public static Size DrawText(Graphics g, String text, Color dwPenColor, Font font, int x, int y) {
            Size tSize = g.MeasureString(text, font).ToSize();
            Rectangle tRect = new Rectangle(x, y, tSize.Width + 1, tSize.Height);
            Brush brush = new SolidBrush(dwPenColor);
            g.DrawString(text, font, brush, tRect);
            brush.Dispose();
            return tSize;
        }

        /// <summary>
        /// 根据价格获取颜色
        /// </summary>
        /// <param name="price">价格</param>
        /// <param name="comparePrice">比较价格</param>
        /// <returns>颜色</returns>
        public static Color GetPriceColor(double price, double comparePrice) {
            if (price != 0) {
                if (price > comparePrice) {
                    return Color.FromArgb(255, 80, 80);
                } else if (price < comparePrice) {
                    return Color.FromArgb(80, 255, 80);
                }
            }
            return Color.FromArgb(255, 255, 255);
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
        public static int DrawUnderLineNum(Graphics g, double value, int digit, Font font, Color fontColor, bool zeroAsEmpty, int x, int y) {
            if (zeroAsEmpty && value == 0) {
                String text = "-";
                Size size = g.MeasureString(text, font).ToSize();
                DrawText(g, text, fontColor, font, x, y);
                return size.Width;
            } else {
                String[] nbs = LbCommon.GetValueByDigit(value, digit, true).Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (nbs.Length == 1) {
                    Size size = g.MeasureString(nbs[0], font).ToSize();
                    DrawText(g, nbs[0], fontColor, font, x, y);
                    return size.Width;
                } else {
                    Size decimalSize = g.MeasureString(nbs[0], font).ToSize();
                    Size size = g.MeasureString(nbs[1], font).ToSize();
                    DrawText(g, nbs[0], fontColor, font, x, y);
                    DrawText(g, nbs[1], fontColor, font, x
                        + decimalSize.Width - 4, y);
                    Pen pen = new Pen(fontColor);
                    g.DrawLine(pen, x
                        + decimalSize.Width, y + decimalSize.Height - 3,
                        x + decimalSize.Width + size.Width - 7, y + decimalSize.Height - 3);
                    pen.Dispose();
                    return decimalSize.Width + size.Width - 7;
                }
            }
        }

        /// <summary>
        /// 重绘方法
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            Graphics g = null;
            BufferedGraphics myBuffer = null;
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            myBuffer = currentContext.Allocate(pe.Graphics, DisplayRectangle);
            g = myBuffer.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int width = Width;
            int height = Height;
            if (width > 0 && height > 0) {
                SecurityLatestData ssLatestData = new SecurityLatestData();
                SecurityLatestData szLatestData = new SecurityLatestData();
                SecurityLatestData cyLatestData = new SecurityLatestData();
                if(SecurityService.GetLatestData("000001.SH", ref ssLatestData) > 0
                    && SecurityService.GetLatestData("399001.SZ", ref szLatestData) > 0
                    && SecurityService.GetLatestData("399006.SZ", ref cyLatestData) > 0) {
                    
                }
                Color titleColor = Color.FromArgb(255, 255, 80);
                Font font = new Font("微软雅黑", 12);
                Font indexFont = new Font("微软雅黑", 12);
                Color grayColor = Color.FromArgb(200, 200, 200);
                //上证指数
                Color indexColor = GetPriceColor(ssLatestData.m_close, ssLatestData.m_lastClose);
                int left = 1;
                DrawText(g, "上证", titleColor, font, left, 3);
                left += 40;
                Pen grayPen = new Pen(grayColor);
                g.DrawLine(grayPen, left, 0, left, height);
                String amount = (ssLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                Size amountSize = g.MeasureString(amount, indexFont).ToSize();
                DrawText(g, amount, titleColor, indexFont, width / 3 - amountSize.Width, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4;
                int length = DrawUnderLineNum(g, ssLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                left += length + (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(g, ssLatestData.m_close - ssLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                //深证指数
                left = width / 3;
                g.DrawLine(grayPen, left, 0, left, height);
                indexColor = GetPriceColor(szLatestData.m_close, szLatestData.m_lastClose);
                DrawText(g, "深证", titleColor, font, left, 3);
                left += 40;
                g.DrawLine(grayPen, left, 0, left, height);
                amount = (szLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                amountSize = g.MeasureString(amount, indexFont).ToSize();
                DrawText(g, amount, titleColor, indexFont, width * 2 / 3 - amountSize.Width, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(g, szLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                left += length + (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(g, szLatestData.m_close - szLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                //创业指数
                left = width * 2 / 3;
                g.DrawLine(grayPen, left, 0, left, height);
                indexColor = GetPriceColor(cyLatestData.m_close, cyLatestData.m_lastClose);
                DrawText(g, "创业", titleColor, font, left, 3);
                left += 40;
                g.DrawLine(grayPen, left, 0, left, height);
                amount = (cyLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                amountSize = g.MeasureString(amount, indexFont).ToSize();
                DrawText(g, amount, titleColor, indexFont, width - amountSize.Width, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4;
                length = DrawUnderLineNum(g, cyLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                left += (width / 3 - 40 - amountSize.Width) / 4 + length;
                length = DrawUnderLineNum(g, cyLatestData.m_close - cyLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                g.DrawRectangle(grayPen, new Rectangle(0, 0, width - 1, height - 1));
                grayPen.Dispose();
            }

            myBuffer.Render();
            myBuffer.Dispose();
            g.Dispose();
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
