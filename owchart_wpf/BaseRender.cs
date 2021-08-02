using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing;
using owchart;
using owchart_net;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace owchart_wpf
{
    /// <summary>
    /// 渲染控件
    /// </summary>
    public class BaseRender : System.Windows.Controls.Control
    {
        /// <summary>
        /// 渲染控件
        /// </summary>
        /// <param name="native">方法库</param>
        public BaseRender()
        {
        }

        public static IEventBase clickEventBase;

        /// <summary>
        /// 表格控件
        /// </summary>
        public IEventBase eventBase;

        /// <summary>
        /// 绘图模式
        /// </summary>
        public int writeMode = 1;

        /// <summary>
        /// 缓存的WPF图像
        /// </summary>
        public WriteableBitmap wBitmap;

        /// <summary>
        /// 缓存的GDI+图像
        /// </summary>
        public Bitmap bitMap;

        /// <summary>
        /// 缓存的绘图对象
        /// </summary>
        public Graphics bitMapG;

        /// <summary>
        /// 缓存的WPF位图对象
        /// </summary>
        public RenderTargetBitmap rtb;

        /// <summary>
        /// WPF可视对象
        /// </summary>
        private DrawingVisual _drawingVisual;

        /// <summary>
        /// 重绘方法
        /// </summary>
        /// <param name="drawingContext">参数</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            int width = (int)ActualWidth, height = (int)ActualHeight;
            WPFPaint paintCore = eventBase.PaintCore as WPFPaint;
            paintCore.render = this;
            //GDI+绘图
            if (writeMode == 0)
            {
                if(rtb == null || wBitmap.Width != width || wBitmap.Height != height){
                    wBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
                    if (bitMap != null)
                    {
                        bitMap.Dispose();
                    }
                    if (bitMapG != null)
                    {
                        bitMapG.Dispose();
                    }
                    bitMap = new Bitmap(width, height, wBitmap.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, wBitmap.BackBuffer);
                    bitMapG = Graphics.FromImage(bitMap);
                }
                paintCore.context = drawingContext;
                paintCore.g = bitMapG;
                paintCore.BeginPaint(new Rectangle(0, 0, width, height));
                eventBase.OnPaintEx(paintCore);
                paintCore.EndPaint();
                wBitmap.Lock();
                wBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
                wBitmap.Unlock();
                drawingContext.DrawImage(wBitmap, new Rect(0, 0, width, height));
            }
            //WPF绘图
            else if(writeMode ==1)
            {
                paintCore.context = drawingContext;
                paintCore.BeginPaint(new Rectangle(0, 0, width, height));
                eventBase.OnPaintEx(paintCore);
                paintCore.EndPaint();
            }
            //WPF Visual绘图
            else if (writeMode == 2)
            {
                if (_drawingVisual == null)
                {
                    _drawingVisual = new DrawingVisual();
                }
                if (rtb == null || rtb.Width != width || rtb.Height != height)
                {
                    rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
                }
                paintCore.context = _drawingVisual.RenderOpen();
                paintCore.BeginPaint(new Rectangle(0, 0, width, height));
                eventBase.OnPaintEx(paintCore);
                paintCore.EndPaint();
                rtb.Render(_drawingVisual);
                drawingContext.DrawImage(rtb, new Rect(0, 0, width, height));
                paintCore.context.Close();
                return;
            }
        }

        /// <summary>
        /// 鼠标点击次数
        /// </summary>
        private int m_clickCount = 0;

        /// <summary>
        /// 获取鼠标按钮
        /// </summary>
        /// <param name="e">鼠标参数</param>
        /// <returns>鼠标按钮</returns>
        private MouseButtons GetMouseButton(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                return MouseButtons.Left;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                return MouseButtons.Right;
            }
            else
            {
                return MouseButtons.None;
            }
        }

        /// <summary>
        /// 键盘事件扩展
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyDownEx(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (eventBase != null && eventBase == clickEventBase)
            {
                Keys key = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);
                eventBase.OnKeyDownEx(key);
            }
        }

        public void OnKeyUpEx(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (eventBase != null && eventBase == clickEventBase)
            {
                Keys key = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);
            }
        }

        /// <summary>
        /// 鼠标双击方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            m_clickCount = 2;
            if (eventBase != null)
            {
                System.Windows.Forms.MouseEventArgs newE = new System.Windows.Forms.MouseEventArgs(GetMouseButton(e), m_clickCount, (int)GetMousePoint().X, (int)GetMousePoint().Y, 0);
                eventBase.OnMouseDownEx(newE);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <returns>坐标</returns>
        public System.Drawing.Point GetMousePoint()
        {
            POINT mp = new POINT();
            GetCursorPos(out mp);
            System.Windows.Point clientPoint = PointFromScreen(new System.Windows.Point(mp.x, mp.y));
            mp.x = (int)clientPoint.X;
            mp.y = (int)clientPoint.Y;
            return new System.Drawing.Point(mp.x, mp.y);
        }

        /// <summary>
        /// 鼠标按下方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            m_clickCount = 1;
            clickEventBase = eventBase;
            Focus();
            if (eventBase != null)
            {
                System.Windows.Forms.MouseEventArgs newE = new System.Windows.Forms.MouseEventArgs(GetMouseButton(e), m_clickCount, (int)GetMousePoint().X, (int)GetMousePoint().Y, 0);
                eventBase.OnMouseDownEx(newE);
            }
        }

        /// <summary>
        /// 鼠标移动方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (eventBase != null)
            {
                System.Windows.Forms.MouseEventArgs newE = new System.Windows.Forms.MouseEventArgs(GetMouseButton(e), m_clickCount, (int)GetMousePoint().X, (int)GetMousePoint().Y, 0);
                eventBase.OnMouseMoveEx(newE);
            }
        }

        /// <summary>
        /// 鼠标抬起方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (eventBase != null)
            {
                System.Windows.Forms.MouseEventArgs newE = new System.Windows.Forms.MouseEventArgs(GetMouseButton(e), m_clickCount, (int)GetMousePoint().X, (int)GetMousePoint().Y, 0);
                eventBase.OnMouseUpEx(newE);
            }
            m_clickCount = 0;
        }

        /// <summary>
        /// 鼠标滚动方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (eventBase != null)
            {
                System.Windows.Forms.MouseEventArgs newE = new System.Windows.Forms.MouseEventArgs(GetMouseButton(e), m_clickCount, (int)GetMousePoint().X, (int)GetMousePoint().Y, e.Delta);
                eventBase.OnMouseWheelEx(newE);
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
            ResetLayout(new System.Drawing.Size((int)newSize.Width, (int)newSize.Height));
        }

        /// <summary>
        /// 重置大小
        /// </summary>
        private void ResetLayout(System.Drawing.Size size)
        {
            if (eventBase != null)
            {
                eventBase.setControlSize(new System.Drawing.Size(size.Width, size.Height));
            }
        }
    }
}
