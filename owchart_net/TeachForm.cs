using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace owchart_net
{
    public partial class TeachForm : Form
    {
        public TeachForm()
        {
            InitializeComponent();
            listBox1.ItemHeight = 30;
            listBox1.Font = new Font("宋体", 10.5f, FontStyle.Regular);
            listBox1.Items.Add("owchart控件介绍");
            listBox1.Items.Add("教程一:如何创建项目和添加布局?");
            listBox1.Items.Add("教程二:如何添加K线?");
            listBox1.Items.Add("教程三:如何添加成交量?");
            listBox1.Items.Add("教程四:如何添加趋势线?");
            listBox1.Items.Add("教程五:如何加载通达信日线数据?");
            listBox1.Items.Add("教程六:如何加载通达信分钟线数据?");
            listBox1.Items.Add("教程七:如何添加画线?");
            listBox1.Items.Add("教程八:如何创建分时图?");
            listBox1.Items.Add("教程九:如何添加MACD和KDJ指标?");
            listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);
            webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484604&idx=1&sn=8efeff8a2766e105c0e3743cdaa9f838&chksm=c0571a1cf720930aa1c706a14eeed4e8e2f7a5cb990e066bce52ef53a1c84ed982d54cc9b664&scene=21#wechat_redirect");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484604&idx=1&sn=8efeff8a2766e105c0e3743cdaa9f838&chksm=c0571a1cf720930aa1c706a14eeed4e8e2f7a5cb990e066bce52ef53a1c84ed982d54cc9b664&scene=21#wechat_redirect");
                    break;
                case 1:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484624&idx=1&sn=aeace7300b7b26c8181b9f64c3917abe&chksm=c0571a70f72093666bdc6e2cb40de430939a86517f92ed529fb3fddb23beaec085936def1c78&scene=21#wechat_redirect");
                    break;
                case 2:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484634&idx=1&sn=11d2bd2863f76675961b67174e19c300&chksm=c0571a7af720936ca399dcd2d8f4a5b50f00e83a3f49720af52753c8a43ac00abbaa494413ec&scene=21#wechat_redirect");
                    break;
                case 3:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484639&idx=1&sn=22d979675dccf6fb88fdbf9facc337f7&chksm=c0571a7ff7209369df18edbf6b24ce3b98ce73322b5624e1a0cf98bec6838a589752fae77a8b&scene=21#wechat_redirect");
                    break;
                case 4:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484645&idx=1&sn=f257e27e31d6e02483eb76552c9031d2&chksm=c0571a45f72093536dbaf5e9375ebdd67164a9f47bd61721a2713cbb48b6d7034e502ecbf613&scene=21#wechat_redirect");
                    break;
                case 5:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484650&idx=1&sn=34b49b94236b6dac373db4aa0a715b82&chksm=c0571a4af720935c509ec04030c233f21052ddb0d7933602fd5935ed1df53cda276de877a502&scene=21#wechat_redirect");
                    break;
                case 6:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484658&idx=1&sn=299b229bbef4c15f2c835bc350f13d8d&chksm=c0571a52f7209344704f4b9962ef06f686bb133d2b969374ca55c6a2499293e1725320bfdc80&scene=21#wechat_redirect");
                    break;
                case 7:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484664&idx=1&sn=0815bb008161352a18f7e51ab8fe8abb&chksm=c0571a58f720934e594ec5805cbb2e5ee9f147818dc33a3103a13ad65b176395ecd4b49cabea&scene=21#wechat_redirect");
                    break;
                case 8:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484689&idx=1&sn=4e4d45716bf2b8b0aaee13534d0fa457&chksm=c0571bb1f72092a7fabf9cd842f4daefde678f34a3337837e46c13e93454aa3e1a1fc82720c8&scene=21#wechat_redirect");
                    break;
                case 9:
                    webBrowser1.Navigate("https://mp.weixin.qq.com/s?__biz=Mzg5OTIzODg2Mw==&mid=2247484696&idx=1&sn=6d925462041adb940b86c0c6297b5c35&chksm=c0571bb8f72092aed2222d0eb9f26094548db63516315e89727df084618a8146a05bb89591e4&token=523628195&lang=zh_CN#rd");
                    break;
            }
        }
    }
}