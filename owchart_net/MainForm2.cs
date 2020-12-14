using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace owchart_net
{
    public partial class MainForm2 : Form
    {
        /// <summary>
        /// Í¼ÐÎ²ã
        /// </summary>
        public ChartExtend2 chartExtend2;

        public MainForm2()
        {
            InitializeComponent();

            chartExtend2 = new ChartExtend2();
            chartExtend2.Dock = DockStyle.Fill;
            Controls.Add(chartExtend2);
        }
    }
}