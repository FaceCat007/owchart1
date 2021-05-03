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

namespace owchart_net {
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainForm : Form {
        /// <summary>
        /// 静态对象
        /// </summary>
        public static MainForm instance;

        /// <summary>
        /// 指数层
        /// </summary>
        public IndexDiv indexDiv;

        /// <summary>
        /// 最新数据层
        /// </summary>
        public LatestDiv latestDiv;

        /// <summary>
        /// 表格层
        /// </summary>
        public GridExtend gridExtend;

        /// <summary>
        /// 图形层
        /// </summary>
        public ChartExtend chartExtend;

        /// <summary>
        /// 创建窗体
        /// </summary>
        public MainForm() {
            instance = this;
            InitializeComponent();
            indexDiv = new IndexDiv();
            indexDiv.Dock = DockStyle.Fill;
            panel1.Controls.Add(indexDiv);
            indexDiv.MouseDown += new MouseEventHandler(indexDiv_MouseDown);

            latestDiv = new LatestDiv();
            latestDiv.Dock = DockStyle.Fill;
            panel5.Controls.Add(latestDiv);

            gridExtend = new GridExtend();
            gridExtend.Dock = DockStyle.Fill;
            panel4.Controls.Add(gridExtend);
            SecurityService.Start();

            chartExtend = new ChartExtend();
            chartExtend.Dock = DockStyle.Fill;
            panel6.Controls.Add(chartExtend);
            latestDiv.SecurityCode = "600000.SH";
            chartExtend.ChangeSecurity("600000.SH");

            ////浮动层
            //FloatDiv floatDiv = new FloatDiv();
            //floatDiv.GridLineColor = Color.Red;
            //floatDiv.Width = 60;
            //floatDiv.Dock = DockStyle.Right;
            //Controls.Add(floatDiv);

            //floatDiv.HeaderHeight = 0;
            //floatDiv.AddColumn(new GridColumn("1"));
            //floatDiv.UpdateGrid();

            //String[] strs = new String[] { "功能1", "功能2", "功能3", "功能4", "功能5", "功能6", "功能7", "功能8", "功能9", "功能10" };
            //for (int i = 0; i < strs.Length; i++)
            //{
            //    GridRow gridRow = new GridRow();
            //    gridRow.Height = 60;
            //    floatDiv.AddRow(gridRow);
            //    GridStringCell cell = new GridStringCell(strs[i]);
            //    cell.Style = new GridCellStyle();
            //    cell.Style.Font = new Font("微软雅黑", 12, FontStyle.Regular);
            //    gridRow.AddCell(0, cell);
            //}
            //floatDiv.UpdateGrid();
            //floatDiv.Invalidate();
            //floatDiv.CellClick += new GridCellMouseEvent(floatDiv_CellClick);
        }

        /// <summary>
        /// 单元格点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        private void floatDiv_CellClick(object sender, GridCell cell, MouseEventArgs e)
        {
            MessageBox.Show(cell.GetString());
        }

        private void indexDiv_MouseDown(object sender, MouseEventArgs e)
        {
            int width = indexDiv.Width;
            if (e.Location.X < width / 3) {
                latestDiv.SecurityCode = "000001.SH";
                chartExtend.ChangeSecurity("000001.SH");
            } else if (e.Location.X >= width / 3 && e.Location.X <= width * 2 / 3) {
                latestDiv.SecurityCode = "399001.SZ";
                chartExtend.ChangeSecurity("399001.SZ");
            } else {
                latestDiv.SecurityCode = "399006.SZ";
                chartExtend.ChangeSecurity("399006.SZ");
            }
        } 

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            Environment.Exit(0);
        }


    }
}