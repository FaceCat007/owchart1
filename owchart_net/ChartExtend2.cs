/*
 * OWCHART֤ȯͼ�οؼ�
 * ����Ȩ��ţ�2012SR088937
 * �Ϻ����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using owchart;

namespace owchart_net {
    /// <summary>
    /// K�߿ؼ�
    /// </summary>
    public class ChartExtend2 : Chart {
        /// <summary>
        /// �����ؼ�
        /// </summary>
        public ChartExtend2() {
            InitControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.tsmiAddIndicator = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddPlot = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDay = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMinute = new System.Windows.Forms.ToolStripMenuItem();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddIndicator,
            this.tsmiAddPlot,
            this.ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            // 
            // tsmiAddIndicator
            // 
            this.tsmiAddIndicator.Name = "tsmiAddIndicator";
            this.tsmiAddIndicator.Size = new System.Drawing.Size(152, 22);
            this.tsmiAddIndicator.Text = "���ָ��";
            // 
            // tsmiAddPlot
            // 
            this.tsmiAddPlot.Name = "tsmiAddPlot";
            this.tsmiAddPlot.Size = new System.Drawing.Size(152, 22);
            this.tsmiAddPlot.Text = "���߹���";
            // 
            // ToolStripMenuItem
            // 
            this.ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMinute,
            this.tsmiDay});
            this.ToolStripMenuItem.Name = "ToolStripMenuItem";
            this.ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem.Text = "�л�����";
            // 
            // tsmi60M
            // 
            this.tsmiDay.Name = "tsmiDay";
            this.tsmiDay.Size = new System.Drawing.Size(152, 22);
            this.tsmiDay.Text = "����";
            this.tsmiDay.Click += new System.EventHandler(this.tsmiDay_Click);
            // 
            // tsmiMinute
            // 
            this.tsmiMinute.Name = "tsmiMinute";
            this.tsmiMinute.Size = new System.Drawing.Size(152, 22);
            this.tsmiMinute.Text = "��ʱͼ";
            this.tsmiMinute.Click += new System.EventHandler(this.tsmiMinute_Click);

            //����ָ��
            foreach (string enumName in GetSystemIndicators()) {
                ToolStripMenuItem indicatorButton = new ToolStripMenuItem(enumName);
                indicatorButton.Click += new EventHandler(indicatorButton_Click);
                tsmiAddIndicator.DropDownItems.Add(indicatorButton);
            }

            ToolStripMenuItem deleteButton = new ToolStripMenuItem();
            deleteButton.Text = "[ɾ����ѡ]";
            deleteButton.Width = 100;
            deleteButton.Click += new EventHandler(plotButton_Click);
            deleteButton.Tag = "DELETE";
            tsmiAddPlot.DropDownItems.Add(deleteButton);
            //���ػ��߹���
            foreach (string enumName in GetSystemPlots()) {
                PlotBase plb = CreatePlot(enumName);
                if (plb != null) {
                    ToolStripMenuItem plotButton = new ToolStripMenuItem();
                    plotButton.Text = plb.Desc;
                    plotButton.Width = 100;
                    plotButton.Click += new EventHandler(plotButton_Click);
                    plotButton.Tag = enumName;
                    tsmiAddPlot.DropDownItems.Add(plotButton);
                }
            }
            ContextMenuStrip = contextMenuStrip1;
            ChangeCycle();

            UseNewRectangle = true;
        }

        /// <summary>
        /// ָ������
        /// </summary>
        private String indicatorName = "MACD";

        /// <summary>
        /// ��ӻ��߹���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plotButton_Click(object sender, EventArgs e) {
            curPaintLine = (sender as ToolStripMenuItem).Tag.ToString();
            if (curPaintLine == "DELETE") {
                if (SelectedPlot != null) {
                    DeletePlot(SelectedPlot);
                    RefreshGraph();
                }
                curPaintLine = "";
            }
        }

        /// <summary>
        /// ���ָ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void indicatorButton_Click(object sender, EventArgs e) {
            ChangeIndicator((sender as ToolStripMenuItem).Text);
        }

        /// <summary>
        /// ��ӷ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiMinute_Click(object sender, EventArgs e) {
            minuteMode = true;
            ChangeCycle();
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiDay_Click(object sender, EventArgs e) {
            minuteMode = false;
            ChangeCycle();
        }

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddIndicator;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddPlot;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiMinute;
        private System.Windows.Forms.ToolStripMenuItem tsmiDay;


        /// <summary>
        /// ��ǰ��ѡ�еĻ��߹���
        /// </summary>
        public string curPaintLine = string.Empty;

        //�Ƿ��ʱ��
        public bool minuteMode = false;

        /// <summary>
        /// ָ��
        /// </summary>
        private List<BaseIndicator> indicators = new List<BaseIndicator>();

        /// <summary>
        /// ���ݴ����ȡ������ʷ����
        /// </summary>
        /// <param name="code"></param>
        public static List<SecurityData> GetSinaHistoryDatasByStr(String code, int cycle) {
            String str = SecurityService.GetSinaHistoryDatasStrByCode(code, cycle);
            List<SecurityData> datas = JsonConvert.DeserializeObject<List<SecurityData>>(str);
            for (int i = 0; i < datas.Count; i++) {
                SecurityData securityData = datas[i];
                securityData.date = ((TimeSpan)(Convert.ToDateTime(datas[i].day) - new DateTime(1970, 1, 1))).TotalSeconds;
            }
            return datas;
        }

        /// <summary>
        /// ��갴���¼�
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseDownEx(System.Windows.Forms.MouseEventArgs e) {
            base.OnMouseDownEx(e);
            System.Drawing.Point mp = e.Location;
            if (curPaintLine != null && curPaintLine.Length > 0) {
                ChartDiv mouseOverDiv = GetMouseOverDiv();
                if (mouseOverDiv != null) {
                    if (Program.BlackOrWhite)
                    {
                        AddPlot(System.Drawing.Color.FromArgb(200, 200, 200), System.Drawing.Color.White, curPaintLine, mp, 1, null, mouseOverDiv);
                    }
                    else
                    {
                        AddPlot(System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.Black, curPaintLine, mp, 1, null, mouseOverDiv);
                    }
                    Cursor = System.Windows.Forms.Cursors.Default;
                    curPaintLine = "";
                    RefreshGraph();
                }
            }
        }

        /// <summary>
        /// ��ʼ���ؼ�
        /// </summary>
        public void InitControl() {
            indicators.Clear();
            RemoveAll();
            if (minuteMode) {
                AllowDrag = false;
                DataSource.SetColsCapacity(20);
                IsMinute = true;
                AutoFillXScale = true;
                ScrollAddSpeed = true;
                XFieldText = "����";
                CanMoveShape = true;
                LeftYScaleWidth = 80;
                RightYScaleWidth = 80;
                XScalePixel = 21;
                mainDiv = AddChartDiv(70);
                mainDiv.ShowVGrid = true;
                MinuteLineMax = 0;
                MinuteLineMin = 0;
                mainDiv.LeftYScale.ScaleType = YScaleType.EqualRatio;
                mainDiv.RightYScale.ScaleType = YScaleType.EqualRatio;
                mainDiv.Title = "��ʱ��";
                mainDiv.XScale.Visible = false;
                List<double> scaleSteps = new List<double>();
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 10, 0, 0, 0));
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 10, 30, 0, 0));
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 11, 0, 0, 0));
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 11, 30, 0, 0));
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 13, 30, 0, 0));
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 14, 0, 0, 0));
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 14, 30, 0, 0));
                scaleSteps.Add(LbCommon.GetDateNum(1970, 1, 1, 15, 0, 0, 0));
                mainDiv.XScale.ScaleSteps = scaleSteps;
                mainDiv.PaddingBottom = 10;
                mainDiv.PaddingTop = 10;
                LineShape lineShape = AddLine("��ʱ��", COLUMN_CLOSE, mainDiv);
                lineShape.LineColor = Color.White;
                //�ɽ���
                volumeDiv = AddChartDiv(30);
                volumeDiv.LeftYScale.Distance = 60;
                volumeDiv.Title = "�ɽ���";
                volumeDiv.RightYScale.Distance = 60;
                volumeDiv.XScale.ScaleSteps = scaleSteps;
                BarShape barShape = AddBar("�ɽ���", COLUMN_VOLUME, volumeDiv);
                volumeDiv.XScale.Format = "hh:mm";
                barShape.Title = "";
                barShape.BarStyle = BarStyle.Line;
                SetBar("�ɽ���", System.Drawing.Color.FromArgb(255, 255, 80), System.Drawing.Color.FromArgb(255, 255, 80));
                if (!Program.BlackOrWhite)
                {
                    mainDiv.ForeColor = Color.Black;
                    mainDiv.BackColor = Color.White;
                    mainDiv.LeftYScale.TipForeColor = Color.White;
                    mainDiv.RightYScale.TipForeColor = Color.White;
                    volumeDiv.LeftYScale.TipForeColor = Color.White;
                    volumeDiv.RightYScale.TipForeColor = Color.White;
                    volumeDiv.XScale.TipForeColor = Color.White;
                    volumeDiv.BackColor = Color.White;
                    mainDiv.BorderColor = Color.Black;
                    volumeDiv.BorderColor = Color.Black;
                    macdDiv.BorderColor = Color.Black;
                    mainDiv.LeftYScale.ForeColor = Color.Black;
                    mainDiv.RightYScale.ForeColor = Color.Black;
                    mainDiv.LeftYScale.ScaleColor = Color.Black;
                    mainDiv.RightYScale.ScaleColor = Color.Black;
                    volumeDiv.LeftYScale.ForeColor = Color.Black;
                    volumeDiv.RightYScale.ForeColor = Color.Black;
                    volumeDiv.LeftYScale.ScaleColor = Color.Black;
                    volumeDiv.RightYScale.ScaleColor = Color.Black;
                    mainDiv.GridColor = Color.Black;
                    volumeDiv.GridColor = Color.Black;
                    lineShape.LineColor = Color.Black;
                    barShape.UpColor = Color.Black;
                    barShape.DownColor = Color.Black;
                    volumeDiv.XScale.ForeColor = Color.Black;
                    volumeDiv.XScale.ScaleColor = Color.Black;
                }
            } else {
                AllowDrag = true;
                DataSource.SetColsCapacity(20);
                IsMinute = false;
                AutoFillXScale = false;
                ScrollAddSpeed = true;
                XFieldText = "����";
                CanMoveShape = true;
                LeftYScaleWidth = 80;
                RightYScaleWidth = 80;
                XScalePixel = 11;
                mainDiv = AddChartDiv(60);
                mainDiv.Title = "����";
                mainDiv.XScale.Visible = false;
                mainDiv.PaddingBottom = 10;
                mainDiv.PaddingTop = 10;
                //mainDiv.LeftYScale.System = VScaleSystem.Logarithmic;
                mainDiv.RightYScale.ScaleType = YScaleType.Percent;
                CandleShape candleShape = AddCandle("K��", COLUMN_OPEN, COLUMN_HIGH, COLUMN_LOW, COLUMN_CLOSE, mainDiv);
                candleShape.UpColor = Color.FromArgb(255, 80, 80);
                candleShape.DownColor = Color.FromArgb(80, 255, 255);
                candleShape.CandleStyle = CandleStyle.CloseLine;
                candleShape.StyleField = CTableEx.AutoField;
                candleShape.ColorField = CTableEx.AutoField;
                dataSource.AddColumn(candleShape.StyleField);
                dataSource.AddColumn(candleShape.ColorField);
                IndicatorMovingAverage indBoll = (IndicatorMovingAverage)AddIndicator("MA");
                indBoll.SetParam(COLUMN_CLOSE, mainDiv);
                //�ɽ���
                volumeDiv = AddChartDiv(15);
                volumeDiv.XScale.Visible = false;
                volumeDiv.LeftYScale.Magnitude = 10000;
                volumeDiv.RightYScale.Magnitude = 10000;
                volumeDiv.LeftYScale.Digit = 0;
                volumeDiv.RightYScale.Digit = 0;
                BarShape barShape = AddBar("�ɽ���", COLUMN_VOLUME, volumeDiv);
                barShape.Title = "�ɽ���";
                barShape.BarStyle = BarStyle.Bar;
                barShape.Digit = 0;
                barShape.StyleField = CTableEx.AutoField;
                barShape.ColorField = CTableEx.AutoField;
                dataSource.AddColumn(barShape.StyleField);
                dataSource.AddColumn(barShape.ColorField);
                SetBar("�ɽ���", System.Drawing.Color.FromArgb(255, 255, 80), System.Drawing.Color.FromArgb(125, 206, 235));
                macdDiv = AddChartDiv(25);
                macdDiv.XScale.Format = "hh:mm";
                div2Indicator = ChangeIndicator(indicatorName);
                indicators.Add(indBoll);
                indicators.Add(div2Indicator);
                if (!Program.BlackOrWhite)
                {
                    mainDiv.LeftYScale.TipForeColor = Color.White;
                    mainDiv.RightYScale.TipForeColor = Color.White;
                    volumeDiv.LeftYScale.TipForeColor = Color.White;
                    volumeDiv.RightYScale.TipForeColor = Color.White;
                    mainDiv.ForeColor = Color.Black;
                    mainDiv.BackColor = Color.White;
                    volumeDiv.BackColor = Color.White;
                    macdDiv.BackColor = Color.White;
                    mainDiv.BorderColor = Color.Black;
                    volumeDiv.BorderColor = Color.Black;
                    macdDiv.BorderColor = Color.Black;
                    mainDiv.LeftYScale.ForeColor = Color.Black;
                    mainDiv.RightYScale.ForeColor = Color.Black;
                    mainDiv.LeftYScale.ScaleColor = Color.Black;
                    mainDiv.RightYScale.ScaleColor = Color.Black;
                    volumeDiv.LeftYScale.ForeColor = Color.Black;
                    volumeDiv.RightYScale.ForeColor = Color.Black;
                    volumeDiv.LeftYScale.ScaleColor = Color.Black;
                    volumeDiv.RightYScale.ScaleColor = Color.Black;
                    macdDiv.LeftYScale.ForeColor = Color.Black;
                    macdDiv.RightYScale.ForeColor = Color.Black;
                    macdDiv.LeftYScale.ScaleColor = Color.Black;
                    macdDiv.RightYScale.ScaleColor = Color.Black;
                    macdDiv.XScale.ForeColor = Color.Black;
                    macdDiv.XScale.ScaleColor = Color.Black;
                    mainDiv.GridColor = Color.Black;
                    volumeDiv.GridColor = Color.Black;
                    macdDiv.GridColor = Color.Black;

                    candleShape.UpColor = Color.Black;
                    candleShape.DownColor = Color.Black;
                    barShape.UpColor = Color.Black;
                    barShape.DownColor = Color.Black;

                    candleShape.HighTitleColor = Color.Black;
                    candleShape.LowTitleColor = Color.Black;
                    candleShape.CloseTitleColor = Color.Black;
                    candleShape.OpenTitleColor = Color.Black;
                    List<BaseShape> shapes = indBoll.GetShapeList();
                    for (int i = 0; i < shapes.Count; i++)
                    {
                        BaseShape bShape = shapes[i] as BaseShape;
                        if (bShape is BarShape)
                        {
                            (bShape as BarShape).DownColor = Color.Black;
                            (bShape as BarShape).UpColor = Color.Black;
                        }
                        else if (bShape is LineShape)
                        {
                            (bShape as LineShape).LineColor = Color.Black;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �ڶ���ͼ���ָ��
        /// </summary>
        private BaseIndicator div2Indicator;

        /// <summary>
        /// ��ͼ��
        /// </summary>
        private ChartDiv mainDiv;
        /// <summary>
        /// �ɽ�����
        /// </summary>
        private ChartDiv volumeDiv;

        /// <summary>
        /// ָ��2��
        /// </summary>
        private ChartDiv macdDiv;

        public static int COLUMN_VOLUME = CTableEx.AutoField;
        public static int COLUMN_OPEN = CTableEx.AutoField;
        public static int COLUMN_CLOSE = CTableEx.AutoField;
        public static int COLUMN_HIGH = CTableEx.AutoField;
        public static int COLUMN_LOW = CTableEx.AutoField;

        /// <summary>
        /// �ı����
        /// </summary>
        public void ChangeCycle() {
            InitControl();
            if (minuteMode) {
                List<SecurityData> minuteDatas = GetSecurityMinuteDatas(Application.StartupPath + "\\SH600000_M.txt");
                UpdateDataToGraphMinute(minuteDatas, minuteDatas[minuteDatas.Count - 1], true);
            } else {
                List<SecurityData> dayDatas = GetSecurityDatas(Application.StartupPath + "\\SH600000.txt");
                UpdateDataToGraph(dayDatas, true);
            }
        }

        /// <summary>
        /// �������ݵ�ͼ��
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateDataToGraph(List<SecurityData> list, bool clear) {
            if (clear) {
                DataSource.Clear();
            }
            BarShape barVolume = GetShape("�ɽ���") as BarShape;
            CandleShape candleShape = GetShape("K��") as CandleShape;
            int startIndex = DataSource.RowsCount;
            if (startIndex < 0) {
                startIndex = 0;
            }
            for (int i = 0; i < list.Count; i++) {
                SecurityData data = list[i];
                if (!double.IsNaN(data.close)) {
                    DataSource.Set(data.date, COLUMN_VOLUME, data.volume);
                    int index = DataSource.GetRowIndex(data.date);
                    DataSource.Set2(index, COLUMN_OPEN, data.open);
                    DataSource.Set2(index, COLUMN_HIGH, data.high);
                    DataSource.Set2(index, COLUMN_LOW, data.low);
                    DataSource.Set2(index, COLUMN_CLOSE, data.close);
                    if (data.open > data.close)
                    {
                        DataSource.Set2(index, barVolume.StyleField, 0);
                        DataSource.Set2(index, candleShape.StyleField, 0);
                        if (Program.BlackOrWhite)
                        {
                            DataSource.Set2(index, barVolume.ColorField, System.Drawing.Color.FromArgb(80, 255, 255).ToArgb());
                        }
                        else
                        {
                            DataSource.Set2(index, barVolume.ColorField, System.Drawing.Color.FromArgb(0, 0, 0).ToArgb());
                        }

                    }
                    else
                    {
                        DataSource.Set2(index, barVolume.StyleField, 1);
                        DataSource.Set2(index, candleShape.StyleField, 1);
                        if (Program.BlackOrWhite)
                        {
                            DataSource.Set2(index, barVolume.ColorField, System.Drawing.Color.FromArgb(255, 80, 80).ToArgb());
                        }
                        else
                        {
                            DataSource.Set2(index, barVolume.ColorField, System.Drawing.Color.FromArgb(0, 0, 0).ToArgb());
                        }
                    }
                }
            }

            int indicatorsSize = indicators.Count;
            for (int i = 0; i < indicatorsSize; i++) {
                indicators[i].OnCalculate(startIndex);
            }
            RefreshGraph();
        }

        /// <summary>
        /// �������ݵ�ͼ��
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateDataToGraphMinute(List<SecurityData> list, SecurityData latestData, bool clear)
        {
            if (clear)
            {
                DataSource.Clear();
            }
            System.Drawing.Color color = System.Drawing.Color.SkyBlue;
            BarShape barVolume = GetShape("�ɽ���") as BarShape;
            for (int i = 0; i < list.Count; i++)
            {
                SecurityData data = list[i];
                if (latestData != null)
                {
                    if (data.date > latestData.date)
                    {
                        continue;
                    }
                }
                bool isFirst = i == 0;
                double date = data.date;
                if (latestData == null)
                {
                    DataSource.Set(date, COLUMN_VOLUME, double.NaN);
                    DataSource.Set(date, COLUMN_CLOSE, double.NaN);
                } else {
                    DataSource.Set(date, COLUMN_VOLUME, data.volume);
                    DataSource.Set(date, COLUMN_CLOSE, data.close);
                }
                if (isFirst) {
                    LastClose = data.open;
                    if (latestData == null)
                    {
                        DataSource.Set(date, COLUMN_VOLUME, LastClose);
                        DataSource.Set(date, COLUMN_CLOSE, LastClose);
                    }
                }
            }
            int indicatorsSize = indicators.Count;
            for (int i = 0; i < indicatorsSize; i++) {
                indicators[i].OnCalculate(0);
            }
            RefreshGraph();
        }

        /// <summary>
        /// ���ָ�깫ʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public BaseIndicator ChangeIndicator(String text)
        {
            indicatorName = text;
            BaseIndicator indicator = AddIndicator(text);
            if (div2Indicator != null) {
                DeleteIndicator(div2Indicator);
                switch (text) {
                    case "ASI":
                        (indicator as IndicatorAccumulationSwingIndex).SetParam(COLUMN_OPEN, COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 6, macdDiv);
                        break;
                    case "ADTM":
                        (indicator as IndicatorADTM).SetParam(COLUMN_OPEN, COLUMN_HIGH, COLUMN_LOW, 23, 8, macdDiv);
                        break;
                    case "ATR":
                        (indicator as IndicatorAverageTrueRange).SetParam(COLUMN_HIGH, COLUMN_LOW, 14, macdDiv);
                        break;
                    case "BBI":
                        (indicator as IndicatorBullandBearIndex).SetParam(COLUMN_CLOSE, 3, 6, 12, 24, macdDiv);
                        break;
                    case "BIAS":
                        (indicator as IndicatorBIAS).SetParam(COLUMN_CLOSE, 6, macdDiv);
                        break;
                    case "BOLL":
                        (indicator as IndicatorBollingerBands).SetParam(COLUMN_CLOSE, 20, 2, macdDiv);
                        break;
                    case "CCI":
                        (indicator as IndicatorCommodityChannelIndex).SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 14, macdDiv);
                        break;
                    case "CHAIKIN":
                        (indicator as IndicatorChaikinOscillator).SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, COLUMN_VOLUME, 10, 20, 6, macdDiv);
                        break;
                    case "DDI":
                        (indicator as IndicatorDirectionDeviationIndex).SetParam(COLUMN_HIGH, COLUMN_LOW, 13, 30, 10, 5, macdDiv);
                        break;
                    case "DMA":
                        (indicator as IndicatorDifferentOfMovingAverage).SetParam(COLUMN_CLOSE, 10, 50, 10, macdDiv);
                        break;
                    case "DMI":
                        (indicator as IndicatorDirectionalMovementIndex).SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 14, 6, macdDiv);
                        break;
                    case "DPO":
                        (indicator as IndicatorDetrendedPriceOscillator).SetParam(COLUMN_CLOSE, 20, 11, 6, macdDiv);
                        break;
                    case "EMA":
                        (indicator as IndicatorExponentialMovingAverage).SetParam(COLUMN_CLOSE, 5, macdDiv);
                        break;
                    case "KDJ":
                        (indicator as IndicatorStochasticOscillator).SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 9, 3, 3, macdDiv);
                        break;
                    case "LWR":
                        (indicator as IndicatorLWR).SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 9, 3, 3, macdDiv);
                        break;
                    case "MACD":
                        (indicator as IndicatorMACD).SetParam(COLUMN_CLOSE, 12, 26, 9, macdDiv);
                        break;
                    case "MASS":
                        (indicator as IndicatorMassIndex).SetParam(COLUMN_HIGH, COLUMN_LOW, 25, 9, macdDiv);
                        break;
                    case "MTM":
                        (indicator as IndicatorMomentumIndex).SetParam(COLUMN_CLOSE, 12, 6, macdDiv);
                        break;
                    case "NVI":
                        (indicator as IndicatorNegativeVolumeIndex).SetParam(COLUMN_VOLUME, COLUMN_CLOSE, 72, macdDiv);
                        break;
                    case "OBV":
                        (indicator as IndicatorOnBalanceVolume).SetParam(COLUMN_CLOSE, COLUMN_VOLUME, macdDiv);
                        break;
                    case "OSC":
                        (indicator as IndicatorOscillator).SetParam(COLUMN_CLOSE, 10, 6, macdDiv);
                        break;
                    case "PBX":
                        (indicator as IndicatorPBX).SetParam(COLUMN_CLOSE, 4, macdDiv);
                        break;
                    case "PSY":
                        (indicator as IndicatorPsychologicalLine).SetParam(COLUMN_CLOSE, 12, macdDiv);
                        break;
                    case "PVI":
                        (indicator as IndicatorPositiveVolumeIndex).SetParam(COLUMN_VOLUME, COLUMN_CLOSE, 72, macdDiv);
                        break;
                    case "ROC":
                        (indicator as IndicatorRateOfChange).SetParam(COLUMN_CLOSE, 12, 6, macdDiv);
                        break;
                    case "RSI":
                        (indicator as IndicatorRelativeStrengthIndex).SetParam(COLUMN_CLOSE, 6, macdDiv);
                        break;
                    case "SAR":
                        (indicator as IndicatorStopAndReveres).SetParam(COLUMN_HIGH, COLUMN_LOW, 4, 2, 20, macdDiv);
                        break;
                    case "SD":
                        (indicator as IndicatorStandardDeviation).SetParam(COLUMN_CLOSE, 14, 2, macdDiv);
                        break;
                    case "SLOWKD":
                        (indicator as IndicatorSlowStochasticOscillator).SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 9, 3, 3, 3, macdDiv);
                        break;
                    case "MA":
                        (indicator as IndicatorMovingAverage).SetParam(COLUMN_CLOSE, macdDiv);
                        break;
                    case "SMA":
                        (indicator as IndicatorSimpleMovingAverage).SetParam(COLUMN_CLOSE, 5, 1, macdDiv);
                        break;
                    case "TRIX":
                        (indicator as IndicatorTripleExponentiallySmoothedMovingAverage).SetParam(COLUMN_CLOSE, 12, 12, macdDiv);
                        break;
                    case "VR":
                        (indicator as IndicatorVolumeRatio).SetParam(COLUMN_CLOSE, COLUMN_VOLUME, 26, 6, macdDiv);
                        break;
                    case "WR":
                        (indicator as IndicatorWilliamsAndRate).SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 10, macdDiv);
                        break;
                    case "WVAD":
                        (indicator as IndicatorWVAD).SetParam(COLUMN_VOLUME, COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, COLUMN_OPEN, 24, 6, macdDiv);
                        break;
                }
                indicators.Remove(indicator);
                div2Indicator = indicator;
                indicators.Add(div2Indicator);
                div2Indicator.OnCalculate(0);
                RefreshGraph();
            }
            if (!Program.BlackOrWhite)
            {
                List<BaseShape> shapes = indicator.GetShapeList();
                for (int i = 0; i < shapes.Count; i++)
                {
                    BaseShape bShape = shapes[i] as BaseShape;
                    if (bShape is BarShape)
                    {
                        (bShape as BarShape).DownColor = Color.Black;
                        (bShape as BarShape).UpColor = Color.Black;
                    }
                    else if (bShape is LineShape)
                    {
                        (bShape as LineShape).LineColor = Color.Black;
                    }
                }
            }
            return indicator;
        }

        /// <summary>
        /// ��ȡ��ʱ������
        /// </summary>
        /// <returns></returns>
        public static List<SecurityData> GetSecurityMinuteDatas(String path) {
            List<SecurityData> datas = new List<SecurityData>();
            String appPath = Application.StartupPath;
            String filePath = path;
            String content = File.ReadAllText(filePath, Encoding.Default);
            String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int strsSize = strs.Length;
            for (int i = 2; i < strs.Length - 1; i++) {
                String str = strs[i];
                String[] subStrs = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                SecurityData securityData = new SecurityData();
                int hour = Convert.ToInt32(subStrs[1].Substring(0, 2));
                int minute = Convert.ToInt32(subStrs[1].Substring(2, 2));
                DateTime dayDate = Convert.ToDateTime(subStrs[0]);
                securityData.date = (new DateTime(1970, 1, 1, hour, minute, 0) - new DateTime(1970, 1, 1)).TotalSeconds;
                securityData.open = Convert.ToDouble(subStrs[2]);
                securityData.high = Convert.ToDouble(subStrs[3]);
                securityData.low = Convert.ToDouble(subStrs[4]);
                securityData.close = Convert.ToDouble(subStrs[5]);
                securityData.volume = Convert.ToDouble(subStrs[6]);
                securityData.amount = Convert.ToDouble(subStrs[7]);
                if (securityData.close <= 0) {
                    continue;
                }
                datas.Add(securityData);
            }
            return datas;
        }

        public override Color GetPriceColor(double value1, double value2)
        {
            if (Program.BlackOrWhite)
            {
                if (value1 >= value2)
                {
                    return Color.FromArgb(255, 80, 80);
                }
                else
                {
                    return Color.FromArgb(80, 255, 80);
                }
            }
            else
            {
                return Color.Black;
            }
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <returns></returns>
        public static List<SecurityData> GetSecurityDatas(String path)
        {
            List<SecurityData> datas = new List<SecurityData>();
            String appPath = Application.StartupPath;
            String filePath = path;
            String content = File.ReadAllText(filePath, Encoding.Default);
            String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int strsSize = strs.Length;
            for (int i = 2; i < strs.Length - 1; i++)
            {
                String str = strs[i];
                String[] subStrs = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                SecurityData securityData = new SecurityData();
                DateTime dayDate = Convert.ToDateTime(subStrs[0]);
                securityData.date = (dayDate - new DateTime(1970, 1, 1)).TotalSeconds;
                securityData.open = Convert.ToDouble(subStrs[1]);
                securityData.high = Convert.ToDouble(subStrs[2]);
                securityData.low = Convert.ToDouble(subStrs[3]);
                securityData.close = Convert.ToDouble(subStrs[4]);
                securityData.volume = Convert.ToDouble(subStrs[5]);
                securityData.amount = Convert.ToDouble(subStrs[6]);
                if (securityData.close <= 0)
                {
                    continue;
                }
                datas.Add(securityData);
            }
            return datas;
        }
    }
}
