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

namespace owchart_net {
    /// <summary>
    /// 最新数据层
    /// </summary>
    public partial class LatestDiv : Control {
        public LatestDiv() {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            timer.Enabled = true;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000;
        }

        /// <summary>
        /// 秒表
        /// </summary>
        private System.Windows.Forms.Timer timer = new Timer();

        /// <summary>
        /// 上次的数据
        /// </summary>
        private SecurityLatestData lastData = new SecurityLatestData();

        /// <summary>
        /// 买卖文字
        /// </summary>
        private List<String> m_buySellStrs = new List<String>();

        /// <summary>
        /// 秒表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e) {
            SecurityLatestData newData = new SecurityLatestData();
            SecurityService.GetLatestData(securityCode, ref newData);
            if (!newData.equal(lastData)) {
                lastData = newData;
                Invalidate();
            }
        }

        private int digit = 2;

        /// <summary>
        /// 保留小数的位数
        /// </summary>
        public int Digit {
            get { return digit; }
            set { digit = value; }
        }

        private String securityCode = "";

        /// <summary>
        /// 股票代码
        /// </summary>
        public String SecurityCode {
            get { return securityCode; }
            set { securityCode = value; }
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
        /// 获取最大值
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns>最大值</returns>
        public double Max(List<double> list) {
            double max = 0;
            for (int i = 0; i < list.Count; i++) {
                if (i == 0) {
                    max = list[i];
                } else {
                    if (list[i] > max) {
                        max = list[i];
                    }
                }
            }
            return max;
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
            
            SecurityLatestData latestData = new SecurityLatestData();
            Security security = new Security();
            SecurityService.GetSecurityByCode(SecurityCode, ref security);
            SecurityService.GetLatestData(SecurityCode, ref latestData);
            String securityName = security.m_name;
            int width = Width - 1;
            int height = Height - 1;
            if (width > 0 && height > 0) {
                Font font = new Font("微软雅黑", 12);
                Font lfont = new Font("微软雅黑", 12);
                Color wordColor = Color.FromArgb(100, 100, 100);
                int top = 32, step = 22;
                //画买卖盘
                DrawText(g, "卖", wordColor, font, 1, 47);
                DrawText(g, "盘", wordColor, font, 1, 100);
                DrawText(g, "买", wordColor, font, 1, 157);
                DrawText(g, "盘", wordColor, font, 1, 210);
                String buySellStr = "5,4,3,2,1,1,2,3,4,5";
                String[] buySellStrs = buySellStr.Split(',');
                int strsSize = buySellStrs.Length;
                for (int i = 0; i < strsSize; i++) {
                    DrawText(g, buySellStrs[i], wordColor, font, 25, top);
                    top += step;
                }
                font = new Font("微软雅黑", 12);
                top = 260;
                DrawText(g, "最新", wordColor, font, 1, top);
                DrawText(g, "升跌", wordColor, font, 1, top + 20);
                DrawText(g, "幅度", wordColor, font, 1, top + 40);
                DrawText(g, "总手", wordColor, font, 1, top + 60);
                DrawText(g, "涨停", wordColor, font, 1, top + 80);
                DrawText(g, "外盘", wordColor, font, 1, top + 100);
                DrawText(g, "开盘", wordColor, font, 110, top);
                DrawText(g, "最高", wordColor, font, 110, top + 20);
                DrawText(g, "最低", wordColor, font, 110, top + 40);
                DrawText(g, "换手", wordColor, font, 110, top + 60);
                DrawText(g, "跌停", wordColor, font, 110, top + 80);
                DrawText(g, "内盘", wordColor, font, 110, top + 100);
                font = new Font("微软雅黑", 14, FontStyle.Bold);
                //画股票代码
                Color yellowColor = Color.FromArgb(255, 255, 80);
                if (latestData.m_code != null && latestData.m_code.Length > 0) {
                    double close = latestData.m_close, open = latestData.m_open, high = latestData.m_high, low = latestData.m_low, lastClose = latestData.m_lastClose;
                    if (close == 0) {
                        if (latestData.m_buyPrice1 > 0) {
                            close = latestData.m_buyPrice1;
                            open = latestData.m_buyPrice1;
                            high = latestData.m_buyPrice1;
                            low = latestData.m_buyPrice1;
                        } else if (latestData.m_sellPrice1 > 0) {
                            close = latestData.m_sellPrice1;
                            open = latestData.m_sellPrice1;
                            high = latestData.m_sellPrice1;
                            low = latestData.m_sellPrice1;
                        }
                    }
                    if (lastClose == 0) {
                        lastClose = close;
                    }
                    List<double> plist = new List<double>();
                    List<double> vlist = new List<double>();
                    plist.Add(latestData.m_sellPrice5);
                    plist.Add(latestData.m_sellPrice4);
                    plist.Add(latestData.m_sellPrice3);
                    plist.Add(latestData.m_sellPrice2);
                    plist.Add(latestData.m_sellPrice1);
                    vlist.Add(latestData.m_sellVolume5);
                    vlist.Add(latestData.m_sellVolume4);
                    vlist.Add(latestData.m_sellVolume3);
                    vlist.Add(latestData.m_sellVolume2);
                    vlist.Add(latestData.m_sellVolume1);
                    plist.Add(latestData.m_buyPrice1);
                    plist.Add(latestData.m_buyPrice2);
                    plist.Add(latestData.m_buyPrice3);
                    plist.Add(latestData.m_buyPrice4);
                    plist.Add(latestData.m_buyPrice5);
                    vlist.Add(latestData.m_buyVolume1);
                    vlist.Add(latestData.m_buyVolume2);
                    vlist.Add(latestData.m_buyVolume3);
                    vlist.Add(latestData.m_buyVolume4);
                    vlist.Add(latestData.m_buyVolume5);
                    Color color = Color.Empty;
                    double mx = Max(vlist);
                    font = new Font("微软雅黑", 12);
                    if (mx > 0) {
                        //绘制买卖盘
                        int pLength = plist.Count;
                        top = 32;
                        for (int i = 0; i < pLength; i++) {
                            color = GetPriceColor(plist[i], lastClose);
                            DrawUnderLineNum(g, plist[i], digit, font, color, true, 60, top);
                            DrawUnderLineNum(g, vlist[i], 0, font, yellowColor, false, 110, top);
                            Brush sBrush = new SolidBrush(color);
                            int dWidth = (int)(vlist[i] / mx * 60);
                            if(dWidth < 2){
                                dWidth = 2;
                            }
                            g.FillRectangle(sBrush, new Rectangle(width - dWidth, top + step / 2 - 2, dWidth, 4));
                            sBrush.Dispose();
                            top += step;
                        }
                    }
                    vlist.Clear();
                    plist.Clear();
                    top = 260;
                    //成交
                    color = GetPriceColor(close, lastClose);
                    DrawUnderLineNum(g, close, digit, font, color, true, 45, top);
                    //升跌
                    double sub = 0;
                    if (close == 0) {
                        sub = latestData.m_buyPrice1 - lastClose;
                        double rate = 100 * (latestData.m_buyPrice1 - lastClose) / lastClose;
                        int pleft = DrawUnderLineNum(g, rate, 2, font, color, false, 45, top + 40);
                        DrawText(g, "%", color, font, pleft + 47, top + 40);
                    } else {
                        sub = close - latestData.m_lastClose;
                        double rate = 100 * (close - lastClose) / lastClose;
                        int pleft = DrawUnderLineNum(g, rate, 2, font, color, false, 45, top + 40);
                        DrawText(g, "%", color, font, pleft + 47, top + 40);
                    }
                    DrawUnderLineNum(g, sub, digit, font, color, false, 45, top + 20);
                    double volume = latestData.m_volume / 100;
                    String unit = "";
                    if (volume > 100000000) {
                        volume /= 100000000;
                        unit = "亿";
                    } else if (volume > 10000) {
                        volume /= 10000;
                        unit = "万";
                    }
                    //总手
                    int cleft = DrawUnderLineNum(g, volume, unit.Length > 0 ? digit : 0, font, yellowColor, true, 45, top + 60);
                    if (unit.Length > 0) {
                        DrawText(g, unit, yellowColor, font, cleft + 47, top + 60);
                    }
                    //换手
                    double turnoverRate = latestData.m_turnoverRate;
                    cleft = DrawUnderLineNum(g, turnoverRate, 2, font, yellowColor, true, 155, top + 60);
                    if (turnoverRate > 0) {
                        DrawText(g, "%", yellowColor, font, cleft + 157, top + 60);
                    }
                    //开盘
                    color = GetPriceColor(open, lastClose);
                    DrawUnderLineNum(g, open, digit, font, color, true, 155, top);
                    //最高
                    color = GetPriceColor(high, lastClose);
                    DrawUnderLineNum(g, high, digit, font, color, true, 155, top + 20);
                    //最低
                    color = GetPriceColor(low, lastClose);
                    DrawUnderLineNum(g, low, digit, font, color, true, 155, top + 40);
                    //涨停
                    double upPrice = lastClose * 1.1;
                    if (securityName != null && securityName.Length > 0) {
                        if (securityName.StartsWith("ST") || securityName.StartsWith("*ST")) {
                            upPrice = lastClose * 1.05;
                        }
                    }
                    DrawUnderLineNum(g, upPrice, digit, font, Color.FromArgb(255, 80, 80), true, 45, top + 80);
                    //跌停
                    double downPrice = lastClose * 0.9;
                    if (securityName != null && securityName.Length > 0) {
                        if (securityName.StartsWith("ST") || securityName.StartsWith("*ST")) {
                            downPrice = lastClose * 0.95;
                        }
                    }
                    DrawUnderLineNum(g, downPrice, digit, font, Color.FromArgb(80, 255, 80), true, 155, top + 80);
                    //外盘
                    double outerVol = latestData.m_outerVol;
                    unit = "";
                    if (outerVol > 100000000) {
                        outerVol /= 100000000;
                        unit = "亿";
                    } else if (outerVol > 10000) {
                        outerVol /= 10000;
                        unit = "万";
                    }
                    cleft = DrawUnderLineNum(g, outerVol, unit.Length > 0 ? digit : 0, font, Color.FromArgb(255, 80, 80), false, 45, top + 100);
                    if (unit.Length > 0) {
                        DrawText(g, unit, Color.FromArgb(255, 80, 80), font, cleft + 47, top + 100);
                    }
                    unit = "";
                    double innerVol = latestData.m_innerVol;
                    if (innerVol > 100000000) {
                        innerVol /= 100000000;
                        unit = "亿";
                    } else if (innerVol > 10000) {
                        innerVol /= 10000;
                        unit = "万";
                    }
                    //内盘
                    cleft = DrawUnderLineNum(g, innerVol, unit.Length > 0 ? digit : 0, font, Color.FromArgb(80, 255, 80), true, 155, top + 100);
                    if (unit.Length > 0) {
                        DrawText(g, unit, Color.FromArgb(80, 255, 80), font, cleft + 157, top + 100);
                    }
                }
                font = new Font("微软雅黑", 14);
                //股票代码
                if (securityCode != null && securityCode.Length > 0) {
                    DrawText(g, securityCode, Color.FromArgb(255, 255, 255), font, 2, 4);
                }
                //股票名称
                if (securityName != null && securityName.Length > 0) {
                    DrawText(g, securityName, Color.FromArgb(80, 255, 255), font, 110, 3);
                }
                //画边框
                Color frameColor = Color.FromArgb(150, 0, 0);
                Pen framePen = new Pen(frameColor);
                g.DrawLine(framePen, 0, 0, 0, height);
                g.DrawLine(framePen, 0, 30, width, 30);
                g.DrawLine(framePen, 24, 30, 24, top - 2);
                g.DrawLine(framePen, 0, 140, width, 140);
                g.DrawLine(framePen, 0, top - 2, width, top - 2);
                g.DrawLine(framePen, width, 0, width, height);
                g.DrawLine(framePen, 0, top + 130, width, top + 130);
                framePen.Dispose();
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
