using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Configuration;

using System.Collections.Concurrent;
using System.Data.SqlClient;
using DataPostWS.DB;

namespace DataPostWS.Util
{
    public class Common
    {
        
        static public string GetVendorConnectionString(string SiteName)
        {
            SiteDB _SiteDB = new SiteDB();
            return GetConnectionString(_SiteDB.GetSiteList(SiteName));
        }

        static public string GetConnectionString(string key)
        {
            NLog.Logger logger = NLog.LogManager.GetLogger("GetConnectionString");
            string connectionString = "";
            if (key == "SiteConfig")
                connectionString = GetAppSetting(key);
            else
                connectionString = key;

            SqlConnectionStringBuilder SCSB = new SqlConnectionStringBuilder(connectionString);
            if (SCSB.Password == "") return SCSB.ConnectionString;
            SCSB.Password = Decrypt(SCSB.Password);

            return SCSB.ConnectionString;
        }
        static private string Decrypt(string strText)
        {
            string sDecrKey = "&％#@?,:*!~^.";
            try
            {
                byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                byte[] bykey = System.Text.Encoding.UTF8.GetBytes(sDecrKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                strText = strText.Replace("=", "");
                strText += (new string('=', strText.Length % 4 == 0 ? 0 : 4 - strText.Length % 4)); // 以 '=' 將字元數湊滿四的倍數
                byte[] inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(IV, bykey), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }
        static private bool _isTest = GetAppSetting("isTest").ToLower() == "true";
        static public bool isTest { get { return _isTest; } }
        static public List<string> dicSiteName = SiteNameDictionary();
        static List<string> SiteNameDictionary()
        {
            var dic = new List<string>();
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                foreach (var SubLic in ConfigurationManager.AppSettings["SiteName"].Split(','))
                {
                    dic.Add(SubLic);
                }
            }

            return dic;
        }


   
    }
}
