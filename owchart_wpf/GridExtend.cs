/*
 * OWCHART证券图形控件
 * 著作权编号：2012SR088937
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using owchart;
using owchart_wpf;

namespace owchart_net {
    /// <summary>
    /// 表格扩展
    /// </summary>
    public class GridExtend : Grid, LatestDataListener {
        /// <summary>
        /// 创建表格
        /// </summary>
        public GridExtend() {
            ShowHScrollBar = true;
            ShowVScrollBar = true;
            RowStyle.HoveredBackColor = Color.Empty;
            AddColumn(new GridColumn("colP1", "股票代码", 100));
            AddColumn(new GridColumn("colP2", "股票名称", 100));
            AddColumn(new GridColumn("colP3", "最新价", 90));
            AddColumn(new GridColumn("colP4", "涨跌额", 90));
            AddColumn(new GridColumn("colP5", "涨跌幅", 90));
            AddColumn(new GridColumn("colP6", "最高价", 90));
            AddColumn(new GridColumn("colP7", "最低价", 90));
            AddColumn(new GridColumn("colP8", "开盘价", 90));
            AddColumn(new GridColumn("colP9", "成交量", 120));
            AddColumn(new GridColumn("colP10", "成交额", 110));
            AddColumn(new GridColumn("colP11", "买一价", 90));
            AddColumn(new GridColumn("colP12", "买一量", 110));
            AddColumn(new GridColumn("colP13", "卖一价", 90));
            AddColumn(new GridColumn("colP14", "卖一量", 110));
            GetColumn("colP9").CellAlign = HorizontalAlign.Left;
            GetColumn("colP10").CellAlign = HorizontalAlign.Left;
            GetColumn("colP12").CellAlign = HorizontalAlign.Left;
            GetColumn("colP14").CellAlign = HorizontalAlign.Left;
            BeginUpdate();
            //绑定数据
            foreach (Security security in SecurityService.codedMaps.Values) {
                GridRow row = new GridRow();
                AddRow(row);
                row.Height = 30;
                row.AddCell("colP1", new GridStringCell(security.m_code));
                row.AddCell("colP2", new GridStringCell(security.m_name));
                row.AddCell("colP3", new GridDoubleCell());
                row.AddCell("colP4", new GridDoubleCell());
                row.AddCell("colP5", new GridPercentCell());
                row.AddCell("colP6", new GridDoubleCell());
                row.AddCell("colP7", new GridDoubleCell());
                row.AddCell("colP8", new GridDoubleCell());
                row.AddCell("colP9", new GridLongCell());
                row.AddCell("colP10", new GridLongCell());
                row.AddCell("colP11", new GridDoubleCell());
                row.AddCell("colP12", new GridLongCell());
                row.AddCell("colP13", new GridDoubleCell());
                row.AddCell("colP14", new GridLongCell());
                rowsMap[security.m_code] = row;
                for (int i = 0; i < row.Cells.Count; i++) {
                    GridCellStyle cellStyle = new GridCellStyle();
                    if (i > 1) {
                        cellStyle.Align = HorizontalAlign.Right;
                    }
                    if (WPFPaint.BlackOrWhite)
                    {
                        cellStyle.TextColor = Color.FromArgb(255, 255, 255);
                    }
                    else
                    {
                        cellStyle.TextColor = Color.Black;
                    }

                    cellStyle.Font = new Font("微软雅黑", 12);
                    row.Cells[i].Style = cellStyle;
                }
            }
            EndUpdate();
            SecurityService.m_listener = this;
            SecurityService.Start();

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 300;
            timer.Enabled = true;
            if (!WPFPaint.BlackOrWhite)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    GridColumn gridColumn = Columns[i];
                    gridColumn.BackColor = Color.White;
                    gridColumn.TextColor = Color.Black;
                }
                BackColor = Color.White;
                GridRowStyle gridRowStyle = RowStyle;
                gridRowStyle.BackColor = Color.White;
                gridRowStyle.TextColor = Color.Black;
                gridRowStyle.SelectedBackColor = Color.FromArgb(200, 200, 200);
                gridRowStyle.HoveredBackColor = Color.Empty;
            }
        }

        public ChartExtend chartExtend;

        /// <summary>
        /// 新数据代码
        /// </summary>
        private Dictionary<String, String> newDataCodes = new Dictionary<String, String>();

        /// <summary>
        /// 行的缓存
        /// </summary>
        private Dictionary<String, GridRow> rowsMap = new Dictionary<String, GridRow>();

        /// <summary>
        /// 要更新的单元格
        /// </summary>
        private List<GridCell> updateCells = new List<GridCell>();

        /// <summary>
        /// 根据价格获取颜色
        /// </summary>
        /// <param name="price">价格</param>
        /// <param name="comparePrice">比较价格</param>
        /// <returns>颜色</returns>
        public Color GetPriceColor(double price, double comparePrice) {
            if (WPFPaint.BlackOrWhite)
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
        /// 最新数据回调
        /// </summary>
        /// <param name="code"></param>
        public void LatestDataCallBack(string code) {
            lock (newDataCodes) {
                newDataCodes[code] = "";
            }
        }

        /// <summary>
        /// 单元格点击事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnCellClick(GridCell cell, System.Windows.Forms.MouseEventArgs e) {
            base.OnCellClick(cell, e);
            if (e.Clicks == 1) {
                String code = cell.Row.GetCell("colP1").GetString();
                Security security = new Security();
                SecurityLatestData latestData = new SecurityLatestData();
                if (SecurityService.GetSecurityByCode(code, ref security) > 0 && SecurityService.GetLatestData(code, ref latestData) > 0) {
                    chartExtend.ChangeSecurity(code);
                }
                Console.WriteLine("1");
            }
        }

        /// <summary>
        /// 键盘按下事件
        /// </summary>
        /// <param name="e"></param>
        public override void OnKeyDownEx(Keys key) {
            base.OnKeyDownEx(key);
            if (key == Keys.Enter) {
                if (SelectedRows.Count > 0) {
                    String code = SelectedRows[0].GetCell("colP1").GetString();
                    Security security = new Security();
                    SecurityLatestData latestData = new SecurityLatestData();
                    if (SecurityService.GetSecurityByCode(code, ref security) > 0 && SecurityService.GetLatestData(code, ref latestData) > 0) {
                        chartExtend.ChangeSecurity(code);
                    }
                }
            }
        }

        /// <summary>
        /// 秒表方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e) {
            List<String> refreshCodes = new List<string>();
            lock (newDataCodes) {
                if (newDataCodes.Count > 0) {
                    foreach (String key in newDataCodes.Keys) {
                        refreshCodes.Add(key);
                    }
                    newDataCodes.Clear();
                }
            }
            int refreshCodesSize = refreshCodes.Count;
            if (refreshCodes.Count > 0) {
                if (updateCells.Count > 0) {
                    if (updateCells.Count > 10) {
                        for (int i = 0; i < updateCells.Count; i++) {
                            GridCell updateCell = updateCells[i];
                            updateCell.Style.BackColor = Color.Empty;
                        }
                    } else {
                        for (int i = 0; i < updateCells.Count; i++) {
                            GridCell updateCell = updateCells[i];
                            updateCell.Style.BackColor = Color.Empty;
                        }
                    }
                    updateCells.Clear();
                }
                for (int i = 0; i < refreshCodesSize; i++) {
                    GridRow row = null;
                    String code = refreshCodes[i];
                    if (rowsMap.ContainsKey(code)) {
                        row = rowsMap[code];
                        SecurityLatestData lastestData = new SecurityLatestData();
                        SecurityService.GetLatestData(code, ref lastestData);
                        double lastClose = lastestData.m_lastClose;
                        double diff = 0, diffRange = 0;
                        diff = lastestData.m_close - lastestData.m_lastClose;
                        if (lastestData.m_lastClose != 0) {
                            diffRange = diff / lastestData.m_lastClose;
                        }
                        GridCell cell3 = row.GetCell("colP3");
                        if (lastestData.m_close != cell3.GetDouble()) {
                            cell3.SetDouble(lastestData.m_close);
                            cell3.Style.TextColor = GetPriceColor(lastestData.m_close, lastClose);
                            updateCells.Add(cell3);
                        }

                        GridCell cell4 = row.GetCell("colP4");
                        if (diff != cell4.GetDouble()) {
                            cell4.SetDouble(Convert.ToDouble(LbCommon.GetValueByDigit(diff, 2, true)));
                            cell4.Style.TextColor = GetPriceColor(lastestData.m_close, lastClose);
                            updateCells.Add(cell4);
                        }

                        GridCell cell5 = row.GetCell("colP5");
                        if (diffRange != cell5.GetDouble()) {
                            cell5.SetDouble(diffRange);
                            cell5.Style.TextColor = GetPriceColor(lastestData.m_close, lastClose);
                            updateCells.Add(cell5);
                        }

                        GridCell cell6 = row.GetCell("colP6");
                        if (lastestData.m_high != cell6.GetDouble()) {
                            cell6.SetDouble(lastestData.m_high);
                            cell6.Style.TextColor = GetPriceColor(lastestData.m_high, lastClose);
                            updateCells.Add(cell6);
                        }

                        GridCell cell7 = row.GetCell("colP7");
                        if (lastestData.m_low != cell7.GetDouble()) {
                            cell7.SetDouble(lastestData.m_low);
                            cell7.Style.TextColor = GetPriceColor(lastestData.m_low, lastClose);
                            updateCells.Add(cell7);
                        }

                        GridCell cell8 = row.GetCell("colP8");
                        if (lastestData.m_open != cell8.GetDouble()) {
                            cell8.SetDouble(lastestData.m_open);
                            cell8.Style.TextColor = GetPriceColor(lastestData.m_open, lastClose);
                            updateCells.Add(cell8);
                        }

                        GridCell cell9 = row.GetCell("colP9");
                        if (lastestData.m_volume != cell9.GetDouble()) {
                            cell9.SetDouble(lastestData.m_volume);
                            updateCells.Add(cell9);
                        }

                        GridCell cell10 = row.GetCell("colP10");
                        if (lastestData.m_amount != cell10.GetDouble()) {
                            cell10.SetDouble(lastestData.m_amount);
                            updateCells.Add(cell10);
                        }

                        GridCell cell11 = row.GetCell("colP11");
                        if (lastestData.m_buyPrice1 != cell11.GetDouble()) {
                            cell11.SetDouble(lastestData.m_buyPrice1);
                            cell11.Style.TextColor = GetPriceColor(lastestData.m_buyPrice1, lastClose);
                            updateCells.Add(cell11);
                        }

                        GridCell cell12 = row.GetCell("colP12");
                        if (lastestData.m_buyVolume1 != cell12.GetDouble()) {
                            cell12.SetDouble(lastestData.m_buyVolume1);
                            updateCells.Add(cell12);
                        }

                        GridCell cell13 = row.GetCell("colP13");
                        if (lastestData.m_sellPrice1 != cell13.GetDouble()) {
                            cell13.SetDouble(lastestData.m_sellPrice1);
                            cell13.Style.TextColor = GetPriceColor(lastestData.m_sellPrice1, lastClose);
                            updateCells.Add(cell13);
                        }

                        GridCell cell14 = row.GetCell("colP14");
                        if (lastestData.m_sellVolume1 != cell14.GetDouble()) {
                            cell14.SetDouble(lastestData.m_sellVolume1);
                            updateCells.Add(cell14);
                        }
                    }
                }
                if (updateCells.Count > 0) {
                    for (int i = 0; i < updateCells.Count; i++) {
                        updateCells[i].Style.BackColor = Color.FromArgb(50, 255, 255, 255);
                    }
                    UpdateGrid();
                    paintCore.Invalidate();
                }
            }
        }

        /// <summary>
        /// 鼠标移入单元格事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnCellMouseEnter(GridCell cell, MouseEventArgs e)
        {
            base.OnCellMouseEnter(cell, e);
        }

        /// <summary>
        /// 鼠标移出单元格事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnCellMouseLeave(GridCell cell, MouseEventArgs e)
        {
            base.OnCellMouseLeave(cell, e);
        }

        /// <summary>
        /// 鼠标在单元格移动事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnCellMouseMove(GridCell cell, MouseEventArgs e)
        {
            base.OnCellMouseMove(cell, e);
        }

        /// <summary>
        /// 鼠标在单元格抬起事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnCellMouseUp(GridCell cell, MouseEventArgs e)
        {
            base.OnCellMouseUp(cell, e);
        }

        /// <summary>
        /// 鼠标在单元格按下事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnCellMouseDown(GridCell cell, MouseEventArgs e)
        {
            base.OnCellMouseDown(cell, e);
        }

        /// <summary>
        /// 选中单元格改变事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnSelectedCellsChanged()
        {
            base.OnSelectedCellsChanged();
        }

        /// <summary>
        /// 选中列改变事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnSelectedColumnsChanged()
        {
            base.OnSelectedColumnsChanged();
        }

        /// <summary>
        /// 选中行改变事件
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="e"></param>
        public override void OnSelectedRowsChanged()
        {
            base.OnSelectedRowsChanged();
        }
    }
}
