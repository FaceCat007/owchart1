using System;
using System.Collections.Generic;
using System.Text;
using owchart;
using System.Drawing;

namespace owchart_net
{
    /// <summary>
    /// 画线交易线
    /// </summary>
    public class TradeLine : PlotHorizontal
    {
        private String bs = "多头";

        /// <summary>
        /// 获取或设置多空方向
        /// </summary>
        public String Bs
        {
            get { return bs; }
            set { bs = value; }
        }

        /// <summary>
        /// 文字的字体
        /// </summary>
        private Font wordFont = LbCommon.GetDefaultFont();

        public Font WordFont
        {
            get { return wordFont; }
            set { wordFont = value; }
        }

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public double Value
        {
            get { return PointList[0].Value; }
            set { 
                PlotMark mark = new PlotMark(PointList[0].Index, PointList[0].Key, value);
                PointList[0] = mark;
            }
        }

        /// <summary>
        /// 画线
        /// </summary>
        public override void DrawPlot(Graphics g, List<PlotBase.PlotMark> pList, Color curColor)
        {
            if (pList.Count == 0)
            {
                return;
            }
            ChartDiv div = ChartDiv;
            float y1 = Chart.GetY(div, pList[0].Value, AttachYScale.Left) - div.DisplayRectangle.Y - div.TitleHeight;
            Brush pBrush = new SolidBrush(curColor);
            Pen pPen = new Pen(pBrush);
            pPen.Width = LineWidth;
            if (Style != null)
            {
                pPen.DashPattern = Style;
            }
            g.DrawLine(pPen, 0, y1, Chart.GetWorkSpaceX(), y1);
            String str = bs + " " + LbCommon.GetValueByDigit(pList[0].Value, 2, true);
            SizeF sizeF = g.MeasureString(str, wordFont);
            g.DrawString(str, wordFont, pBrush, new PointF((float)Chart.GetWorkSpaceX() - sizeF.Width, y1 - sizeF.Height));
            pPen.Dispose();
            pBrush.Dispose();
        }
    }
}
