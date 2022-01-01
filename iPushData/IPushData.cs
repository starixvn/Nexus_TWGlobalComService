using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iPushData
{
    public interface IPushData
    {
        aParanPushData PushData(string stieName, IEnumerable<Bet> aryBet);// use "IEnumerable" is more flexible, no limmit type
        //void Stop();
    }
    public class aParanPushData
    {
        public Boolean update_check { set; get; }
        public Int64 version_key { set; get; }
        public Boolean stopService { set; get; }
    }
    public class Bet
    {
        public int IBCCustId { get; set; }
        public Int64 trans_id { get; set; }
        public int bet_type { get; set; }
        public int oddsid { get; set; }
        public decimal odds { get; set; }
        public decimal modds { get; set; }
        public decimal stake { get; set; }
        public Int64? betdaqid { get; set; }
        public DateTime transaction_time { get; set; }
        public string ticket_status { get; set; }
        public string winlost { get; set; }
        public Int64 parlay_ref_no { get; set; }
        public Int64 refno { get; set; }
        public Int64 version_key { get; set; }
        public decimal winlostamount { get; set; }
        public decimal afteramount { get; set; }
        public int currency { get; set; }
        public int matchid { get; set; }
        public DateTime winlost_datetime { get; set; }
        public string bet_team { get; set; }
        public string betfrom { get; set; }
        public string betteam { get; set; }
        public string betcheck { get; set; }
        public decimal comm { get; set; }
        public decimal hdp1 { get; set; }
        public decimal hdp2 { get; set; }
        public int oddstype { get; set; }
        public bool liveindicator { get; set; }
        public int livehomescore { get; set; }
        public int liveawayscore { get; set; }
        public Int64 betid { get; set; }
        public string transdesc { get; set; }
        public byte SportType { get; set; }
        public string ExtraInfo { get; set; }
        public int recommend { get; set; }
        public decimal oddsspread { get; set; }
        public string Username { get; set; }
        public decimal actual_stake { get; set; }

    }
}
