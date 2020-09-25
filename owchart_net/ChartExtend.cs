/*
 * OWCHART֤ȯͼ�οؼ�
 * ����Ȩ��ţ�2012SR088937
 * �Ϻ�����è��Ϣ�������޹�˾
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
    public class ChartExtend : Chart {
        /// <summary>
        /// �����ؼ�
        /// </summary>
        public ChartExtend() {
            InitControl();
            minuteDatas = GetSecurityMinuteDatas(Application.StartupPath + "\\SH600000_M.txt");
            thisTimer.Enabled = true;
            thisTimer.Tick += new EventHandler(thisTimer_Tick);
            thisTimer.Interval = 100;
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.tsmiAddIndicator = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddPlot = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGuanyu = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi5M = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi15M = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi30M = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi60M = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMinute = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBS = new System.Windows.Forms.ToolStripMenuItem();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddIndicator,
            this.tsmiAddPlot,
            this.ToolStripMenuItem,
            this.tsmiBS,
            this.tsmiGuanyu});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            // 
            // tsmiAddIndicator
            // 
            this.tsmiAddIndicator.Name = "tsmiAddIndicator";
            this.tsmiAddIndicator.Size = new System.Drawing.Size(152, 22);
            this.tsmiAddIndicator.Text = "����ָ��";
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
            this.tsmi5M,
            this.tsmi15M,
            this.tsmi30M,
            this.tsmi60M});
            this.ToolStripMenuItem.Name = "ToolStripMenuItem";
            this.ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem.Text = "�л�����";

            this.tsmiBS.Name = "ToolStripMenuItem";
            this.tsmiBS.Size = new System.Drawing.Size(152, 22);
            this.tsmiBS.Text = "�������";
            this.tsmiBS.Click += new EventHandler(tsmiBS_Click);

            this.tsmiGuanyu.Name = "ToolStripMenuItem";
            this.tsmiGuanyu.Size = new System.Drawing.Size(152, 22);
            this.tsmiGuanyu.Text = "����";
            this.tsmiGuanyu.Click += new EventHandler(tsmiGuanyu_Click);
            // 
            // tsmi5M
            // 
            this.tsmi5M.Name = "tsmi5M";
            this.tsmi5M.Size = new System.Drawing.Size(152, 22);
            this.tsmi5M.Text = "5����";
            this.tsmi5M.Click += new System.EventHandler(this.tsmi5M_Click);
            // 
            // tsmi15M
            // 
            this.tsmi15M.Name = "tsmi15M";
            this.tsmi15M.Size = new System.Drawing.Size(152, 22);
            this.tsmi15M.Text = "15����";
            this.tsmi15M.Click += new System.EventHandler(this.tsmi15M_Click);
            // 
            // tsmi30M
            // 
            this.tsmi30M.Name = "tsmi30M";
            this.tsmi30M.Size = new System.Drawing.Size(152, 22);
            this.tsmi30M.Text = "30����";
            this.tsmi30M.Click += new System.EventHandler(this.tsmi30M_Click);
            // 
            // tsmi60M
            // 
            this.tsmi60M.Name = "tsmi60M";
            this.tsmi60M.Size = new System.Drawing.Size(152, 22);
            this.tsmi60M.Text = "60����";
            this.tsmi60M.Click += new System.EventHandler(this.tsmi60M_Click);
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
        }

        /// <summary>
        /// ��ʾ�������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiBS_Click(object sender, EventArgs e) {
            if (!minuteMode) {
                if (GetShape("�������") == null) {
                    int fieldName = CTableEx.AutoField;
                    LineShape lineShape = AddLine("�������", fieldName, mainDiv);
                    lineShape.DisplayTitle = false;
                    lineShape.StyleField = CTableEx.AutoField;
                    lineShape.ColorField = CTableEx.AutoField;
                    dataSource.AddColumn(lineShape.StyleField);
                    dataSource.AddColumn(lineShape.ColorField);
                    for (int i = 0; i < DataSource.RowsCount; i++) {
                        int rdx = i % 6;
                        if (rdx == 0) {
                            DataSource.Set2(i, fieldName, DataSource.Get2(i, COLUMN_LOW));
                            DataSource.Set2(i, lineShape.StyleField, 4);
                            DataSource.Set2(i, lineShape.ColorField, Color.FromArgb(80, 255, 80).ToArgb());
                        } else if (rdx == 1) {
                            DataSource.Set2(i, fieldName, DataSource.Get2(i, COLUMN_HIGH));
                            DataSource.Set2(i, lineShape.StyleField, 5);
                            DataSource.Set2(i, lineShape.ColorField, Color.FromArgb(255, 80, 80).ToArgb());
                        } else if (rdx == 2) {
                            DataSource.Set2(i, fieldName, DataSource.Get2(i, COLUMN_LOW));
                            DataSource.Set2(i, lineShape.StyleField, 6);
                            DataSource.Set2(i, lineShape.ColorField, Color.FromArgb(80, 255, 80).ToArgb());
                        } else if (rdx == 3) {
                            DataSource.Set2(i, fieldName, DataSource.Get2(i, COLUMN_HIGH));
                            DataSource.Set2(i, lineShape.StyleField, 7);
                            DataSource.Set2(i, lineShape.ColorField, Color.FromArgb(255, 80, 80).ToArgb());
                        } else if (rdx == 4) {
                            DataSource.Set2(i, fieldName, DataSource.Get2(i, COLUMN_LOW));
                            DataSource.Set2(i, lineShape.StyleField, 8);
                            DataSource.Set2(i, lineShape.ColorField, Color.FromArgb(80, 255, 80).ToArgb());
                        } else if (rdx == 5) {
                            DataSource.Set2(i, fieldName, DataSource.Get2(i, COLUMN_HIGH));
                            DataSource.Set2(i, lineShape.StyleField, 9);
                            DataSource.Set2(i, lineShape.ColorField, Color.FromArgb(255, 80, 80).ToArgb());
                        }
                    }
                    RefreshGraph();
                }
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiGuanyu_Click(object sender, EventArgs e) {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        /// <summary>
        /// �ϴε�����
        /// </summary>
        private SecurityLatestData lastData = new SecurityLatestData();

        /// <summary>
        /// ���ӻ��߹���
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
        /// ����ָ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void indicatorButton_Click(object sender, EventArgs e) {
            ChangeIndicator((sender as ToolStripMenuItem).Text);
        }

        /// <summary>
        /// ���ӷ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiMinute_Click(object sender, EventArgs e) {
            minuteMode = true;
            ChangeSecurity(currentCode);
        }

        /// <summary>
        /// ����5������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi5M_Click(object sender, EventArgs e) {
            minuteMode = false;
            Cycle = 5;
            ChangeSecurity(currentCode);
        }

        /// <summary>
        /// ����15������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi15M_Click(object sender, EventArgs e) {
            minuteMode = false;
            Cycle = 15;
            ChangeSecurity(currentCode);
        }

        /// <summary>
        /// ����30������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi30M_Click(object sender, EventArgs e) {
            minuteMode = false;
            Cycle = 30;
            ChangeSecurity(currentCode);
        }

        /// <summary>
        /// ����60������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi60M_Click(object sender, EventArgs e) {
            minuteMode = false;
            Cycle = 60;
            ChangeSecurity(currentCode);
        }

        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void thisTimer_Tick(object sender, EventArgs e) {
            if (minuteMode) {
                if (minuteDatasPos < minuteDatas.Count) {
                    minuteDatasPos++;
                    UpdateDataToGraphMinute(minuteDatas, false);
                    Invalidate();
                }
            } else {
                SecurityLatestData newData = new SecurityLatestData();
                SecurityService.GetLatestData(currentCode, ref newData);
                if (!newData.equal(lastData) && newData.m_volume > 0) {
                    double close = newData.m_close;
                    double dVolume = 0;
                    if (lastData.m_code.Length > 0) {
                        dVolume = newData.m_volume - lastData.m_volume;
                    }
                    SecurityData securityData = new SecurityData();
                    securityData.date = (double)((long)newData.m_date / (cycle * 60) * (cycle * 60));
                    securityData.close = close;
                    if (DataSource.RowsCount > 0) {
                        if (securityData.date < DataSource.GetXValue(DataSource.RowsCount - 1)) {
                            securityData.date += (cycle * 60);
                        }
                        if (DataSource.GetXValue(DataSource.RowsCount - 1) == securityData.date) {
                            if (securityData.close > DataSource.Get2(DataSource.RowsCount - 1, COLUMN_HIGH)) {
                                securityData.high = close;
                            } else {
                                securityData.high = DataSource.Get2(DataSource.RowsCount - 1, COLUMN_HIGH);
                            }
                            if (securityData.close < DataSource.Get2(DataSource.RowsCount - 1, COLUMN_LOW)) {
                                securityData.low = close;
                            } else {
                                securityData.low = DataSource.Get2(DataSource.RowsCount - 1, COLUMN_LOW);
                            }
                            securityData.open = DataSource.Get2(DataSource.RowsCount - 1, COLUMN_OPEN);
                            double oldVolume = DataSource.Get2(DataSource.RowsCount - 1, COLUMN_VOLUME);
                            oldVolume += dVolume;
                            securityData.volume = oldVolume;

                        } else {
                            securityData.high = close;
                            securityData.low = close;
                            securityData.open = close;
                            securityData.volume = dVolume;
                        }
                    } else {
                        securityData.high = close;
                        securityData.low = close;
                        securityData.open = close;
                        securityData.volume = dVolume;
                    }
                    List<SecurityData> datas = new List<SecurityData>();
                    datas.Add(securityData);
                    UpdateDataToGraph(datas, false);
                    datas.Clear();
                    lastData = newData;
                }
            }
        }

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddIndicator;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddPlot;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiMinute;
        private System.Windows.Forms.ToolStripMenuItem tsmi5M;
        private System.Windows.Forms.ToolStripMenuItem tsmi15M;
        private System.Windows.Forms.ToolStripMenuItem tsmi30M;
        private System.Windows.Forms.ToolStripMenuItem tsmi60M;
        private System.Windows.Forms.ToolStripMenuItem tsmiGuanyu;
        private System.Windows.Forms.ToolStripMenuItem tsmiBS;


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
        /// ��������
        /// </summary>
        private List<SecurityData> minuteDatas = new List<SecurityData>();

        /// <summary>
        /// �����ߵ�λ��
        /// </summary>
        private int minuteDatasPos = 0;

        /// <summary>
        /// ���
        /// </summary>
        private System.Windows.Forms.Timer thisTimer = new Timer();

        private int cycle = 5;

        /// <summary>
        /// ����
        /// </summary>
        public int Cycle {
            get { return cycle; }
            set { cycle = value; }
        }

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
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
            base.OnMouseDown(e);
            System.Drawing.Point mp = e.Location;
            if (curPaintLine != null && curPaintLine.Length > 0) {
                ChartDiv mouseOverDiv = GetMouseOverDiv();
                if (mouseOverDiv != null) {
                    AddPlot(System.Drawing.Color.FromArgb(200, 200, 200), System.Drawing.Color.White, curPaintLine, mp, 1, null, mouseOverDiv);
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
                scaleSteps.Add(LbCommon.getDateNum(1970, 1, 1, 10, 0, 0, 0));
                scaleSteps.Add(LbCommon.getDateNum(1970, 1, 1, 10, 30, 0, 0));
                scaleSteps.Add(LbCommon.getDateNum(1970, 1, 1, 11, 0, 0, 0));
                scaleSteps.Add(LbCommon.getDateNum(1970, 1, 1, 11, 30, 0, 0));
                scaleSteps.Add(LbCommon.getDateNum(1970, 1, 1, 13, 30, 0, 0));
                scaleSteps.Add(LbCommon.getDateNum(1970, 1, 1, 14, 0, 0, 0));
                scaleSteps.Add(LbCommon.getDateNum(1970, 1, 1, 14, 30, 0, 0));
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
            } else {
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
                mainDiv.Title = cycle.ToString() + "������";
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
                IndicatorMACD indMacd = (IndicatorMACD)AddIndicator("MACD");
                indMacd.SetParam(COLUMN_CLOSE, 12, 26, 9, macdDiv);

                //IndicatorStochasticOscillator indKdj = (IndicatorStochasticOscillator)AddIndicator("KDJ");
                //indKdj.SetParam(COLUMN_CLOSE, COLUMN_HIGH, COLUMN_LOW, 9, 3, 3, kdjDiv);
                div2Indicator = indMacd;

                indicators.Add(indBoll);
                indicators.Add(indMacd);
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
        /// ��ǰ����
        /// </summary>
        private String currentCode;

        /// <summary>
        /// �ı����
        /// </summary>
        /// <param name="code"></param>
        public void ChangeSecurity(String code) {
            if (currentCode != code) {
                currentCode = code;
            }
            InitControl();
            if (minuteMode) {
                minuteDatasPos = 0;
                minuteDatas.Clear();
                minuteDatas = GetSecurityMinuteDatas(Application.StartupPath + "\\SH600000_M.txt");
                UpdateDataToGraphMinute(minuteDatas, true);
            } else {
                lastData = new SecurityLatestData();
                try {
                    List<SecurityData> datas = ChartExtend.GetSinaHistoryDatasByStr(code, cycle);
                    UpdateDataToGraph(datas, true);
                    datas.Clear();
                } catch (Exception ex) {
                }
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
                    if (data.open > data.close) {
                        DataSource.Set2(index, barVolume.StyleField, 0);
                        DataSource.Set2(index, candleShape.StyleField, 0);
                        DataSource.Set2(index, barVolume.ColorField, System.Drawing.Color.FromArgb(80, 255, 255).ToArgb());
                    } else {
                        DataSource.Set2(index, barVolume.StyleField, 1);
                        DataSource.Set2(index, candleShape.StyleField, 1);
                        DataSource.Set2(index, barVolume.ColorField, System.Drawing.Color.FromArgb(255, 80, 80).ToArgb());
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
        public void UpdateDataToGraphMinute(List<SecurityData> list, bool empty) {
            System.Drawing.Color color = System.Drawing.Color.SkyBlue;
            BarShape barVolume = GetShape("�ɽ���") as BarShape;
            int endIndex = list.Count;
            if (!empty) {
                endIndex = minuteDatasPos;
            }
            for (int i = 0; i < endIndex; i++) {
                SecurityData data = list[i];
                bool isFirst = i == 0;
                double date = data.date;
                if (empty) {
                    DataSource.Set(date, COLUMN_VOLUME, double.NaN);
                    DataSource.Set(date, COLUMN_CLOSE, double.NaN);
                } else {
                    DataSource.Set(date, COLUMN_VOLUME, data.volume);
                    DataSource.Set(date, COLUMN_CLOSE, data.close);
                }
                if (isFirst) {
                    LastClose = data.open;
                    if (empty) {
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
        /// ����ָ�깫ʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeIndicator(String text) {
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
                        (indicator as IndicatorStopAndReveres).SetParam(COLUMN_HIGH, COLUMN_LOW, 5, 0.02, 0.2, macdDiv);
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

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.F5) {
                minuteMode = !minuteMode;
                ChangeSecurity(currentCode);
            }
        }
    }
}