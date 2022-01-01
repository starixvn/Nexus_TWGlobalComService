using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using DataPostWS.DataType;
using DataPostWS.Util;
namespace DataPostWS.DB
{
    public class SiteDB
    {
        static private ObjectCache cache;
        static private int cache_period;
        NLog.Logger loggerVK = NLog.LogManager.GetLogger("VersionKey");
        public SiteDB()
        {
            cache = MemoryCache.Default;
            cache_period = ConfigurationManager.AppSettings["cache_period"] == null ? 5 : Convert.ToInt32(ConfigurationManager.AppSettings["cache_period"]);

        }

        public string GetSiteList(string site_name)
        {
            string siteName = site_name + "_SiteInfo";

            if (cache[siteName] == null)
            {
                try
                {
                    string dbString = Common.GetConnectionString("SiteConfig");
                    using (SqlConnection conn = new SqlConnection(dbString))
                    {

                        conn.Open();
                        SqlCommand cmd = new SqlCommand("Portal_GetVendorInfo_Select", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Add(new SqlParameter("@vendorID", site_name));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (string.Equals(rdr["SiteName"] as string, site_name, System.StringComparison.OrdinalIgnoreCase) && rdr["LicRepConnectionString"] != DBNull.Value)
                                {
                                    string strLicRepConnectionString = rdr["LicRepConnectionString"] as string;

                                    CacheItemPolicy policy = new CacheItemPolicy();
                                    policy.AbsoluteExpiration = DateTime.Now.AddMinutes(cache_period);
                                   // loggerVK.Debug("stieName=" + siteName + ", strLicRepConnectionString= " + strLicRepConnectionString);
                                   cache.Set(siteName, strLicRepConnectionString, policy);
                                    return strLicRepConnectionString;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }

                return null;
            }
            else
            {
                return cache[siteName] as string;
            }

        }

    }
}
