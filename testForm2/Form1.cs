using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataPostWS;
using System.Web;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Ivorybet;
using iPushData;
using Dapper;
using System.Data.SqlClient;
using NLog;
using System.Collections.Concurrent;
using DataPostWS.Util;
using DataPostWS.DB;
using Newtonsoft.Json;


namespace testForm
{
    public partial class Form1 : Form
    {
        NLog.Logger logger = NLog.LogManager.GetLogger("getAudit");
        // DataPosting test = new DataPosting();
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Service1 test = new Service1();
            //test.Start();
            //  test.Start();
            //test.beforehand(sender);
            // test.PostGetBetDetail("BBin");


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stop();
            // test.Stop();
        }
        //=======================================================
        //public static bool Ready = true;
        private void button3_Click(object sender, EventArgs e)
        {

            //string url = "https://api.globalcomrci.com/pagcor-auth/v1/authorize";
            //aParan _aParan = new aParan();
            //_aParan.response_type = "code";
            //_aParan.client_id = "ac2abcc6-6532-43c2-834b-ef9b9859293c";
            //_aParan.client_secret = "c44d77b0-5a58-4493-aaac-a6261466c441";
            //string sData = PostRequest(url, GetQueryString(_aParan), 1000);
            //string _sData = sData;

        }
        internal string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

        class aParan
        {
            public string response_type { set; get; }
            public string client_id { set; get; }
            public string client_secret { set; get; }

        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
        internal string PostRequest(string url, string postData, int timeout, string Access_Token)
        {
            if (System.Net.ServicePointManager.DefaultConnectionLimit < 100)
                System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.ContentType = Access_Token == null ? "application/x-www-form-urlencoded" : "application/json";
            if (Access_Token != null)
            {
                req.Headers.Add("Authorization", "Bearer " + Access_Token);
            }

            req.Method = "POST";
            if (timeout > 0)
                req.Timeout = timeout;

            req.KeepAlive = false;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            }

            string MyProxyHostString = ConfigurationManager.AppSettings["ProxyHost"];
            int MyProxyPort = 0;
            if (ConfigurationManager.AppSettings["ProxyHost"] != null && int.TryParse(ConfigurationManager.AppSettings["ProxyPort"], out MyProxyPort))
            {
                req.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
            }

            string result_Content = "";
            try
            {
                using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(postData);
                    sw.Close();
                }

