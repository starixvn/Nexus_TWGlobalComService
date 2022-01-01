namespace DataPostWS
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.ServiceProcess;
    using DataPostWS.DB;
    using DataPostWS.Util;
    using iPushData;
    using Ivorybet;

    public partial class Service1 : ServiceBase
    {
        // NLog.Logger logger = NLog.LogManager.GetLogger("DataPostingWS_test");
        private bool isStop;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                IvorybetFun IvorybetAPI = new IvorybetFun();
                PushDataList.Add("Ivorybet", IvorybetAPI);//IvoryBet
                IvorybetAPI = new IvorybetFun();
                PushDataList.Add("Opal88", IvorybetAPI);//Opal88 follow IvoryBet fun
                IvorybetAPI = new IvorybetFun();
                PushDataList.Add("Nova88", IvorybetAPI);//Nova88 follow IvoryBet fun
                ////RngSummaryFun RngSummaryFunAPI = new RngSummaryFun();
                ////PushDataList.Add("Nova88_RNG", RngSummaryFunAPI);//RNG delay push fun
                Start();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected override void OnStop()
        {
            Stop();
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

        //===========================================================================

        private List<string> siteName = DataPostWS.Util.Common.dicSiteName; // initial SiteName lsit
        private ConcurrentDictionary<string, long> currenctVKey = new ConcurrentDictionary<string, long>();
        private Dictionary<string, System.Threading.Timer> TaskList = new Dictionary<string, System.Threading.Timer>(StringComparer.OrdinalIgnoreCase);//each site Timer
        private Dictionary<string, IPushData> PushDataList = new Dictionary<string, IPushData>(StringComparer.OrdinalIgnoreCase);// identify each site API function

        public void Start()
        {
            foreach (string _siteName in siteName)
            {
                isStop = false;
                currenctVKey.TryAdd(_siteName, 0);
                TaskList.Add(_siteName, new System.Threading.Timer(new System.Threading.TimerCallback(PostGetBetDetail), _siteName, 0, 0));
            }
        }

        public void PostGetBetDetail(object oSiteName)//object obj string stieName
        {
            NLog.Logger logger = NLog.LogManager.GetLogger("PostGetBetDetail");
            string siteName = (string)oSiteName;
            if (PushDataList.ContainsKey(siteName))
            {
                bool is_test = Common.isTest; // check config
                int iCount = 0;
                int iVK = 3;
                string sConnect = siteName;
                bool bRNG = false;

                if (siteName.EndsWith("_RNG"))
                {
                    iVK = 4;
                    sConnect = siteName.Remove(siteName.LastIndexOf("_"), 4);
                    bRNG = true;
                    logger.Debug(siteName + ", RNG, iVK:" + iVK);
                }

                bool bDelayServiceWorking = false;
                try
                {
                    NLog.Logger loggerVK = NLog.LogManager.GetLogger("VersionKey");
                    DateTime today = DateTime.Now;//.AddDays(-1);
                    today = today.Date;
                    bDelayServiceWorking = false;
                    LicRepDB rep_db = new LicRepDB(sConnect);// new DB connencting 
            
                    long LastVersionKey;
                    if (currenctVKey[siteName] == 0)
                    {
                        LastVersionKey = rep_db.Dot_Dep_GetLastVersionKey(is_test, iVK);//Common.isTest initial
                        loggerVK.Debug("sConnect=" + sConnect +", stieName=" + siteName + ", LastVersionKey=" + LastVersionKey + ", Dot_Dep_GetLastVersionKey:" + iVK);
                    }
                    else
                    {
                        LastVersionKey = currenctVKey[siteName];
                        //loggerVK.Debug("sConnect=" + sConnect + ", stieName=" + stieName + ", currenctVKey[stieName]=" + currenctVKey[stieName] + ", Dot_Dep_GetLastVersionKey:" + iVK);
                    }

                    Int64 max_version_key = 0;
                    Int64 ticket_max_version_key = 0;
                    Bet[] aryBetInfo = rep_db.Dot_Dep_GetBetDetail_All(LastVersionKey, is_test, out max_version_key);// get last verstion key
                    ticket_max_version_key = aryBetInfo.Length == 0 ? 0 : aryBetInfo.Max(v => v.version_key);
                    if (max_version_key < LastVersionKey)
                    {
                        loggerVK.Error("stieName=" + siteName + ", max_version_key=" + max_version_key + ", ticket_max_version_key=" + ticket_max_version_key + ", LastVersionKey=" + LastVersionKey);
                    }
                    if (ticket_max_version_key == 0)
                    {
                        LastVersionKey = max_version_key;
                        currenctVKey[siteName] = max_version_key;
                    }
                    else
                    {
                        LastVersionKey = new Int64[] { max_version_key, ticket_max_version_key }.Min();
                    }
                  
                    if (bRNG)
                    {
                        var vBetInfo = from item in aryBetInfo
                                       where item.winlost_datetime < today
                                       select item;
                        aryBetInfo = vBetInfo.ToArray();
                        logger.Debug(siteName + ", winlost_datetime < today today:" + today + ", LastVersionKey:" + LastVersionKey);
                    }

                    if (aryBetInfo != null)
                        iCount = aryBetInfo.Count();

                    if (iCount > 0)//iCount >= 499
                    {
                        aParanPushData _result = PushDataList[siteName].PushData(siteName, aryBetInfo);// get Site specific API
                        currenctVKey[siteName] = _result.version_key; //= _result.version_key;
                     //   loggerVK.Debug("stieName=" + stieName + ",  _result.version_key, currenctVKey[stieName]=" + currenctVKey[stieName] );
                        if (_result.update_check)
                        {
                            rep_db.Dot_Dep_UpdLastVersionKey(_result.version_key, iVK);// update current version key
                            //currenctVKey[stieName] = 0;
                        }

                        if (_result.stopService)
                        {

                            bDelayServiceWorking = true;

                            //Stop();
                            //ServiceController service = new ServiceController("DataPostingService");
                            //service.Stop();
                            //service.WaitForStatus(ServiceControllerStatus.Stopped);
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
                    if (bDelayServiceWorking) // cause got error message: unable_to_process_request
                    {
                        /* 
                        SendMail(stieName, "Get Error Message From ComRCI - " + stieName, json, sData, url, "MailReciver");
                        logger.Warn("service top, current version_key: " + Tmp_version_key);
                        _result.update_check = false;
                         */

                        TaskList[siteName].Change(3600000, System.Threading.Timeout.Infinite);
                    }
                    else
                    {
                        if (!isStop)
                        {
                            if (iCount >= 499)// if count >= 499, it's main there has left data.
                            {
                                TaskList[siteName].Change(1, System.Threading.Timeout.Infinite);
                            }
                            else// otherwise, waiting to over count its more than 499 in 10 sec.
                            {
                                if (bRNG)
                                {
                                    if (iCount == 0)
                                    {
                                        logger.Debug("bRNG, 3600000" + siteName);
                                        TaskList[siteName].Change(3600000, System.Threading.Timeout.Infinite);//1 hour
                                    }
                                    else {
                                        logger.Debug("bRNG, 5000" + siteName);
                                        TaskList[siteName].Change(5000, System.Threading.Timeout.Infinite);
                                    }

                                }
                                else
                                {
                                    TaskList[siteName].Change(10000, System.Threading.Timeout.Infinite);
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
