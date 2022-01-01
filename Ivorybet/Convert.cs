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
namespace Ivorybet
{
    public class Convert
    {
        static public string GetConnectionString(string key)
        {
            NLog.Logger logger = NLog.LogManager.GetLogger("GetConnectionString");
            SqlConnectionStringBuilder SCSB = new SqlConnectionStringBuilder(key);
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
                byte[] inputByteArray = System.Convert.FromBase64String(strText);
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

        static public string ConvertGameName(int sportid)
        {
            switch (sportid)
            {
                case 0: return "Parlay";
                case 1: return "Soccer";
                case 2: return "Basketball";
                case 3: return "Football";
                case 4: return "Ice Hockey";
                case 5: return "Tennis";
                case 6: return "Volleyball";
                case 7: return "Billiards";
                case 8: return "Baseball";
                case 9: return "Badminton";
                case 10: return "Golf";
                case 11: return "Motorsports";
                case 12: return "Swimming";
                case 13: return "Politics";
                case 14: return "Water Polo";
                case 15: return "Diving";
                case 16: return "Boxing";
                case 17: return "Archery";
                case 18: return "Table Tennis";
                case 19: return "Weightlifting";
                case 20: return "Canoeing";
                case 21: return "Gymnastics";
                case 22: return "Athletics";
                case 23: return "Equestrian";
                case 24: return "Handball";
                case 25: return "Darts";
                case 26: return "Rugby";
                case 27: return "Cricket";
                case 28: return "Field Hockey";
                case 29: return "Winter Sport";
                case 30: return "Squash";
                case 31: return "Entertainment";
                case 32: return "Net Ball";
                case 33: return "Cycling ";
                case 34: return "Fencing";
                case 35: return "Judo";
                case 36: return "M. Pentathlon";
                case 37: return "Rowing";
                case 38: return "Sailing";
                case 39: return "Shooting";
                case 40: return "Taekwondo ";
                case 41: return "Triathlon";
                case 42: return "Wrestling";
                case 43: return "E Sports";
                case 44: return "Muay Thai";
                case 50: return "Cricket";
                case 99: return "Other Sports";
                case 154: return "HorseRacing FixedOdds";
                case 161: return "Number Game";
                case 162: return "Live Casino";
                case 180: return "Virtual Soccer";
                case 181: return "Virtual Horse Racing";
                case 182: return "Virtual Greyhound";
                case 183: return "Virtual Speedway";
                case 184: return "Virtual F1";
                case 185: return "Virtual Cycling";
                case 186: return "Virtual Tennis";
                case 202: return "Keno";
                case 251: return "Casino";
                case 208: return "RNG Game";
                case 209: return "Mini Game";
                case 210: return "Mobile";
                case 204: return "Colossus Bet";
                case 219: return "Fishing World";
                case 220: return "Keno Lottery";
                case 211: return "Allbet";
                case 212: return "Macau Games";
                case 190: return "Virtual Soccer League";
                case 191: return "Virtual Soccer Nation";
                case 192: return "Virtual Soccer Euro";
                case 193: return "Virtual Basketball";
                case 194: return "Virtual Soccer Asian Cup";
                case 195: return "Virtual Dog";
                case 196: return "Virtual Soccer Champions Cup";
                case 197: return "Virtual Soccer Euro Cup";
                case 199: return "Virtual Sports Parlay";
                case 248: return "Pragmatic Play RNG Games";
                default:
                    return "-";

            }

        }
        static public string ConvertGameType(int sportid)
        {
            switch (sportid)
            {
                case 0: return "SB";
                case 1: return "SB";
                case 2: return "SB";
                case 3: return "SB";
                case 4: return "SB";
                case 5: return "SB";
                case 6: return "SB";
                case 7: return "SB";
                case 8: return "SB";
                case 9: return "SB";
                case 10: return "SB";
                case 11: return "SB";
                case 12: return "SB";
                case 13: return "SB";
                case 14: return "SB";
                case 15: return "SB";
                case 16: return "SB";
                case 17: return "SB";
                case 18: return "SB";
                case 19: return "SB";
                case 20: return "SB";
                case 21: return "SB";
                case 22: return "SB";
                case 23: return "SB";
                case 24: return "SB";
                case 25: return "SB";
                case 26: return "SB";
                case 27: return "SB";
                case 28: return "SB";
                case 29: return "SB";
                case 30: return "SB";
                case 31: return "SB";
                case 32: return "SB";
                case 33: return "SB";
                case 34: return "SB";
                case 35: return "SB";
                case 36: return "SB";
                case 37: return "SB";
                case 38: return "SB";
                case 39: return "SB";
                case 40: return "SB";
                case 41: return "SB";
                case 42: return "SB";
                case 43: return "SB";
                case 44: return "SB";
                case 50: return "SB";
                case 99: return "SB";
                case 154: return "SB";
                case 161: return "RNG";
                case 162: return "CA";
                case 180: return "RNG";//SB, replace to RNG
                case 181: return "RNG";//SB, replace to RNG
                case 182: return "RNG";//SB, replace to RNG
                case 183: return "RNG";//SB, replace to RNG
                case 184: return "RNG";//SB, replace to RNG
                case 185: return "RNG";//SB, replace to RNG
                case 186: return "RNG";//SB, replace to RNG
                case 202: return "SB";
                case 251: return "SB";
                case 208: return "RNG";
                case 209: return "RNG";
                case 210: return "RNG";
                case 204: return "SB";
                case 219: return "RNG";
                case 220: return "SB";
                case 211: return "CA";//SB, replace to RNG
                case 212: return "RNG";//SB, replace to RNG
                case 190: return "RNG";//SB, replace to RNG
                case 191: return "RNG";//SB, replace to RNG
                case 192: return "RNG";//SB, replace to RNG
                case 193: return "RNG";//SB, replace to RNG
                case 194: return "RNG";//SB, replace to RNG
                case 195: return "RNG";//SB, replace to RNG
                case 196: return "RNG";//SB, replace to RNG
                case 199: return "RNG";//SB, replace to RNG
                case 248: return "RNG";

                default:
                    return "-";

            }

        }
        static public string ConvertCurrencyRCI(int currency)
        {
            switch (currency)
            {
                case 1:
                    return "SGD";
                case 2:
                    return "MYR";
                case 3:
                    return "USD";
                case 4:
                    return "THB";
                case 5:
                    return "HKD";
                case 6:
                    return "EUR";
                case 9:
                    return "AUD";
                case 12:
                    return "GBP";
                case 13:
                    return "CNY";
                case 15:
                    return "IDR";
                case 20:
                    return "UUS";
                case 32:
                    return "JPY";
                case 41:
                    return "CHF";
                case 45:
                    return "KRW";
                case 46:
                    return "BND";
                case 49:
                    return "MXN";
                case 50:
                    return "CAD";
                case 51:
                    return "VND";
                case 52:
                    return "DKK";
                case 53:
                    return "SEK";
                case 54:
                    return "NOK";
                case 55:
                    return "RUB";
                case 56:
                    return "PLN";
                case 57:
                    return "CZK";
                case 58:
                    return "RON";
                case 61:
                    return "INR";
                case 70:
                    return "MMK";
                case 71:
                    return "KHR";
                case 73:
                    return "TRY";
                default:
                    return "-";
            }
        }
        static public string CovnertBetType(int betType)
        {
            switch (betType)
            {
                case 1: return "Handicap";
                case 2: return "Odd/Even";
                case 3: return "Over/Under";
                case 4: return "Correct Score (Soccer)";
                case 5: return "FT.1X2";
                case 6: return "Total Goal (Soccer)";
                case 7: return "1st Handicap";
                case 8: return "1st Over/Under";
                case 10: return "Outright";
                case 11: return "Total Corners (Soccer)";
                case 12: return "1st Odds/Even";
                case 13: return "Clean Sheet (Soccer)";
                case 14: return "First Goal/Last Goal (Soccer)";
                case 15: return "1st 1X2";
                case 16: return "HT/FT (Soccer)";
                case 17: return "2nd HDP (Soccer)";
                case 18: return "2nd Over/Under (Soccer)";
                case 19: return "Substitutes";
                case 20: return "Money Line";
                case 21: return "1st Money Line";
                case 22: return "Next Goal (Soccer)";
                case 23: return "Next Corner";
                case 24: return "Double chance (Soccer)";
                case 25: return "Draw No Bet (Soccer)";
                case 26: return "Both/One/Neither team to score (Soccer)";
                case 27: return "To win to nil (Soccer)";
                case 28: return "3-way handicap (Soccer)";
                case 29: return "Combo Parlay (Other Sports)";
                case 30: return "1st Correct Score (Soccer)";
                case 71: return "Casino Games (Casino)";
                case 81: return "1st ball O/U (Number Game)";
                case 82: return "Last ball O/U (Number Game)";
                case 83: return "1st ball O/E (Number Game)";
                case 84: return "Last ball O/E (Number Game)";
                case 85: return "Over/Under (Number Game)";
                case 86: return "Odd/Even (Number Game)";
                case 87: return "Next High/Low (Number Game)";
                case 88: return "Warrior (Number Game)";
                case 89: return "Next Combo (Number Game)";
                case 90: return "Number wheel (Number Game)";
                case 121: return "Home No Bet (Soccer)";
                case 122: return "Away No Bet (Soccer)";
                case 123: return "Draw / No Draw (Soccer)";
                case 124: return "FT.1X2 HDP (Soccer)";
                case 125: return "1st 1X2 HDP (Soccer)";
                case 126: return "1H Total Goal (Soccer)";
                case 127: return "1H First Goal/Last Goal (Soccer)";
                case 128: return "HT/FT Odd/Even (Soccer)";
                case 133:
                case 438: return "Home To Win Both Halves (Soccer)";
                case 134:
                case 439: return "Away To Win Both Halves (Soccer)";
                case 135: return "Penalty Shootout (Soccer)";
                case 140:
                case 442: return "Highest Scoring Half (Soccer)";
                case 141:
                case 443: return "Highest Scoring Half Home Team (Soccer)";
                case 142:
                case 444: return "Highest Scoring Half Away Team (Soccer)";
                case 145: return "Both Teams To Score (Soccer)";
                case 146:
                case 433: return "2nd Half Both Teams To Score (Soccer)";
                case 147:
                case 436: return "Home To Score In Both Halves (Soccer)";
                case 148:
                case 437: return "Away To Score In Both Halves (Soccer)";
                case 149:
                case 440: return "Home To Win Either Half (Soccer)";
                case 150:
                case 441: return "Away To Win Either Half (Soccer)";
                case 151:
                case 410: return "1st Half Double Chance (Soccer)";
                case 152:
                case 416: return "Half Time/Full Time Correct Score (Soccer)";
                case 1201: return "Handicap (Virtual Soccer)";
                case 1203: return "Over/Under 2.5 Goals (Virtual Soccer)";
                case 1204: return "Correct Score (Virtual Soccer)";
                case 1205: return "1X2 (Virtual Soccer)";
                case 1206: return "Total Goal (Virtual Soccer)";
                case 1220: return "Player Win (Virtual Tennis)";
                case 1224: return "Double chance (Virtual Soccer)";
                case 1231: return "Win (Virtual Sport)";
                case 1232: return "Place (Virtual Sport)";
                case 1233: return "Win/Place (Virtual Sport)";
                case 1235: return "Score Bet (Virtual Tennis)";
                case 1236: return "Total Points (Virtual Tennis)";
                case 1237: return "Forecast (Virtual Motorsports)";
                case 1238: return "Tricast (Virtual Motorsports)";
                case 153: return "Game Handicap (Tennis)";
                case 154: return "Set x Winner (Tennis)";
                case 155: return "Set x Game Handicap (Tennis)";
                case 156: return "Set x Total Game O/U (Tennis)";
                case 1301: return "Match Winner (Tennis)";
                case 1302: return "Match Correct Score (Tennis)";
                case 1303: return "Set Handicap (Tennis)";
                case 1305: return "Match Total Games Odd/Even (Tennis)";
                case 1306: return "Match Total Games Over/under (Tennis)";
                case 1308: return "Match Games Handicap (Tennis)";
                case 1311: return "Set x Winner (Tennis)";
                case 1312: return "Set x Total Games (Tennis)";
                case 1316: return "Set x Game Handicap (Tennis)";
                case 1317: return "Set x Correct Score (Tennis)";
                case 1318: return "Set x Total Game Odd/Even (Tennis)";
                case 1324: return "Set x Game y Winner (Tennis)";
                case 157: return "Odd/Even (Soccer)";
                case 159:
                case 406: return "Exact Total Goals (Soccer)";
                case 160: return "Next Goal (Soccer)";
                case 161:
                case 407: return "Exact Home Team Goals (Soccer)";
                case 162:
                case 409: return "Exact Away Team Goals (Soccer)";
                case 163:
                case 144: return "Result/Total Goals (Soccer)";
                case 164: return "Extra Time Next Goal (Soccer)";
                case 165: return "Extra Time 1H Correct Score (Soccer)";
                case 166: return "Extra Time Correct Score (Soccer)";
                case 167: return "Extra Time 1H 1X2 (Soccer)";
                case 168: return "Who advances to next round (Soccer)";
                case 169: return "Next Goal Time (Soccer)";
                case 170: return "Teams To Score (Soccer)";
                case 171:
                case 408: return "Winning Margin (Soccer)";
                case 172:
                case 415: return "Result And First Team To Score (Soccer)";
                case 173: return "Extra Time Yes/No (Soccer)";
                case 174: return "Extra Time and Goal (Soccer)";
                case 175: return "Match Decided Method (Soccer)";
                case 176: return "First Ten Minutes 1X2 (Soccer)";
                case 177:
                case 430: return "2H 1x2 (Soccer)";
                case 178: return "2H Over/Under (Soccer)";
                case 179: return "Exact 1H Goals (Soccer)";
                case 180: return "1H Next Goal (Soccer)";
                case 181: return "1H Exact Home Team Goals (Soccer)";
                case 182: return "1H Exact Away Team Goals (Soccer)";
                case 184:
                case 428: return "2nH Odd/Even (Soccer)";
                case 185:
                case 432: return "2H Draw No Bet (Soccer)";
                case 186:
                case 431: return "2H Double Chance (Soccer)";
                case 187: return "Exact 2H Goals (Soccer)";
                case 188:
                case 427: return "1H Both Teams To Score (Soccer)";
                case 189:
                case 434: return "Both Halves Over 1.5 Yes/No (Soccer)";
                case 190:
                case 435: return "Both Halves Under 1.5 Yes/No (Soccer)";
                case 191:
                case 411: return "1H Draw No Bet (Soccer)";
                case 192: return "First Goal Time(10 min) (Soccer)";
                case 193: return "First Goal Time(15 min) (Soccer)";
                case 194: return "Corners Odd/Even (Soccer)";
                case 195: return "Home Team Exact Corners (Soccer)";
                case 196: return "Away Team Exact Corners (Soccer)";
                case 197: return "Home Team Total Corners Over/Under (Soccer)";
                case 198: return "Away Team Total Corners Over/Under (Soccer)";
                case 199: return "Total Corners (Soccer)";
                case 200: return "1H Home Team Exact Corners (Soccer)";
                case 201: return "1H Away Team Exact Corners (Soccer)";
                case 202: return "1H Total Corners (Soccer)";
                case 203: return "1H Corners Odd/Even (Soccer)";
                case 204: return "1H Home Corner Over/Under (Soccer)";
                case 205: return "1H Away Corner Over/Under (Soccer)";
                case 206: return "First Corner (Soccer)";
                case 207: return "1H First Corner (Soccer)";
                case 208: return "Last Corner (Soccer)";
                case 209: return "1H Last Corner (Soccer)";
                case 210: return "Player Sent Off (Soccer)";
                case 211: return "1H Player Sent off (Soccer)";
                case 212: return "Home Team Player Sent Off (Soccer)";
                case 213: return "1H Home Team Player Sent Off (Soccer)";
                case 214: return "Away Team Player Sent Off (Soccer)";
                case 215: return "1H Away Team Player Sent Off (Soccer)";
                case 221: return "Next 1 minutes (Soccer)";
                case 222: return "Next 5 minute (Soccer)";
                case 223: return "What will happen first in next 1 minute (Soccer)";
                case 224: return "What will happen first in next 5 minute (Soccer)";
                case 225: return "Next 1 minute set piece (Soccer)";
                case 2101: return "Baccarat (RNG – RNG Game)";
                case 2102: return "Battle War (RNG – RNG Game)";
                case 2103: return "Blackjack (RNG – RNG Game)";
                case 2104: return "Poker Tri (RNG – RNG Game)";
                case 2105: return "Casino Hold’em (RNG – RNG Game)";
                case 2106: return "Pai Gow Poker (RNG – RNG Game)";
                case 2107: return "Mini Roulette (RNG – RNG Game)";
                case 2108: return "Roulette Pro (RNG – RNG Game)";
                case 2109: return "Sic Bo (RNG – RNG Game)";
                case 2110: return "Jacks or Better (RNG – RNG Game)";
                case 2111: return "Roulette Pro (Fast) (RNG – RNG Game)";
                case 2112: return "Blackjack Perfect Pair (RNG – RNG Game)";
                case 2113: return "Baccarat Super 6 (RNG – RNG Game)";
                case 2114: return "Fish Prawn Crab (RNG – RNG Game)";
                case 2115: return "Dragon Tiger (RNG – RNG Game)";
                case 2116: return "Three Knights (RNG – RNG Game)";
                case 2117: return "Money Wheel (RNG – RNG Game)";
                case 2118: return "Fan Tan (RNG – RNG Game)";
                case 2119: return "Card Wheel (RNG – RNG Game)";
                case 2120: return "Texas Hold’em (RNG – RNG Game)";
                case 2121: return "Face Up 21 (RNG – RNG Game)";
                case 2122: return "Three Cards Rummy (RNG – RNG Game)";
                case 2123: return "Red Dog (RNG – RNG Game)";
                case 2124: return "Multi Wheel Roulette (RNG – RNG Game)";
                case 2125: return "Monkey HiLo (RNG – RNG Game)";
                case 2126: return "Mahjong HiLo (RNG – RNG Game)";
                case 2127: return "Keno Camera (RNG – RNG Game)";
                case 2128: return "Keno Chinese (RNG – RNG Game)";
                case 2129: return "Turf King (RNG – RNG Game)";
                case 2130: return "SpaKz (RNG – RNG Game)";
                case 2131: return "Maya Push (RNG – RNG Game)";
                case 2132: return "Gong Xi Fa Chai (RNG – RNG Game)";
                case 2133: return "Love Story (RNG – RNG Game)";
                case 2134: return "Penalty Hat-Trick (RNG – RNG Game)";
                case 2135: return "Fruit Scratch (RNG – RNG Game)";
                case 2136: return "Ho Ho Ho (RNG – RNG Game)";
                case 2137: return "Knock2 Eggs (RNG – RNG Game)";
                case 2138: return "Poke Mole (RNG – RNG Game)";
                case 2139: return "Fancy Scratch (RNG – RNG Game)";
                case 2140: return "Flames of Olympics (RNG – RNG Game)";
                case 2141: return "Sushi Bar (RNG – RNG Game)";
                case 2142: return "Baseball Battle (RNG – RNG Game)";
                case 2143: return "10’s or Better (RNG – RNG Game)";
                case 2144: return "4-Line Jacks or Better (RNG – RNG Game)";
                case 2145: return "25-Line Jacks or Better (RNG – RNG Game)";
                case 2146: return "50-Line Jacks or Better (RNG – RNG Game)";
                case 2147: return "Jungle Wild (RNG – RNG Game)";
                case 2148: return "Engagement (RNG – RNG Game)";
                case 2149: return "God of Prosperity (RNG – RNG Game)";
                case 2150: return "Money Slot (RNG – RNG Game)";
                case 2151: return "Fox Force 5 (RNG – RNG Game)";
                case 2152: return "Journey to the West (RNG – RNG Game)";
                case 2153: return "Winter Slot (RNG – RNG Game)";
                case 2154: return "Western Zodiac (RNG – RNG Game)";
                case 2155: return "Chinese Zodiac (RNG – RNG Game)";
                case 2156: return "Water Margin (RNG – RNG Game)";
                case 2157: return "Champions (RNG – RNG Game)";
                case 2158: return "Crazy Hospital (RNG – RNG Game)";
                case 2159: return "Mahjong Legend (RNG – RNG Game)";
                case 2160: return "Halloween (RNG – RNG Game)";
                case 2161: return "Happy Vampire (RNG – RNG Game)";
                case 2162: return "Football Fans (RNG – RNG Game)";
                case 2163: return "Oriental Beauty (RNG – RNG Game)";
                case 2164: return "Ninja Hero (RNG – RNG Game)";
                case 2165: return "Tales of Brothel (RNG – RNG Game)";
                case 2166: return "Fortune Gods (RNG – RNG Game)";
                case 2167: return "Magician (RNG – RNG Game)";
                case 2168: return "GOP(both way) (RNG – RNG Game)";
                case 2169: return "Iron Fist (RNG – RNG Game)";
                case 2170: return "Jungle Wild(Both Way) (RNG – RNG Game)";
                case 2171: return "Dragon Slot (RNG – RNG Game)";
                case 2172: return "Cowboy Fever (RNG – RNG Game)";
                case 2173: return "Dino Camp (RNG – RNG Game)";
                case 2174: return "Tattoo Art (RNG – RNG Game)";
                case 2175: return "Mamak Corner (RNG – RNG Game)";
                case 2176: return "Sea Orchestra (RNG – RNG Game)";
                case 2177: return "Cash Out (RNG – RNG Game)";
                case 2201: return "Baccarat (RNG – Mini Game)";
                case 2202: return "Casino Hold’em (RNG – Mini Game)";
                case 2203: return "BlackJack (RNG – Mini Game)";
                case 2204: return "Jacks or Better (RNG – Mini Game)";
                case 2205: return "Sic Bo (RNG – Mini Game)";
                case 2206: return "Money Slot (RNG – Mini Game)";
                case 2301: return "Baccarat (RNG – Mobile)";
                case 2302: return "Casino Hold’ em (RNG – Mobile)";
                case 2303: return "Blackjack (RNG – Mobile)";
                case 2304: return "Jungle Wild (RNG – Mobile)";
                case 2305: return "God of Prosperity (RNG – Mobile)";
                case 2306: return "Sic Bo (RNG – Mobile)";
                case 2307: return "Oriental Beauty (RNG – Mobile)";
                case 2308: return "Magician (RNG – Mobile)";
                case 2309: return "Tales of Brothel (RNG – Mobile)";
                case 2310: return "Mini Roulette (RNG – Mobile)";
                case 2311: return "Baccarat Super 6 (RNG – Mobile)";
                case 2312: return "Blackjack Perfect Pair (RNG – Mobile)";
                case 2314: return "Face Up 21 (RNG – Mobile)";
                case 501: return "Match Winner (Cricket)";
                case 401: return "Home Team OU (Basketball,Dec)";
                case 402: return "Away Team OU (Basketball,Dec)";
                case 403: return "1H Home Team OU (Basketball,Dec)";
                case 404: return "1H Away Team OU (Basketball,Dec)";
                case 405: return "2nd Half Correct Score (Soccer)";
                case 412: return "Exact 1st Goal (Soccer)";
                case 413: return "FT Correct Score (Soccer)";
                case 414: return "1H Correct Score (Soccer)";
                case 417: return "Both Teams To Score/Result (Soccer)";
                case 418: return "Both Teams To Score/Total Goal (Soccer)";
                case 419: return "Which Half First Goal (Soccer)";
                case 420: return "Home Team Which Half First Goal (Soccer)";
                case 421: return "Away Team Which Half First Goal (Soccer)";
                case 422: return "First Team 2 Goals (Soccer)";
                case 423: return "First Team 3 Goals (Soccer)";
                case 424: return "First Goal Method (Soccer)";
                case 425: return "To Win From Behind (Soccer)";
                case 426: return "1H Winning Margin (Soccer)";
                case 429: return "Exact 2nd Half Goals (Soccer)";
                case 445: return "Both Team To Score in 1H/2H (Soccer)";
                case 446: return "Home 1H To Score/2H To Score (Soccer)";
                case 447: return "Away 1H To Score/2H To Score (Soccer)";
                case 18000: return "Colossus Bets (Colossus Bets)";
                case 18001: return "Pool Bet (Colossus Bets)";
                case 18002: return "Cash-Out (Colossus Bets)";
                case 18004: return "Consolation Prize (Colossus Bets)";
                case 18005: return "Jackpot Prize (Colossus Bets)";
                case 601: return "FT Winning Margin 14 Way (Basketball)";
                case 602: return "FT Winning Margin 12 Way (Basketball)";
                case 603: return "FT Which team to score the highest quarter (Basketball)";
                case 604: return "FT Which team to score the first basket (Basketball)";
                case 605: return "FT Which team to score the last basket (Basketball)";
                case 606: return "1H Race to X (Basketball)";
                case 607: return "2H Race to X (Basketball)";
                case 608: return "1H Winning Margin 13 Way (Basketball)";
                case 609: return "Quarter X Handicap (Basketball)";
                case 610: return "Quarter X Over Under (Basketball)";
                case 611: return "Quarter X Odd Even (Basketball)";
                case 612: return "Quarter X Moneyline (Basketball)";
                case 613: return "Quarter X Race To Y Points (Basketball)";
                case 614: return "Quarter X Winning Margin 7 Way (Basketball)";
                case 615: return "Quarter X Home Team Over Under (Basketball)";
                case 616: return "Quarter X Away Team Over Under (Basketball)";
                case 617: return "Quarter X Which team to score the last basket (Basketball)";
                case 1011: return "Fishing World (Fishing World)";
                case 4002: return "BigHaul (Macau Games)";
                case 4003: return "BushidoCode (Macau Games)";
                case 4004: return "CasinoStudPoker (Macau Games)";
                case 4005: return "CatchtheWaves (Macau Games)";
                case 4008: return "DancingLions (Macau Games)";
                case 4009: return "DragonKing (Macau Games)";
                case 4010: return "DragonsandPearls (Macau Games)";
                case 4011: return "Fish’O’Mania (Macau Games)";
                case 4012: return "FortunePanda (Macau Games)";
                case 4013: return "GodofWealth (Macau Games)";
                case 4014: return "GoldofRa (Macau Games)";
                case 4015: return "GuardianLion (Macau Games)";
                case 4018: return "KingofTime (Macau Games)";
                case 4019: return "LadyLuck (Macau Games)";
                case 4020: return "MysticRiches (Macau Games)";
                case 4021: return "PhoenixPrincess (Macau Games)";
                case 4022: return "Roulette (Macau Games)";
                case 4023: return "TempleTreasures (Macau Games)";
                case 4024: return "ThaiDragon (Macau Games)";
                case 4025: return "TreasureReef (Macau Games)";
                case 4026: return "Venetia (Macau Games)";
                case 4028: return "WildDolphin (Macau Games)";
                case 4029: return "WolfQuest (Macau Games)";
                case 2401: return "Baccarat (Allbet)";
                case 2402: return "VIP Baccarat (Allbet)";
                case 2403: return "Roulette (Allbet)";
                case 2404: return "Sic Bo (Allbet)";
                case 2405: return "Dragon Tiger (Allbet)";
                case 2701: return "FT 1X2 (Virtual Soccer League, Virtual Soccer Nation)";
                case 2702: return "1H 1x2 (Virtual Soccer League, Virtual Soccer Nation)";
                case 2703: return "FT Over/Under (Virtual Soccer League, Virtual Soccer Nation)";
                case 2704: return "1H Over/Under (Virtual Soccer League, Virtual Soccer Nation)";
                case 2705: return "FT Handicap (Virtual Soccer League, Virtual Soccer Nation)";
                case 2706: return "1H Handicap (Virtual Soccer League, Virtual Soccer Nation)";
                case 2707: return "Correct Score (Virtual Soccer League, Virtual Soccer Nation)";
                case 2799: return "Mix Parlay (Virtual Soccer League, Virtual Soccer Nation)";
                case 1031: return "Max5D 60 (Lottery)";
                case 1032: return "Max5D 90 (Lottery)";
                case 1033: return "Max3D 60 (Lottery)";
                case 1034: return "Max3D 90 (Lottery)";
                case 1035: return "Max11x5 60 (Lottery)";
                case 1036: return "Max11x5 90 (Lottery)";
                case 1037: return "MaxDice 60 (Lottery)";
                case 1038: return "MaxDice 90 (Lottery)";
                case 461: return "Home Team OU (Soccer) ";
                case 462: return "Away Team OU (Soccer)";
                case 463: return "1H Home Team OU (Soccer)";
                case 464: return "1H Away Team OU (Soccer)";
                case 448: return "Last Team To Score";
                case 449: return "Double Chance & Total Goals";
                case 450: return "Odd/Even & Total Goals";
                case 451: return "Both Teams to Score / Double Chance";
                case 452: return "Highest Scoring Half (2 Way)";
                case 453: return "1H 3-Way Handicap";
                case 454: return "Double Chance & First Team To Score";
                case 455: return "Time of First Goal";
                case 456: return "1H Both Teams To Score / Result";
                case 457: return "1H Both Teams To Score / Total Goals";
                case 458: return "Asian 1X2";
                case 459: return "1H Asian 1X2";
                case 460: return "Which Team Will Win By 5+ Goals";
                case 911: return "Cash Out";
                case 8000: return "Exchange";
                case 143: return "1H Result/Total Goals";
                //case 1501: return "Australia";
                //case 1502: return "Bejing";
                //case 1503: return "Slovakia";
                //case 1504: return "Canada";
                //case 1505: return "West Canada";
                //case 1506: return "Taiwan";
                //case 1507: return "Massachusetts";
                //case 1508: return "Ohio";
                //case 1509: return "Malta";
                //case 1511: return "Kentucky";
                //case 1513: return "Michigan";
                //case 1514: return "Korea";
                //case 1515: return "Max Keno";
                //case 1516: return "Max Keno2";

                case 2801: return "Match Winner";
                case 2802: return "1H Winner";
                case 2803: return "Over/Under";
                case 2804: return "1H Over/Under";
                case 2805: return "Handicap";
                case 2806: return "1H Handicap";
                case 2807: return "Winning Margin 6 Way";
                case 2808: return "1H Winning Margin 7 Way";
                case 2809: return "FT Race To X Points";
                case 2811: return "Home Team Over/Under";
                case 2812: return "Away Team Over/Under";
                case 4101: return "888 Dragons™";
                case 4102: return "Triple Tigers™";
                case 4103: return "Diamonds are Forever 3 Lines";
                case 4104: return "Gold Train™";
                case 4105: return "888 Gold™";
                case 4106: return "7 Piggies™";
                case 4107: return "7 Monkeys";
                case 4108: return "Monkey Madness™";
                case 4109: return "Diamond Strike™";
                case 4110: return "Fairytale Fortune™";
                case 4111: return "8 Dragons™";
                case 4112: return "Lady Godiva";
                case 4113: return "Journey to the West";
                case 4114: return "Wolf Gold™";
                case 4115: return "Dragon Kingdom";
                case 4116: return "Hot Safari";
                case 4117: return "3 Kingdoms - Battle of Red Cliffs";
                case 4118: return "Chilli Heat";
                case 4119: return "Caishen’s Gold™";
                case 4120: return "Pixie Wings™";
                case 4121: return "Aztec Gems";
                case 4122: return "Lucky New Year";
                case 4123: return "Busy Bees™";
                case 4124: return "Fruity Blast™";
                case 4125: return "Crazy 7s™";
                case 4126: return "Devil's 13™";
                case 4127: return "Spooky Fortune™";
                case 4128: return "Panda's Fortune™";
                case 4129: return "Money Roll™";
                case 4130: return "Vegas Nights™";
                case 4131: return "Wild Spells™";
                case 4132: return "Jurassic Giants™";
                case 4133: return "Panther Queen™";
                case 4134: return "Irish Charms™";
                case 4135: return "Queen of Atlantis™";
                case 4136: return "Queen of Gold™";
                case 4137: return "Hercules Son of Zeus";
                case 4138: return "3 Genie Wishes";
                case 4139: return "The Catfather Part II";
                case 4140: return "Beowulf";
                case 4141: return "Lucky Dragons";
                case 4142: return "Dwarven Gold Deluxe";
                case 4143: return "Romeo and Juliet";
                case 4144: return "Hockey League Wild Match";
                case 4145: return "Lady of the Moon";
                case 4146: return "KTV";
                case 4147: return "Sugar Rush";
                case 4148: return "Sugar Rush Summer Time";
                case 4149: return "Sugar Rush Valentine's Day";
                case 4150: return "Sugar Rush Winter";
                case 4151: return "Tales of Egypt";
                case 4152: return "Hockey League";
                case 4153: return "Glorious Rome";
                case 4154: return "Magic Crystals";
                case 4155: return "Dwarven Gold";
                case 4156: return "Great Reef";
                case 4157: return "Aladdin's Treasure";
                case 4158: return "Mighty Kong";
                case 4159: return "The Catfather";
                case 4160: return "Gold Rush™";
                case 4161: return "Ancient Egypt";
                case 4162: return "Santa";
                case 4163: return "Joker's Jewels™";
                case 4164: return "Fire 88™";
                case 4165: return "The Champions™";
                case 4166: return "5 Lions™";
                case 4167: return "Asgard™";
                case 4168: return "Madame Destiny™";
                case 4169: return "Great Rhino™";
                case 4170: return "Jade Butterfly™";
                case 4171: return "Leprechaun Song™";
                case 4172: return "Da Vinci's Treasure™";
                case 4173: return "Master Chen's Fortune™";
                case 4174: return "Vegas Magic™";
                case 4031: return "Caishen's Quest";
                case 4032: return "Lucky Dragons";
                case 4033: return "Highway Cash";
                case 4035: return "Goldbeard's Treasure";
                case 4036: return "Double Triple 8";
                case 4037: return "Gods of Fortune";
                case 4038: return "Busy Bees";
                case 4039: return "Funny Monkey";
                case 4040: return "Gods of Fortune Deluxe";
                case 4041: return "Golf Tour";
                case 4042: return "Lucky 8's";
                case 4043: return "Triple Lion";
                case 9001: return "Map X Moneyline";
                case 9002: return "Map X Total Kills Handicap";
                case 9003: return "Map X Total Kills Over/Under";
                case 9004: return "Map X Total Kills Moneyline";
                case 9005: return "Map X Total Kills Odd/Even";
                case 9006: return "Map X First Blood";
                case 9007: return "Map X First to Y Kills";
                case 9008: return "Map X Total Towers Handicap";
                case 9009: return "Map X Total Towers Over/Under";
                case 9010: return "Map X Total Towers Moneyline";
                case 9011: return "Map X First Tier Y Tower";
                case 9012: return "Map X Total Roshans Handicap";
                case 9013: return "Map X Total Roshans Over/Under";
                case 9014: return "Map X Total Roshans Moneyline";
                case 9015: return "Map X 1st Roshan";
                case 9016: return "Map X 2nd Roshan";
                case 9017: return "Map X 3rd Roshan";
                case 9018: return "Map X Total Barracks Handicap";
                case 9019: return "Map X Total Barracks Over/Under";
                case 9020: return "Map X Total Barracks Moneyline";
                case 9021: return "Map X Barracks 1st Lane";
                case 9022: return "Map X Barracks 2nd Lane";
                case 9023: return "Map X Barracks 3rd Lane";
                case 9024: return "Map X Total Turrets Handicap";
                case 9025: return "Map X Total Turrets Over/Under";
                case 9026: return "Map X Total Turrets Moneyline";
                case 9027: return "Map X First Tier Y Turret";
                case 9028: return "Map X Total Dragons Handicap";
                case 9029: return "Map X Total Dragons Over/Under";
                case 9030: return "Map X Total Dragons Moneyline";
                case 9031: return "Map X 1st Dragon";
                case 9032: return "Map X 2nd Dragon";
                case 9033: return "Map X 3rd Dragon";
                case 9034: return "Map X Total Barons Handicap";
                case 9035: return "Map X Total Barons Over/Under";
                case 9036: return "Map X Total Barons Moneyline";
                case 9037: return "Map X 1st Baron";
                case 9038: return "Map X 2nd Baron";
                case 9039: return "Map X 3rd Baron";
                case 9040: return "Map X Total Inhibitors Handicap";
                case 9041: return "Map X Total Inhibitors Over/Under";
                case 9042: return "Map X Total Inhibitors Moneyline";
                case 9043: return "Map X 1st Inhibitor";
                case 9044: return "Map X 2nd Inhibitor";
                case 9045: return "Map X 3rd Inhibitor";
                case 9046: return "Map X Total Tyrants Handicap";
                case 9047: return "Map X Total Tyrants Over/Under";
                case 9048: return "Map X Total Tyrants Moneyline";
                case 9049: return "Map X 1st Tyrant";
                case 9050: return "Map X 2nd Tyrant";
                case 9051: return "Map X 3rd Tyrant";
                case 9052: return "Map X Total Overlords Handicap";
                case 9053: return "Map X Total Overlords Over/Under";
                case 9054: return "Map X Total Overlords Moneyline";
                case 9055: return "Map X 1st Overlord";
                case 9056: return "Map X 2nd Overlord";
                case 9057: return "Map X 3rd Overlord";
                case 9058: return "Map X Duration Over/Under";
                case 9059: return "Map X Rounds Handicap";
                case 9060: return "Map X Rounds Over/Under";
                case 9061: return "Map X Rounds Odd/Even";
                case 9062: return "Map X First to Y Rounds";
                case 9063: return "Map X First Half";
                case 9064: return "Map X Second Half";
                case 9065: return "Map X Most First Kill";
                case 9066: return "Map X Clutches";
                case 9067: return "Map X 16th Round";
                case 9068: return "Map X Round Y Moneyline";
                case 9069: return "Map X Round Y Total Kills Moneyline";
                case 9070: return "Map X Round Y Total Kills Over/Under";
                case 9071: return "Map X Round Y Total Kills Odd/Even";
                case 9072: return "Map X Round Y First Kill";
                case 9073: return "Map X Round Y Bomb Plant";
                case 9074: return "Map X Rounds Over/Under";
                case 9075: return "Map X Final Round Bomb Plant";
                case 9076: return "Map X Clutches Handicap";
                case 9077: return "Map X Round Y Total Kills Handicap";
                case 9078: return "Map X Total Towers Odd/Even";
                case 9079: return "Map X Total Roshans Odd/Even";
                case 9080: return "Map X Total Barracks Odd/Even";
                case 9081: return "Map X Total Turrets Odd/Even";
                case 9082: return "Map X Total Dragons Odd/Even";
                case 9083: return "Map X Total Barons Odd/Even";
                case 9084: return "Map X Total Inhibitors Odd/Even";
                case 9085: return "Map X Total Tyrants Odd/Even";
                case 9086: return "Map X Total Overlords Odd/Even";
                case 8101: return "Big/Small";
                case 8102: return "Odd/Even";
                case 8103: return "4 Seasons";
                case 8104: return "More Odd/More Even";
                case 8105: return "Combo";
                case 4175: return "Leprechaun Carol™";
                case 4176: return "Mustang Gold™";
                case 4177: return "Triple Dragons™";
                case 4178: return "Safari King™";
                case 1039: return "Max Racing";
                case 1040: return "Max Racing 2";
                case 1041: return "Penalty Shoot-out";
                case 1042: return "Penalty Shoot-out 2";
                case 1043: return "Se Die";
                case 1044: return "Se Die 2";
                case 1045: return "Lottery Bull";
                case 1046: return "Lottery Bull 2";
                case 1047: return "Max Se Die";
                case 1048: return "Max Se Die 2";
                case 4179: return "Treasure Horse™";
                case 4180: return "Golden Pig™";
                case 4181: return "Wild Gladiators™";
                case 4182: return "Extra Juicy™";
                case 4183: return "Egyptian Fortunes™";
                case 4184: return "The Dog House™";
                case 4185: return "Caishen’s Cash™";
                case 4186: return "Sweet Bonanza™";
                case 4187: return "7 Monkeys JP";
                case 4188: return "Hot Safari JP";
                case 4189: return "The Catfather Part II JP";
                case 4300: return "Promotion Prize";
                case 1901: return "Video Poker";
                case 467: return "Half Time/Full Time Exact Total Goals";
                case 468: return "Score Box Handicap";
                case 469: return "Score Box Over/Under";
                case 470: return "Corners Odd/Even";
                case 471: return "1H Corners Odd/Even";
                case 472: return "2H Corners Odd/Even";
                case 473: return "Total Corners";
                case 474: return "1H Total Corners";
                case 475: return "Alternative Corners";
                case 476: return "1H Alternative Corners";
                case 477: return "Corner 3-Way Handicap";
                case 478: return "1H Corner 3-Way Handicap";
                case 479: return "Time Of First Corner";
                case 481: return "Time Of 2H First Corner";
                case 482: return "Home Team Over/Under Corners";
                case 483: return "Away Team Over/Under Corners";
                case 484: return "1H Home Team Over/Under Corners";
                case 485: return "1H Away Team Over/Under Corners";
                case 486: return "Corners Race";
                case 487: return "1H Corners Race";
                case 488: return "First Corner";
                case 489: return "1H First Corner";
                case 490: return "2H First Corner";
                case 491: return "Last Corner";
                case 492: return "1H Last Corner";
                case 493: return "Half Time/Full Time Total Corners";
                case 494: return "1H Correct Corners";
                case 495: return "2H Correct Corners";
                case 496: return "Corner Highest Scoring Half";
                case 497: return "Corner Highest Scoring Half";

                default:
                    return "-";

            }


        }

        static public string ConvertTicketStatus(int betType, string ticket_status)
        {

            if (ticket_status == "WON" && new int[] { 1, 3, 7, 8, 17, 18 }.Contains(betType) && Math.Abs(betType) == (decimal)0.25)
                return "Half WON";
            else if (ticket_status == "LOSE" && new int[] { 1, 3, 7, 8, 17, 18 }.Contains(betType) && Math.Abs(betType) == (decimal)0.25)
                return "Half LOSE";
            else
                return ticket_status;

        }

    }
}