                using (WebResponse resp = req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.UTF8))
                    {
                        result_Content = sr.ReadToEnd();
                        sr.Close();
                    }
                    resp.Close();
                }

            }
            catch (WebException wEx)
            {
                // wEx.Response.GetResponseStream();
                // NLog.Logger webExpectationlogger = NLog.LogManager.GetLogger(siteNamelog + "web_exception_log");
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                //return "{\"status\":\"unable_to_process_request\"}";
                StreamReader readStream = new StreamReader(wEx.Response.GetResponseStream(), encode);
                return readStream.ReadToEnd();
                //webExpectationlogger.Error(readStream.ReadToEnd());

            }
            return result_Content;

        }
        internal string GetRequest(string url, string json, int timeout, string Access_Token)
        {
            if (System.Net.ServicePointManager.DefaultConnectionLimit < 100)
                System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.ContentType = "application/json";
            if (Access_Token != null)
            {
                req.Headers.Add("Authorization", "Bearer " + Access_Token);
            }

            req.Method = "GET";
            if (timeout > 0)
                req.Timeout = timeout;

            req.KeepAlive = false;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            }

            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            string MyProxyHostString = ConfigurationManager.AppSettings["ProxyHost"];
            int MyProxyPort = 0;
            if (ConfigurationManager.AppSettings["ProxyHost"] != null && int.TryParse(ConfigurationManager.AppSettings["ProxyPort"], out MyProxyPort))
            {
                req.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
            }

            string result_Content = "";
            try
            {

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result_Content = sr.ReadToEnd();
                        sr.Close();
                    }
                    response.Close();
                }
            }
            catch (WebException wEx)
            {
                // wEx.Response.GetResponseStream();
                // NLog.Logger webExpectationlogger = NLog.LogManager.GetLogger(siteNamelog + "web_exception_log");
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                //return "{\"status\":\"unable_to_process_request\"}";
                StreamReader readStream = new StreamReader(wEx.Response.GetResponseStream(), encode);
                return readStream.ReadToEnd();
                //webExpectationlogger.Error(readStream.ReadToEnd());

            }
            return result_Content;

        }


        public void Stop()
        {
            isStop = true;
            foreach (KeyValuePair<string, System.Threading.Timer> list in TaskList)
            {
                string siteName = list.Key;
                currenctVKey[siteName] = 0;
                list.Value.Dispose();
            }
        }
        //=======================================================
        IvorybetFun test = new IvorybetFun();

        private void button4_Click(object sender, System.EventArgs e)
        {
            IvorybetFun IvorybetAPI = new IvorybetFun();
            PushDataList.Add("BBin", IvorybetAPI);//IvoryBet
            //IvorybetAPI = new IvorybetFun();
            //PushDataList.Add("ABC", IvorybetAPI);//Opal88 follow IvoryBet fun
            //IvorybetFun IvorybetAPI = new IvorybetFun();
            //PushDataList.Add("Ivorybet", IvorybetAPI);//IvoryBet
            //IvorybetAPI = new IvorybetFun();
            //PushDataList.Add("Opal88", IvorybetAPI);//Opal88 follow IvoryBet fun

            Start();


        }
        private bool isStop;
        private List<string> siteName = DataPostWS.Util.Common.dicSiteName; // initial SiteName lsit

        private Dictionary<string, System.Threading.Timer> TaskList = new Dictionary<string, System.Threading.Timer>(StringComparer.OrdinalIgnoreCase);//each site Timer
        private Dictionary<string, IPushData> PushDataList = new Dictionary<string, IPushData>(StringComparer.OrdinalIgnoreCase);// identify each site API function


        // Int64 currenctKey;
        private ConcurrentDictionary<string, long> currenctVKey = new ConcurrentDictionary<string, long>();


        public void Start()
        {
            foreach (string _siteName in siteName)
            {
                isStop = false;
                currenctVKey.TryAdd(_siteName, 0);
                TaskList.Add(_siteName, new System.Threading.Timer(new System.Threading.TimerCallback(PostGetBetDetail), _siteName, 0, 0));
            }
        }

        public void PostGetBetDetail(object oStieName)//object obj string stieName
        {

            string stieName = (string)oStieName;
            if (PushDataList.ContainsKey(stieName))
            {
                bool is_test = Common.isTest; // check config
                int iCount = 0;
                try
                {

                    LicRepDB rep_db = new LicRepDB(stieName);// new DB connencting 
                    long LastVersionKey;
                    if (currenctVKey[stieName] == 0)
                    {
                        LastVersionKey = rep_db.Dot_Dep_GetLastVersionKey(is_test);//Common.isTest initial
                    }
                    else
                    {
                        LastVersionKey = currenctVKey[stieName];
                    }

                    Bet[] aryBetInfo = rep_db.Dot_Dep_GetBetDetail_All(LastVersionKey, is_test);// get last verstion key

                    if (aryBetInfo != null)
                        iCount = aryBetInfo.Count();

                    if (iCount > 0)//iCount >= 499
                    {
                        aParanPushData _result = PushDataList[stieName].PushData(stieName, aryBetInfo);// get Site specific API
                        currenctVKey[stieName] = _result.version_key; //= _result.version_key;
                        if (_result.update_check)
                        {
                            rep_db.Dot_Dep_UpdLastVersionKey(_result.version_key);// update current version key
                            //currenctVKey[stieName] = 0;
                        }

                        if (_result.stopService)
                        {
                            Stop();
                        }
                    }

                }
                catch (Exception ex)
                {
                    //currenctVKey[stieName] = 0;
                    logger.Error("DataPosting Service Start Fail:" + ex);
                }
                finally
                {
                    if (!isStop)
                    {
                        if (iCount >= 499)// if count >= 499, it's main there has left data.
                        {
                            TaskList[stieName].Change(1, System.Threading.Timeout.Infinite);
                        }
                        else// otherwise, waiting to over count its more than 499 in 10 sec.
                        {
                            TaskList[stieName].Change(10000, System.Threading.Timeout.Infinite);
                        }
                    }
                }
            }
        }
        private void button5_Click(object sender, System.EventArgs e)
        {
            string stieName = "Opal88";
            string ClientId = ConfigurationManager.AppSettings[stieName + "_client_id"];
            string ClientSecret = ConfigurationManager.AppSettings[stieName + "_client_secret"];
            IvorybetFun IvorybetAPI = new IvorybetFun();
            string test = IvorybetAPI.testauthorize(ClientId, ClientSecret);
            textBox1.Text = test.ToString();

        }

        private void button6_Click(object sender, System.EventArgs e)
        {
            string filePath = @"D:\Task\20180316_DataPosting\Opal88\rowdata";//Opal88_log.log0000000
            DirectoryInfo info = new DirectoryInfo(filePath);
            FileInfo[] fileEntries = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            //string[] fileEntries = Directory.GetFiles(filePath).OrderBy(p => p.).ToArray();
            foreach (FileInfo fileName in fileEntries)
            {
                int te = fileName.Name.IndexOf(".");
                //if (fileName.Name.Substring(0, te) == "Opal88")
                //    openTxt(fileName.DirectoryName + "\\" + fileName.Name);

                if (fileName.Name.Substring(0, te) == "Opal88_log")
                    openTxt(fileName.DirectoryName + "\\" + fileName.Name);

                if (fileName.Name.Substring(0, te) == "Opal88_log_INFO")
                    openTxt(fileName.DirectoryName + "\\" + fileName.Name);
            }

            exportF();
        }
        private List<GameTransaction> _tempObject = new List<GameTransaction>();
        public void exportF()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("player_no", typeof(string));
            dt.Columns.Add("player_name", typeof(string));
            dt.Columns.Add("player_type", typeof(string));
            dt.Columns.Add("transaction_date", typeof(DateTime));
            dt.Columns.Add("transaction_id", typeof(string));
            dt.Columns.Add("settlement_date", typeof(DateTime));
            dt.Columns.Add("brand", typeof(string));
            dt.Columns.Add("brand_url", typeof(string));
            dt.Columns.Add("game_code", typeof(string));
            dt.Columns.Add("game_name", typeof(string));
            dt.Columns.Add("game_type", typeof(string));
            dt.Columns.Add("currency_code", typeof(string));
            dt.Columns.Add("bet_type", typeof(string));
            dt.Columns.Add("bet_amount", typeof(decimal));
            dt.Columns.Add("debit_amount", typeof(decimal));
            dt.Columns.Add("credit_amount", typeof(decimal));
            dt.Columns.Add("net_amount", typeof(decimal));
            foreach (var str in _tempObject)
            {
                DataRow row = dt.NewRow();
                row["player_no"] = str.player_no;
                row["player_name"] = str.player_name;
                row["player_type"] = str.player_type;
                row["transaction_date"] = str.transaction_date;
                row["transaction_id"] = str.transaction_id;
                row["settlement_date"] = str.settlement_date;
                row["brand"] = str.brand;
                row["brand_url"] = str.brand_url;
                row["game_code"] = str.game_code;
                row["game_name"] = str.game_name;
                row["game_type"] = str.game_type;
                row["currency_code"] = str.currency_code;
                row["bet_type"] = str.bet_type;
                row["bet_amount"] = str.bet_amount;
                row["debit_amount"] = str.debit_amount;
                row["credit_amount"] = str.credit_amount;
                row["net_amount"] = str.net_amount;
                dt.Rows.Add(row);
            }

            //open file
            StreamWriter wr = new StreamWriter(@"D:\Task\20180316_DataPosting\Opal88\rowdata\201801026.xls", true, Encoding.Default);


            try
            {

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    wr.Write(dt.Columns[i].ToString().ToUpper() + "\t");
                }

                wr.WriteLine();

                //write rows to excel file
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i][j] != null)
                        {
                            wr.Write(System.Convert.ToString(dt.Rows[i][j]) + "\t");
                        }
                        else
                        {
                            wr.Write("\t");
                        }
                    }
                    //go to next line
                    wr.WriteLine();
                }
                //close file
                wr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string batchid = "";
        public void openTxt(string filePath)
        {
            //透過路徑讀取檔案
            //使txt檔的中文字編碼改成big5 
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("big5");
            //透過路徑讀取檔案
            StreamReader sr = new StreamReader(filePath, encode);

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string jsonList; //要存入的陣列
                int json = line.IndexOf("json:");

                //int ibatchid = line.IndexOf("batch_transaction_id:");
                //if (ibatchid > 0)
                //{
                //    int iEndbatchid = line.IndexOf("; created_date");
                //    string tempbatchid = line.Substring(0, ibatchid+21);
                //    batchid = line.Replace(tempbatchid, "");
                //}
                if (json > 0)
                {
                    jsonList = line.Substring(0, json + 5);
                    line = line.Replace(jsonList, "");
                    RootObject RootObject = JsonConvert.DeserializeObject<RootObject>(line);
                    int c = RootObject.game_transactions.Count();
                    foreach (var a in RootObject.game_transactions)
                    {
                        //  a.batch_id = batchid;
                        _tempObject.Add(a);
                    }
                }

            }
        }

        public class GameTransaction
        {
            public string batch_id { get; set; }
            public string player_no { get; set; }
            public string player_name { get; set; }
            public string player_type { get; set; }
            public DateTime transaction_date { get; set; }
            public string transaction_id { get; set; }
            public DateTime settlement_date { get; set; }
            public string brand { get; set; }
            public string brand_url { get; set; }
            public string game_code { get; set; }
            public string game_name { get; set; }
            public string game_type { get; set; }
            public string currency_code { get; set; }
            public string bet_type { get; set; }
            public decimal bet_amount { get; set; }
            public decimal debit_amount { get; set; }
            public decimal credit_amount { get; set; }
            public decimal net_amount { get; set; }
        }

        public class RootObject
        {
            public string provider_code { get; set; }
            public string operator_code { get; set; }
            public string aggregator_code { get; set; }
            public List<GameTransaction> game_transactions { get; set; }
        }

        public void openTxtB(string filePath)
        {
            //透過路徑讀取檔案
            //使txt檔的中文字編碼改成big5 
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("big5");
            //透過路徑讀取檔案
            StreamReader sr = new StreamReader(filePath, encode);

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string jsonList; //要存入的陣列
                int json = line.IndexOf("json:");

                int ibatchid = line.IndexOf("batch_transaction_id:");
                if (ibatchid > 0)
                {
                    int iEndbatchid = line.IndexOf("; created_date");
                    string tempbatchid = line.Substring(0, ibatchid + 21);
                    batchid = line.Replace(tempbatchid, "");
                    GameTransaction a = new GameTransaction();
                    a.batch_id = batchid;
                    _tempObject.Add(a);
                }


            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string filePath = @"D:\Task\20180316_DataPosting\Opal88\20180615\DataLog\batchidLog";//Opal88_log.log0000000
            DirectoryInfo info = new DirectoryInfo(filePath);
            FileInfo[] fileEntries = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            //string[] fileEntries = Directory.GetFiles(filePath).OrderBy(p => p.).ToArray();
            foreach (FileInfo fileName in fileEntries)
            {
                int te = fileName.Name.IndexOf(".");
                if (fileName.Name.Substring(0, te) == "Opal88_batch_transaction_id_log_INFO")
                    openTxtB(fileName.DirectoryName + "\\" + fileName.Name);
            }

            exportF();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string a = DateTime.Now.ToString("yyyy-MM-dd");
            string b = DateTime.Now.ToString("yyyy-MM-dd HHmm");
            NLog.Logger logger = NLog.LogManager.GetLogger(b);
            logger.Info("test");
            string filePath = @"D:\LicenseeCode\DataPostWS\testForm2\bin\Release\Logs\" + a + @"\" + b + "_INFO.log";
            textBox1.Text = filePath;
            string MailServer = "mail.onelab.tw";
            string MailSender = "test@onelab.tw";
            string[] MailReciver = "candice.yeh@onelab.tw".Split(';');
            Mail mail = new Mail(MailServer, MailSender, MailReciver);
            mail.Subject = "testSubject";
            string sBody = "test";


            //string filePath = @"D:\Task\20180316_DataPosting\aaa.txt";
            mail.AddAttachmentFile(filePath);
            mail.Body = sBody;
            mail.Send();
        }
        private tokenObject AccessTokenObj;
        public class tokenObject
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public double expires_in { get; set; }
            public DateTime expires_time { get; set; }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            string url = "https://api.globalcomrci.com/pagcor-auth/v1/authorize";
            aParan _aParan = new aParan();
            _aParan.response_type = "code";
            _aParan.client_id = "ac2abcc6-6532-43c2-834b-ef9b9859293c";
            _aParan.client_secret = "c44d77b0-5a58-4493-aaac-a6261466c441";
            string sData = PostRequest(url, GetQueryString(_aParan), 5000, null);
            AccessTokenObj = Newtonsoft.Json.JsonConvert.DeserializeObject<tokenObject>(sData);
            AccessTokenObj.expires_time = DateTime.Now.AddSeconds(AccessTokenObj.expires_in - 10d);
            logger.Info("request feedback aut:" + sData);
            url = "https://api.globalcomrci.com/pagcor-api/v1/game-transactions-audit?status=posted";
            //aParan2 _aParan2 = new aParan2();
            //_aParan2.Status = "Posted";
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(_aParan2);
            sData = GetRequest(url, "", 60000, AccessTokenObj.access_token);
            logger.Info("request feedback:" + sData);
        }


        class aParan2
        {
            public string Status { set; get; }


        }

        private void button10_Click(object sender, EventArgs e)
        {
            string filePath = @"D:\Task\20180316_DataPosting\IvoryBet\InsertToDB";
            DirectoryInfo info = new DirectoryInfo(filePath);
            FileInfo[] fileEntries = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            //string[] fileEntries = Directory.GetFiles(filePath).OrderBy(p => p.).ToArray();
            foreach (FileInfo fileName in fileEntries)
            {
                int te = fileName.Name.IndexOf(".");
                //if (fileName.Name.Substring(0, te) == "Ivorybet")
                //    openTxtDB(fileName.DirectoryName + "\\" + fileName.Name);

                if (fileName.Name.Substring(0, te) == "Ivorybet_log")
                    openTxtDB(fileName.DirectoryName + "\\" + fileName.Name);

                if (fileName.Name.Substring(0, te) == "Ivorybet_log_INFO")
                    openTxtDB(fileName.DirectoryName + "\\" + fileName.Name);
            }

           

        }

        public void insertToDB(string filePath)
        {
            NLog.Logger logger = NLog.LogManager.GetLogger("InsertDB");
            try
            {
                using (SqlConnection Rep_db = new SqlConnection(Common.GetConnectionString(Common.GetAppSetting("bodb_CustomerLog"))))
                {
                    Rep_db.Open();
                    SqlTransaction Rep_db_Tran = Rep_db.BeginTransaction();
                    foreach (var item in _tempObjectDB)
                    {
                        string processQuery = "INSERT INTO [dbo].[PushDataLog]  VALUES (@player_no , @player_name, @player_type, @transaction_date, "+
                        "@transaction_id, @settlement_date, @brand, @brand_url, @game_code, @game_name," +
                        "@game_type, @currency_code, @bet_type,@bet_amount, @debit_amount, @credit_amount, @net_amount)";

                        Rep_db.Execute(processQuery, item, Rep_db_Tran);
                    }

                    Rep_db_Tran.Commit();
                    _tempObjectDB.Clear();
                    logger.Info(filePath);
                }
                

            }
            catch (Exception ex)
            {
                _tempObjectDB.Clear();
                throw ex;
            }
        }
        private List<GameTransaction> _tempObjectDB = new List<GameTransaction>();


        public void openTxtDB(string filePath)
        {
            //透過路徑讀取檔案
            //使txt檔的中文字編碼改成big5 
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("big5");
            //透過路徑讀取檔案
            StreamReader sr = new StreamReader(filePath, encode);

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string jsonList; //要存入的陣列
                int json = line.IndexOf("json:");

                if (json > 0)
                {
                    jsonList = line.Substring(0, json + 5);
                    line = line.Replace(jsonList, "");
                    RootObject RootObject = JsonConvert.DeserializeObject<RootObject>(line);
                    int c = RootObject.game_transactions.Count();
                    foreach (var a in RootObject.game_transactions)
                    {
                        //  a.batch_id = batchid;
                        _tempObjectDB.Add(a);
                    }
                }

            }
            insertToDB(filePath);
        }
    }
}
