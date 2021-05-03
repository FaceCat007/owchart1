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
    public partial class FloatDiv : Grid
    {
        public FloatDiv()
        {
            InitializeComponent();
        }

        public override void OnPaintAfter(Graphics g)
        {
            Color pColor = Color.FromArgb(255, 0, 0);
            Point[] points = new Point[3];
            points[0] = new Point(0, 0);
            points[1] = new Point(10, 0);
            points[2] = new Point(0, 10);
            Brush brush = new SolidBrush(pColor);
            g.FillPolygon(brush, points);
            brush.Dispose();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Location.X < 10 && e.Location.Y < 10)
            {
                if (Width > 10)
                {
                    Width = 10;
                }
                else
                {
                    Width = 60;
                }
            }
            else
            {
                base.OnMouseDown(e);
            }
        }
    }
}
