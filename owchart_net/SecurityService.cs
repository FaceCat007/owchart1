/*
 * OWCHART证券图形控件
 * 著作权编号：2012SR088937
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace owchart_net {
    /// <summary>
    /// 最新数据回调
    /// </summary>
    public interface LatestDataListener {
        /// <summary>
        /// 回调接口
        /// </summary>
        /// <param name="code"></param>
        void LatestDataCallBack(String code);
    }

    /// <owgrid>
    /// 股票服务
    /// </summary>
    public class SecurityService {
        /// <summary>
        /// 代码表字典
        /// </summary>
        public static Dictionary<String, Security> codedMaps = new Dictionary<String, Security>();

        /// <summary>
        /// 最新数据
        /// </summary>
        public static Dictionary<String, SecurityLatestData> latestDatasCache = new Dictionary<String, SecurityLatestData>();

        /// <summary>
        /// 主框架
        /// </summary>
        public static LatestDataListener m_listener;

        /// <summary>
        /// 上证交易时间
        /// </summary>
        public static double shTradeTime;

        /// <summary>
        /// 通过代码返回合约信息
        /// </summary>
        /// <param name="code">合约代码</param>
        /// <param name="sd">out 股票基本信息</param>
        public static int GetSecurityByCode(String code, ref Security security) {
            int ret = 0;
            foreach (String key in codedMaps.Keys) {
                if (key == code) {
                    security = codedMaps[key];
                    ret = 1;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="latestData">最新数据</param>
        /// <returns>状态</returns>
        public static int GetLatestData(String code, ref SecurityLatestData latestData) {
            int state = 0;
            lock (latestDatasCache) {
                if (latestDatasCache.ContainsKey(code)) {
                    latestData.copy(latestDatasCache[code]);
                    state = 1;
                }
            }
            return state;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public static void Load() {
            //加载代码表//step 1
            codesTextCache = "";
            if (codedMaps.Count == 0) {
                String content = File.ReadAllText(Application.StartupPath + "\\codes.txt", Encoding.Default);
                String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String str in strs) {
                    String[] substrs = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    Security security = new Security();
                    security.m_code = substrs[0];
                    security.m_name = substrs[1];
                    codedMaps[security.m_code] = security;
                    codesTextCache += security.m_code;
                    codesTextCache += ",";
                }
                Console.WriteLine("1");
            }
        }

        /// <summary>
        /// 代码列表
        /// </summary>
        private static String codesTextCache;

        /// <summary>
        /// 开始工作
        /// </summary>
        private static void RunWork() {
            while (true) {
                if (codesTextCache != null && codesTextCache.Length > 0) {
                    if (codesTextCache.EndsWith(",")) {
                        codesTextCache.Remove(codesTextCache.Length - 1);
                    }
                    String[] strCodes = codesTextCache.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    int codesSize = strCodes.Length;
                    String latestCodes = "";
                    for (int i = 0; i < codesSize; i++) {
                        latestCodes += strCodes[i];
                        if (i == codesSize - 1 || (i > 0 && i % 50 == 0)) {
                            String latestDatasResult = GetSinaLatestDatasStrByCodes(latestCodes);
                            if (latestDatasResult != null && latestDatasResult.Length > 0) {
                                List<SecurityLatestData> latestDatas = new List<SecurityLatestData>();
                                GetLatestDatasBySinaStr(latestDatasResult, 0, latestDatas);
                                String[] subStrs = latestDatasResult.Split(new String[] { ";\n" }, StringSplitOptions.RemoveEmptyEntries);
                                int latestDatasSize = latestDatas.Count;
                                for (int j = 0; j < latestDatasSize; j++) {
                                    SecurityLatestData latestData = latestDatas[j];
                                    if (latestData.m_close == 0) {
                                        latestData.m_close = latestData.m_buyPrice1;
                                    }
                                    if (latestData.m_close == 0) {
                                        latestData.m_close = latestData.m_sellPrice1;
                                    }
                                    lock (latestDatasCache) {
                                        bool newData = false;
                                        if (!latestDatasCache.ContainsKey(latestData.m_code)) {
                                            latestDatasCache[latestData.m_code] = latestData;
                                            newData = true;
                                        } else {
                                            if (!latestDatasCache[latestData.m_code].equal(latestData)) {
                                                latestDatasCache[latestData.m_code].copy(latestData);
                                                newData = true;
                                            }
                                        }
                                        if (newData) {
                                            if (m_listener != null) {
                                                m_listener.LatestDataCallBack(latestData.m_code);
                                            }
                                        }
                                        if (latestData.m_code == "000001.SH") {
                                            shTradeTime = latestData.m_date;
                                        }
                                    }
                                }
                                latestDatas.Clear();
                            }
                            latestCodes = "";
                        } else {
                            latestCodes += ",";
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 开始策略
        /// </summary>
        public static void Start() {
            Thread thread = new Thread(new ThreadStart(RunWork));
            thread.Start();
        }

        /// <summary>
        /// 获取网页数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>页面源码</returns>
        public static String Get(String url) {
            String content = "";
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader streamReader = null;
            Stream resStream = null;
            try {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = 10000;
                ServicePointManager.DefaultConnectionLimit = 50;
                response = (HttpWebResponse)request.GetResponse();
                resStream = response.GetResponseStream();
                streamReader = new StreamReader(resStream, Encoding.Default);
                content = streamReader.ReadToEnd();
            } catch (Exception ex) {
            } finally {
                if (response != null) {
                    response.Close();
                }
                if (resStream != null) {
                    resStream.Close();
                }
                if (streamReader != null) {
                    streamReader.Close();
                }
            }
            return content;
        }

        /// <summary>
        /// 根据字符串获取新浪的最新数据
        /// </summary>
        /// <param name="str">数据字符串</param>
        /// <param name="formatType">格式</param>
        /// <param name="data">最新数据</param>
        /// <returns>状态</returns>
        public static int GetLatestDataBySinaStr(String str, int formatType, ref SecurityLatestData data) {
            //分析数据
            String date = "";
            String[] strs2 = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int strLen2 = strs2.Length;
            bool szIndex = false;
            for (int j = 0; j < strLen2; j++) {
                String str2 = strs2[j];
                switch (j) {
                    case 0:
                        data.m_code = FCStrEx.convertSinaCodeToDBCode(str2);
                        if (data.m_code.StartsWith("399")) {
                            szIndex = true;
                        }
                        break;
                    case 1: {
                            data.m_open = Convert.ToDouble(str2);
                            break;
                        }
                    case 2: {
                            data.m_lastClose = Convert.ToDouble(str2);
                            break;
                        }
                    case 3: {
                            data.m_close = Convert.ToDouble(str2);
                            break;
                        }
                    case 4: {
                            data.m_high = Convert.ToDouble(str2);
                            break;
                        }
                    case 5: {
                            data.m_low = Convert.ToDouble(str2);
                            break;
                        }
                    case 8: {
                            data.m_volume = Convert.ToDouble(str2);
                            if (szIndex) {
                                data.m_volume /= 100;
                            }
                            break;
                        }
                    case 9: {
                            data.m_amount = Convert.ToDouble(str2);
                            break;
                        }
                    case 10: {
                            if (formatType == 0) {
                                data.m_buyVolume1 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 11: {
                            if (formatType == 0) {
                                data.m_buyPrice1 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 12: {
                            if (formatType == 0) {
                                data.m_buyVolume2 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 13: {
                            if (formatType == 0) {
                                data.m_buyPrice2 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 14: {
                            if (formatType == 0) {
                                data.m_buyVolume3 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 15: {
                            if (formatType == 0) {
                                data.m_buyPrice3 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 16: {
                            if (formatType == 0) {
                                data.m_buyVolume4 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 17: {
                            if (formatType == 0) {
                                data.m_buyPrice4 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 18: {
                            if (formatType == 0) {
                                data.m_buyVolume5 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 19: {
                            if (formatType == 0) {
                                data.m_buyPrice5 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 20: {
                            if (formatType == 0) {
                                data.m_sellVolume1 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 21: {
                            if (formatType == 0) {
                                data.m_sellPrice1 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 22: {
                            if (formatType == 0) {
                                data.m_sellVolume2 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 23: {
                            if (formatType == 0) {
                                data.m_sellPrice2 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 24: {
                            if (formatType == 0) {
                                data.m_sellVolume3 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 25: {
                            if (formatType == 0) {
                                data.m_sellPrice3 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 26: {
                            if (formatType == 0) {
                                data.m_sellVolume4 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 27: {
                            if (formatType == 0) {
                                data.m_sellPrice4 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 28: {
                            if (formatType == 0) {
                                data.m_sellVolume5 = (int)Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 29: {
                            if (formatType == 0) {
                                data.m_sellPrice5 = Convert.ToDouble(str2);
                            }
                            break;
                        }
                    case 30:
                        date = str2;
                        break;
                    case 31:
                        date += " " + str2;
                        break;
                }
            }
            //获取时间
            if (date != null && date.Length > 0) {
                DateTime dateTime = Convert.ToDateTime(date);
                data.m_date = (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
                //data.m_date = FCTran.getDateNum(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
            }
            //价格修正
            if (data.m_close != 0) {
                if (data.m_open == 0) {
                    data.m_open = data.m_close;
                }
                if (data.m_high == 0) {
                    data.m_high = data.m_close;
                }
                if (data.m_low == 0) {
                    data.m_low = data.m_close;
                }
            }
            return 0;
        }

        /// <summary>
        /// 根据字符串获取新浪最新数据
        /// </summary>
        /// <param name="str">数据字符串</param>
        /// <param name="formatType">格式</param>
        /// <param name="datas">最新数据</param>
        /// <returns>状态</returns>
        public static int GetLatestDatasBySinaStr(String str, int formatType, List<SecurityLatestData> datas) {
            String[] strs = str.Split(new String[] { ";\n" }, StringSplitOptions.RemoveEmptyEntries);
            int strLen = strs.Length;
            for (int i = 0; i < strLen; i++) {
                SecurityLatestData latestData = new SecurityLatestData();
                String dataStr = strs[i];
                GetLatestDataBySinaStr(strs[i], formatType, ref latestData);
                if (latestData.m_date > 0) {
                    datas.Add(latestData);
                }
            }
            return 1;
        }

        /// <summary>
        /// 根据股票代码获取新浪最新数据
        /// </summary>
        /// <param name="codes">股票代码列表</param>
        /// <returns>字符串</returns>
        public static String GetSinaLatestDatasStrByCodes(String codes) {
            String[] strs = codes.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int strLen = strs.Length;
            List<String> sinaCodes = new List<String>();
            List<String> dcCodes = new List<String>();
            for (int i = 0; i < strLen; i++) {
                String postCode = strs[i];
                sinaCodes.Add(FCStrEx.convertDBCodeToSinaCode(postCode));
            }
            String requestCode = "";
            int sinaCodesSize = sinaCodes.Count;
            for (int i = 0; i < sinaCodesSize; i++) {
                String postCode = sinaCodes[i];
                requestCode += postCode;
                if (i != strLen - 1) {
                    requestCode += ",";
                }
            }
            String result = "";
            if (sinaCodesSize > 0) {
                String url = "http://hq.sinajs.cn/list=" + requestCode.ToLower();
                result = Get(url);
            }
            return result;
        }

        /// <summary>
        /// 根据代码获取新浪历史数据的字符串
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static String GetSinaHistoryDatasStrByCode(String code, int cycle) {
            String url = "https://quotes.sina.cn/cn/api/json_v2.php/CN_MarketDataService.getKLineData?symbol=" 
                + FCStrEx.convertDBCodeToSinaCode(code) + "&scale=" + cycle.ToString() + "&ma=no&datalen=1023";
            return Get(url);
        }
    }
}
