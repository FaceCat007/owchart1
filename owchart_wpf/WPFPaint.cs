using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using owchart;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows;

namespace owchart_wpf
{
    public class WPFPaint : CPaint
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

        public DrawingContext context;

        /// <summary>
        /// 是否裁剪
        /// </summary>
        private bool hasClip = false;

        /// <summary>
        /// 目标控件
        /// </summary>
        public System.Windows.Controls.Control render;

        /// <summary>
        /// 开始绘图
        /// </summary>
        /// <param name="rect"></param>
        public override void BeginPaint(Rectangle rect)
        {
            if (gInput != null)
            {
                base.BeginPaint(rect);
            }
            hasClip = false;
        }

        /// <summary>
        /// 清除颜色
        /// </summary>
        /// <param name="color"></param>
        public override void Clear(System.Drawing.Color color)
        {
            if (g != null)
            {
                base.Clear(color);
            }
            else
            {
                FillRectangle(color, new Rectangle(0, 0, (int)render.ActualWidth, (int)render.ActualWidth));
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public override void DrawRectangle(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, Rectangle rect)
        {
            if (g != null)
            {
                base.DrawRectangle(color, lineWidth, dashStyle, rect);
            }
            else
            {
                Rect wpfRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                Geometry geoRect = new RectangleGeometry(wpfRect);
                context.DrawGeometry(null, GetPenWPF(color, lineWidth, dashStyle), geoRect);
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public override void DrawEllipse(System.Drawing.Color color, int width, System.Drawing.Drawing2D.DashStyle style, Rectangle rect)
        {
            if (g != null)
            {
                base.DrawRectangle(color, width, style, rect);
            }
            else
            {
                double rw = rect.Right - rect.Left - 1;
                if (rw < 1) rw = 1;
                double rh = rect.Bottom - rect.Top - 1;
                if (rh < 1) rh = 1;
                int centerX = (int)(rect.Left + rw / 2);
                int centerY = (int)(rect.Top + rw / 2);
                Geometry geoEllipse = new EllipseGeometry(new System.Windows.Point(centerX, centerY), rw / 2, rh / 2);
                context.DrawGeometry(null, GetPenWPF(color, width, style), geoEllipse);
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public override void DrawEllipse(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, RectangleF rect)
        {
            if (g != null)
            {
                base.DrawEllipse(color, lineWidth, dashStyle, rect);
            }
            else
            {
                double rw = rect.Right - rect.Left - 1;
                if (rw < 1) rw = 1;
                double rh = rect.Bottom - rect.Top - 1;
                if (rh < 1) rh = 1;
                int centerX = (int)(rect.Left + rw / 2);
                int centerY = (int)(rect.Top + rw / 2);
                Geometry geoEllipse = new EllipseGeometry(new System.Windows.Point(centerX, centerY), rw / 2, rh / 2);
                context.DrawGeometry(null, GetPenWPF(color, lineWidth, dashStyle), geoEllipse);
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public override void DrawEllipse(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, float x, float y, float width, float height)
        {
            if (g != null)
            {
                base.DrawEllipse(color, lineWidth, dashStyle, x, y, width, height);
            }
            else
            {
                double rw = width;
                if (rw < 1) rw = 1;
                double rh = height;
                if (rh < 1) rh = 1;
                int centerX = (int)(x + rw / 2);
                int centerY = (int)(y + rw / 2);
                Geometry geoEllipse = new EllipseGeometry(new System.Windows.Point(centerX, centerY), rw / 2, rh / 2);
                context.DrawGeometry(null, GetPenWPF(color, lineWidth, dashStyle), geoEllipse);
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public override void DrawRectangle(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, float x, float y, float width, float height)
        {
            if (g != null)
            {
                base.DrawRectangle(color, lineWidth, dashStyle, x, y, width, height);
            }
            else
            {
                Rect wpfRect = new Rect(x, y, width, height);
                Geometry geoRect = new RectangleGeometry(wpfRect);
                context.DrawGeometry(null, GetPenWPF(color, lineWidth, dashStyle), geoRect);
            }
        }


        /// <summary>
        /// 画线
        /// </summary>
        public override void DrawCurve(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, PointF[] points)
        {
            if (g != null)
            {
                base.DrawCurve(color, lineWidth, dashStyle, points);
            }
        }

        /// <summary>
        /// 绘制文字
        /// </summary>=
        public override void DrawString(String text, Font font, System.Drawing.Color color, PointF point)
        {
            if (g != null)
            {
                font = new Font(font.FontFamily, font.Size / 2, font.Style);
                base.DrawString(text, font, color, point);
            }
            else
            {
                FormattedText ft = GetFont(text, font, color);
                context.DrawText(ft, new System.Windows.Point(point.X, point.Y));
            }
        }

        /// <summary>
        /// 绘制文字
        /// </summary>=
        public override void DrawString(String text, Font font, System.Drawing.Color color, float x, float y)
        {
            if (g != null)
            {
                font = new Font(font.FontFamily, font.Size / 2, font.Style);
                base.DrawString(text, font, color, x, y);
            }
            else
            {
                FormattedText ft = GetFont(text, font, color);
                context.DrawText(ft, new System.Windows.Point(x, y));
            }
        }

        /// <summary>
        /// 绘制文字
        /// </summary>=
        public override void DrawString(String text, Font font, System.Drawing.Color color, Rectangle rect)
        {
            if (g != null)
            {

                font = new Font(font.FontFamily, font.Size / 2, font.Style);
                base.DrawString(text, font, color, rect);
            }
            else
            {
                FormattedText ft = GetFont(text, font, color);
                context.DrawText(ft, new System.Windows.Point(rect.Left, rect.Top));
            }
        }

        /// <summary>
        /// 绘制文字
        /// </summary>=
        public override void DrawStringAutoEllipsis(String text, Font font, System.Drawing.Color color, Rectangle rect)
        {
            if (g != null)
            {
                base.DrawString(text, font, color, rect);
            }
            else
            {
                FormattedText ft = GetFont(text, font, color);
                context.DrawText(ft, new System.Windows.Point(rect.Left, rect.Top));
            }
        }

        /// <summary>
        /// 画线
        /// </summary>
        public override void DrawLine(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, float x1, float y1, float x2, float y2)
        {
            if (g != null)
            {
                base.DrawLine(color, lineWidth, dashStyle, x1, y1, x2, y2);
            }
            else
            {
                context.DrawLine(GetPenWPF(color, lineWidth, dashStyle), new System.Windows.Point(x1, y1), new System.Windows.Point(x2, y2));
            }
        }

        /// <summary>
        /// 画线
        /// </summary>
        public override void DrawLines(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, System.Drawing.Point[] points)
        {
            if (g != null)
            {
                base.DrawLines(color, lineWidth, dashStyle, points);
            }
            else
            {
                List<System.Windows.Point> wpfPoints = new List<System.Windows.Point>();
                for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    wpfPoints.Add(new System.Windows.Point(x, y));
                }
                PathGeometry pathGeo = new PathGeometry();
                PolyLineSegment polyline = new PolyLineSegment(wpfPoints, true);
                List<PolyLineSegment> segments = new List<PolyLineSegment>();
                segments.Add(polyline);
                PathFigure pathFigure = new PathFigure(wpfPoints[0], segments, false);
                pathGeo.Figures.Add(pathFigure);
                context.DrawGeometry(null, GetPenWPF(color, lineWidth, dashStyle), pathGeo);
            }
        }

        /// <summary>
        /// 画线
        /// </summary>
        public override void DrawLines(System.Drawing.Color color, int lineWidth, System.Drawing.Drawing2D.DashStyle dashStyle, System.Drawing.PointF[] points)
        {
            if (g != null)
            {
                base.DrawLines(color, lineWidth, dashStyle, points);
            }
            else
            {
                List<System.Windows.Point> wpfPoints = new List<System.Windows.Point>();
                for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    wpfPoints.Add(new System.Windows.Point(x, y));
                }
                PathGeometry pathGeo = new PathGeometry();
                PolyLineSegment polyline = new PolyLineSegment(wpfPoints, true);
                List<PolyLineSegment> segments = new List<PolyLineSegment>();
                segments.Add(polyline);
                PathFigure pathFigure = new PathFigure(wpfPoints[0], segments, false);
                pathGeo.Figures.Add(pathFigure);
                context.DrawGeometry(null, GetPenWPF(color, lineWidth, dashStyle), pathGeo);
            }
        }


        /// <summary>
        /// 新的绘制矩形
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void DrawRectangleNew(System.Drawing.Color borderColor, System.Drawing.Color backColor, RectangleF rect)
        {
            if (g != null)
            {
                base.DrawRectangleNew(borderColor, backColor, rect);
            }
            else
            {
                FillRectangle(borderColor, rect);
                RectangleF newRectangle = new RectangleF(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
                if (newRectangle.Width > 0 && newRectangle.Height > 0)
                {
                    FillRectangle(backColor, newRectangle);
                }
            }
        }

        /// <summary>
        /// 结束绘图
        /// </summary>
        public override void EndPaint()
        {
            hasClip = false;
        }

        /// <summary>
        /// 填充椭圆
        /// </summary>
        public override void FillEllipse(System.Drawing.Color color, Rectangle rect)
        {
            if (g != null)
            {
                base.FillEllipse(color, rect);
            }
            else
            {
                Rect wpfRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                Geometry geoRect = new RectangleGeometry(wpfRect);
                context.DrawGeometry(GetBrushWPF(color), null, geoRect);
            }
        }

        /// <summary>
        /// 填充椭圆
        /// </summary>
        public override void FillEllipse(System.Drawing.Color color, RectangleF rect)
        {
            if (g != null)
            {
                base.FillEllipse(color, rect);
            }
            else
            {
                Rect wpfRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                Geometry geoRect = new RectangleGeometry(wpfRect);
                context.DrawGeometry(GetBrushWPF(color), null, geoRect);
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public override void FillRectangle(System.Drawing.Color color, float x, float y, float width, float height)
        {
            if (g != null)
            {
                base.FillRectangle(color, x, y, width, height);
            }
            else
            {
                Rect wpfRect = new Rect(x, y, width, height);
                Geometry geoRect = new RectangleGeometry(wpfRect);
                context.DrawGeometry(GetBrushWPF(color), null, geoRect);
            }
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        public override void FillRectangle(System.Drawing.Color color, Rectangle rect)
        {
            if (g != null)
            {
                base.FillRectangle(color, rect);
            }
            else
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Rect wpfRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                    Geometry geoRect = new RectangleGeometry(wpfRect);
                    context.DrawGeometry(GetBrushWPF(color), null, geoRect);
                }
            }
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        public override void FillRectangle(System.Drawing.Color color, RectangleF rect)
        {
            if (g != null)
            {
                base.FillRectangle(color, rect);
            }
            else
            {
                Rect wpfRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                Geometry geoRect = new RectangleGeometry(wpfRect);
                context.DrawGeometry(GetBrushWPF(color), null, geoRect);
            }
        }

        /// <summary>
        /// 画线
        /// </summary>
        public override void FillPolygon(System.Drawing.Color color, System.Drawing.Point[] points)
        {
            if (g != null)
            {
                base.FillPolygon(color, points);
            }
            else
            {
                List<System.Windows.Point> wpfPoints = new List<System.Windows.Point>();
                for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    wpfPoints.Add(new System.Windows.Point(x, y));
                }
                PathGeometry pathGeo = new PathGeometry();
                PolyLineSegment polyline = new PolyLineSegment(wpfPoints, true);
                List<PolyLineSegment> segments = new List<PolyLineSegment>();
                segments.Add(polyline);
                PathFigure pathFigure = new PathFigure(wpfPoints[0], segments, true);
                pathGeo.Figures.Add(pathFigure);
                context.DrawGeometry(GetBrushWPF(color), null, pathGeo);
            }
        }

        /// <summary>
        /// 画线
        /// </summary>
        public override void FillPolygon(System.Drawing.Color color, System.Drawing.PointF[] points)
        {
            if (g != null)
            {
                base.FillPolygon(color, points);
            }
            else
            {
                List<System.Windows.Point> wpfPoints = new List<System.Windows.Point>();
                for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    wpfPoints.Add(new System.Windows.Point(x, y));
                }
                PathGeometry pathGeo = new PathGeometry();
                PolyLineSegment polyline = new PolyLineSegment(wpfPoints, true);
                List<PolyLineSegment> segments = new List<PolyLineSegment>();
                segments.Add(polyline);
                PathFigure pathFigure = new PathFigure(wpfPoints[0], segments, true);
                pathGeo.Figures.Add(pathFigure);
                context.DrawGeometry(GetBrushWPF(color), null, pathGeo);
            }
        }


        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="dwPenColor">整型颜色</param>
        /// <returns>Gdi颜色</returns>
        private System.Windows.Media.Color GetWPFColor(System.Drawing.Color dwPenColor)
        {
            System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(dwPenColor.A, dwPenColor.R, dwPenColor.G, dwPenColor.B);
            return wpfColor;
        }

        /// <summary>
        /// 获取格式化字体
        /// </summary>
        /// <param name="text">文字</param>
        /// <param name="font">字体</param>
        /// <param name="font">颜色</param>
        /// <returns>格式化字体</returns>
        private FormattedText GetFont(string text, Font font, System.Drawing.Color dwPenColor)
        {
            FormattedText ft = new FormattedText(text, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(font.FontFamily.ToString()), font.Size * 1.3f, GetBrushWPF(dwPenColor));
            if (font.Bold)
            {
                ft.SetFontWeight(FontWeights.Bold);
            }
            if (font.Italic)
            {
                ft.SetFontStyle(FontStyles.Italic);
            }
            if (font.Underline)
            {
                ft.SetTextDecorations(TextDecorations.Underline);
            }
            return ft;
        }

        /// <summary>
        /// 获取画刷
        /// </summary>
        /// <param name="dwPenColor">颜色</param>
        /// <returns>画刷</returns>
        private SolidColorBrush GetBrushWPF(System.Drawing.Color dwPenColor)
        {
            SolidColorBrush brush = new SolidColorBrush(GetWPFColor(dwPenColor));
            return brush;
        }

        /// <summary>
        /// 获取画笔
        /// </summary>
        /// <param name="dwPenColor">颜色</param>
        /// <param name="width">宽度</param>
        /// <param name="style">样式</param>
        /// <returns>画笔</returns>
        private System.Windows.Media.Pen GetPenWPF(System.Drawing.Color dwPenColor, int width, System.Drawing.Drawing2D.DashStyle style)
        {
            System.Windows.Media.Color wpfColor = GetWPFColor(dwPenColor);
            System.Windows.Media.Brush brush = new SolidColorBrush(wpfColor);
            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(brush, width);
            if (style == System.Drawing.Drawing2D.DashStyle.Solid)
            {
                pen.DashStyle = DashStyles.Solid;
            }
            else if (style == System.Drawing.Drawing2D.DashStyle.Dash)
            {
                pen.DashStyle = DashStyles.Dash;
            }
            else if (style == System.Drawing.Drawing2D.DashStyle.Dot)
            {
                pen.DashStyle = DashStyles.Dot;
            }
            return pen;
        }

        /// <summary>
        /// 刷新绘图
        /// </summary>
        public override void Invalidate()
        {
            if (render != null)
            {
                render.InvalidateVisual();
            }
        }

        /// <summary>
        /// 绘制文字
        /// </summary>=
        public override SizeF MeasureString(String text, Font font)
        {
            if (g != null)
            {
                font = new Font(font.FontFamily, font.Size / 2, font.Style);
                return base.MeasureString(text, font);
            }
            else
            {
                FormattedText ft = GetFont(text, font, System.Drawing.Color.Empty);
                return new SizeF((float)ft.Width, (float)ft.Height);
            }
        }

        /// <summary>
        /// 恢复状态
        /// </summary>
        public override void RestoreState()
        {
            if (g != null)
            {
                base.RestoreState();
            }
        }

        /// <summary>
        /// 设置裁剪
        /// </summary>
        /// <param name="rect"></param>
        public override void SetClip(Rectangle rect)
        {
            if (g != null)
            {
                base.SetClip(rect);
            }
            else
            {
                if (hasClip)
                {
                    context.Pop();
                }
                Rect wpfRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                RectangleGeometry rectGeo = new RectangleGeometry(wpfRect);
                context.PushClip(rectGeo);
                hasClip = true;
            }
        }

        /// <summary>
        /// 设置裁剪
        /// </summary>
        /// <param name="rect"></param>
        public override void SetClip(RectangleF rect)
        {
            if (g != null)
            {
                base.SetClip(rect);
            }
            else
            {
                if (hasClip)
                {
                    context.Pop();
                }
                Rect wpfRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                RectangleGeometry rectGeo = new RectangleGeometry(wpfRect);
                context.PushClip(rectGeo);
                hasClip = true;
            }
        }

        /// <summary>
        /// 保存状态
        /// </summary>
        public override void SaveState()
        {
            if (g != null)
            {
                 base.SaveState();
            }
        }

        /// <summary>
        /// 横轴和纵轴变换
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void TranslateTransform(float x, float y)
        {
            if (g != null)
            {
                base.TranslateTransform(x, y);
            }
        }

        /// <summary>
        /// 设置清晰度
        /// </summary>
        /// <param name="state"></param>
        public override void SetAntiAlias(int state)
        {
            if (g != null)
            {
                base.SetAntiAlias(state);
            }
        }
    }

    /// <summary>
    /// 坐标
    /// </summary>
    public struct POINT
    {
        public int x;
        public int y;
    }
}
