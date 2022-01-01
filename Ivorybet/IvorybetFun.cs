namespace Ivorybet
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Web;
    using Dapper;
    using iPushData;

    public class IvorybetFun : IPushData
    {
        private List<GameTransaction> _tempObject = new List<GameTransaction>();
        private List<GameTransactionBet> _tempObjectBet = new List<GameTransactionBet>();
        private string Host = ConfigurationManager.AppSettings["APIHost"];
        private tokenObject AccessTokenObj;
        private string siteNamelog = "";

        public aParanPushData PushData(string siteName, IEnumerable<Bet> aryBet)
        {
            siteNamelog = siteName;
            aParanPushData _result = new aParanPushData();
            NLog.Logger logger = NLog.LogManager.GetLogger(siteName + "_log");
            //NLog.Logger loggerVK = NLog.LogManager.GetLogger("VK");
            NLog.Logger batchidlogger = NLog.LogManager.GetLogger(siteName + "_batch_transaction_id_log");
            string ClientId = ConfigurationManager.AppSettings[siteName + "_client_id"];
            string ClientSecret = ConfigurationManager.AppSettings[siteName + "_client_secret"];
            // make data first, than get effective  token
            #region makeData
            apiParan _aParan = new apiParan();
            _aParan.aggregator_code = "";
            _aParan.operator_code = ConfigurationManager.AppSettings[siteName + "_operator_code"];// "OP00031";
            _aParan.provider_code = ConfigurationManager.AppSettings[siteName + "_provider_code"];// "PR00050";
            _aParan.game_transactions = new System.Collections.Generic.List<GameTransaction>();

            long Tmp_version_key = 0;
            int limitcount = 400;
            foreach (var bet in aryBet.OrderBy(item => item.version_key))
            {


                GameTransaction _object = new GameTransaction();
                GameTransactionBet _objectBet = new GameTransactionBet();
                _object.player_no = bet.IBCCustId.ToString();
                _object.player_name = bet.Username;
                _object.player_type = "Real";//bet.currency == 20 ? "Test" : "Real";
                _object.transaction_date = bet.transaction_time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff"); // ToUniversalTime(). GMT+8
                _object.transaction_id = bet.trans_id.ToString();

                bet.winlost_datetime = bet.winlost_datetime.AddHours(-4);
                while (bet.transaction_time > bet.winlost_datetime)
                {
                    bet.winlost_datetime = bet.winlost_datetime.AddMinutes(30);
                }
                if (bet.winlost_datetime.ToUniversalTime() > System.DateTime.UtcNow)//avoid error code = "Settlement date should be before or equal to current date" for Parlay ticket
                {
                    bet.winlost_datetime = bet.transaction_time.AddMinutes(1);
                }
                _object.settlement_date = bet.winlost_datetime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff");//.ToUniversalTime()??
                if (siteName == "Ivorybet")// mapping from Servoce1 in DataPostWS
                {
                    _object.brand = "ivorysport";//stieName
                    _object.brand_url = "http://www.ivorysport.com/";//bet.transdesc;
                }
                else if (siteName == "Opal88")
                {
                    _object.brand = "Opal88";//stieName
                    _object.brand_url = "www.opal88.com";//bet.transdesc;
                }
                else if (siteName == "Nova88")
                {
                    _object.brand = "Nova88";//stieName
                    _object.brand_url = "www.Nova88.com";//bet.transdesc;
                }
                _object.game_code = bet.SportType.ToString();
                _object.game_name = Convert.ConvertGameName(bet.SportType);
                _object.game_type = Convert.ConvertGameType(bet.SportType);
                _object.currency_code = Convert.ConvertCurrencyRCI(bet.currency);
                _object.bet_type = Convert.CovnertBetType(bet.bet_type);
                _object.bet_amount = Math.Floor(bet.actual_stake * 100) / 100;//  Decimal.Parse(bet.stake.ToString("0.00"));
                _object.debit_amount = Math.Floor(bet.actual_stake * 100) / 100;//player loss
                _object.credit_amount = Math.Floor((bet.winlostamount + bet.actual_stake) * 100) / 100;//player wins
                _object.net_amount = Math.Floor((_object.debit_amount - _object.credit_amount) * 100) / 100;//Decimal.Parse((bet.winlostamount * -1).ToString("0.00")); ; //bet.actual_stake - bet.winlostamount-bet.actual_stake
                Tmp_version_key = bet.version_key;
                if (bet.winlost == "" || bet.winlost.Equals(null) || bet.winlost == "draw")
                {
                    continue;
                }
                #region rules

                //Nova88
                if (siteName == "Nova88" && !new string[] { "CNY", "THB", "MYR", "IDR" }.Contains(_object.currency_code)) //RMB==CNY
                {
                    continue;
                }
                if (siteName == "Ivorybet" && !new string[] { "CNY", "THB", "MYR", "VND", "IDR" }.Contains(_object.currency_code)) //RMB==CNY
                {
                    continue;
                }
                if (siteName == "Opal88" && !new string[] { "CNY", "MYR", "THB", "IDR" }.Contains(_object.currency_code)) //RMB==CNY  
                {
                    continue;
                }

                if (new string[] { "KRW", "VND", "IDR" }.Contains(_object.currency_code))
                {
                    _object.bet_amount = _object.bet_amount * 1000;
                    _object.debit_amount = _object.debit_amount * 1000;
                    _object.credit_amount = _object.credit_amount * 1000;
                    _object.net_amount = _object.net_amount * 1000;
                }

                //remove SportType; Number Game:161, VS:180~186, VS2:190~197,199
                if (siteName == "Opal88" && new int[] { 161, 180, 181, 182, 183, 184, 185, 186, 190, 191, 192, 193, 194, 195, 196, 199 }.Contains(bet.SportType))
                {
                    continue;
                }

                //remove SportType; VS:180~186, VS2:190~197,199
                if (siteName == "Ivorybet" && new int[] { 180, 181, 182, 183, 184, 185, 186, 190, 191, 192, 193, 194, 195, 196, 199 }.Contains(bet.SportType))
                {
                    continue;
                }

                if (siteName == "Nova88" && !new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 15, 16, 18, 22, 24, 25, 26, 28, 29, 31, 32, 33, 37, 43, 44, 48, 49, 50, 53, 99 }.Contains(bet.SportType))
                {
                    continue;
                }

                //push data of sport type as below array, no RNG:  AllBet: 211, Pragmatic (PP): 248,RNG Casino (ONE RNG): 208,Macau Games: 212
                if (!new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 50, 99, 161, 154, 180, 181, 182, 183, 184, 185, 186, 190, 191, 192, 193, 194, 195, 196, 199 }.Contains(bet.SportType))
                {
                    continue;
                }

                if (bet.SportType == 0 && bet.bet_type != 29)
                {
                    continue;
                }
                #endregion
                #region common rules

                //all amount cannot be zero
                if (_object.debit_amount == 0 && _object.credit_amount == 0 && _object.net_amount == 0)//remover _object.bet_amount == 0 &&
                {
                    continue;
                }
                if (bet.bet_type >= 911 && bet.bet_type <= 914)
                {
                    continue;
                }
                #endregion
                #region _objectBet
                _objectBet.Playerno = _object.player_no;
                _objectBet.Playername = _object.player_name;
                _objectBet.Playertype = _object.player_type;
                _objectBet.Transactionid = _object.transaction_id;
                _objectBet.Transactionid = _object.transaction_id;
                _objectBet.Brand = _object.brand;
                _objectBet.Brandurl = _object.brand_url;
                _objectBet.Gamecode = _object.game_code;
                _objectBet.Gamename = _object.game_name;
                _objectBet.Gametype = _object.game_type;
                _objectBet.Currencycode = _object.currency_code;
                _objectBet.Bettype = _object.bet_type;
                _objectBet.Betamount = _object.bet_amount;
                _objectBet.Debitamount = _object.debit_amount;
                _objectBet.Creditamount = _object.credit_amount;
                _objectBet.Netamount = _object.net_amount;
                if (new string[] { "KRW", "VND", "IDR" }.Contains(_object.currency_code))
                {
                    _objectBet.Betamount = _objectBet.Betamount / 1000;
                    _objectBet.Debitamount = _objectBet.Debitamount / 1000;
                    _objectBet.Creditamount = _objectBet.Creditamount / 1000;
                    _objectBet.Netamount = _objectBet.Netamount / 1000;
                }
                _objectBet.Transactiondate = bet.transaction_time.ToUniversalTime().AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                _objectBet.Settlementdate = bet.winlost_datetime.ToUniversalTime().AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                    #endregion

                    _tempObject.Add(_object);
                _tempObjectBet.Add(_objectBet);
                if (_tempObject.Count() >= limitcount)
                    break;
            }
            #endregion
            //bool checkFlag = insertToDB(_tempObjectBet);
            //_tempObject.Clear();
            //_tempObjectBet.Clear();
           // loggerVK.Info("1."+siteNamelog + ","+Tmp_version_key);
            #region call external API
            if (_tempObject.Count() >= 10)//if (_aParan.game_transactions.Count() >= 1000)
            {
                _aParan.game_transactions = _tempObject;

                string url = "";
                var json = "";
                string sData = "";
                try
                {
                    if (AccessTokenObj == null || AccessTokenObj.expires_time < DateTime.Now)
                    {
                        authorize(ClientId, ClientSecret);
                    }
                    //url = string.Format("{0}/pagcor-api/v1/game-transactions", Host);
                    url = string.Format("{0}/api/pogo", Host);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_aParan);
                    sData = PostRequest(url, json, 60000, AccessTokenObj.access_token);
                    logger.Info("request feedback:" + sData);
                    resultObject resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<resultObject>(sData);
                    if (resultData.status.ToLower() == "unable_to_process_request")
                    {
                     //   loggerVK.Info("2." + siteNamelog + "," + Tmp_version_key);
                        SendMail(siteName, "Get Error Message From ComRCI - " + siteName, json, sData, url, "MailReciver");
                        logger.Warn("service top, current version_key: " + Tmp_version_key);
                        _result.update_check = false;
                        _result.version_key = Tmp_version_key;
                        _result.stopService = true;
                        return _result;

                    }
                    if (resultData.status.ToLower() == "posted")
                    {
                      //  loggerVK.Info("3." + siteNamelog + "," + Tmp_version_key);
                        _result.update_check = true;
                        _result.version_key = Tmp_version_key;
                        int iCount = _tempObject.Count();
                        _tempObject.Clear();
                        batchidlogger.Info("status:" + resultData.status + "; batch_transaction_id:" + resultData.batch_transaction_id + "; created_date:" + resultData.created_date + "; transactions counts:" + iCount);
                        logger.Info("status:" + resultData.status + "; batch_transaction_id:" + resultData.batch_transaction_id + "; created_date:" + resultData.created_date);
                        logger.Info("json:" + json);
                        logger.Info("transactions counts:" + iCount);

                        #region ineset to DB
                        bool checkFlag = insertToDB(_tempObjectBet);
                        if (checkFlag)
                        {
                            logger.Info("insert to DB");
                            _tempObjectBet.Clear();
                        }
                        else
                        {
                            logger.Warn("insert to DB fail");
                            logger.Warn("service top, current version_key: " + Tmp_version_key);
                            _result.stopService = true;
                            return _result;
                        }
                        #endregion
                    }
                    else
                    {
                      //  loggerVK.Info("4." + siteNamelog + ",0");
                        _result.update_check = false;
                        _result.version_key = 0;
                        _tempObject.Clear();
                        _tempObjectBet.Clear();
                        logger.Warn("status:" + resultData.status + "; batch_transaction_id:" + resultData.batch_transaction_id + "; created_date:" + resultData.created_date);
                        logger.Warn("json:" + json);

                    }
                }
                catch (Exception e)
                {
                  //  loggerVK.Info("5." + siteNamelog + ",0");
                    _result.update_check = false;
                    _result.version_key = 0;
                    _tempObject.Clear();
                    _tempObjectBet.Clear();
                    logger.Error(e);
                }
            }
            else
            {
              //  loggerVK.Info("6." + siteNamelog + "," + Tmp_version_key);
                _result.update_check = false;
                _result.version_key = Tmp_version_key;
            }
            #endregion
            return _result;
        }

        public void authorize(string _ClientId, string _ClientSecret)
        {
            NLog.Logger logger = NLog.LogManager.GetLogger("authorize");
            //string url = string.Format("{0}/pagcor-auth/v1/authorize", Host);
            string url = string.Format("{0}/api/authrequest", Host);
            aParan _aParan = new aParan();
            _aParan.response_type = "code";
            _aParan.client_id = _ClientId;
            _aParan.client_secret = _ClientSecret;
            logger.Debug(url + "?" + GetQueryString(_aParan));
            string sData = PostRequest(url, GetQueryString(_aParan), 5000, null);
            logger.Debug(sData);
            AccessTokenObj = Newtonsoft.Json.JsonConvert.DeserializeObject<tokenObject>(sData);
            AccessTokenObj.expires_time = DateTime.Now.AddSeconds(AccessTokenObj.expires_in - 10d);
        }
        public string testauthorize(string _ClientId, string _ClientSecret)
        {
            string url = string.Format("{0}/pagcor-auth/v1/authorize", Host);
            aParan _aParan = new aParan();
            _aParan.response_type = "code";
            _aParan.client_id = _ClientId;
            _aParan.client_secret = _ClientSecret;
            string sData = PostRequest(url, GetQueryString(_aParan), 5000, null);
            AccessTokenObj = Newtonsoft.Json.JsonConvert.DeserializeObject<tokenObject>(sData);
            return AccessTokenObj.access_token;

        }

        public class tokenObject
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public double expires_in { get; set; }
            public DateTime expires_time { get; set; }
        }
        public class aParan
        {
            public string response_type { set; get; }
            public string client_id { set; get; }
            public string client_secret { set; get; }

        }

        public class resultObject
        {
            public string batch_transaction_id { get; set; }
            public DateTime created_date { get; set; }
            public string status { get; set; }
        }

        public class GameTransaction
        {
            public string player_no { get; set; }
            public string player_name { get; set; }
            public string player_type { get; set; }
            public string transaction_date { get; set; }
            public string transaction_id { get; set; }
            public string settlement_date { get; set; }
            public string brand { get; set; }
            public string brand_url { get; set; }
            public string game_code { get; set; }
            public string game_name { get; set; }
            public string game_type { get; set; }
            public string currency_code { get; set; }
            public string bet_type { get; set; }
            public Decimal bet_amount { get; set; }
            public Decimal debit_amount { get; set; }
            public Decimal credit_amount { get; set; }
            public Decimal net_amount { get; set; }
        }
        public class GameTransactionBet
        {
            public string Playerno { get; set; }
            public string Playername { get; set; }
            public string Playertype { get; set; }
            public string Transactiondate { get; set; }
            public string Transactionid { get; set; }
            public string Settlementdate { get; set; }
            public string Brand { get; set; }
            public string Brandurl { get; set; }
            public string Gamecode { get; set; }
            public string Gamename { get; set; }
            public string Gametype { get; set; }
            public string Currencycode { get; set; }
            public string Bettype { get; set; }
            public Decimal Betamount { get; set; }
            public Decimal Debitamount { get; set; }
            public Decimal Creditamount { get; set; }
            public Decimal Netamount { get; set; }
        }
        public class apiParan
        {
            public string provider_code { get; set; }
            public string operator_code { get; set; }
            public string aggregator_code { get; set; }
            public List<GameTransaction> game_transactions { get; set; }
        }

        internal string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }

        internal string PostRequest(string url, string postData, int timeout, string Access_Token)
        {
            if (System.Net.ServicePointManager.DefaultConnectionLimit < 100)
                System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            HttpWebRequest req;

            if (Access_Token != null)
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Headers.Add("Authorization", "Bearer " + Access_Token);
                req.ContentType = "application/json";
                req.Method = "POST";
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(url + $"?{postData}");
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "GET";
            }

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
                if (Access_Token != null)
                {
                    using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                    {
                        sw.Write(postData);
                        sw.Close();
                    }
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
                NLog.Logger WebException_PostRequest = NLog.LogManager.GetLogger(siteNamelog + "_WebException_log");
                // wEx.Response.GetResponseStream();
                // NLog.Logger webExpectationlogger = NLog.LogManager.GetLogger(siteNamelog + "web_exception_log");
                WebException_PostRequest.Error(wEx.Status);
                WebException_PostRequest.Error(wEx.Message);

                if (wEx.Response != null)
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    //return "{\"status\":\"unable_to_process_request\"}";
                    StreamReader readStream = new StreamReader(wEx.Response.GetResponseStream(), encode);
                    //WebException_PostRequest.Error(readStream.ReadToEnd());
                    return readStream.ReadToEnd();
                }
                throw wEx;
            }
            catch (Exception e)
            {
                NLog.Logger PostRequest = NLog.LogManager.GetLogger(siteNamelog + "_PostRequest_log");
                PostRequest.Error(e);
                throw e;
            }
            return result_Content;

        }

        public static void SendMail(string sitename, string nSubject, string nBody, string errorMessage, string URL, string nMailReciver)
        {
            string a = DateTime.Now.ToString("yyyy-MM-dd");
            string b = DateTime.Now.ToString("yyyy-MM-dd HHmm");
            string mailLog = sitename + "_" + b;
            NLog.Logger logger = NLog.LogManager.GetLogger(mailLog);
            try
            {
                logger.Info("API URL: " + URL);
                logger.Info("error message: " + errorMessage);
                logger.Info("push json format: " + nBody);
                string MailServer = GetAppSetting("MailServer");
                string MailSender = GetAppSetting("MailSender");
                string sStyle = "body{font-family:'Microsoft JhengHei'}";
                string sBody = "";
                string[] MailReciver = null;
                string filePath = "";
                if ("InsertDBIssue_Mail" == nMailReciver)
                {
                    MailReciver = GetAppSetting(nMailReciver).Split(';');
                    string sContent = "<br> data push API already stop until push data confirmed from your side.<br>";
                    sBody = "<html><head><style>" + sStyle + "</style></head><body>";
                    sBody = sBody + sContent;
                    sBody = sBody + "API URL: " + URL + " <br><br>";
                    sBody = sBody + "error message: " + errorMessage + "<br><br>";
                }
                else
                {
                    MailReciver = GetAppSetting(sitename + "_" + nMailReciver).Split(';');
                    string sTitle = "Hi sir,";
                    string sContent = "<br><br>We found the error message when calling API, data push already stop until push data confirmed from your side.<br>Please find information as detail and let us know if there is any updates.<br><br>";
                    sBody = "<html><head><style>" + sStyle + "</style></head><body>";
                    sBody = sBody + sTitle;
                    sBody = sBody + sContent;
                    sBody = sBody + "API URL: " + URL + " <br><br>";
                    sBody = sBody + "IP: 61.14.176.9 <br><br>";
                    sBody = sBody + "error message: " + errorMessage + "<br><br>";
                    sBody = sBody + "push json format:  Please refer to attachment.<br><br></body></html>";
                    filePath = @"E:\DataPostingWS\Logs\" + a + @"\" + mailLog + "_INFO.log";
                }

                Mail mail = new Mail(MailServer, MailSender, MailReciver);
                mail.Subject = nSubject;
                if (filePath.Length > 0)
                    mail.AddAttachmentFile(filePath);
                mail.Body = sBody;
                mail.Send();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Send Mail Error!!");
            }
        }
        static public string GetAppSetting(string key)
        {
            if (System.Web.HttpContext.Current != null)
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings[key] ?? string.Empty;
            }
            else
            {
                return System.Configuration.ConfigurationManager.AppSettings[key] ?? string.Empty;
            }
        }
        public bool insertToDB(List<GameTransactionBet> _objectBet)
        {
            bool flag = true;
            NLog.Logger logger = NLog.LogManager.GetLogger(siteNamelog + "_InsertDB");
            try
            {
                using (var Rep_db = new SqlConnection(Convert.GetConnectionString(GetAppSetting("bodb_CustomerLog"))))
                {
                    Rep_db.Open();
                    using (var trans = Rep_db.BeginTransaction())
                    {
                        var sqlcommand = "  Dot_Member_rptDep_AuditTickets_InsertOrUpdate @Playerno,@Playername,@Playertype,@Transactiondate,@Transactionid,@Settlementdate,@Brand,@Brandurl,@Gamecode,@Gamename,@Gametype,@Currencycode,@Bettype,@Betamount,@Debitamount,@Creditamount,@Netamount";
                        Rep_db.Execute(sqlcommand, _objectBet, trans);

                        trans.Commit();
                    }
                  //  logger.Info(logger);
                }
            }
            catch (Exception ex)
            {
                logger.Error(logger + "_" + ex.Message);
                SendMail("Audit API", "Audit API - log insert DB fail", "", ex.Message, "no url", "InsertDBIssue_Mail");
                return false;
            }
            return flag;
        }
    }
}
