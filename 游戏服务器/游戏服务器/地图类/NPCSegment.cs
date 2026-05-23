using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using _001D_000F_0007_0013_0011_0015;
using 游戏服务器.副本类;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using DevExpress.Data.Extensions;

namespace 游戏服务器.地图类
{
    public class NPCSegment
    {
        public NPCPage Page;

        private static Dictionary<int, 守卫实例> 脚本刷出的守卫;

        public readonly string Key;

        public List<NPCChecks> CheckList = new List<NPCChecks>();

        public List<NPCActions> ActList = new List<NPCActions>();

        public List<NPCActions> ElseActList = new List<NPCActions>();

        public List<string> Say;

        public List<string> ElseSay;

        public List<string> Buttons;

        public List<string> ElseButtons;

        public List<string> GotoButtons;

        public string Param1;

        public int Param1Instance;

        public int Param2;

        public int Param3;

        private bool orMode;

        public List<string> Args = new List<string>();

        public NPCSegment(NPCPage page, List<string> say, List<string> buttons, List<string> elseSay, List<string> elseButtons, List<string> gotoButtons)
        {
            this.Page = page;
            this.Say = say;
            this.Buttons = buttons;
            this.ElseSay = elseSay;
            this.ElseButtons = elseButtons;
            this.GotoButtons = gotoButtons;
        }

        public string[] ParseArguments(string[] words)
        {
            Regex regex;
            regex = new Regex("\\%ARG\\((\\d+)\\)");
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "\"\"")
                {
                    words[i] = string.Empty;
                }
                foreach (Match item in regex.Matches(words[i].ToUpper()))
                {
                    if (item.Success)
                    {
                        int num;
                        num = Convert.ToInt32(item.Groups[1].Value);
                        if (this.Page.Args.Count >= num + 1)
                        {
                            words[i] = words[i].Replace(item.Groups[0].Value, this.Page.Args[num]);
                        }
                    }
                }
            }
            return words;
        }

        public void AddVariable(地图对象 mapobject, string key, string value)
        {
            string text;
            text = key.ToUpper();
            uint result4;
            if (!(text == "PPOINT"))
            {
                ushort result3;
                if (!(text == "DAYPOINT"))
                {
                    if (!new Regex("[A-Za-z][0-9]").Match(key).Success)
                    {
                        return;
                    }
                    int result;
                    result = 0;
                    int result2;
                    switch (key[0])
                    {
                        case 'G':
                        case 'g':
                            if (int.TryParse(key.Substring(1), out result2) && int.TryParse(value, out result))
                            {
                                系统数据.数据.脚本数字[result2] = result;
                            }
                            return;
                        case 'B':
                        case 'b':
                            if (int.TryParse(key.Substring(1), out result2))
                            {
                                mapobject.当前地图.字符变量[result2] = value;
                            }
                            return;
                        case 'A':
                        case 'a':
                            if (int.TryParse(key.Substring(1), out result2))
                            {
                                系统数据.数据.脚本字符[result2] = value;
                            }
                            return;
                        case 'Q':
                        case 'q':
                            {
                                if (mapobject is 玩家实例 玩家实例4 && 地图处理网关.守卫对象表.TryGetValue(玩家实例4.NPCObjectID, out var value2) && int.TryParse(key.Substring(1), out result2) && int.TryParse(value, out result))
                                {
                                    value2.脚本数字[result2] = result;
                                }
                                return;
                            }
                        case 'T':
                        case 't':
                            if (mapobject is 玩家实例 玩家实例3 && int.TryParse(key.Substring(1), out result2))
                            {
                                玩家实例3.角色数据.脚本字符[result2] = value;
                            }
                            return;
                        case 'U':
                        case 'u':
                            if (mapobject is 玩家实例 玩家实例5 && int.TryParse(key.Substring(1), out result2) && int.TryParse(value, out result))
                            {
                                玩家实例5.角色数据.脚本数字[result2] = result;
                            }
                            return;
                        case 'M':
                        case 'm':
                            if (int.TryParse(key.Substring(1), out result2) && int.TryParse(value, out result))
                            {
                                mapobject.当前地图.数字变量[result2] = result;
                            }
                            return;
                        case 'J':
                        case 'j':
                            if (mapobject is 玩家实例 玩家实例2 && int.TryParse(key.Substring(1), out result2) && int.TryParse(value, out result))
                            {
                                玩家实例2.角色数据.零点数字[result2] = result;
                            }
                            return;
                    }
                    int num;
                    num = 0;
                    while (true)
                    {
                        if (num < mapobject.NPCVar.Count)
                        {
                            if (string.Equals(mapobject.NPCVar[num].Key, key, StringComparison.CurrentCultureIgnoreCase))
                            {
                                break;
                            }
                            num++;
                            continue;
                        }
                        mapobject.NPCVar.Add(new KeyValuePair<string, string>(key, value));
                        return;
                    }
                    mapobject.NPCVar[num] = new KeyValuePair<string, string>(mapobject.NPCVar[num].Key, value);
                }
                else if (mapobject is 玩家实例 玩家实例6 && ushort.TryParse(value, out result3))
                {
                    玩家实例6.角色数据.日程进度.V = result3;
                }
            }
            else if (mapobject is 玩家实例 玩家实例7 && uint.TryParse(value, out result4))
            {
                玩家实例7.所属账号.推广点.V = result4;
            }
        }

        public string FindVariable(地图对象 mapobject, string key)
        {
            string text;
            text = key.ToUpper();
            if (!(text == "PPOINT"))
            {
                if (!(text == "DAYPOINT"))
                {
                    if (!new Regex("\\%[A-Za-z][0-9]").Match(key).Success)
                    {
                        return key;
                    }
                    string text2;
                    text2 = key.Substring(1);
                    int result;
                    result = 0;
                    switch (text2[0])
                    {
                        case 'G':
                        case 'g':
                            if (int.TryParse(text2.Substring(1), out result))
                            {
                                return 系统数据.数据.脚本数字[result].ToString();
                            }
                            break;
                        case 'B':
                        case 'b':
                            if (int.TryParse(text2.Substring(1), out result))
                            {
                                return mapobject.当前地图.字符变量[result];
                            }
                            break;
                        case 'A':
                        case 'a':
                            if (int.TryParse(text2.Substring(1), out result))
                            {
                                return 系统数据.数据.脚本字符[result];
                            }
                            break;
                        case 'Q':
                        case 'q':
                            {
                                if (mapobject is 玩家实例 玩家实例4 && 地图处理网关.守卫对象表.TryGetValue(玩家实例4.NPCObjectID, out var value) && int.TryParse(key.Substring(1), out result))
                                {
                                    return value.脚本数字[result].ToString();
                                }
                                break;
                            }
                        default:
                            foreach (KeyValuePair<string, string> item in mapobject.NPCVar)
                            {
                                if (string.Equals(item.Key, text2, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    return item.Value;
                                }
                            }
                            break;
                        case 'T':
                        case 't':
                            if (mapobject is 玩家实例 玩家实例5 && int.TryParse(text2.Substring(1), out result))
                            {
                                return 玩家实例5.角色数据.脚本字符[result];
                            }
                            break;
                        case 'U':
                        case 'u':
                            if (mapobject is 玩家实例 玩家实例3 && int.TryParse(text2.Substring(1), out result))
                            {
                                return 玩家实例3.角色数据.脚本数字[result].ToString();
                            }
                            break;
                        case 'M':
                        case 'm':
                            if (int.TryParse(text2.Substring(1), out result))
                            {
                                return mapobject.当前地图.数字变量[result].ToString();
                            }
                            break;
                        case 'J':
                        case 'j':
                            if (mapobject is 玩家实例 玩家实例2 && int.TryParse(text2.Substring(1), out result))
                            {
                                return 玩家实例2.角色数据.零点数字[result].ToString();
                            }
                            break;
                    }
                    return key;
                }
                if (mapobject is 玩家实例 玩家实例6)
                {
                    return 玩家实例6.角色数据.日程进度.V.ToString();
                }
                return "";
            }
            if (mapobject is 玩家实例 玩家实例7)
            {
                return 玩家实例7.所属账号.推广点.V.ToString();
            }
            return "";
        }

        public void ParseCheck(string line, bool orMode)
        {
            string[] array;
            array = this.ParseArguments(line.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            if (array.Length == 0)
            {
                return;
            }
            Regex regex;
            regex = new Regex("\\[(.*?)\\]");
            Regex regex2;
            regex2 = new Regex("\"([^\"]*)\"");
            bool notMode;
            notMode = false;
            this.orMode = orMode;
            if (array[0].ToUpper() == "NOT" && array.Length > 1)
            {
                string[] array2;
                array2 = new string[array.Length - 1];
                Array.Copy(array, 1, array2, 0, array.Length - 1);
                notMode = true;
                array = array2;
            }
            switch (array[0].ToUpper())
            {
                case "MIN":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckMinute, array[1]));
                    }
                    break;
                case "HOUR":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckHour, array[1]));
                    }
                    break;
                case "LEVEL":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.Level, array[1], array[2], (array.Length > 3) ? array[3] : string.Empty));
                    }
                    break;
                case "CHECK":
                    if (array.Length >= 3)
                    {
                        Match match2;
                        match2 = regex.Match(array[1]);
                        if (match2.Success)
                        {
                            string value;
                            value = match2.Groups[1].Captures[0].Value;
                            this.CheckList.Add(new NPCChecks(notMode, CheckType.Check, value, array[2]));
                        }
                    }
                    break;
                case "RANDOM":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.Random, array[1]));
                    }
                    break;
                case "ISADMIN":
                    this.CheckList.Add(new NPCChecks(notMode, CheckType.IsAdmin));
                    break;
                case "INGUILD":
                    {
                        string text5;
                        text5 = string.Empty;
                        if (array.Length > 1)
                        {
                            text5 = array[1];
                        }
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.InGuild, text5));
                        break;
                    }
                case "HASBUFF":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.HasBuff, array[1]));
                    }
                    break;
                case "RANDOMEX":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.RandomEx, array[1], (array.Length < 3) ? "" : array[2]));
                    }
                    break;
                case "PETLEVEL":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.PetLevel, array[1], array[2]));
                    }
                    break;
                case "CHECKHUM":
                    if (array.Length >= 4)
                    {
                        string text2;
                        text2 = ((array.Length < 5) ? "1" : array[4]);
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckHum, array[1], array[2], array[3], text2));
                    }
                    break;
                case "CHECKMON":
                    if (array.Length >= 4)
                    {
                        string text2;
                        text2 = ((array.Length < 5) ? "1" : array[4]);
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckMon, array[1], array[2], array[3], text2));
                    }
                    break;
                case "CHECKMAP":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckMap, array[1]));
                    }
                    break;
                case "CHECKPET":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckPet, array[1]));
                    }
                    break;
                case "PETCOUNT":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.PetCount, array[1], array[2]));
                    }
                    break;
                case "DAYOFWEEK":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckDay, array[1]));
                    }
                    break;
                case "CHECKLIST":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckList, array[1], array[2]));
                    }
                    break;
                case "CHECKCALC":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckCalc, array[1], array[2], (array.Length < 4) ? "" : array[3]));
                    }
                    break;
                case "CHECKDATA":
                    if (array.Length >= 4)
                    {
                        MemberInfo fieldOrPropertyInfo;
                        fieldOrPropertyInfo = NPCSegment.GetFieldOrPropertyInfo(typeof(玩家实例), array[1]);
                        if (!(fieldOrPropertyInfo == null))
                        {
                            NPCChecks nPCChecks;
                            nPCChecks = new NPCChecks(notMode, CheckType.CheckData, array[2], array[3]);
                            nPCChecks.F = new 类数据读写(fieldOrPropertyInfo);
                            this.CheckList.Add(nPCChecks);
                        }
                    }
                    break;
                case "CHECKGOLD":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckGold, array[1], array[2], (array.Length > 3) ? array[3] : ""));
                    }
                    break;
                case "CHECKITEM":
                    if (array.Length >= 2)
                    {
                        string text2;
                        text2 = ((array.Length < 3) ? "1" : array[2]);
                        string text3;
                        text3 = ((array.Length > 3) ? array[3] : "");
                        string text4;
                        text4 = ((array.Length > 4) ? array[4] : "");
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckItem, array[1], text2, text3, text4));
                    }
                    break;
                case "CHECKQUEST":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckQuest, array[1], array[2]));
                    }
                    break;
                case "AFFORDWALL":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.AffordWall, array[1], array[2]));
                    }
                    break;
                case "AFFORDGATE":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.AffordGate, array[1], array[2]));
                    }
                    break;
                case "CHECKCLASS":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckClass, array[1]));
                    }
                    break;
                case "GROUPCOUNT":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.GroupCount, array[1], array[2]));
                    }
                    break;
                case "CHECKTITLE":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckTitle, array[1]));
                    }
                    break;
                case "CHECKSKILL":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckSkill, array[1], (array.Length > 2) ? array[2] : "0"));
                    }
                    break;
                case "CHECKTIMER":
                    if (array.Length >= 4)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckTimer, array[1], array[2], array[3]));
                    }
                    break;
                case "ISNEWHUMAN":
                    this.CheckList.Add(new NPCChecks(notMode, CheckType.IsNewHuman));
                    break;
                case "CHECKRANGE":
                    if (array.Length >= 4)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckRange, array[1], array[2], array[3]));
                    }
                    break;
                case "CHECKLOONG":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckLoong, array[1]));
                    }
                    break;
                case "AFFORDSIEGE":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.AffordSiege, array[1], array[2]));
                    }
                    break;
                case "ISCASTLEWAR":
                    this.CheckList.Add(new NPCChecks(notMode, CheckType.IsCastleWar));
                    break;
                case "GROUPLEADER":
                    this.CheckList.Add(new NPCChecks(notMode, CheckType.Groupleader));
                    break;
                case "AFFORDGUARD":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.AffordGuard, array[1], array[2]));
                    }
                    break;
                case "HASBAGSPACE":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.HasBagSpace, array[1], array[2]));
                    }
                    break;
                case "CHECKGENDER":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckGender, array[1]));
                    }
                    break;
                case "CHECKITEMIDX":
                    if (array.Length >= 2)
                    {
                        string text2;
                        text2 = ((array.Length > 2) ? array[2] : "1");
                        string text3;
                        text3 = ((array.Length > 3) ? array[3] : "0");
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckItemIdx, array[1], text2, text3));
                    }
                    break;
                case "CHECKGUILDLV":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckGuildLv, array[1], array[2]));
                    }
                    break;
                case "CHECKSKILLLV":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckSkillLv, array[1], array[2], array[3]));
                    }
                    break;
                case "CHECKPKPOINT":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckPkPoint, array[1], array[2]));
                    }
                    break;
                case "CHECKBINDGOLD":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckBindGold, array[1], array[2]));
                    }
                    break;
                case "CHECKNAMELIST":
                    if (array.Length >= 2)
                    {
                        Match match;
                        match = regex2.Match(line);
                        string path;
                        path = array[1];
                        if (match.Success)
                        {
                            path = match.Groups[1].Captures[0].Value;
                        }
                        string text;
                        text = Path.Combine(Settings.NameListPath, path);
                        Directory.CreateDirectory(Path.GetDirectoryName(text));
                        if (File.Exists(text))
                        {
                            this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckNameList, text));
                        }
                    }
                    break;
                case "CHECKGAMEGOLD":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckGameGold, array[1], array[2]));
                    }
                    break;
                case "CHECKEXACTMON":
                    if (array.Length >= 5)
                    {
                        string text2;
                        text2 = ((array.Length < 6) ? "1" : array[5]);
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckExactMon, array[1], array[2], array[3], array[4], text2));
                    }
                    break;
                case "CHECKCONQUEST":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckConquest, array[1]));
                    }
                    break;
                case "CHECKGROUPVAR":
                    if (array.Length >= 4)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckGroupVar, array[1], array[2], array[3]));
                    }
                    break;
                case "CONQUESTOWNER":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.ConquestOwner, array[1]));
                    }
                    break;
                case "CHECKITEMLUCK":
                    if (array.Length >= 4)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckItemLuck, array[1], array[2], array[3]));
                    }
                    break;
                case "CHECKITEMVALUE":
                    if (array.Length >= 6)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckItemValue, array[1], array[2], array[3], array[4], array[5]));
                    }
                    break;
                case "CHECKDOUBLEEXP":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckDoubleExp, array[1], array[2]));
                    }
                    break;
                case "CHECKATTRIBUTE":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckAttribute, array[1], array[2], (array.Length >= 4) ? array[3] : "0"));
                    }
                    break;
                case "CHECKPERMISSION":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckPermission, array[1]));
                    }
                    break;
                case "CHECKPLAYERTITLE":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckPlayerTitle, array[1], array[2]));
                    }
                    break;
                case "GROUPCHECKNEARBY":
                    this.CheckList.Add(new NPCChecks(notMode, CheckType.GroupCheckNearby));
                    break;
                case "CHECKCASTLEOWNER":
                    if (array.Length >= 1)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckCastleOwner));
                    }
                    break;
                case "CHECKRELATIONSHIP":
                    this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckRelationship));
                    break;
                case "CONQUESTAVAILABLE":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.ConquestAvailable, array[1]));
                    }
                    break;
                case "CHECKBAGATTRIBUTE":
                    if (array.Length >= 3)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckBagAttribute, array[1], array[2], (array.Length >= 4) ? array[3] : "0"));
                    }
                    break;
                case "CHECKGUILDNAMELIST":
                    if (array.Length >= 2)
                    {
                        Match match;
                        match = regex2.Match(line);
                        string path;
                        path = array[1];
                        if (match.Success)
                        {
                            path = match.Groups[1].Captures[0].Value;
                        }
                        string text;
                        text = Path.Combine(Settings.NameListPath, path);
                        Directory.CreateDirectory(Path.GetDirectoryName(text));
                        if (File.Exists(text))
                        {
                            this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckGuildNameList, text));
                        }
                    }
                    break;
                case "ISOPENSEVENCARNIVAL":
                    this.CheckList.Add(new NPCChecks(notMode, CheckType.IsOpenSevenCarnival));
                    break;
                case "CHECKATTRIBUTECOUNT":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckAttributeCount, array[1], (array.Length >= 3) ? array[2] : "0", (array.Length >= 4) ? array[3] : "0"));
                    }
                    break;
                case "CHECKMAPSAMEMONCOUNT":
                    if (array.Length >= 6)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckMapSameMonCount, array[1], array[2], array[3], array[4], (array.Length > 5) ? array[5] : "0"));
                    }
                    break;
                case "CHECKBAGATTRIBUTECOUNT":
                    if (array.Length >= 2)
                    {
                        this.CheckList.Add(new NPCChecks(notMode, CheckType.CheckBagAttributeCount, array[1], (array.Length >= 3) ? array[2] : "0", (array.Length >= 4) ? array[3] : "0"));
                    }
                    break;
                default:
                    主程.添加系统日志($"脚本检查未找到: {array[0]}");
                    break;
            }
        }

        public void ParseAct(List<NPCActions> acts, string line)
        {
            string[] array;
            array = this.ParseArguments(line.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            if (array.Length == 0)
            {
                return;
            }
            Regex regex;
            regex = new Regex("\"([^\"]*)\"");
            Regex regex2;
            regex2 = new Regex("\\[(.*?)\\]");
            Match match2;
            match2 = null;
            string text;
            text = array[0].ToUpper();
            if (text == null)
            {
                return;
            }
            switch (text.Length)
            {
                case 3:
                    switch (text[0])
                    {
                        case 'S':
                            if (text == "SET" && array.Length >= 3)
                            {
                                Match match3;
                                match3 = regex2.Match(array[1]);
                                if (match3.Success)
                                {
                                    string value5;
                                    value5 = match3.Groups[1].Captures[0].Value;
                                    acts.Add(new NPCActions(ActionType.Set, value5, array[2]));
                                }
                            }
                            break;
                        case 'M':
                            if (text == "MOV" && array.Length >= 3)
                            {
                                Match match3;
                                match3 = Regex.Match(array[1], "[A-Z][0-9]+", RegexOptions.IgnoreCase);
                                match2 = regex.Match(line);
                                string text32;
                                text32 = array[2];
                                if (match2.Success)
                                {
                                    text32 = match2.Groups[1].Captures[0].Value;
                                }
                                if (match3.Success)
                                {
                                    acts.Add(new NPCActions(ActionType.Mov, array[1], text32));
                                }
                            }
                            break;
                        case 'L':
                            if (text == "LOG" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.Log, array[1]));
                            }
                            break;
                    }
                    break;
                case 4:
                    switch (text[3])
                    {
                        case 'L':
                            if (text == "CALL" && array.Length >= 2)
                            {
                                match2 = regex.Match(line);
                                string path;
                                path = array[1];
                                if (match2.Success)
                                {
                                    path = match2.Groups[1].Captures[0].Value;
                                }
                                if (File.Exists(Path.Combine(Settings.NPCPath, path + ".txt")))
                                {
                                    NPCScript orAdd;
                                    orAdd = NPCScript.GetOrAdd(0, path, NPCScriptType.Called);
                                    this.Page.ScriptCalls.Add(orAdd.ScriptID);
                                    acts.Add(new NPCActions(ActionType.Call, orAdd.ScriptID.ToString()));
                                }
                            }
                            break;
                        case 'O':
                            if (text == "GOTO" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.Goto, array[1]));
                            }
                            break;
                        case 'P':
                            if (!(text == "STOP"))
                            {
                                _ = text == "DROP";
                            }
                            else
                            {
                                acts.Add(new NPCActions(ActionType.STOP));
                            }
                            break;
                        case 'R':
                            if (text == "MOVR" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.Movr, array[1], array[2], (array.Length < 4) ? string.Empty : array[3]));
                            }
                            break;
                        case 'E':
                            if (text == "MOVE" && array.Length >= 2)
                            {
                                string text2;
                                text2 = ((array.Length > 2) ? array[2] : "0");
                                string text3;
                                text3 = ((array.Length > 3) ? array[3] : "0");
                                string text4;
                                text4 = ((array.Length > 4) ? array[4] : "0");
                                acts.Add(new NPCActions(ActionType.Move, array[1], text2, text3, text4));
                            }
                            break;
                        case 'C':
                            if (text == "CALC" && array.Length >= 4)
                            {
                                Match match3;
                                match3 = Regex.Match(array[1], "[A-Z][0-9]+", RegexOptions.IgnoreCase);
                                match2 = regex.Match(line);
                                string text32;
                                text32 = array[3];
                                if (match2.Success)
                                {
                                    text32 = match2.Groups[1].Captures[0].Value;
                                }
                                if (match3.Success)
                                {
                                    acts.Add(new NPCActions(ActionType.Calc, "%" + array[1], array[2], text32, array[1].Insert(1, "-")));
                                }
                            }
                            break;
                    }
                    break;
                case 5:
                    switch (text[0])
                    {
                        case 'P':
                            if (text == "PGOTO" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.PGoto, array[1], array[2]));
                            }
                            break;
                        case 'B':
                            if (text == "BREAK")
                            {
                                acts.Add(new NPCActions(ActionType.Break));
                            }
                            break;
                    }
                    break;
                case 6:
                    switch (text[5])
                    {
                        case 'P':
                            if (!(text == "GIVEHP"))
                            {
                                if (text == "GIVEMP" && array.Length >= 2)
                                {
                                    acts.Add(new NPCActions(ActionType.GiveMP, array[1]));
                                }
                            }
                            else if (array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.GiveHP, array[1]));
                            }
                            break;
                        case 'N':
                            if (!(text == "MONGEN"))
                            {
                                if (text == "NPCGEN" && array.Length >= 5)
                                {
                                    acts.Add(new NPCActions(ActionType.NpcGen, array[1], array[2], array[3], array[4], (array.Length < 6) ? "*" : array[5]));
                                }
                            }
                            else if (array.Length >= 2)
                            {
                                string text5;
                                text5 = ((array.Length < 3) ? "1" : array[2]);
                                acts.Add(new NPCActions(ActionType.Mongen, array[1], text5));
                            }
                            break;
                        case '1':
                            if (text == "PARAM1" && array.Length >= 2)
                            {
                                string text8;
                                text8 = ((array.Length < 3) ? "1" : array[2]);
                                acts.Add(new NPCActions(ActionType.Param1, array[1], text8));
                            }
                            break;
                        case '2':
                            if (text == "PARAM2" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.Param2, array[1]));
                            }
                            break;
                        case '3':
                            if (text == "PARAM3" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.Param3, array[1]));
                            }
                            break;
                    }
                    break;
                case 7:
                    switch (text[4])
                    {
                        case 'I':
                            if (!(text == "ADDLIST"))
                            {
                                if (text == "DELLIST" && array.Length >= 3)
                                {
                                    acts.Add(new NPCActions(ActionType.DelList, array[1], array[2]));
                                }
                            }
                            else if (array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.AddList, array[1], array[2]));
                            }
                            break;
                        case 'A':
                            if (text == "MOVEALL" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.MoveAll, array[1], array[2], (array.Length > 3) ? array[3] : ""));
                            }
                            break;
                        case 'D':
                            if (text == "ROLLDIE" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.RollDie, array[1], array[2]));
                            }
                            break;
                        case 'E':
                            if (text == "GIVEEXP" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.GiveExp, array[1]));
                            }
                            break;
                        case 'Y':
                            if (text == "ROLLYUT" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.RollYut, array[1], array[2]));
                            }
                            break;
                        case 'L':
                            if (text == "CALLLUA")
                            {
                                string[] array3;
                                array3 = new string[array.Length - 1];
                                Array.Copy(array, 1, array3, 0, array.Length - 1);
                                acts.Add(new NPCActions(ActionType.CallLua, array3));
                            }
                            break;
                        case 'O':
                            if (text == "SYSGOTO" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.SysGoto, array[1]));
                            }
                            break;
                        case 'P':
                            if (text == "GIVEPET" && array.Length >= 2)
                            {
                                string text30;
                                text30 = ((array.Length > 2) ? array[2] : "1");
                                string text31;
                                text31 = ((array.Length > 3) ? array[3] : "0");
                                acts.Add(new NPCActions(ActionType.GivePet, array[1], text30, text31));
                            }
                            break;
                    }
                    break;
                case 8:
                    switch (text[4])
                    {
                        case 'B':
                            if (text == "GIVEBUFF" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.GiveBuff, array[1]));
                            }
                            break;
                        case 'E':
                            if (!(text == "MONGENEX"))
                            {
                                if (text == "NPCGENEX" && array.Length >= 6)
                                {
                                    acts.Add(new NPCActions(ActionType.NpcGenEx, array[1], array[2], array[3], array[4], array[5]));
                                }
                            }
                            else if (array.Length >= 5)
                            {
                                string text5;
                                text5 = ((array.Length < 6) ? "1" : array[5]);
                                acts.Add(new NPCActions(ActionType.MongenEx, array[1], array[2], array[3], array[4], text5, (array.Length < 7) ? "0" : array[6], (array.Length < 8) ? "0" : array[7]));
                            }
                            break;
                        case 'G':
                            switch (text)
                            {
                                case "GAMEGOLD":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.GameGold, array[1], array[2]));
                                    }
                                    break;
                                case "OPENGATE":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.OpenGate, array[1], array[2]));
                                    }
                                    break;
                                case "TAKEGOLD":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.TakeGold, array[1]));
                                    }
                                    break;
                                case "GIVEGOLD":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.GiveGold, array[1]));
                                    }
                                    break;
                            }
                            break;
                        case 'H':
                            if (text == "SEALHERO")
                            {
                                acts.Add(new NPCActions(ActionType.SealHero));
                            }
                            break;
                        case 'I':
                            switch (text)
                            {
                                case "DELTITLE":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.DelTitle, array[1]));
                                    }
                                    break;
                                case "USETITLE":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.UseTitle, array[1]));
                                    }
                                    break;
                                case "SETTIMER":
                                    if (array.Length >= 4)
                                    {
                                        string text25;
                                        text25 = ((array.Length < 5) ? "" : array[4]);
                                        acts.Add(new NPCActions(ActionType.SetTimer, array[1], array[2], array[3], text25));
                                    }
                                    break;
                                case "TAKEITEM":
                                    if (array.Length >= 2)
                                    {
                                        string text5;
                                        text5 = ((array.Length < 3) ? string.Empty : array[2]);
                                        acts.Add(new NPCActions(ActionType.TakeItem, array[1], text5, (array.Length < 4) ? string.Empty : array[3]));
                                    }
                                    break;
                                case "GIVEITEM":
                                    if (array.Length >= 2)
                                    {
                                        string text5;
                                        text5 = ((array.Length < 3) ? string.Empty : array[2]);
                                        string text6;
                                        text6 = ((array.Length < 4) ? string.Empty : array[3]);
                                        acts.Add(new NPCActions(ActionType.GiveItem, array[1], text5, text6, (array.Length < 5) ? string.Empty : array[4]));
                                    }
                                    break;
                            }
                            break;
                        case 'K':
                            if (text == "ADDSKILL" && array.Length >= 3)
                            {
                                string text27;
                                text27 = ((array.Length > 2) ? array[2] : "0");
                                acts.Add(new NPCActions(ActionType.AddSkill, array[1], text27));
                            }
                            break;
                        case 'L':
                            if (text == "MONCLEAR" && array.Length >= 2)
                            {
                                string text8;
                                text8 = ((array.Length < 3) ? "1" : array[2]);
                                string text28;
                                text28 = ((array.Length < 4) ? "" : array[3]);
                                acts.Add(new NPCActions(ActionType.MonClear, array[1], text8, text28));
                            }
                            break;
                        case 'M':
                            if (text == "SENDMAIL")
                            {
                                string 空格;
                                空格 = "$b8.b8$";
                                array = Regex.Replace(line, "\"([^\\\"]*)\"", (Match match) => "\"" + Regex.Replace(match.Groups[1].Value, "\\s+", 空格) + "\"").Replace("\"", "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                array = this.ParseArguments(array);
                                if (array.Length >= 4)
                                {
                                    string text26;
                                    text26 = array[1];
                                    ArrayList arrayList;
                                    arrayList = new ArrayList(array);
                                    arrayList.RemoveAt(1);
                                    array = (string[])arrayList.ToArray(typeof(string));
                                    acts.Add(new NPCActions(ActionType.SendMail, text26, array[1].Replace(空格, " "), array[2].Replace(空格, " "), (array.Length < 4) ? "" : array[3], (array.Length < 5) ? "" : array[4], (array.Length < 6) ? "" : array[5]));
                                }
                            }
                            break;
                        case 'P':
                            if (text == "OPENPAGE" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.OpenPage, array[1], (array.Length > 2) ? array[2] : "0"));
                            }
                            break;
                        case 'R':
                            if (text == "ENTERMAP")
                            {
                                acts.Add(new NPCActions(ActionType.EnterMap));
                            }
                            break;
                        case 'T':
                            if (text == "INPUTBOX" && array.Length >= 2)
                            {
                                string text23;
                                text23 = array[1];
                                string text24;
                                text24 = ((array.Length > 2) ? array[2] : "");
                                acts.Add(new NPCActions(ActionType.InputBox, text23, text24));
                            }
                            break;
                        case 'C':
                        case 'D':
                        case 'F':
                        case 'J':
                        case 'N':
                        case 'O':
                        case 'Q':
                        case 'S':
                            break;
                    }
                    break;
                case 9:
                    switch (text[4])
                    {
                        case 'P':
                            if (!(text == "DELNPCGEN"))
                            {
                                if (text == "GROUPGOTO" && array.Length >= 2)
                                {
                                    acts.Add(new NPCActions(ActionType.GroupGoto, array[1]));
                                }
                            }
                            else if (array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.DelNpcGen, array[1]));
                            }
                            break;
                        case 'R':
                            if (text == "CLEARPETS")
                            {
                                acts.Add(new NPCActions(ActionType.ClearPets));
                            }
                            break;
                        case 'S':
                            if (text == "PLAYSOUND" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.PlaySound, array[1]));
                            }
                            break;
                        case 'T':
                            if (text == "GIVETITLE" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.GiveTitle, array[1], (array.Length < 3) ? "0" : array[2]));
                            }
                            break;
                        case 'V':
                            switch (text)
                            {
                                case "SAVEVALUE":
                                    {
                                        if (array.Length < 5)
                                        {
                                            break;
                                        }
                                        MatchCollection matchCollection;
                                        matchCollection = regex.Matches(line);
                                        if (matchCollection.Count > 0 && matchCollection[0].Success)
                                        {
                                            string text7;
                                            text7 = Path.Combine(Settings.ValuePath, matchCollection[0].Groups[1].Captures[0].Value);
                                            string text18;
                                            text18 = array[^1];
                                            if (matchCollection.Count > 1 && matchCollection[1].Success)
                                            {
                                                text18 = matchCollection[1].Groups[1].Captures[0].Value;
                                            }
                                            string[] array2;
                                            array2 = line.Replace(text18, string.Empty).Replace("\"", "").Trim()
                                                .Split(' ');
                                            string text19;
                                            text19 = array2[^2];
                                            string text20;
                                            text20 = array2[^1];
                                            Directory.CreateDirectory(Path.GetDirectoryName(text7));
                                            if (!File.Exists(text7))
                                            {
                                                File.Create(text7).Close();
                                            }
                                            acts.Add(new NPCActions(ActionType.SaveValue, text7, text19, text20, text18));
                                        }
                                        break;
                                    }
                                case "LOADVALUE":
                                    if (array.Length < 5)
                                    {
                                        break;
                                    }
                                    match2 = regex.Match(line);
                                    if (match2.Success)
                                    {
                                        string text7;
                                        text7 = Path.Combine(Settings.ValuePath, match2.Groups[1].Captures[0].Value);
                                        string text21;
                                        text21 = array[^2];
                                        string text22;
                                        text22 = array[^1];
                                        Directory.CreateDirectory(Path.GetDirectoryName(text7));
                                        if (!File.Exists(text7))
                                        {
                                            File.Create(text7).Close();
                                        }
                                        acts.Add(new NPCActions(ActionType.LoadValue, array[1], text7, text21, text22));
                                    }
                                    break;
                                case "REMOVEPET":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.RemovePet, array[1]));
                                    }
                                    break;
                            }
                            break;
                        case 'W':
                            if (text == "THROWITEM" && array.Length >= 8)
                            {
                                acts.Add(new NPCActions(ActionType.ThrowItem, array[1], array[2], array[3], array[4], array[5], array[6], array[7], (array.Length > 8) ? array[8] : "0", (array.Length > 9) ? array[9] : "0", (array.Length > 10) ? array[10] : "0"));
                            }
                            break;
                        case 'Y':
                            if (text == "DELAYGOTO" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.DelayGoto, array[1], array[2]));
                            }
                            break;
                        case 'E':
                            if (text == "CLOSEGATE" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.CloseGate, array[1], array[2]));
                            }
                            break;
                    }
                    break;
                case 10:
                    switch (text[8])
                    {
                        case 'D':
                            if (text == "CHANGENODE" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.ChangeNode, array[1], array[2], array[3]));
                            }
                            break;
                        case 'E':
                            switch (text)
                            {
                                case "SKILLLEVEL":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.SkillLevel, array[1], array[2]));
                                    }
                                    break;
                                case "SETONTIMER":
                                    if (array.Length >= 3)
                                    {
                                        string text29;
                                        text29 = "0";
                                        if (array.Length > 3)
                                        {
                                            text29 = array[3];
                                        }
                                        acts.Add(new NPCActions(ActionType.SetOnTimer, array[1], array[2], text29));
                                    }
                                    break;
                                case "GAMEGOLDEX":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.GameGoldEx, array[1], array[2], array[3]));
                                    }
                                    break;
                                case "TAKEGOLDEX":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.TakeGoldEx, array[1], array[2]));
                                    }
                                    break;
                                case "GIVEGOLDEX":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.GiveGoldEx, array[1], array[2]));
                                    }
                                    break;
                            }
                            break;
                        case 'F':
                            if (text == "REMOVEBUFF" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.RemoveBuff, array[1]));
                            }
                            break;
                        case 'G':
                            if (text == "TOPMESSAGE")
                            {
                                Match match3;
                                match3 = regex.Match(line);
                                if (match3.Success)
                                {
                                    string value4;
                                    value4 = match3.Groups[1].Captures[0].Value;
                                    int num4;
                                    num4 = array.Count() - 1;
                                    acts.Add(new NPCActions(ActionType.TopMessage, value4, array[num4]));
                                }
                            }
                            break;
                        case 'I':
                            if (text == "CHANGEHAIR")
                            {
                                if (array.Length < 2)
                                {
                                    acts.Add(new NPCActions(ActionType.ChangeHair));
                                    break;
                                }
                                acts.Add(new NPCActions(ActionType.ChangeHair, array[1]));
                            }
                            break;
                        case 'L':
                            switch (text)
                            {
                                case "ADDTOGUILD":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.AddToGuild, array[1]));
                                    }
                                    break;
                                case "TIMERECALL":
                                    if (array.Length >= 2)
                                    {
                                        string text11;
                                        text11 = ((array.Length > 2) ? array[2] : "");
                                        acts.Add(new NPCActions(ActionType.TimeRecall, array[1], text11));
                                    }
                                    break;
                                case "TAKEPEARLS":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.TakePearls, array[1]));
                                    }
                                    break;
                                case "GIVEPEARLS":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.GivePearls, array[1]));
                                    }
                                    break;
                            }
                            break;
                        case 'N':
                            if (text == "SETPKPOINT" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.SetPkPoint, array[1]));
                            }
                            break;
                        case 'R':
                            if (!(text == "NOWVARSORT"))
                            {
                                if (text == "REVIVEHERO")
                                {
                                    acts.Add(new NPCActions(ActionType.ReviveHero));
                                }
                            }
                            else if (array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.NowVarSort, array[1], (array.Length < 3) ? string.Empty : array[2]));
                            }
                            break;
                        case 'S':
                            if (text == "STARTQUEST" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.StartQuest, array[1]));
                            }
                            break;
                        case 'T':
                            if (text == "CHANGEDATA" && array.Length >= 4)
                            {
                                MemberInfo fieldOrPropertyInfo;
                                fieldOrPropertyInfo = NPCSegment.GetFieldOrPropertyInfo(typeof(玩家实例), array[1]);
                                if (!(fieldOrPropertyInfo == null))
                                {
                                    NPCActions nPCActions;
                                    nPCActions = new NPCActions(ActionType.ChangeData, array[2], array[3]);
                                    nPCActions.F = new 类数据读写(fieldOrPropertyInfo);
                                    acts.Add(nPCActions);
                                }
                            }
                            break;
                        case 'U':
                            if (text == "MARFAVALUE" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.MarfaValue, array[1], (array.Length < 3) ? string.Empty : array[2], (array.Length < 4) ? string.Empty : array[3]));
                            }
                            break;
                        case 'X':
                            if (text == "CANGAINEXP" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.CanGainExp, array[1]));
                            }
                            break;
                        case 'Z':
                            if (text == "SETBAGSIZE" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.SetBagSize, array[1], array[2]));
                            }
                            break;
                        case 'H':
                        case 'J':
                        case 'K':
                        case 'M':
                        case 'O':
                        case 'P':
                        case 'Q':
                        case 'V':
                        case 'W':
                        case 'Y':
                            break;
                    }
                    break;
                case 11:
                    switch (text[0])
                    {
                        case 'A':
                            switch (text)
                            {
                                case "AUTOPICKCFG":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.AutoPickCfg, array[1], array[2]));
                                    }
                                    break;
                                case "ADDMAILITEM":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.AddMailItem, array[1], array[2]));
                                    }
                                    break;
                                case "ADDMAILGOLD":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.AddMailGold, array[1]));
                                    }
                                    break;
                                case "ADDNAMELIST":
                                    if (array.Length >= 2)
                                    {
                                        match2 = regex.Match(line);
                                        string path;
                                        path = array[1];
                                        if (match2.Success)
                                        {
                                            path = match2.Groups[1].Captures[0].Value;
                                        }
                                        string text7;
                                        text7 = Path.Combine(Settings.NameListPath, path);
                                        Directory.CreateDirectory(Path.GetDirectoryName(text7));
                                        if (!File.Exists(text7))
                                        {
                                            File.Create(text7).Close();
                                        }
                                        acts.Add(new NPCActions(ActionType.AddNameList, text7));
                                    }
                                    break;
                            }
                            break;
                        case 'B':
                            if (text == "BOXITEMRATE" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.BoxItemRate, array[1]));
                            }
                            break;
                        case 'C':
                            switch (text)
                            {
                                case "COMPOSEMAIL":
                                    {
                                        Match match3;
                                        match3 = regex.Match(line);
                                        if (match3.Success)
                                        {
                                            string value3;
                                            value3 = match3.Groups[1].Captures[0].Value;
                                            int num3;
                                            num3 = array.Count() - 1;
                                            acts.Add(new NPCActions(ActionType.ComposeMail, value3, array[num3]));
                                        }
                                        break;
                                    }
                                case "CHANGECLASS":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.ChangeClass, array[1]));
                                    }
                                    break;
                                case "CHANGELEVEL":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.ChangeLevel, array[1], array[2]));
                                    }
                                    break;
                            }
                            break;
                        case 'D':
                            if (text == "DELNAMELIST" && array.Length >= 2)
                            {
                                match2 = regex.Match(line);
                                string path;
                                path = array[1];
                                if (match2.Success)
                                {
                                    path = match2.Groups[1].Captures[0].Value;
                                }
                                string text7;
                                text7 = Path.Combine(Settings.NameListPath, path);
                                Directory.CreateDirectory(Path.GetDirectoryName(text7));
                                if (File.Exists(text7))
                                {
                                    acts.Add(new NPCActions(ActionType.DelNameList, text7));
                                }
                            }
                            break;
                        case 'E':
                            if (text == "EXPIRETIMER" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.ExpireTimer, array[1]));
                            }
                            break;
                        case 'G':
                            if (!(text == "GROUPRECALL"))
                            {
                                if (text == "GIVEITEMIDX" && array.Length >= 2)
                                {
                                    string text5;
                                    text5 = ((array.Length < 3) ? string.Empty : array[2]);
                                    string text6;
                                    text6 = ((array.Length < 4) ? string.Empty : array[3]);
                                    string text17;
                                    text17 = ((array.Length < 5) ? "0" : array[4]);
                                    acts.Add(new NPCActions(ActionType.GiveItemIdx, array[1], text5, text6, text17, (array.Length < 6) ? "" : array[5]));
                                }
                            }
                            else
                            {
                                acts.Add(new NPCActions(ActionType.GroupRecall));
                            }
                            break;
                        case 'O':
                            switch (text)
                            {
                                case "OPENBROWSER":
                                    if (array.Length >= 2)
                                    {
                                        acts.Add(new NPCActions(ActionType.OpenBrowser, array[1]));
                                    }
                                    break;
                                case "OPENITEMBOX":
                                    acts.Add(new NPCActions(ActionType.OpenItemBox, array[1], array[2], array[3], (array.Length < 5) ? "*" : array[4], (array.Length < 6) ? "*" : array[5]));
                                    break;
                                case "OPENVARSORT":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.OpenVarSort, array[1], array[2], (array.Length < 4) ? string.Empty : array[3]));
                                    }
                                    break;
                            }
                            break;
                        case 'R':
                            if (text == "REMOVESKILL" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.RemoveSkill, array[1]));
                            }
                            break;
                        case 'S':
                            if (text == "SETOFFTIMER" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.SetOffTimer, array[1]));
                            }
                            break;
                        case 'T':
                            if (text == "TAKEITEMIDX" && array.Length >= 2)
                            {
                                string text5;
                                text5 = ((array.Length < 3) ? string.Empty : array[2]);
                                acts.Add(new NPCActions(ActionType.TakeItemIdx, array[1], text5, (array.Length < 4) ? string.Empty : array[3]));
                            }
                            break;
                        case 'U':
                            if (text == "UNEQUIPITEM")
                            {
                                string text16;
                                text16 = "";
                                if (array.Length >= 2)
                                {
                                    text16 = array[1];
                                }
                                acts.Add(new NPCActions(ActionType.UnequipItem, text16));
                            }
                            break;
                        case 'F':
                        case 'H':
                        case 'I':
                        case 'J':
                        case 'K':
                        case 'L':
                        case 'M':
                        case 'N':
                        case 'P':
                        case 'Q':
                            break;
                    }
                    break;
                case 12:
                    switch (text[2])
                    {
                        case 'N':
                            if (!(text == "CONQUESTGATE"))
                            {
                                if (text == "CONQUESTWALL" && array.Length >= 3)
                                {
                                    acts.Add(new NPCActions(ActionType.ConquestWall, array[1], array[2]));
                                }
                            }
                            else if (array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.ConquestGate, array[1], array[2]));
                            }
                            break;
                        case 'R':
                            if (text == "ERRORMESSAGE" && array.Length >= 2)
                            {
                                string text14;
                                text14 = ((array.Length < 3) ? string.Empty : array[2]);
                                string text15;
                                text15 = ((array.Length < 4) ? string.Empty : array[3]);
                                acts.Add(new NPCActions(ActionType.ErrorMessage, array[1], text14, text15));
                            }
                            break;
                        case 'S':
                            if (text == "INSTANCEMOVE" && array.Length >= 5)
                            {
                                string text2;
                                text2 = ((array.Length > 2) ? array[2] : "0");
                                string text3;
                                text3 = ((array.Length > 3) ? array[3] : "0");
                                string text4;
                                text4 = ((array.Length > 4) ? array[4] : "0");
                                acts.Add(new NPCActions(ActionType.InstanceMove, array[1], text2, text3, text4));
                            }
                            break;
                        case 'T':
                            if (text == "GETITEMCOUNT" && array.Length >= 3)
                            {
                                Match match3;
                                match3 = Regex.Match(array[2], "[A-Z][0-9]+", RegexOptions.IgnoreCase);
                                match2 = regex.Match(line);
                                string text13;
                                text13 = array[1];
                                if (match2.Success)
                                {
                                    text13 = match2.Groups[1].Captures[0].Value;
                                }
                                if (match3.Success)
                                {
                                    acts.Add(new NPCActions(ActionType.GetItemCount, array[2], text13));
                                }
                            }
                            break;
                        case 'V':
                            if (text == "GIVEBINDGOLD" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.GiveBindGold, array[1]));
                            }
                            break;
                        case 'K':
                            if (text == "TAKEBINDGOLD" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.TakeBindGold, array[1]));
                            }
                            break;
                        case 'A':
                            if (text == "CHANGEGENDER" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.ChangeGender, array[1]));
                            }
                            break;
                        case 'C':
                            if (text == "LOCALMESSAGE")
                            {
                                Match match3;
                                match3 = regex.Match(line);
                                if (match3.Success)
                                {
                                    string value2;
                                    value2 = match3.Groups[1].Captures[0].Value;
                                    int num2;
                                    num2 = array.Count() - 1;
                                    acts.Add(new NPCActions(ActionType.LocalMessage, value2, array[num2]));
                                }
                            }
                            break;
                        case 'E':
                            if (text == "CLEARVARSORT" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.ClearVarSort, array[1], (array.Length < 3) ? string.Empty : array[2]));
                            }
                            break;
                    }
                    break;
                case 13:
                    switch (text[6])
                    {
                        case 'M':
                            if (text == "GLOBALMESSAGE")
                            {
                                Match match3;
                                match3 = regex.Match(line);
                                if (match3.Success)
                                {
                                    string value;
                                    value = match3.Groups[1].Captures[0].Value;
                                    int num;
                                    num = array.Count() - 1;
                                    acts.Add(new NPCActions(ActionType.GlobalMessage, value, array[num]));
                                }
                            }
                            break;
                        case 'O':
                            if (text == "STARTCONQUEST" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.StartConquest, array[1]));
                            }
                            break;
                        case 'P':
                            if (text == "REDUCEPKPOINT" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.ReducePkPoint, array[1]));
                            }
                            break;
                        case 'S':
                            switch (text)
                            {
                                case "CHANGESELFVAR":
                                    if (array.Length >= 4)
                                    {
                                        acts.Add(new NPCActions(ActionType.ChangeSelfVar, array[1], array[2], array[3]));
                                    }
                                    break;
                                case "CONQUESTGUARD":
                                    if (array.Length >= 3)
                                    {
                                        acts.Add(new NPCActions(ActionType.ConquestGuard, array[1], array[2]));
                                    }
                                    break;
                                case "STOPCASTLEWAR":
                                    if (array.Length >= 1)
                                    {
                                        acts.Add(new NPCActions(ActionType.StopCastleWar));
                                    }
                                    break;
                            }
                            break;
                        case 'U':
                            if (!(text == "GIVEDOUBLEEXP"))
                            {
                                if (text == "TAKEDOUBLEEXP" && array.Length >= 2)
                                {
                                    acts.Add(new NPCActions(ActionType.TakeDoubleExp, array[1]));
                                }
                            }
                            else if (array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.GiveDoubleExp, array[1]));
                            }
                            break;
                        case 'A':
                            if (text == "CLEARNAMELIST" && array.Length >= 2)
                            {
                                match2 = regex.Match(line);
                                string path;
                                path = array[1];
                                if (match2.Success)
                                {
                                    path = match2.Groups[1].Captures[0].Value;
                                }
                                string text7;
                                text7 = Path.Combine(Settings.NameListPath, path);
                                Directory.CreateDirectory(Path.GetDirectoryName(text7));
                                if (File.Exists(text7))
                                {
                                    acts.Add(new NPCActions(ActionType.ClearNameList, text7));
                                }
                            }
                            break;
                        case 'D':
                            if (text == "GETRANDOMTEXT" && array.Length >= 3 && Regex.Match(array[2], "[A-Z][0-9]+", RegexOptions.IgnoreCase).Success)
                            {
                                acts.Add(new NPCActions(ActionType.GetRandomText, array[1], array[2], (array.Length > 3) ? array[3] : "-1"));
                            }
                            break;
                        case 'E':
                            if (text == "GROUPTELEPORT" && array.Length >= 2)
                            {
                                string text8;
                                string text9;
                                string text10;
                                if (array.Length == 4)
                                {
                                    text8 = "1";
                                    text9 = array[2];
                                    text10 = array[3];
                                }
                                else
                                {
                                    text8 = ((array.Length < 3) ? "1" : array[2]);
                                    text9 = ((array.Length < 4) ? "0" : array[3]);
                                    text10 = ((array.Length < 5) ? "0" : array[4]);
                                }
                                acts.Add(new NPCActions(ActionType.GroupTeleport, array[1], text8, text9, text10));
                            }
                            break;
                        case 'G':
                            if (text == "CHANGEGUILDLV" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.ChangeGuildLv, array[1]));
                            }
                            break;
                    }
                    break;
                case 14:
                    switch (text[1])
                    {
                        case 'E':
                            if (text == "REFRESHEFFECTS")
                            {
                                acts.Add(new NPCActions(ActionType.RefreshEffects));
                            }
                            break;
                        case 'A':
                            if (!(text == "TAKEBINDGOLDEX"))
                            {
                                if (text == "RANDOMGIVEITEM" && array.Length >= 2)
                                {
                                    string text5;
                                    text5 = ((array.Length < 3) ? string.Empty : array[2]);
                                    string text6;
                                    text6 = ((array.Length < 4) ? string.Empty : array[3]);
                                    acts.Add(new NPCActions(ActionType.RandomGiveItem, array[1], text5, text6, (array.Length < 5) ? string.Empty : array[4]));
                                }
                            }
                            else if (array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.TakeBindGoldEx, array[1], array[2]));
                            }
                            break;
                        case 'P':
                            if (text == "OPENCASTLEDOOR" && array.Length >= 1)
                            {
                                acts.Add(new NPCActions(ActionType.OpenCastleDoor));
                            }
                            break;
                        case 'R':
                            if (text == "CREATEINSTANCE" && array.Length >= 4)
                            {
                                acts.Add(new NPCActions(ActionType.CreateInstance, array[1], array[2], array[3], array[4]));
                            }
                            break;
                        case 'T':
                            if (text == "STARTCASTLEWAR" && array.Length >= 1)
                            {
                                acts.Add(new NPCActions(ActionType.StartCastleWar));
                            }
                            break;
                        case 'U':
                            if (text == "AUTOTAKEONITEM" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.AutoTakeOnItem, array[1], array[2]));
                            }
                            break;
                        case 'L':
                            if (text == "CLEARATTRIBUTE" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.ClearAttribute, array[1]));
                            }
                            break;
                        case 'I':
                            if (text == "GIVEBINDGOLDEX" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.GiveBindGoldEx, array[1], array[2]));
                            }
                            break;
                    }
                    break;
                case 15:
                    switch (text[4])
                    {
                        case 'R':
                            if (text == "TIMERECALLGROUP" && array.Length >= 2)
                            {
                                string text11;
                                text11 = ((array.Length > 2) ? array[2] : "");
                                acts.Add(new NPCActions(ActionType.TimeRecallGroup, array[1], text11));
                            }
                            break;
                        case 'T':
                            if (text == "GETITEMIDXCOUNT" && array.Length >= 3)
                            {
                                Match match3;
                                match3 = Regex.Match(array[2], "[A-Z][0-9]+", RegexOptions.IgnoreCase);
                                match2 = regex.Match(line);
                                string text12;
                                text12 = array[1];
                                if (match2.Success)
                                {
                                    text12 = match2.Groups[1].Captures[0].Value;
                                }
                                if (match3.Success)
                                {
                                    acts.Add(new NPCActions(ActionType.GetItemIdxCount, array[2], text12));
                                }
                            }
                            break;
                        case 'V':
                            if (text == "REMOVEFROMGUILD" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.RemoveFromGuild, array[1]));
                            }
                            break;
                        case 'O':
                            if (!(text == "SETCONQUESTRATE"))
                            {
                                if (text == "ADDBODYITEMLUCK" && array.Length >= 3)
                                {
                                    acts.Add(new NPCActions(ActionType.AddBodyItemLuck, array[1], array[2]));
                                }
                            }
                            else if (array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.SetConquestRate, array[1], array[2]));
                            }
                            break;
                        case 'A':
                            if (text == "ADDBAGYITEMLUCK" && array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.AddBagItemLuck, array[1], array[2]));
                            }
                            break;
                        case 'E':
                            if (text == "INCREASEPKPOINT" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.IncreasePkPoint, array[1]));
                            }
                            break;
                        case 'G':
                            if (text == "CHANGEITEMVALUE" && array.Length >= 5)
                            {
                                acts.Add(new NPCActions(ActionType.ChangeItemValue, array[1], array[2], array[3], array[4]));
                            }
                            break;
                        case 'H':
                            if (text == "SETCHESTKEYNAME" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.SetChestKeyName, array[1], (array.Length > 2) ? array[2] : "1"));
                            }
                            break;
                        case 'I':
                            if (!(text == "TAKEITEMLUCKIDX"))
                            {
                                if (text == "SENDITEMRESTORE" && array.Length >= 2)
                                {
                                    acts.Add(new NPCActions(ActionType.SendItemRestore, array[1]));
                                }
                            }
                            else if (array.Length >= 3)
                            {
                                acts.Add(new NPCActions(ActionType.TakeItemLuckIdx, array[1], array[2], array[3]));
                            }
                            break;
                        case 'K':
                            if (text == "BREAKTIMERECALL")
                            {
                                acts.Add(new NPCActions(ActionType.BreakTimeRecall));
                            }
                            break;
                    }
                    break;
                case 16:
                    switch (text[1])
                    {
                        case 'P':
                            if (text == "SPECIALREPAIRALL")
                            {
                                acts.Add(new NPCActions(ActionType.SpecialRepairAll));
                            }
                            break;
                        case 'A':
                            if (text == "TAKECONQUESTGOLD" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.TakeConquestGold, array[1]));
                            }
                            break;
                        case 'C':
                            if (text == "SCHEDULECONQUEST" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.ScheduleConquest, array[1]));
                            }
                            break;
                        case 'D':
                            if (text == "ADDGUILDNAMELIST" && array.Length >= 2)
                            {
                                match2 = regex.Match(line);
                                string path;
                                path = array[1];
                                if (match2.Success)
                                {
                                    path = match2.Groups[1].Captures[0].Value;
                                }
                                string text7;
                                text7 = Path.Combine(Settings.NameListPath, path);
                                Directory.CreateDirectory(Path.GetDirectoryName(text7));
                                if (!File.Exists(text7))
                                {
                                    File.Create(text7).Close();
                                }
                                acts.Add(new NPCActions(ActionType.AddGuildNameList, text7));
                            }
                            break;
                        case 'E':
                            if (text == "DELGUILDNAMELIST" && array.Length >= 2)
                            {
                                match2 = regex.Match(line);
                                string path;
                                path = array[1];
                                if (match2.Success)
                                {
                                    path = match2.Groups[1].Captures[0].Value;
                                }
                                string text7;
                                text7 = Path.Combine(Settings.NameListPath, path);
                                Directory.CreateDirectory(Path.GetDirectoryName(text7));
                                if (File.Exists(text7))
                                {
                                    acts.Add(new NPCActions(ActionType.DelGuildNameList, text7));
                                }
                            }
                            break;
                    }
                    break;
                case 17:
                    switch (text[0])
                    {
                        case 'R':
                            if (text == "RANDOMGIVEITEMIDX" && array.Length >= 2)
                            {
                                string text5;
                                text5 = ((array.Length < 3) ? string.Empty : array[2]);
                                string text6;
                                text6 = ((array.Length < 4) ? string.Empty : array[3]);
                                acts.Add(new NPCActions(ActionType.RandomGiveItemIdx, array[1], text5, text6, (array.Length < 5) ? string.Empty : array[4]));
                            }
                            break;
                        case 'G':
                            if (text == "GROUPINSTANCEMOVE" && array.Length >= 5)
                            {
                                string text2;
                                text2 = ((array.Length > 2) ? array[2] : "0");
                                string text3;
                                text3 = ((array.Length > 3) ? array[3] : "0");
                                string text4;
                                text4 = ((array.Length > 4) ? array[4] : "0");
                                acts.Add(new NPCActions(ActionType.GroupInstanceMove, array[1], text2, text3, text4));
                            }
                            break;
                        case 'C':
                            if (text == "CONQUESTREPAIRALL" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.ConquestRepairAll, array[1]));
                            }
                            break;
                    }
                    break;
                case 18:
                    if (text == "CLEARGUILDNAMELIST" && array.Length >= 2)
                    {
                        match2 = regex.Match(line);
                        string path;
                        path = array[1];
                        if (match2.Success)
                        {
                            path = match2.Groups[1].Captures[0].Value;
                        }
                        string text7;
                        text7 = Path.Combine(Settings.NameListPath, path);
                        Directory.CreateDirectory(Path.GetDirectoryName(text7));
                        if (File.Exists(text7))
                        {
                            acts.Add(new NPCActions(ActionType.ClearGuildNameList, text7));
                        }
                    }
                    break;
                case 19:
                    switch (text[0])
                    {
                        case 'O':
                            if (text == "OPENBOXNEEDTIMEMSEC" && array.Length >= 2)
                            {
                                acts.Add(new NPCActions(ActionType.OpenBoxNeedTimeMSec, array[1]));
                            }
                            break;
                        case 'C':
                            if (text == "CHANGESEVENCARNIVAL" && array.Length >= 4)
                            {
                                acts.Add(new NPCActions(ActionType.ChangeSevenCarnival, array[1], array[2], array[3]));
                            }
                            break;
                    }
                    break;
                case 20:
                    if (text == "DELBAGITEMRANDOMABIL" && array.Length >= 3)
                    {
                        acts.Add(new NPCActions(ActionType.DelBagItemRandomAbil, array[1], array[2], (array.Length < 4) ? string.Empty : array[3]));
                    }
                    break;
                case 21:
                    if (text == "DELBODYITEMRANDOMABIL" && array.Length >= 3)
                    {
                        acts.Add(new NPCActions(ActionType.DelBodyItemRandomAbil, array[1], array[2], (array.Length < 4) ? string.Empty : array[3]));
                    }
                    break;
            }
        }

        public List<string> ParseSay(玩家实例 player, List<string> speech)
        {
            for (int i = 0; i < speech.Count; i++)
            {
                string[] array;
                array = speech[i].Split(new char[1] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 0)
                {
                    string[] array2;
                    array2 = array;
                    foreach (string text in array2)
                    {
                        speech[i] = speech[i].Replace(text + ">", this.ReplaceValue(player, text + ">"));
                    }
                }
            }
            return speech;
        }

        public string ReplaceValue(玩家实例 player, string param)
        {
            Regex regex;
            regex = new Regex("\\<\\$([^>]*)\\>");
            Regex regex2;
            regex2 = new Regex("(.*?)\\(([A-Z][0-9]+)\\)");
            Regex regex3;
            regex3 = new Regex("(.*?)\\(((.*?))\\)");
            Regex regex4;
            regex4 = new Regex("(.*?)\\(((.*?),(.*?))\\)");
            Regex regex5;
            regex5 = new Regex("(.*?)\\(((.*?),(.*?),(.*?))\\)");
            Match match;
            match = regex.Match(param);
            bool flag;
            flag = false;
            if (!match.Success)
            {
                return param;
            }
            string text;
            text = match.Groups[1].Captures[0].Value.ToUpper();
            Match match2;
            match2 = regex2.Match(text);
            Match match3;
            match3 = regex3.Match(text);
            Match match4;
            match4 = regex4.Match(text);
            Match match5;
            match5 = regex5.Match(text);
            if (regex2.Match(text).Success)
            {
                text = text.Replace(match2.Groups[2].Captures[0].Value.ToUpper(), "");
            }
            else if (regex5.Match(text).Success)
            {
                text = text.Replace(match5.Groups[2].Captures[0].Value.ToUpper(), "");
            }
            else if (regex4.Match(text).Success)
            {
                text = text.Replace(match4.Groups[2].Captures[0].Value.ToUpper(), "");
            }
            else if (regex3.Match(text).Success)
            {
                text = text.Replace(match3.Groups[2].Captures[0].Value.ToUpper(), "");
            }
            string text2;
            text2 = string.Empty;
            flag = true;
            object obj3;
            object obj2;
            object obj;
            switch (text)
            {
                case "DC":
                    text2 = player[游戏对象属性.最小攻击].ToString();
                    break;
                case "AC":
                    text2 = player[游戏对象属性.最小防御].ToString();
                    break;
                case "SC":
                    text2 = player[游戏对象属性.最小道术].ToString();
                    break;
                case "MP":
                    text2 = player.当前魔力.ToString();
                    break;
                case "MC":
                    text2 = player[游戏对象属性.最小魔法].ToString();
                    break;
                case "HP":
                    text2 = player.当前体力.ToString();
                    break;
                case "HIT":
                    text2 = player[游戏对象属性.物理准确].ToString();
                    break;
                case "ARC":
                    text2 = player[游戏对象属性.最小弓术].ToString();
                    break;
                case "ASC":
                    text2 = player[游戏对象属性.最小刺术].ToString();
                    break;
                case "DAY":
                    text2 = 主程.当前时间.Day.ToString();
                    break;
                case "EXP":
                    text2 = player.当前经验.ToString(CultureInfo.InvariantCulture);
                    break;
                case "VIP":
                    text2 = player.本期特权.ToString(CultureInfo.InvariantCulture);
                    break;
                case "SPD":
                    text2 = player[游戏对象属性.物理敏捷].ToString();
                    break;
                case "MAC":
                    text2 = player[游戏对象属性.最小魔防].ToString();
                    break;
                case "MAP":
                    text2 = player.当前地图.地图编号.ToString();
                    break;
                case "LUCK":
                    text2 = player[游戏对象属性.幸运等级].ToString();
                    break;
                case "DATE":
                    text2 = 主程.当前时间.ToShortDateString();
                    break;
                case "GOLD":
                    text2 = player.金币数量.ToString(CultureInfo.InvariantCulture);
                    break;
                case "HOUR":
                    text2 = 主程.当前时间.Hour.ToString();
                    break;
                case "YEAR":
                    text2 = 主程.当前时间.Year.ToString();
                    break;
                case "WEEK":
                    text2 = ((int)主程.当前时间.DayOfWeek).ToString();
                    break;
                case "MAXAC":
                    text2 = player[游戏对象属性.最大防御].ToString();
                    break;
                case "MAXDC":
                    text2 = player[游戏对象属性.最大攻击].ToString();
                    break;
                case "PCODE":
                    text2 = player.所属账号.推荐人码.V;
                    break;
                case "POWER":
                    text2 = player.当前战力.ToString();
                    break;
                case "LEVEL":
                    text2 = player.当前等级.ToString(CultureInfo.InvariantCulture);
                    break;
                case "MAXHP":
                    text2 = player[游戏对象属性.最大体力].ToString();
                    break;
                case "AWAKE":
                    text2 = (player.开启觉醒面板 ? "1" : "0");
                    break;
                case "TITLE":
                    text2 = player.当前称号.ToString(CultureInfo.InvariantCulture);
                    break;
                case "HITMC":
                    text2 = player[游戏对象属性.魔法命中].ToString();
                    break;
                case "MAXMP":
                    text2 = player[游戏对象属性.最大魔力].ToString();
                    break;
                case "MAXMC":
                    text2 = player[游戏对象属性.最大魔法].ToString();
                    break;
                case "STR()":
                    text2 = this.FindVariable(player, "%" + match2.Groups[2].Captures[0].Value.ToUpper());
                    break;
                case "MONTH":
                    text2 = 主程.当前时间.Month.ToString();
                    break;
                case "MAXSC":
                    text2 = player[游戏对象属性.最大道术].ToString();
                    break;
                case "CLASS":
                    text2 = player.角色职业.ToString();
                    break;
                case "NODE()":
                    {
                        if (ushort.TryParse(match3.Groups[2].Captures[0].Value, out var result9))
                        {
                            text2 = player.获取玩家节点(result9).ToString();
                        }
                        break;
                    }
                case "MAXARC":
                    text2 = player[游戏对象属性.最大弓术].ToString();
                    break;
                case "MAXASC":
                    text2 = player[游戏对象属性.最大刺术].ToString();
                    break;
                case "MINUTE":
                    text2 = 主程.当前时间.Minute.ToString();
                    break;
                case "SILVER":
                    text2 = player.银币数量.ToString(CultureInfo.InvariantCulture);
                    break;
                case "HOLYDM":
                    text2 = player[游戏对象属性.最小圣伤].ToString();
                    break;
                case "PPOINT":
                    text2 = player.所属账号.推广点.V.ToString(CultureInfo.InvariantCulture);
                    break;
                case "MAXMAC":
                    text2 = player[游戏对象属性.最大魔防].ToString();
                    break;
                case "UTCNOW":
                    text2 = 计算类.GetTimeStamp(主程.当前时间).ToString();
                    break;
                case "SECOND":
                    text2 = 主程.当前时间.Second.ToString();
                    break;
                case "MCHITHP":
                    text2 = player[游戏对象属性.魔法击回].ToString();
                    break;
                case "ACCOUNT":
                    text2 = player.所属账号.账号名字.V.ToString();
                    break;
                case "BREAKAC":
                    text2 = player[游戏对象属性.破物防].ToString();
                    break;
                case "BAGSIZE":
                    text2 = player.背包大小.ToString();
                    break;
                case "CURDATE":
                    text2 = 计算类.时间转换(主程.当前时间.Date).ToString();
                    break;
                case "DCHITHP":
                    text2 = player[游戏对象属性.物理击回].ToString();
                    break;
                case "GUILDLV":
                    text2 = player.行会等级.ToString();
                    break;
                case "VERSION":
                    text2 = 主程.开区节点.ToString(CultureInfo.InvariantCulture);
                    break;
                case "X_COORD":
                    text2 = player.当前坐标.X.ToString();
                    break;
                case "Y_COORD":
                    text2 = player.当前坐标.Y.ToString();
                    break;
                case "PKPOINT":
                    text2 = player.PK值惩罚.ToString();
                    break;
                case "PAYMENT":
                    text2 = player.角色数据.累计充值.V.ToString(CultureInfo.InvariantCulture);
                    break;
                case "DAYPOINT":
                    text2 = player.角色数据.日程进度.V.ToString();
                    break;
                case "USERNAME":
                    text2 = player.对象名字;
                    break;
                case "BLASTATT":
                    text2 = player[游戏对象属性.暴击概率].ToString();
                    break;
                case "PHYSIQUE":
                    text2 = (player.开通龙卫觉醒 ? "1" : "0");
                    break;
                case "BREAKMAC":
                    text2 = player[游戏对象属性.破魔防].ToString();
                    break;
                case "MONCNT()":
                    {
                        if (byte.TryParse(match4.Groups[3].Captures[0].Value, out var result17))
                        {
                            地图实例 地图实例3;
                            地图实例3 = 地图处理网关.已分配地图(result17);
                            if (地图实例3 != null)
                            {
                                text2 = 地图实例3.获取怪物列表(match4.Groups[4].Captures[0].Value).Count.ToString();
                            }
                        }
                        break;
                    }
                case "GAMEGOLD":
                    text2 = player.元宝数量.ToString(CultureInfo.InvariantCulture);
                    break;
                case "MARFADAY":
                    text2 = ((int)Math.Round((player.特权时间 - 主程.当前时间).TotalDays)).ToString();
                    break;
                case "DOUBLEEXP":
                    text2 = player.双倍经验.ToString();
                    break;
                case "ANTIMAGIC":
                    text2 = player[游戏对象属性.魔法闪避].ToString();
                    break;
                case "USERCOUNT":
                    text2 = 地图处理网关.玩家对象表.Count.ToString(CultureInfo.InvariantCulture);
                    break;
                case "POWERRATE":
                    text2 = player[游戏对象属性.伤害加成].ToString();
                    break;
                case "GUILDNAME":
                    {
                        行会数据 所属行会;
                        所属行会 = player.所属行会;
                        if (所属行会 == null)
                        {
                            obj3 = null;
                        }
                        else
                        {
                            数据监视器<string> 行会名字2;
                            行会名字2 = 所属行会.行会名字;
                            if (行会名字2 == null)
                            {
                                obj3 = null;
                            }
                            else
                            {
                                obj3 = 行会名字2.V;
                                if (obj3 != null)
                                {
                                    goto IL_1166;
                                }
                            }
                        }
                        obj3 = "";
                        goto IL_1166;
                    }
                case "MARFADAY()":
                    {
                        if (byte.TryParse(match3.Groups[2].Captures[0].Value, out var result16))
                        {
                            text2 = (player.剩余特权.TryGetValue(result16, out var v4) ? v4 : 0).ToString();
                        }
                        break;
                    }
                case "CASTLELORD":
                    {
                        数据监视器<行会数据> 占领行会2;
                        占领行会2 = 系统数据.数据.占领行会;
                        if (占领行会2 == null)
                        {
                            obj2 = null;
                        }
                        else
                        {
                            行会数据 v2;
                            v2 = 占领行会2.V;
                            if (v2 == null)
                            {
                                obj2 = null;
                            }
                            else
                            {
                                数据监视器<角色数据> 行会会长;
                                行会会长 = v2.行会会长;
                                if (行会会长 == null)
                                {
                                    obj2 = null;
                                }
                                else
                                {
                                    角色数据 v3;
                                    v3 = 行会会长.V;
                                    if (v3 == null)
                                    {
                                        obj2 = null;
                                    }
                                    else
                                    {
                                        数据监视器<string> 角色名字;
                                        角色名字 = v3.角色名字;
                                        if (角色名字 == null)
                                        {
                                            obj2 = null;
                                        }
                                        else
                                        {
                                            obj2 = 角色名字.V;
                                            if (obj2 != null)
                                            {
                                                goto IL_127b;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        obj2 = "";
                        goto IL_127b;
                    }
                case "ANTIPOISON":
                    text2 = player[游戏对象属性.中毒躲避].ToString();
                    break;
                case "MAXHOLYDAM":
                    text2 = player[游戏对象属性.最大圣伤].ToString();
                    break;
                case "PAYMENTDAY":
                    text2 = player.角色数据.今日充值.V.ToString(CultureInfo.InvariantCulture);
                    break;
                case "TALENTLV()":
                    {
                        if (byte.TryParse(match3.Groups[2].Captures[0].Value, out var result15))
                        {
                            text2 = player.角色数据.天赋等级[result15].ToString();
                        }
                        break;
                    }
                case "ITEMNAME()":
                    {
                        if (int.TryParse(match3.Groups[2].Captures[0].Value, out var result14) && 游戏物品.数据表.TryGetValue(result14, out var value))
                        {
                            text2 = value.物品名字;
                        }
                        break;
                    }
                case "ITEMVALUE()":
                    {
                        string text3;
                        text3 = this.FindVariable(player, "%" + match5.Groups[3].Captures[0].Value.ToUpper());
                        string text4;
                        text4 = this.FindVariable(player, "%" + match5.Groups[4].Captures[0].Value.ToUpper());
                        string text5;
                        text5 = this.FindVariable(player, "%" + match5.Groups[5].Captures[0].Value.ToUpper());
                        if (byte.TryParse(text3.Replace("%", ""), out var result11) && byte.TryParse(text4.Replace("%", ""), out var result12) && byte.TryParse(text5.Replace("%", ""), out var result13))
                        {
                            物品数据 item;
                            item = player.GetItem(result11, result12);
                            if (item != null)
                            {
                                text2 = ((result13 != 51) ? player.GetItemValue(item, result13).ToString() : item.物品名字);
                            }
                        }
                        break;
                    }
                case "LASTKILLMON":
                    text2 = player.最后杀死的怪物编号.ToString();
                    break;
                case "MAPMONCNT()":
                    {
                        if (byte.TryParse(match3.Groups[2].Captures[0].Value, out var result10))
                        {
                            地图实例 地图实例2;
                            地图实例2 = 地图处理网关.已分配地图(result10);
                            if (地图实例2 != null)
                            {
                                text2 = 地图实例2.获取怪物列表().Count.ToString();
                            }
                        }
                        break;
                    }
                case "ATTACTSPEED":
                    text2 = player[游戏对象属性.攻击速度].ToString();
                    break;
                case "BLASTATTDEC":
                    text2 = player[游戏对象属性.减暴击].ToString();
                    break;
                case "BLASTATTDAM":
                    text2 = player[游戏对象属性.暴击伤害].ToString();
                    break;
                case "CASTLEGUILD":
                    {
                        数据监视器<行会数据> 占领行会;
                        占领行会 = 系统数据.数据.占领行会;
                        if (占领行会 == null)
                        {
                            obj = null;
                        }
                        else
                        {
                            行会数据 v;
                            v = 占领行会.V;
                            if (v == null)
                            {
                                obj = null;
                            }
                            else
                            {
                                数据监视器<string> 行会名字;
                                行会名字 = v.行会名字;
                                if (行会名字 == null)
                                {
                                    obj = null;
                                }
                                else
                                {
                                    obj = 行会名字.V;
                                    if (obj != null)
                                    {
                                        goto IL_16ad;
                                    }
                                }
                            }
                        }
                        obj = "";
                        goto IL_16ad;
                    }
                case "VARSORTIDX()":
                    {
                        if (byte.TryParse(match4.Groups[3].Captures[0].Value, out var result7) && int.TryParse(match4.Groups[4].Captures[0].Value, out var result8))
                        {
                            系统数据.变量排序结果 变量排序结果3;
                            变量排序结果3 = 系统数据.获取变量排序列表(result7, result8);
                            if (变量排序结果3 != null)
                            {
                                text2 = player.角色数据.当前排名[变量排序结果3.当前类型].ToString();
                            }
                        }
                        break;
                    }
                case "VARSORTNAME()":
                    {
                        if (byte.TryParse(match5.Groups[3].Captures[0].Value, out var result4) && int.TryParse(match5.Groups[4].Captures[0].Value, out var result5) && int.TryParse(match5.Groups[5].Captures[0].Value, out var result6))
                        {
                            系统数据.变量排序结果 变量排序结果2;
                            变量排序结果2 = 系统数据.获取变量排序列表(result4, result5);
                            result6--;
                            if (result6 >= 0 && result6 < 变量排序结果2.Count)
                            {
                                text2 = 变量排序结果2[result6].ToString();
                            }
                        }
                        break;
                    }
                case "VARSORTVALUE()":
                    {
                        if (byte.TryParse(match5.Groups[3].Captures[0].Value, out var result) && int.TryParse(match5.Groups[4].Captures[0].Value, out var result2) && int.TryParse(match5.Groups[5].Captures[0].Value, out var result3))
                        {
                            系统数据.变量排序结果 变量排序结果;
                            变量排序结果 = 系统数据.获取变量排序列表(result, result2);
                            result3--;
                            if (result3 >= 0 && result3 < 变量排序结果.Count)
                            {
                                text2 = 变量排序结果[result3].角.脚本数字[result2].ToString();
                            }
                        }
                        break;
                    }
                case "SCRIPTDROPRATE":
                    text2 = player.脚本爆率.ToString();
                    break;
                case "BLASTATTDAMDEC":
                    text2 = player[游戏对象属性.减暴伤].ToString();
                    break;
                case "LASTKILLPLAYNAME":
                    text2 = player.最后杀死的玩家名字.ToString();
                    break;
                case "LASTKILLMEPLAYNAME":
                    text2 = player.最后杀死自己的玩家名字.ToString();
                    break;
                case "SPECIALREPAIRALLCOST":
                    text2 = player.角色装备.Values.Sum((装备数据 O) => O.能否修理 ? O.特修费用 : 0).ToString();
                    break;
                default:
                    {
                        flag = false;
                        text2 = string.Empty;
                        break;
                    }
                IL_16ad:
                    text2 = (string)obj;
                    break;
                IL_127b:
                    text2 = (string)obj2;
                    break;
                IL_1166:
                    text2 = (string)obj3;
                    break;
            }
            if (!flag && string.IsNullOrEmpty(text2))
            {
                return param;
            }
            return param.Replace(match.Value, text2);
        }

        public string ReplaceValue(怪物实例 Monster, string param)
        {
            Regex regex;
            regex = new Regex("\\<\\$(.*)\\>");
            Regex regex2;
            regex2 = new Regex("(.*?)\\(([A-Z][0-9]+)\\)");
            Match match;
            match = regex.Match(param);
            if (!match.Success)
            {
                return param;
            }
            string text;
            text = match.Groups[1].Captures[0].Value.ToUpper();
            Match match2;
            match2 = regex2.Match(text);
            if (regex2.Match(text).Success)
            {
                text = text.Replace(match2.Groups[2].Captures[0].Value.ToUpper(), "");
            }
            string empty;
            empty = string.Empty;
            empty = text switch
            {
                "HP" => Monster.当前体力.ToString(CultureInfo.InvariantCulture),
                "MAP" => Monster.当前地图.地图编号.ToString(),
                "DATE" => 主程.当前时间.ToShortDateString(),
                "STR()" => this.FindVariable(Monster, "%" + match2.Groups[2].Captures[0].Value.ToUpper()),
                "MAXHP" => Monster[游戏对象属性.最大体力].ToString(CultureInfo.InvariantCulture),
                "LEVEL" => Monster.当前等级.ToString(CultureInfo.InvariantCulture),
                "Y_COORD" => Monster.当前坐标.Y.ToString(),
                "X_COORD" => Monster.当前坐标.X.ToString(),
                "USERNAME" => Monster.对象名字,
                _ => string.Empty,
            };
            if (string.IsNullOrEmpty(empty))
            {
                return param;
            }
            return param.Replace(match.Value, empty);
        }

        public bool Check()
        {
            bool flag;
            flag = false;
            int num;
            num = 0;
            while (true)
            {
                if (num < this.CheckList.Count)
                {
                    NPCChecks nPCChecks;
                    nPCChecks = this.CheckList[num];
                    List<string> list;
                    list = nPCChecks.Params.ToList();
                    int result2;
                    int result5;
                    uint result;
                    switch (nPCChecks.Type)
                    {
                        case CheckType.CheckCalc:
                            {
                                if (int.TryParse(list[0], out var result3) && int.TryParse(list[2], out var result4))
                                {
                                    try
                                    {
                                        flag = !NPCSegment.Compare(list[1], result3, result4);
                                    }
                                    catch (ArgumentException)
                                    {
                                        主程.添加系统日志($"不正确的操作员: {list[1]}, Page: {this.Key}");
                                        return true;
                                    }
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.CheckHum:
                            if (int.TryParse(list[1], out result2) && int.TryParse(list[2], out result5))
                            {
                                地图实例 地图实例2;
                                地图实例2 = 地图处理网关.已分配地图(result5);
                                flag = 地图实例2 == null || !NPCSegment.Compare(list[0], 地图实例2.玩家列表.Count(), result2);
                            }
                            else
                            {
                                flag = true;
                            }
                            break;
                        case CheckType.CheckMon:
                            if (int.TryParse(list[1], out result2) && int.TryParse(list[2], out result5))
                            {
                                地图实例 地图实例2;
                                地图实例2 = 地图处理网关.已分配地图(result5);
                                flag = 地图实例2 == null || !NPCSegment.Compare(list[0], 地图实例2.获取怪物列表().Count, result2);
                            }
                            else
                            {
                                flag = true;
                            }
                            break;
                        case CheckType.Random:
                            flag = !int.TryParse(list[0], out result2) || 主程.随机数.Next(0, result2) != 0;
                            break;
                        case CheckType.CheckDay:
                            flag = 主程.当前时间.DayOfWeek.ToString().ToUpper() != list[0].ToUpper();
                            break;
                        case CheckType.CheckHour:
                            flag = !uint.TryParse(list[0], out result) || 主程.当前时间.Hour != result;
                            break;
                        case CheckType.CheckMinute:
                            flag = !uint.TryParse(list[0], out result) || 主程.当前时间.Minute != result;
                            break;
                    }
                    if (flag)
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                this.Success();
                return true;
            }
            this.Failed();
            return false;
        }

        public bool Check(怪物实例 monster)
        {
            bool flag;
            flag = false;
            int num;
            num = 0;
            while (true)
            {
                if (num < this.CheckList.Count)
                {
                    NPCChecks nPCChecks;
                    nPCChecks = this.CheckList[num];
                    List<string> list;
                    list = nPCChecks.Params.Select((string t) => this.FindVariable(monster, t)).ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        string[] array;
                        array = list[i].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length != 0)
                        {
                            string[] array2;
                            array2 = array;
                            foreach (string text in array2)
                            {
                                list[i] = list[i].Replace(text, this.ReplaceValue(monster, text));
                            }
                        }
                    }
                    uint result2;
                    int result;
                    switch (nPCChecks.Type)
                    {
                        case CheckType.CheckDay:
                            flag = 主程.当前时间.DayOfWeek.ToString().ToUpper() != list[0].ToUpper();
                            break;
                        case CheckType.CheckHour:
                            flag = !uint.TryParse(list[0], out result2) || 主程.当前时间.Hour != result2;
                            break;
                        case CheckType.CheckMinute:
                            flag = !uint.TryParse(list[0], out result2) || 主程.当前时间.Minute != result2;
                            break;
                        case CheckType.CheckRange:
                            {
                                if (int.TryParse(list[0], out var result7) && int.TryParse(list[1], out var result8) && int.TryParse(list[2], out var result9))
                                {
                                    Point point;
                                    point = default(Point);
                                    point.X = result7;
                                    point.Y = result8;
                                    flag = 计算类.网格距离(终点: point, 原点: monster.当前坐标) > result9;
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.CheckHum:
                            {
                                if (int.TryParse(list[1], out result) && int.TryParse(list[2], out var result6))
                                {
                                    地图实例 地图实例2;
                                    地图实例2 = 地图处理网关.已分配地图(result6);
                                    flag = 地图实例2 == null || !NPCSegment.Compare(list[0], 地图实例2.玩家列表.Count(), result);
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.Level:
                            {
                                if (!ushort.TryParse(list[1], out var result3))
                                {
                                    flag = true;
                                    break;
                                }
                                try
                                {
                                    flag = !NPCSegment.Compare(list[0], monster.当前等级, result3);
                                }
                                catch (ArgumentException)
                                {
                                    主程.添加系统日志($"不正确的操作员: {list[0]}, Page: {this.Key}");
                                    return true;
                                }
                                break;
                            }
                        case CheckType.CheckMap:
                            flag = !int.TryParse(list[0], out result) || monster.当前地图.地图编号 != result;
                            break;
                        case CheckType.CheckCalc:
                            {
                                if (int.TryParse(list[0], out var result4) && int.TryParse(list[2], out var result5))
                                {
                                    try
                                    {
                                        flag = !NPCSegment.Compare(list[1], result4, result5);
                                    }
                                    catch (ArgumentException)
                                    {
                                        主程.添加系统日志($"不正确的操作员: {list[1]}, Page: {this.Key}");
                                        return true;
                                    }
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.Random:
                            flag = !int.TryParse(list[0], out result) || 主程.随机数.Next(0, result) != 0;
                            break;
                    }
                    if (!flag)
                    {
                        num++;
                        continue;
                    }
                    break;
                }
                this.Success(monster);
                return true;
            }
            this.Failed(monster);
            return false;
        }

        public bool Check(玩家实例 player)
        {
            bool flag;
            flag = false;
            int num;
            num = 0;
            while (true)
            {
                if (num < this.CheckList.Count)
                {
                    NPCChecks nPCChecks;
                    nPCChecks = this.CheckList[num];
                    List<string> param;
                    param = nPCChecks.Params.Select((string t) => this.FindVariable(player, t)).ToList();
                    for (int i = 0; i < param.Count; i++)
                    {
                        string[] array;
                        array = param[i].Split(new char[1] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length != 0)
                        {
                            string[] array2;
                            array2 = array;
                            foreach (string text in array2)
                            {
                                param[i] = param[i].Replace(text + ">", this.ReplaceValue(player, text + ">"));
                            }
                        }
                    }
                    bool flag2;
                    flag2 = false;
                    ushort result22;
                    byte result23;
                    uint result20;
                    int result8;
                    int result11;
                    int result12;
                    int result29;
                    switch (nPCChecks.Type)
                    {
                        case CheckType.IsAdmin:
                            flag = !player.管理员模式;
                            break;
                        case CheckType.Level:
                            {
                                if (!ushort.TryParse(param[1], out var result6))
                                {
                                    flag = true;
                                    break;
                                }
                                try
                                {
                                    byte result7;
                                    result7 = 0;
                                    if (param.Count > 2 && !byte.TryParse(param[2], out result7))
                                    {
                                        result7 = 0;
                                    }
                                    switch (result7)
                                    {
                                        case 1:
                                            if (player.所属队伍 == null)
                                            {
                                                flag = true;
                                                break;
                                            }
                                            try
                                            {
                                                foreach (角色数据 item2 in player.所属队伍.队伍成员)
                                                {
                                                    if (item2.网络连接 != null && item2.网络连接.绑定角色 != null)
                                                    {
                                                        flag |= !NPCSegment.Compare(param[0], item2.网络连接.绑定角色.当前等级, result6);
                                                    }
                                                    if (flag)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            catch (ArgumentException)
                                            {
                                                主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                                return true;
                                            }
                                            break;
                                        case 0:
                                            try
                                            {
                                                flag = !NPCSegment.Compare(param[0], player.当前等级, result6);
                                            }
                                            catch (ArgumentException)
                                            {
                                                主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                                return true;
                                            }
                                            break;
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                    return true;
                                }
                                break;
                            }
                        case CheckType.CheckList:
                            {
                                string path;
                                path = Path.Combine(Settings.NPCPath, param[1]);
                                if (!Directory.Exists(Path.GetDirectoryName(path)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                                }
                                if (!File.Exists(path))
                                {
                                    flag = true;
                                    break;
                                }
                                try
                                {
                                    flag = 0 > File.ReadAllLines(path).FindIndex((string x) => x == param[0]);
                                }
                                catch (ArgumentException)
                                {
                                    主程.添加系统日志($"检查清单失败: {param[1]}, Page: {this.Key}");
                                    return true;
                                }
                                break;
                            }
                        case CheckType.CheckItem:
                            {
                                if (!ushort.TryParse(param[1], out result22))
                                {
                                    flag = true;
                                    break;
                                }
                                byte b;
                                b = 0;
                                if (param.Count > 2 && !byte.TryParse(param[2], out result23))
                                {
                                    result23 = 0;
                                }
                                switch (b)
                                {
                                    case 1:
                                        if (player.所属队伍 == null)
                                        {
                                            flag = true;
                                            break;
                                        }
                                        foreach (角色数据 item3 in player.所属队伍.队伍成员)
                                        {
                                            if (item3.网络连接 != null && item3.网络连接.绑定角色 != null)
                                            {
                                                flag |= !item3.网络连接.绑定角色.FindItem(param[0], result22);
                                            }
                                            if (flag)
                                            {
                                                break;
                                            }
                                        }
                                        break;
                                    case 0:
                                        flag = !player.FindItem(param[0], result22);
                                        break;
                                }
                                break;
                            }
                        case CheckType.CheckGold:
                            if (!uint.TryParse(param[1], out result20))
                            {
                                flag = true;
                                break;
                            }
                            result23 = 0;
                            if (param.Count > 2 && !byte.TryParse(param[2], out result23))
                            {
                                result23 = 0;
                            }
                            switch (result23)
                            {
                                case 1:
                                    if (player.所属队伍 == null)
                                    {
                                        flag = true;
                                        break;
                                    }
                                    try
                                    {
                                        foreach (角色数据 item4 in player.所属队伍.队伍成员)
                                        {
                                            if (item4.网络连接 != null && item4.网络连接.绑定角色 != null)
                                            {
                                                flag |= !NPCSegment.Compare(param[0], item4.金币数量, result20);
                                            }
                                            if (flag)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    catch (ArgumentException)
                                    {
                                        主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                        return true;
                                    }
                                    break;
                                case 0:
                                    try
                                    {
                                        flag = !NPCSegment.Compare(param[0], player.金币数量, result20);
                                    }
                                    catch (ArgumentException)
                                    {
                                        主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case CheckType.CheckGameGold:
                            if (!uint.TryParse(param[1], out result20))
                            {
                                flag = true;
                                break;
                            }
                            try
                            {
                                flag = !NPCSegment.Compare(param[0], player.元宝数量, result20);
                            }
                            catch (ArgumentException)
                            {
                                主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                return true;
                            }
                            break;
                        case CheckType.CheckBindGold:
                            if (!uint.TryParse(param[1], out result20))
                            {
                                flag = true;
                                break;
                            }
                            try
                            {
                                flag = !NPCSegment.Compare(param[0], player.银币数量, result20);
                            }
                            catch (ArgumentException)
                            {
                                主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                return true;
                            }
                            break;
                        case CheckType.CheckDoubleExp:
                            if (!int.TryParse(param[1], out result8))
                            {
                                flag = true;
                                break;
                            }
                            try
                            {
                                flag = !NPCSegment.Compare(param[0], player.双倍经验, result8);
                            }
                            catch (ArgumentException)
                            {
                                主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                return true;
                            }
                            break;
                        case CheckType.CheckGender:
                            {
                                flag = !Enum.TryParse<游戏对象性别>(param[0], ignoreCase: false, out var result32) || player.角色性别 != result32;
                                break;
                            }
                        case CheckType.CheckClass:
                            {
                                flag = !Enum.TryParse<游戏对象职业>(param[0], ignoreCase: true, out var result19) || player.角色职业 != result19;
                                break;
                            }
                        case CheckType.CheckDay:
                            flag = 主程.当前时间.DayOfWeek.ToString().ToUpper() != param[0].ToUpper();
                            break;
                        case CheckType.CheckHour:
                            flag = !uint.TryParse(param[0], out result20) || 主程.当前时间.Hour != result20;
                            break;
                        case CheckType.CheckMinute:
                            flag = !uint.TryParse(param[0], out result20) || 主程.当前时间.Minute != result20;
                            break;
                        case CheckType.CheckNameList:
                            flag = !File.Exists(param[0]) || !File.ReadAllLines(param[0]).Contains(player.对象名字);
                            break;
                        case CheckType.CheckPkPoint:
                            if (!int.TryParse(param[1], out result8))
                            {
                                flag = true;
                                break;
                            }
                            try
                            {
                                flag = !NPCSegment.Compare(param[0], player.PK值惩罚, result8);
                            }
                            catch (ArgumentException)
                            {
                                主程.添加系统日志($"不正确的操作员: {param[0]}, Page: {this.Key}");
                                return true;
                            }
                            break;
                        case CheckType.CheckRange:
                            {
                                if (int.TryParse(param[0], out var result3) && int.TryParse(param[1], out var result4) && int.TryParse(param[2], out var result5))
                                {
                                    Point point;
                                    point = default(Point);
                                    point.X = result3;
                                    point.Y = result4;
                                    flag = 计算类.网格距离(终点: point, 原点: player.当前坐标) > result5;
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.Check:
                            {
                                if (int.TryParse(param[0], out result8) && uint.TryParse(param[1], out var result35) && result8 <= 1999)
                                {
                                    bool flag3;
                                    flag3 = Convert.ToBoolean(result35);
                                    flag = player.角色数据.任务标识[result8] != flag3;
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.CheckData:
                            {
                                object cv;
                                cv = nPCChecks.F.读(player);
                                int.TryParse(param[1], out result8);
                                flag = nPCChecks.F.检测(param[0], param[1], result8, cv);
                                break;
                            }
                        case CheckType.CheckHum:
                            {
                                if (int.TryParse(param[1], out result8) && int.TryParse(param[2], out var result21))
                                {
                                    地图实例 地图实例2;
                                    地图实例2 = 地图处理网关.已分配地图(result21);
                                    flag = 地图实例2 == null || !NPCSegment.Compare(param[0], 地图实例2.玩家列表.Count(), result8);
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.Random:
                            flag = !int.TryParse(param[0], out result8) || 主程.随机数.Next(0, result8) != 0;
                            break;
                        case CheckType.RandomEx:
                            {
                                flag = !int.TryParse(param[0], out var result39) || !int.TryParse(param[1], out var result40) || 主程.随机数.Next(0, result40) > result39;
                                break;
                            }
                        case CheckType.Groupleader:
                            flag = player.所属队伍 == null || player.所属队伍.队长编号 != player.地图编号;
                            break;
                        case CheckType.GroupCount:
                            flag = !int.TryParse(param[1], out result8) || player.所属队伍 == null || !NPCSegment.Compare(param[0], player.所属队伍.队员数量, result8);
                            break;
                        case CheckType.GroupCheckNearby:
                            {
                                Point point2;
                                point2 = new Point(-1, -1);
                                point2 = 地图处理网关.守卫对象表.Values.FirstOrDefault((守卫实例 o) => o.地图编号 == player.NPCObjectID).当前坐标;
                                if (point2.X == -1)
                                {
                                    flag = true;
                                    break;
                                }
                                if (player.所属队伍 == null)
                                {
                                    flag = true;
                                    break;
                                }
                                foreach (角色数据 item5 in player.所属队伍.队伍成员)
                                {
                                    if (item5.网络连接?.绑定角色 != null && (flag |= 计算类.网格距离(item5.网络连接.绑定角色.当前坐标, point2) > 9 || item5.网络连接.绑定角色.当前地图 != player.当前地图))
                                    {
                                        break;
                                    }
                                }
                                break;
                            }
                        case CheckType.PetCount:
                            flag = !int.TryParse(param[1], out result8) || !NPCSegment.Compare(param[0], player.宠物列表.Count(), result8);
                            break;
                        case CheckType.CheckCalc:
                            if (int.TryParse(param[0], out result11) && int.TryParse(param[2], out result12))
                            {
                                try
                                {
                                    flag = !NPCSegment.Compare(param[1], result11, result12);
                                }
                                catch (ArgumentException)
                                {
                                    主程.添加系统日志($"不正确的操作员: {param[1]}, Page: {this.Key}");
                                    return true;
                                }
                            }
                            else
                            {
                                try
                                {
                                    flag = !NPCSegment.Compare(param[1], param[0], param[2]);
                                }
                                catch (ArgumentException)
                                {
                                    主程.添加系统日志($"不正确的操作员: {param[1]}, Page: {this.Key}");
                                    return true;
                                }
                            }
                            break;
                        case CheckType.InGuild:
                            flag = ((param[0].Length <= 0) ? (player.所属行会 == null) : (player.所属行会 == null || player.所属行会.行会名字.V != param[0]));
                            break;
                        case CheckType.CheckMap:
                            flag = !int.TryParse(param[0], out result8) || player.当前地图.地图编号 != result8;
                            break;
                        case CheckType.CheckQuest:
                            flag = !int.TryParse(param[0], out result8) || ((!(param[1].ToUpper() == "ACTIVE")) ? (!player.是否已完成任务(result8)) : (!player.正在进行任务(result8)));
                            break;
                        case CheckType.CheckSkill:
                            {
                                if (!ushort.TryParse(param[0], out var result24))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!player.主体技能表.TryGetValue(result24, out var v4))
                                {
                                    flag = true;
                                    break;
                                }
                                byte result25;
                                result25 = 0;
                                if (param.Count > 1 && !byte.TryParse(param[1], out result25))
                                {
                                    flag = true;
                                }
                                else if (result25 > 0 && v4.铭文编号 != result25)
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.CheckSkillLv:
                            {
                                if (!ushort.TryParse(param[0], out var result9))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!player.主体技能表.TryGetValue(result9, out var v2))
                                {
                                    flag = true;
                                }
                                flag = param.Count < 1 || byte.TryParse(param[2], out var result10) || !NPCSegment.Compare(param[1], v2.升级等级, result10);
                                break;
                            }
                        case CheckType.CheckLoong:
                            {
                                if (!ushort.TryParse(param[0], out var result38))
                                {
                                    flag = true;
                                }
                                else if (!player.生效龙卫.ContainsKey(result38))
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.HasBagSpace:
                            flag = !int.TryParse(param[1], out result8) || !NPCSegment.Compare(param[0], player.背包剩余, result8);
                            break;
                        case CheckType.IsNewHuman:
                            flag = !player.isNewHuman;
                            break;
                        case CheckType.HasBuff:
                            {
                                ushort.TryParse(param[0], out var result37);
                                flag = !player.Buff列表.ContainsKey(result37);
                                break;
                            }
                        case CheckType.CheckItemIdx:
                            if (!ushort.TryParse(param[1], out result22))
                            {
                                flag = true;
                                break;
                            }
                            if (!int.TryParse(param[0], out result29))
                            {
                                flag = true;
                                break;
                            }
                            result23 = 0;
                            if (param.Count > 2 && !byte.TryParse(param[2], out result23))
                            {
                                result23 = 0;
                            }
                            switch (result23)
                            {
                                case 1:
                                    if (player.所属队伍 == null)
                                    {
                                        flag = true;
                                        break;
                                    }
                                    foreach (角色数据 item6 in player.所属队伍.队伍成员)
                                    {
                                        if (item6.网络连接 != null && item6.网络连接.绑定角色 != null)
                                        {
                                            flag |= !item6.网络连接.绑定角色.FindItem(result29, result22, 0);
                                        }
                                        if (flag)
                                        {
                                            break;
                                        }
                                    }
                                    break;
                                case 0:
                                    flag = !player.FindItem(result29, result22, 0);
                                    break;
                            }
                            break;
                        case CheckType.CheckItemLuck:
                            {
                                flag = !ushort.TryParse(param[1], out result22) || !int.TryParse(param[0], out result29) || !byte.TryParse(param[2], out var result30) || !player.FindItem(result29, result22, result30);
                                break;
                            }
                        case CheckType.CheckItemValue:
                            {
                                if (!byte.TryParse(param[0], out var result14))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!byte.TryParse(param[1], out var result15))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!byte.TryParse(param[2], out var result16))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!int.TryParse(param[4], out var result17))
                                {
                                    flag = true;
                                    break;
                                }
                                物品数据 item;
                                item = player.GetItem(result14, result15);
                                int left;
                                left = 0;
                                if (!(flag = item == null))
                                {
                                    left = player.GetItemValue(item, result16);
                                }
                                if (!flag)
                                {
                                    flag = !NPCSegment.Compare(param[3], left, result17);
                                }
                                break;
                            }
                        case CheckType.CheckGroupVar:
                            if (player.所属队伍 == null)
                            {
                                flag = true;
                                break;
                            }
                            foreach (角色数据 item7 in player.所属队伍.队伍成员)
                            {
                                if (item7.网络连接 != null && item7.网络连接.绑定角色 != null)
                                {
                                    string text2;
                                    text2 = this.FindVariable(item7.网络连接.绑定角色, "%" + param[0]);
                                    string text3;
                                    text3 = this.FindVariable(item7.网络连接.绑定角色, "%" + param[2]);
                                    if (!int.TryParse(text2.Replace("%", ""), out result11) || !int.TryParse(text3.Replace("%", ""), out result12))
                                    {
                                        flag = true;
                                        break;
                                    }
                                    if (flag |= !NPCSegment.Compare(param[1], result11, result12))
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case CheckType.CheckGuildLv:
                            {
                                if (player.所属行会 == null)
                                {
                                    flag = true;
                                    break;
                                }
                                int result13;
                                result13 = 0;
                                int.TryParse(param[1], out result13);
                                flag = param[0] switch
                                {
                                    "=" => player.所属行会.行会等级.V != result13,
                                    ">" => player.所属行会.行会等级.V <= result13,
                                    "<" => player.所属行会.行会等级.V >= result13,
                                    _ => true,
                                };
                                break;
                            }
                        case CheckType.CheckCastleOwner:
                            flag = player.所属行会 == null || player.所属行会 != 系统数据.数据.占领行会.V || 系统数据.数据.占领行会.V.行会成员[player.角色数据] != 行会职位.会长;
                            break;
                        case CheckType.IsCastleWar:
                            flag = 地图处理网关.沙城节点 != 2;
                            break;
                        case CheckType.IsOpenSevenCarnival:
                            flag = !player.开启七天乐;
                            break;
                        case CheckType.CheckMapSameMonCount:
                            {
                                if (!int.TryParse(param[0], out var result33))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!地图处理网关.地图实例表.TryGetValue(result33 * 16 + 1, out var value5))
                                {
                                    flag = true;
                                    break;
                                }
                                string monName;
                                monName = param[1];
                                if (string.IsNullOrWhiteSpace(monName))
                                {
                                    flag = true;
                                    break;
                                }
                                string text4;
                                text4 = param[2];
                                if (!int.TryParse(param[3], out var result34))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!int.TryParse(param[4], out var igoneNumber))
                                {
                                    igoneNumber = 0;
                                }
                                flag = text4 switch
                                {
                                    "=" => value5.对象列表.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && ((igoneNumber != 0) ? (o.对象名字 == monName) : (o.完整名字 == monName))) != result34,
                                    ">" => value5.对象列表.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && ((igoneNumber != 0) ? (o.对象名字 == monName) : (o.完整名字 == monName))) <= result34,
                                    "<" => value5.对象列表.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && ((igoneNumber != 0) ? (o.对象名字 == monName) : (o.完整名字 == monName))) >= result34,
                                    _ => true,
                                };
                                break;
                            }
                        case CheckType.CheckTitle:
                            {
                                flag = !byte.TryParse(param[0], out var result31) || !player.称号列表.ContainsKey(result31);
                                break;
                            }
                        case CheckType.CheckPlayerTitle:
                            {
                                string key;
                                key = this.ReplaceValue(player, param[0]);
                                if (游戏数据网关.角色数据表.检索表.TryGetValue(key, out var value4))
                                {
                                    角色数据 角色数据;
                                    角色数据 = value4 as 角色数据;
                                    flag = !byte.TryParse(param[1], out var result28) || !角色数据.称号列表.ContainsKey(result28);
                                }
                                break;
                            }
                        case CheckType.CheckAttribute:
                            {
                                装备数据 v3;
                                int attId3;
                                随机属性 value2;
                                if (!byte.TryParse(param[0], out var result18))
                                {
                                    flag = true;
                                }
                                else if (!player.角色装备.TryGetValue(result18, out v3))
                                {
                                    flag = true;
                                }
                                else if (!int.TryParse(param[1], out attId3))
                                {
                                    flag = true;
                                }
                                else if (!随机属性.数据表.TryGetValue(attId3, out value2))
                                {
                                    flag = true;
                                }
                                else if (v3.随机属性 != null && v3.随机属性.Count != 0)
                                {
                                    int val2;
                                    val2 = 0;
                                    if (param.Count >= 3)
                                    {
                                        int.TryParse(param[2], out val2);
                                    }
                                    flag = !v3.随机属性.Any((随机属性 a) => a.属性编号 == attId3 && (val2 == 0 || a.属性数值 >= val2));
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.CheckBagAttribute:
                            {
                                物品数据 v6;
                                int attId;
                                随机属性 value6;
                                if (!byte.TryParse(param[0], out var result36))
                                {
                                    flag = true;
                                }
                                else if (!player.角色背包.TryGetValue(result36, out v6))
                                {
                                    flag = true;
                                }
                                else if (!(v6 is 装备数据 装备数据2))
                                {
                                    flag = true;
                                }
                                else if (!int.TryParse(param[1], out attId))
                                {
                                    flag = true;
                                }
                                else if (!随机属性.数据表.TryGetValue(attId, out value6))
                                {
                                    flag = true;
                                }
                                else if (装备数据2.随机属性 != null && 装备数据2.随机属性.Count != 0)
                                {
                                    int val;
                                    val = 0;
                                    if (param.Count >= 3)
                                    {
                                        int.TryParse(param[2], out val);
                                    }
                                    flag = !装备数据2.随机属性.Any((随机属性 a) => a.属性编号 == attId && (val == 0 || a.属性数值 >= val));
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.CheckAttributeCount:
                            {
                                if (!byte.TryParse(param[0], out var result26))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!player.角色装备.TryGetValue(result26, out var v5))
                                {
                                    flag = true;
                                    break;
                                }
                                int attId2;
                                attId2 = 0;
                                随机属性 value3;
                                if (param.Count >= 2 && !int.TryParse(param[1], out attId2))
                                {
                                    flag = true;
                                }
                                else if (attId2 > 0 && !随机属性.数据表.TryGetValue(attId2, out value3))
                                {
                                    flag = true;
                                }
                                else if (v5.随机属性 != null && v5.随机属性.Count != 0)
                                {
                                    int result27;
                                    result27 = 0;
                                    if (param.Count >= 3 && !int.TryParse(param[2], out result27))
                                    {
                                        flag = true;
                                    }
                                    else if (attId2 != 0 || v5.随机属性.Count < result27)
                                    {
                                        flag = v5.随机属性.Count((随机属性 a) => a.属性编号 == attId2) < result27;
                                    }
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                        case CheckType.CheckBagAttributeCount:
                            {
                                if (!byte.TryParse(param[0], out var result))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!player.角色背包.TryGetValue(result, out var v))
                                {
                                    flag = true;
                                    break;
                                }
                                if (!(v is 装备数据 装备数据))
                                {
                                    flag = true;
                                    break;
                                }
                                int attId4;
                                attId4 = 0;
                                随机属性 value;
                                if (param.Count >= 2 && !int.TryParse(param[1], out attId4))
                                {
                                    flag = true;
                                }
                                else if (attId4 > 0 && !随机属性.数据表.TryGetValue(attId4, out value))
                                {
                                    flag = true;
                                }
                                else if (装备数据.随机属性 != null && 装备数据.随机属性.Count != 0)
                                {
                                    int result2;
                                    result2 = 0;
                                    if (param.Count >= 3 && !int.TryParse(param[2], out result2))
                                    {
                                        flag = true;
                                    }
                                    else if (attId4 != 0 || 装备数据.随机属性.Count < result2)
                                    {
                                        flag = 装备数据.随机属性.Count((随机属性 a) => a.属性编号 == attId4) < result2;
                                    }
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                    }
                    if (this.orMode)
                    {
                        flag2 = !flag;
                        if (nPCChecks.Not)
                        {
                            flag2 = !flag2;
                        }
                        if (flag2)
                        {
                            this.Success(player);
                            return true;
                        }
                    }
                    else if (!(!flag ^ nPCChecks.Not))
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                if (this.orMode)
                {
                    this.Failed(player);
                    return false;
                }
                this.Success(player);
                return true;
            }
            this.Failed(player);
            return false;
        }

        private void Act(IList<NPCActions> acts)
        {
            for (int i = 0; i < acts.Count; i++)
            {
                NPCActions nPCActions;
                nPCActions = acts[i];
                List<string> list;
                list = nPCActions.Params.ToList();
                int result6;
                switch (nPCActions.Type)
                {
                    case ActionType.Param1:
                        if (int.TryParse(list[1], out result6))
                        {
                            this.Param1 = list[0];
                            this.Param1Instance = result6;
                            break;
                        }
                        return;
                    case ActionType.Param2:
                        if (int.TryParse(list[0], out result6))
                        {
                            this.Param2 = result6;
                            break;
                        }
                        return;
                    case ActionType.Param3:
                        if (int.TryParse(list[0], out result6))
                        {
                            this.Param3 = result6;
                            break;
                        }
                        return;
                    case ActionType.Mongen:
                        {
                            if (this.Param1 == null || this.Param2 == 0 || this.Param3 == 0 || !int.TryParse(list[1], out var result7))
                            {
                                return;
                            }
                            try
                            {
                                if (!游戏怪物.数据表.TryGetValue(list[0], out var value3) || !游戏地图.数据表.TryGetValue((byte)this.Param1Instance, out var value4))
                                {
                                    return;
                                }
                                地图实例 地图实例3;
                                地图实例3 = 地图处理网关.已分配地图(value4.地图编号);
                                if (地图实例3 != null)
                                {
                                    for (int j = 0; j < result7; j++)
                                    {
                                        new 怪物实例(value3, 地图实例3, int.MaxValue, new Point(this.Param2, this.Param3), 禁止复活: true, 立即刷新: true).存活时间 = DateTime.MaxValue;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到怪物[" + list[0] + "]");
                            }
                            break;
                        }
                    case ActionType.NpcGen:
                        {
                            if (!byte.TryParse(list[0], out var result) || !int.TryParse(list[1], out var result2) || !int.TryParse(list[2], out var result3) || !ushort.TryParse(list[3], out var result4))
                            {
                                return;
                            }
                            if (!int.TryParse(list[4], out var result5))
                            {
                                result5 = int.MaxValue;
                            }
                            try
                            {
                                if (!地图守卫.数据表.TryGetValue(result4, out var value) || !游戏地图.数据表.TryGetValue(result, out var value2))
                                {
                                    return;
                                }
                                地图实例 地图实例2;
                                地图实例2 = 地图处理网关.已分配地图(value2.地图编号);
                                if (地图实例2 != null)
                                {
                                    new 守卫实例(value, 地图实例2, 游戏方向.上方, new Point(result2, result3)).存活时间 = 主程.当前时间.AddSeconds(result5);
                                }
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到地图[" + list[0] + "]");
                            }
                            break;
                        }
                    case ActionType.ClearNameList:
                        File.WriteAllLines(list[0], new string[0]);
                        break;
                    case ActionType.Break:
                        this.Page.BreakFromSegments = true;
                        break;
                }
            }
        }

        private void Act(IList<NPCActions> acts, 玩家实例 player)
        {
            for (int i = 0; i < acts.Count; i++)
            {
                NPCActions nPCActions;
                nPCActions = acts[i];
                List<string> param;
                param = nPCActions.Params.Select((string t) => this.FindVariable(player, t)).ToList();
                for (int j = 0; j < param.Count; j++)
                {
                    string[] array;
                    array = param[j].Split(new char[1] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                    if (array.Length != 0)
                    {
                        string[] array2;
                        array2 = array;
                        foreach (string text in array2)
                        {
                            param[j] = param[j].Replace(text + ">", this.ReplaceValue(player, text + ">"));
                        }
                        if (player.NPCData.TryGetValue("NPCInputStr", out var value))
                        {
                            param[j] = param[j].Replace("%INPUTSTR", (string)value);
                        }
                    }
                }
                byte result114;
                switch (nPCActions.Type)
                {
                    case ActionType.Move:
                        {
                            if (ushort.TryParse(param[0], out var result11) && int.TryParse(param[1], out var result12) && int.TryParse(param[2], out var result13) && int.TryParse(param[3], out var result14))
                            {
                                地图实例 跳转地图;
                                跳转地图 = 地图处理网关.已分配地图(result11);
                                player.玩家切换地图(跳转地图, (地图区域类型)result14, result12, result13);
                                break;
                            }
                            return;
                        }
                    case ActionType.MoveAll:
                        {
                            if (!ushort.TryParse(param[0], out var result149) || !ushort.TryParse(param[1], out var result150))
                            {
                                break;
                            }
                            if (!int.TryParse(param[2], out var result151))
                            {
                                result151 = 0;
                            }
                            地图实例 地图实例11;
                            地图实例11 = 地图处理网关.已分配地图(result149);
                            地图实例 地图实例12;
                            地图实例12 = 地图处理网关.已分配地图(result150);
                            if (地图实例11 == null || 地图实例12 == null)
                            {
                                break;
                            }
                            foreach (玩家实例 item9 in 地图实例11.玩家列表)
                            {
                                item9.玩家切换地图(地图实例12, (地图区域类型)result151);
                            }
                            break;
                        }
                    case ActionType.CreateInstance:
                        {
                            if (!ushort.TryParse(param[0], out var result130) || !ushort.TryParse(param[1], out var result131))
                            {
                                return;
                            }
                            ushort result132;
                            result132 = 0;
                            if (!ushort.TryParse(param[2], out result132))
                            {
                                result132 = 0;
                            }
                            ushort result133;
                            result133 = 0;
                            if (!ushort.TryParse(param[3], out result133))
                            {
                                result133 = 0;
                            }
                            地图实例 地图实例7;
                            地图实例7 = player.查找我的副本(result130);
                            地图实例 地图实例8;
                            地图实例8 = 地图处理网关.已分配地图(result130);
                            if (地图实例8.副本地图)
                            {
                                地图实例7?.关闭副本();
                                地图实例7 = new 地图实例(地图实例8.地图模板, 地图实例8);
                                地图实例7.副本主人 = player.地图编号;
                                地图实例7.关闭时间 = 主程.当前时间.AddMinutes((int)result131);
                                if (result132 != 0)
                                {
                                    副本.脚本创建(地图实例7, player, result133);
                                }
                                地图处理网关.副本实例表.Add(地图实例7);
                                地图实例7.初始化副本怪物(result131);
                                地图实例7.初始化副本守卫();
                            }
                            break;
                        }
                    case ActionType.InstanceMove:
                        {
                            if (int.TryParse(param[0], out var result87) && int.TryParse(param[1], out var result88) && int.TryParse(param[2], out var result89) && int.TryParse(param[3], out var result90))
                            {
                                地图实例 地图实例6;
                                地图实例6 = player.查找我的副本(result87);
                                if (地图实例6 != null)
                                {
                                    player.玩家切换地图(地图实例6, (地图区域类型)result90, result88, result89);
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case ActionType.AddList:
                        {
                            string path;
                            path = Path.Combine(Settings.NPCPath, param[1]);
                            if (!Directory.Exists(Path.GetDirectoryName(path)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                            }
                            File.AppendAllText(path, param[0] + "\r");
                            break;
                        }
                    case ActionType.DelList:
                        {
                            string path3;
                            path3 = Path.Combine(Settings.NPCPath, param[1]);
                            if (!Directory.Exists(Path.GetDirectoryName(path3)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(path3));
                            }
                            if (File.Exists(path3))
                            {
                                File.WriteAllLines(path3, (from x in File.ReadAllLines(path3)
                                                           where x != param[0]
                                                           select x).ToArray());
                            }
                            break;
                        }
                    case ActionType.OpenPage:
                        {
                            if (!int.TryParse(param[0], out var result96))
                            {
                                return;
                            }
                            switch (result96)
                            {
                                case 1:
                                    player.开启觉醒面板 = true;
                                    break;
                                case 2:
                                    player.开通龙卫觉醒 = true;
                                    break;
                                case 3:
                                    {
                                        if (byte.TryParse(param[1], out var result97) && 游戏天赋.数据表.TryGetValue(result97, out var _))
                                        {
                                            if (player.角色数据.天赋等级[result97] == 0)
                                            {
                                                player.角色数据.天赋等级[result97] = 1;
                                                player.刷新天赋属性();
                                                player.更新对象属性();
                                                player.更新玩家战力();
                                            }
                                            player.发送天赋描述();
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case ActionType.GroupInstanceMove:
                        {
                            if (!int.TryParse(param[0], out var result135) || !int.TryParse(param[1], out var result136) || !int.TryParse(param[2], out var result137) || !int.TryParse(param[3], out var result138))
                            {
                                return;
                            }
                            地图实例 地图实例9;
                            地图实例9 = player.查找我的副本(result135);
                            if (地图实例9 == null || player.所属队伍 == null)
                            {
                                return;
                            }
                            foreach (角色数据 item10 in player.所属队伍.队伍成员)
                            {
                                item10.网络连接?.绑定角色?.玩家切换地图(地图实例9, (地图区域类型)result138, result136, result137);
                            }
                            break;
                        }
                    case ActionType.GiveGold:
                        {
                            if (uint.TryParse(param[0], out var result134))
                            {
                                player.修改货币("+", 游戏货币.金币, result134);
                                //主程.WebLog(LogDataType.ScriptLog, Settings.统计UUID代码, Settings.游戏区服名称, "GiveGold", player.角色数据.角色名字.V, player.角色数据.所属账号.V.账号名字.V, player.金币数量.ToString(), "金币");
                                break;
                            }
                            return;
                        }
                    case ActionType.GiveGoldEx:
                        {
                            if (uint.TryParse(param[0], out var result35))
                            {
                                string key;
                                key = this.ReplaceValue(player, param[1]);
                                if (游戏数据网关.角色数据表.检索表.TryGetValue(key, out var value8))
                                {
                                    角色数据 角色数据;
                                    角色数据 = value8 as 角色数据;
                                    uint num10;
                                    num10 = uint.MaxValue - 角色数据.金币数量;
                                    if (num10 < result35)
                                    {
                                        result35 = num10;
                                    }
                                    if (角色数据.网络连接?.绑定角色 != null)
                                    {
                                        角色数据.网络连接.绑定角色.修改货币("+", 游戏货币.金币, result35);
                                    }
                                    else
                                    {
                                        角色数据.金币数量 += result35;
                                    }
                                    //主程.WebLog(LogDataType.ScriptLog, Settings.统计UUID代码, Settings.游戏区服名称, "GiveGoldEx", 角色数据.角色名字.V, 角色数据.所属账号.V.账号名字.V, 角色数据.金币数量.ToString(), "金币");
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case ActionType.TakeGold:
                        {
                            if (uint.TryParse(param[0], out var result112))
                            {
                                if (result112 >= player.金币数量)
                                {
                                    result112 = player.金币数量;
                                }
                                player.扣金币(result112);
                                break;
                            }
                            return;
                        }
                    case ActionType.TakeGoldEx:
                        {
                            if (uint.TryParse(param[0], out var result95))
                            {
                                string key6;
                                key6 = this.ReplaceValue(player, param[1]);
                                if (游戏数据网关.角色数据表.检索表.TryGetValue(key6, out var value26))
                                {
                                    角色数据 角色数据6;
                                    角色数据6 = value26 as 角色数据;
                                    if (result95 >= 角色数据6.金币数量)
                                    {
                                        result95 = 角色数据6.金币数量;
                                    }
                                    if (角色数据6?.网络连接?.绑定角色 != null)
                                    {
                                        角色数据6.网络连接.绑定角色.扣金币(result95);
                                    }
                                    else
                                    {
                                        角色数据6.金币数量 -= result95;
                                    }
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case ActionType.GiveBindGold:
                        {
                            if (uint.TryParse(param[0], out var result115))
                            {
                                //主程.WebLog(LogDataType.ScriptLog, Settings.统计UUID代码, Settings.游戏区服名称, "GiveBindGold", player.角色数据.角色名字.V, player.角色数据.所属账号.V.账号名字.V, player.银币数量.ToString(), "银币");
                                player.修改货币("+", 游戏货币.银币, result115);
                                break;
                            }
                            return;
                        }
                    case ActionType.GiveBindGoldEx:
                        {
                            if (uint.TryParse(param[0], out var result44))
                            {
                                string key2;
                                key2 = this.ReplaceValue(player, param[1]);
                                if (游戏数据网关.角色数据表.检索表.TryGetValue(key2, out var value15))
                                {
                                    角色数据 角色数据5;
                                    角色数据5 = value15 as 角色数据;
                                    uint num13;
                                    num13 = uint.MaxValue - 角色数据5.金币数量;
                                    if (num13 < result44)
                                    {
                                        result44 = num13;
                                    }
                                    if (角色数据5.网络连接?.绑定角色 != null)
                                    {
                                        角色数据5.网络连接.绑定角色.修改货币("+", 游戏货币.银币, result44);
                                    }
                                    else
                                    {
                                        角色数据5.银币数量 += result44;
                                    }
                                    //主程.WebLog(LogDataType.ScriptLog, Settings.统计UUID代码, Settings.游戏区服名称, "GiveBindGoldEx", 角色数据5.角色名字.V, 角色数据5.所属账号.V.账号名字.V, 角色数据5.银币数量.ToString(), "银币");
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case ActionType.TakeBindGold:
                        {
                            if (uint.TryParse(param[0], out var result51))
                            {
                                if (result51 >= player.银币数量)
                                {
                                    result51 = player.银币数量;
                                }
                                player.修改货币("-", 游戏货币.银币, result51);
                                break;
                            }
                            return;
                        }
                    case ActionType.TakeBindGoldEx:
                        {
                            if (uint.TryParse(param[0], out var result145))
                            {
                                string key10;
                                key10 = this.ReplaceValue(player, param[1]);
                                if (游戏数据网关.角色数据表.检索表.TryGetValue(key10, out var value39))
                                {
                                    角色数据 角色数据7;
                                    角色数据7 = value39 as 角色数据;
                                    if (result145 >= 角色数据7.银币数量)
                                    {
                                        result145 = 角色数据7.银币数量;
                                    }
                                    if (角色数据7?.网络连接?.绑定角色 != null)
                                    {
                                        角色数据7.网络连接.绑定角色.修改货币("-", 游戏货币.银币, result145);
                                    }
                                    else
                                    {
                                        角色数据7.银币数量 -= result145;
                                    }
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case ActionType.GiveDoubleExp:
                        {
                            if (int.TryParse(param[0], out var result86))
                            {
                                if (result86 + player.双倍经验 >= int.MaxValue)
                                {
                                    result86 = int.MaxValue - player.双倍经验;
                                }
                                player.双倍经验 += result86;
                                break;
                            }
                            return;
                        }
                    case ActionType.TakeDoubleExp:
                        {
                            if (int.TryParse(param[0], out var result53))
                            {
                                if (result53 >= player.双倍经验)
                                {
                                    result53 = player.双倍经验;
                                }
                                player.双倍经验 -= result53;
                                break;
                            }
                            return;
                        }
                    case ActionType.GiveItem:
                        {
                            if (param.Count < 2 || !int.TryParse(param[1], out var result15))
                            {
                                result15 = 1;
                            }
                            if (param.Count < 3 || !bool.TryParse(param[2], out var result16))
                            {
                                result16 = false;
                            }
                            if (!游戏物品.检索表.TryGetValue(param[0], out var value4) || value4.物品持久 == 0)
                            {
                                break;
                            }
                            string text2;
                            text2 = param[3];
                            物品数据 物品数据;
                            物品数据 = null;
                            int num3;
                            num3 = 0;
                            int num4;
                            num4 = Math.Max(1, (value4.持久类型 == 物品持久分类.堆叠) ? 1 : result15);
                            if (player.角色数据.角色背包.Count + num4 > player.角色数据.背包大小.V)
                            {
                                break;
                            }
                            if (value4.持久类型 == 物品持久分类.堆叠)
                            {
                                byte b;
                                b = byte.MaxValue;
                                byte b2;
                                b2 = 0;
                                while (b2 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色数据.角色背包.ContainsKey(b2))
                                    {
                                        b2++;
                                        continue;
                                    }
                                    b = b2;
                                    break;
                                }
                                player.角色数据.角色背包[b] = new 物品数据(value4, player.角色数据, 1, b, result15, result16, player.对象名字 + "-GiveItem");
                                物品数据 = player.角色数据.角色背包[b];
                                num3 += result15;
                                player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = player.角色数据.角色背包[b].字节描述()
                                });
                            }
                            else
                            {
                                for (int num5 = 0; num5 < num4; num5++)
                                {
                                    byte b3;
                                    b3 = byte.MaxValue;
                                    byte b4;
                                    b4 = 0;
                                    while (b4 < player.角色数据.背包大小.V)
                                    {
                                        if (player.角色数据.角色背包.ContainsKey(b4))
                                        {
                                            b4++;
                                            continue;
                                        }
                                        b3 = b4;
                                        break;
                                    }
                                    if (value4 is 游戏装备 模板)
                                    {
                                        player.角色数据.角色背包[b3] = new 装备数据(模板, player.角色数据, 1, b3, 随机生成: false, result16, player.对象名字 + "-GiveItem");
                                    }
                                    else if (value4.持久类型 == 物品持久分类.容器)
                                    {
                                        player.角色数据.角色背包[b3] = new 物品数据(value4, player.角色数据, 1, b3, 0, result16, player.对象名字 + "-GiveItem");
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b3] = new 物品数据(value4, player.角色数据, 1, b3, value4.物品持久, result16, player.对象名字 + "-GiveItem");
                                    }
                                    物品数据 = player.角色数据.角色背包[b3];
                                    num3++;
                                    if (player.角色数据.角色背包[b3].物品类型 == 物品使用分类.经验容器)
                                    {
                                        player.角色数据.角色背包[b3].当前持久.V = 0;
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b3].当前持久.V = player.角色数据.角色背包[b3].最大持久.V;
                                    }
                                    player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                    {
                                        物品描述 = player.角色数据.角色背包[b3].字节描述()
                                    });
                                }
                            }
                            if (物品数据 != null && text2 != "")
                            {
                                主程.添加物品日志(player, text2, 物品数据, num3, nPCActions.Type.ToString());
                            }
                            break;
                        }
                    case ActionType.TakeItem:
                        {
                            if (param.Count < 2 || !int.TryParse(param[1], out var result80))
                            {
                                result80 = 1;
                            }
                            if (!游戏物品.检索表.TryGetValue(param[0], out var value24) || value24.物品持久 == 0)
                            {
                                return;
                            }
                            string text5;
                            text5 = param[2];
                            物品数据 物品数据7;
                            物品数据7 = null;
                            int num21;
                            num21 = 0;
                            int num22;
                            num22 = result80;
                            if (value24.持久类型 == 物品持久分类.堆叠)
                            {
                                byte b15;
                                b15 = byte.MaxValue;
                                byte b16;
                                b16 = 0;
                                while (b16 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色背包.ContainsKey(b16) && !(player.角色背包[b16].物品名字 != param[0]))
                                    {
                                        物品数据 物品数据8;
                                        物品数据8 = player.角色背包[b16];
                                        物品数据7 = 物品数据8;
                                        num21 += 物品数据8.当前持久.V;
                                        if (num22 < 物品数据8.当前持久.V)
                                        {
                                            物品数据8.当前持久.V -= num22;
                                            player.网络连接?.发送封包(new 玩家物品变动
                                            {
                                                物品描述 = 物品数据8.字节描述()
                                            });
                                            break;
                                        }
                                        player.网络连接?.发送封包(new 删除玩家物品
                                        {
                                            背包类型 = 物品数据8.物品容器.V,
                                            物品位置 = 物品数据8.物品位置.V
                                        });
                                        player.角色背包.Remove(物品数据8.物品位置.V);
                                        物品数据8.删除数据();
                                        num22 -= 物品数据8.当前持久.V;
                                    }
                                    else
                                    {
                                        b16++;
                                    }
                                }
                            }
                            else
                            {
                                for (int num23 = 0; num23 < num22; num23++)
                                {
                                    byte b17;
                                    b17 = byte.MaxValue;
                                    byte b18;
                                    b18 = 0;
                                    while (b18 < player.角色数据.背包大小.V)
                                    {
                                        if (player.角色数据.角色背包.ContainsKey(b18) && !(player.角色背包[b18].物品名字 != param[0]))
                                        {
                                            物品数据 物品数据9;
                                            物品数据9 = player.角色背包[b18];
                                            物品数据7 = 物品数据9;
                                            player.网络连接?.发送封包(new 删除玩家物品
                                            {
                                                背包类型 = 物品数据9.物品容器.V,
                                                物品位置 = 物品数据9.物品位置.V
                                            });
                                            player.角色背包.Remove(物品数据9.物品位置.V);
                                            物品数据9.删除数据();
                                            num22--;
                                            num21++;
                                            if (num22 == 0)
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            b18++;
                                        }
                                    }
                                }
                            }
                            if (物品数据7 != null && text5 != "")
                            {
                                主程.添加物品日志(player, text5, 物品数据7, num21, nPCActions.Type.ToString());
                            }
                            break;
                        }
                    case ActionType.GiveExp:
                        {
                            if (int.TryParse(param[0], out var result54))
                            {
                                player.玩家增加经验(null, result54);
                                break;
                            }
                            return;
                        }
                    case ActionType.DelNameList:
                        {
                            string path2;
                            path2 = param[0];
                            File.WriteAllLines(path2, (from l in File.ReadLines(path2)
                                                       where l != player.对象名字
                                                       select l).ToList());
                            break;
                        }
                    case ActionType.ClearNameList:
                        File.WriteAllLines(param[0], new string[0]);
                        break;
                    case ActionType.OpenVarSort:
                        {
                            if (byte.TryParse(param[0], out var result) && byte.TryParse(param[1], out var result2))
                            {
                                if (!byte.TryParse(param[2], out var result3))
                                {
                                    result3 = 1;
                                }
                                系统数据.开启变量排序(result, result3, result2);
                            }
                            break;
                        }
                    case ActionType.NowVarSort:
                        {
                            if (byte.TryParse(param[0], out var result153))
                            {
                                if (!byte.TryParse(param[1], out var result154))
                                {
                                    result154 = 1;
                                }
                                系统数据.执行变量排序(player.角色数据, result154, result153);
                            }
                            break;
                        }
                    case ActionType.ClearVarSort:
                        {
                            if (byte.TryParse(param[0], out var result107))
                            {
                                if (!byte.TryParse(param[1], out var result108))
                                {
                                    result108 = 1;
                                }
                                系统数据.清理变量数据(result108, result107);
                            }
                            break;
                        }
                    case ActionType.ChangeLevel:
                        {
                            if (byte.TryParse(param[1], out var result109))
                            {
                                result109 = Math.Min(byte.MaxValue, result109);
                                int num32;
                                num32 = NPCSegment.Calculate(param[0], player.当前等级, result109);
                                player.当前等级 = (byte)num32;
                                player.当前经验 = 0L;
                                player.玩家升级处理();
                                break;
                            }
                            return;
                        }
                    case ActionType.SetPkPoint:
                        {
                            if (int.TryParse(param[0], out var result106))
                            {
                                player.PK值惩罚 = result106;
                                break;
                            }
                            return;
                        }
                    case ActionType.ReducePkPoint:
                        {
                            if (int.TryParse(param[0], out var result103))
                            {
                                player.PK值惩罚 -= result103;
                                if (player.PK值惩罚 < 0)
                                {
                                    player.PK值惩罚 = 0;
                                }
                                break;
                            }
                            return;
                        }
                    case ActionType.IncreasePkPoint:
                        {
                            if (int.TryParse(param[0], out var result85))
                            {
                                player.PK值惩罚 += result85;
                                break;
                            }
                            return;
                        }
                    case ActionType.ChangeGender:
                        {
                            if (Enum.TryParse<游戏对象性别>(param[0], ignoreCase: false, out var result79))
                            {
                                player.角色数据.角色性别.V = result79;
                            }
                            break;
                        }
                    case ActionType.ChangeClass:
                        {
                            if (Enum.TryParse<游戏对象职业>(param[0], ignoreCase: true, out var result78))
                            {
                                player.角色数据.角色职业.V = result78;
                            }
                            break;
                        }
                    case ActionType.LocalMessage:
                        {
                            if (!bool.TryParse(param[1], out var result52))
                            {
                                result52 = false;
                            }
                            player.发送系统消息(param[0], result52);
                            break;
                        }
                    case ActionType.TopMessage:
                        {
                            if (!bool.TryParse(param[1], out var result61))
                            {
                                result61 = false;
                            }
                            player.发送顶部公告(param[0], 全服通知: false, result61);
                            break;
                        }
                    case ActionType.Goto:
                        {
                            DelayedAction item5;
                            item5 = new DelayedAction(DelayedType.NPC, DateTime.MinValue, player.NPCObjectID, player.NPCScriptID, "[" + param[0] + "]");
                            player.ActionList.Add(item5);
                            break;
                        }
                    case ActionType.SysGoto:
                        {
                            DelayedAction item4;
                            item4 = new DelayedAction(DelayedType.NPC, DateTime.MinValue, 主程.DefaultNPC.ScriptID, 主程.DefaultNPC.ScriptID, "[" + param[0] + "]");
                            player.ActionList.Add(item4);
                            break;
                        }
                    case ActionType.PGoto:
                        {
                            玩家实例 value9;
                            value9 = null;
                            if (游戏数据网关.角色数据表.检索表.TryGetValue(param[0], out var value10) && value10 is 角色数据 角色数据2)
                            {
                                地图处理网关.玩家对象表.TryGetValue(角色数据2.角色编号, out value9);
                            }
                            if (value9 != null)
                            {
                                DelayedAction item3;
                                item3 = new DelayedAction(DelayedType.NPC, DateTime.MinValue, player.NPCObjectID, player.NPCObjectID, "[" + param[1] + "]");
                                value9.ActionList.Add(item3);
                            }
                            break;
                        }
                    case ActionType.Call:
                        {
                            if (int.TryParse(param[0], out var result31))
                            {
                                DelayedAction item2;
                                item2 = new DelayedAction(DelayedType.NPC, DateTime.MinValue, player.NPCObjectID, result31, "[@MAIN]");
                                player.ActionList.Add(item2);
                                break;
                            }
                            return;
                        }
                    case ActionType.AddSkill:
                        {
                            if (byte.TryParse(param[1], out var result8) && ushort.TryParse(param[0], out var result9) && 铭文技能.数据表.TryGetValue((ushort)(result9 * 10), out var value3) && value3 != null)
                            {
                                player.玩家学习技能(value3.技能编号, result8);
                                break;
                            }
                            return;
                        }
                    case ActionType.RemoveSkill:
                        {
                            if (!ushort.TryParse(param[0], out var result144))
                            {
                                if (param[0] == "*")
                                {
                                    player.主体技能表.Clear();
                                }
                            }
                            else
                            {
                                player.玩家移除技能(result144);
                            }
                            break;
                        }
                    case ActionType.ChangeData:
                        {
                            if (!int.TryParse(param[1], out var result127))
                            {
                                result127 = 0;
                            }
                            nPCActions.F.赋值(player, param[0], param[1], result127);
                            break;
                        }
                    case ActionType.MarfaValue:
                        {
                            if (!int.TryParse(param[1], out var result100))
                            {
                                result100 = 31;
                            }
                            if (!int.TryParse(param[2], out var result101))
                            {
                                result101 = 0;
                            }
                            if (byte.TryParse(param[0], out var result102))
                            {
                                player.获得玛法特权(result102, result100, result101 == 1);
                            }
                            break;
                        }
                    case ActionType.Movr:
                        {
                            if (int.TryParse(param[1], out var result92))
                            {
                                int num26;
                                num26 = 0;
                                num26 = (int.TryParse(param[2], out var result93) ? 主程.随机数.Next(result92, result93) : 主程.随机数.Next(0, result92));
                                this.AddVariable(player, param[0], num26.ToString());
                            }
                            break;
                        }
                    case ActionType.ChangeNode:
                        {
                            if (ushort.TryParse(param[0], out var result81) && result81 != 0 && ushort.TryParse(param[2], out var result82) && result81 != 0)
                            {
                                ushort 值;
                                值 = (ushort)NPCSegment.Calculate(param[1], player.获取玩家节点(result81), result82);
                                player.更改玩家节点(result81, 值);
                            }
                            break;
                        }
                    case ActionType.OpenItemBox:
                        {
                            if (!int.TryParse(param[1], out var result64))
                            {
                                break;
                            }
                            try
                            {
                                地图实例 地图实例5;
                                if (游戏怪物.数据表.TryGetValue(param[0], out var value20))
                                {
                                    地图实例5 = null;
                                    if (param[2] == "*")
                                    {
                                        地图实例5 = player.当前地图;
                                        goto IL_1f54;
                                    }
                                    if (byte.TryParse(param[2], out var result65) && 游戏地图.数据表.TryGetValue(result65, out var value21))
                                    {
                                        地图实例5 = 地图处理网关.已分配地图(value21.地图编号);
                                        goto IL_1f54;
                                    }
                                }
                                goto end_IL_1ed1;
                            IL_1f54:
                                if (!int.TryParse(param[3], out var result66))
                                {
                                    result66 = -1;
                                }
                                if (!int.TryParse(param[4], out var result67))
                                {
                                    result67 = -1;
                                }
                                Point[] array8;
                                array8 = new Point[1]
                                {
                            new Point(result66, result67)
                                };
                                if (result66 < 0)
                                {
                                    array8[0] = 计算类.前方坐标(player.当前坐标, player.当前方向, 1);
                                    goto IL_1fe9;
                                }
                                if (result67 < 0)
                                {
                                    break;
                                }
                                array8 = new Point[1]
                                {
                            new Point(result66, result67)
                                };
                                goto IL_1fe9;
                            IL_1fe9:
                                for (int num18 = 0; num18 < result64; num18++)
                                {
                                    怪物实例 obj;
                                    obj = new 怪物实例(value20, 地图实例5, int.MaxValue, array8, 禁止复活: true, 立即刷新: true);
                                    obj.存活时间 = DateTime.MinValue;
                                    obj.自身死亡处理(player, 技能击杀: false, 脚本击杀: true);
                                }
                            end_IL_1ed1:;
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到怪物[" + param[0] + "]");
                            }
                            break;
                        }
                    case ActionType.Set:
                        {
                            if (int.TryParse(param[0], out var result48) && uint.TryParse(param[1], out var result49) && result48 >= 0 && result48 < 1999)
                            {
                                bool value17;
                                value17 = Convert.ToBoolean(result49);
                                player.角色数据.任务标识[result48] = value17;
                                break;
                            }
                            return;
                        }
                    case ActionType.Param1:
                        {
                            if (int.TryParse(param[1], out var result45))
                            {
                                this.Param1 = param[0];
                                this.Param1Instance = result45;
                                break;
                            }
                            return;
                        }
                    case ActionType.Param2:
                        {
                            if (int.TryParse(param[0], out var result50))
                            {
                                this.Param2 = result50;
                                break;
                            }
                            return;
                        }
                    case ActionType.Param3:
                        {
                            if (int.TryParse(param[0], out var result43))
                            {
                                this.Param3 = result43;
                                break;
                            }
                            return;
                        }
                    case ActionType.Mongen:
                        {
                            if (this.Param1 == null || this.Param2 == 0 || this.Param3 == 0 || !int.TryParse(param[1], out var result42))
                            {
                                return;
                            }
                            try
                            {
                                if (!游戏怪物.数据表.TryGetValue(param[0], out var value13) || !游戏地图.数据表.TryGetValue((byte)this.Param1Instance, out var value14))
                                {
                                    return;
                                }
                                地图实例 地图实例4;
                                地图实例4 = 地图处理网关.已分配地图(value14.地图编号);
                                Point[] 出生范围2;
                                出生范围2 = new Point[1]
                                {
                            new Point(this.Param2, this.Param3)
                                };
                                if (param[2] != "")
                                {
                                    List<怪物刷新> list3;
                                    list3 = 地图实例4.获取怪物区域();
                                    for (int num11 = 0; num11 < list3.Count; num11++)
                                    {
                                        怪物刷新 怪物刷新2;
                                        怪物刷新2 = list3[i];
                                        if (怪物刷新2.区域名字 == param[2])
                                        {
                                            出生范围2 = 怪物刷新2.获取刷新范围();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int num12 = 0; num12 < result42; num12++)
                                    {
                                        new 怪物实例(value13, 地图实例4, int.MaxValue, 出生范围2, 禁止复活: true, 立即刷新: true).存活时间 = DateTime.MaxValue;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到怪物[" + param[0] + "]");
                            }
                            break;
                        }
                    case ActionType.MongenEx:
                        {
                            if (!int.TryParse(param[4], out var result23) || result23 < 1)
                            {
                                break;
                            }
                            try
                            {
                                if (!游戏怪物.数据表.TryGetValue(param[3], out var value5))
                                {
                                    return;
                                }
                                if (!byte.TryParse(param[0], out var result24))
                                {
                                    break;
                                }
                                if (!游戏地图.数据表.TryGetValue(result24, out var value6))
                                {
                                    return;
                                }
                                地图实例 地图实例3;
                                地图实例3 = 地图处理网关.已分配地图(value6.地图编号);
                                Point[] 出生范围;
                                出生范围 = new Point[1]
                                {
                            new Point(player.当前坐标.X, player.当前坐标.Y)
                                };
                                if (!int.TryParse(param[1], out var result25))
                                {
                                    List<怪物刷新> list2;
                                    list2 = 地图实例3.获取怪物区域();
                                    for (int num6 = 0; num6 < list2.Count; num6++)
                                    {
                                        怪物刷新 怪物刷新;
                                        怪物刷新 = list2[num6];
                                        if (怪物刷新.区域名字 == param[2] || param[2] == "*")
                                        {
                                            出生范围 = 怪物刷新.获取刷新范围();
                                            break;
                                        }
                                    }
                                    goto IL_2412;
                                }
                                if (!int.TryParse(param[2], out var result26))
                                {
                                    break;
                                }
                                出生范围 = new Point[1]
                                {
                            new Point(result25, result26)
                                };
                                goto IL_2412;
                            IL_2412:
                                if (!byte.TryParse(param[5], out var result27))
                                {
                                    result27 = 0;
                                }
                                if (!byte.TryParse(param[6], out var result28))
                                {
                                    result28 = 0;
                                }
                                for (int num7 = 0; num7 < result23; num7++)
                                {
                                    new 怪物实例(value5, 地图实例3, int.MaxValue, 出生范围, 禁止复活: true, 立即刷新: true)
                                    {
                                        存活时间 = ((result28 == 0) ? DateTime.MaxValue : DateTime.Now.AddSeconds((int)result28)),
                                        禁止复活 = true,
                                        无归属 = result27
                                    };
                                }
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到怪物[" + param[0] + "]");
                            }
                            break;
                        }
                    case ActionType.NpcGen:
                        {
                            if (!byte.TryParse(param[0], out var result155))
                            {
                                result155 = 0;
                            }
                            if (!int.TryParse(param[1], out var result156) || !int.TryParse(param[2], out var result157) || !ushort.TryParse(param[3], out var result158))
                            {
                                return;
                            }
                            if (!int.TryParse(param[4], out var result159))
                            {
                                result159 = int.MaxValue;
                            }
                            try
                            {
                                if (!地图守卫.数据表.TryGetValue(result158, out var value42) || (!游戏地图.数据表.TryGetValue(result155, out var value43) && result155 != 0))
                                {
                                    return;
                                }
                                new 守卫实例(出生地图: (result155 == 0) ? player.当前地图 : 地图处理网关.已分配地图(value43.地图编号), 对应模板: value42, 出生方向: 游戏方向.上方, 出生坐标: new Point(result156, result157)).存活时间 = 主程.当前时间.AddSeconds(result159);
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到地图[" + param[0] + "]");
                            }
                            break;
                        }
                    case ActionType.NpcGenEx:
                        {
                            if (!ushort.TryParse(param[0], out var result140) || result140 < 1 || !int.TryParse(param[1], out var result141) || result141 < 1)
                            {
                                break;
                            }
                            try
                            {
                                if (NPCSegment.脚本刷出的守卫 == null)
                                {
                                    NPCSegment.脚本刷出的守卫 = new Dictionary<int, 守卫实例>();
                                }
                                地图实例 地图实例10;
                                if (!NPCSegment.脚本刷出的守卫.ContainsKey(result141) && 地图守卫.数据表.TryGetValue(result140, out var value36))
                                {
                                    地图实例10 = null;
                                    if (!byte.TryParse(param[2], out var result142) && param[2] == "*")
                                    {
                                        地图实例10 = player.当前地图;
                                        goto IL_26c2;
                                    }
                                    if (游戏地图.数据表.TryGetValue(result142, out var value37))
                                    {
                                        地图实例10 = 地图处理网关.已分配地图(value37.地图编号);
                                        goto IL_26c2;
                                    }
                                }
                                goto end_IL_2626;
                            IL_26c2:
                                List<Point> list4;
                                list4 = new List<Point>();
                                if (param[3] == "*")
                                {
                                    list4.Add(计算类.前方坐标(player.当前坐标, player.当前方向, 1));
                                }
                                else
                                {
                                    string[] array2;
                                    array2 = param[3].Split("|");
                                    for (int k = 0; k < array2.Length; k++)
                                    {
                                        string[] array11;
                                        array11 = array2[k].Split(',');
                                        if (array11.Length >= 2)
                                        {
                                            list4.Add(new Point(int.Parse(array11[0]), int.Parse(array11[1])));
                                        }
                                    }
                                }
                                Point 出生坐标;
                                出生坐标 = list4[主程.随机数.Next(0, list4.Count)];
                                if (!int.TryParse(param[4], out var result143))
                                {
                                    result143 = 3;
                                }
                                守卫实例 value38;
                                value38 = new 守卫实例(value36, 地图实例10, 游戏方向.上方, 出生坐标)
                                {
                                    存活时间 = 主程.当前时间.AddSeconds(result143)
                                };
                                NPCSegment.脚本刷出的守卫.Add(result141, value38);
                            end_IL_2626:;
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到守卫[" + param[0] + "]");
                            }
                            break;
                        }
                    case ActionType.DelNpcGen:
                        {
                            if (!int.TryParse(param[0], out var result125) || result125 < 1)
                            {
                                break;
                            }
                            try
                            {
                                if (NPCSegment.脚本刷出的守卫 == null)
                                {
                                    NPCSegment.脚本刷出的守卫 = new Dictionary<int, 守卫实例>();
                                }
                                if (NPCSegment.脚本刷出的守卫.TryGetValue(result125, out var value35))
                                {
                                    NPCSegment.脚本刷出的守卫.Remove(result125);
                                    value35.存活时间 = DateTime.MinValue;
                                }
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到守卫[" + param[0] + "]");
                            }
                            break;
                        }
                    case ActionType.SetBagSize:
                        {
                            if (byte.TryParse(param[1], out var result94))
                            {
                                int num27;
                                num27 = NPCSegment.Calculate(param[0], player.背包大小, result94);
                                player.背包大小 = (byte)((num27 >= 0) ? ((uint)((num27 > 64) ? 64 : ((byte)num27))) : 0u);
                                player.网络连接?.发送封包(new 背包容量改变
                                {
                                    背包类型 = 1,
                                    背包容量 = player.背包大小
                                });
                            }
                            break;
                        }
                    case ActionType.TimeRecall:
                        {
                            string text9;
                            text9 = "";
                            if (long.TryParse(param[0], out var result105))
                            {
                                if (param[1].Length > 0)
                                {
                                    text9 = "[" + param[1] + "]";
                                }
                                地图实例 当前地图;
                                当前地图 = player.当前地图;
                                Point 当前坐标;
                                当前坐标 = player.当前坐标;
                                DelayedAction item8;
                                item8 = new DelayedAction(DelayedType.NPC, 主程.当前时间.AddMilliseconds(result105 * 1000L), player.NPCObjectID, player.NPCScriptID, text9, 当前地图, 当前坐标);
                                player.ActionList.Add(item8);
                                break;
                            }
                            return;
                        }
                    case ActionType.TimeRecallGroup:
                        {
                            string text6;
                            text6 = "";
                            if (player.所属队伍 == null || !long.TryParse(param[0], out var result84))
                            {
                                return;
                            }
                            if (param[1].Length > 0)
                            {
                                text6 = "[" + param[1] + "]";
                            }
                            foreach (角色数据 item11 in player.所属队伍.队伍成员)
                            {
                                DelayedAction item7;
                                item7 = new DelayedAction(DelayedType.NPC, 主程.当前时间.AddMilliseconds(result84 * 1000L), player.NPCObjectID, player.NPCScriptID, text6, player.当前地图, player.当前坐标);
                                item11.网络连接?.绑定角色?.ActionList.Add(item7);
                            }
                            break;
                        }
                    case ActionType.BreakTimeRecall:
                        foreach (DelayedAction item12 in player.ActionList.Where((DelayedAction u) => u.Type == DelayedType.NPC))
                        {
                            item12.FlaggedToRemove = true;
                        }
                        break;
                    case ActionType.DelayGoto:
                        {
                            if (long.TryParse(param[0], out var result68))
                            {
                                DelayedAction item6;
                                item6 = new DelayedAction(DelayedType.NPC, 主程.当前时间.AddMilliseconds(result68 * 1000L), player.NPCObjectID, player.NPCScriptID, "[" + param[1] + "]");
                                player.ActionList.Add(item6);
                                break;
                            }
                            return;
                        }
                    case ActionType.Mov:
                        this.AddVariable(key: param[0], mapobject: player, value: param[1]);
                        break;
                    case ActionType.Calc:
                        {
                            if (int.TryParse(param[0], out var result58) & int.TryParse(param[2], out var result59))
                            {
                                try
                                {
                                    int num14;
                                    num14 = NPCSegment.Calculate(param[1], result58, result59);
                                    this.AddVariable(player, param[3].Replace("-", ""), num14.ToString());
                                }
                                catch (ArgumentException)
                                {
                                    主程.添加系统日志($"不正确的操作员: {param[1]}, Page: {this.Key}");
                                }
                            }
                            else
                            {
                                this.AddVariable(player, param[3].Replace("-", ""), param[0] + param[2]);
                            }
                            break;
                        }
                    case ActionType.GiveBuff:
                        {
                            ushort.TryParse(param[0], out var result55);
                            player.添加Buff时处理(result55, player);
                            break;
                        }
                    case ActionType.RemoveBuff:
                        {
                            ushort.TryParse(param[0], out var result60);
                            player.移除Buff时处理(result60);
                            break;
                        }
                    case ActionType.SendMail:
                        {
                            if (!int.TryParse(param[3], out var result39))
                            {
                                result39 = -1;
                            }
                            if (!int.TryParse(param[4], out var result40))
                            {
                                result40 = 1;
                            }
                            if (!int.TryParse(param[5], out var result41))
                            {
                                result41 = 0;
                            }
                            角色数据 角色数据3;
                            角色数据3 = null;
                            if (param[0] == "*")
                            {
                                角色数据3 = player.角色数据;
                            }
                            else
                            {
                                if (!游戏数据网关.角色数据表.检索表.TryGetValue(param[0], out var value12) || !(value12 is 角色数据 角色数据4))
                                {
                                    break;
                                }
                                角色数据3 = 角色数据4;
                            }
                            角色数据3?.发送邮件(null, param[1], param[2], result39, result40, result41 == 1);
                            break;
                        }
                    case ActionType.STOP:
                        if (player.探索道具 != null)
                        {
                            player.探索道具.ScriptOp = true;
                        }
                        break;
                    case ActionType.BoxItemRate:
                        {
                            if (!int.TryParse(param[0], out var result36))
                            {
                                result36 = -1;
                            }
                            if (player.探索道具 != null)
                            {
                                player.探索道具.Rate = result36;
                            }
                            break;
                        }
                    case ActionType.AutoPickCfg:
                        {
                            if (!byte.TryParse(param[0], out var result29))
                            {
                                result29 = 0;
                            }
                            if (!int.TryParse(param[1], out var result30))
                            {
                                result30 = 1000;
                            }
                            player.自动拾取范围 = result29;
                            player.自动拾取间隔 = result30;
                            break;
                        }
                    case ActionType.OpenBoxNeedTimeMSec:
                        {
                            if (!int.TryParse(param[0], out var result10))
                            {
                                result10 = -1;
                            }
                            if (result10 >= 0 && player.探索道具 != null)
                            {
                                player.探索道具.TimeSec = result10;
                            }
                            break;
                        }
                    case ActionType.GroupGoto:
                        if (player.所属队伍 == null)
                        {
                            return;
                        }
                        foreach (角色数据 item13 in player.所属队伍.队伍成员)
                        {
                            DelayedAction item;
                            item = new DelayedAction(DelayedType.NPC, 主程.当前时间, player.NPCObjectID, player.NPCScriptID, "[" + param[0] + "]");
                            item13.网络连接?.绑定角色?.ActionList.Add(item);
                        }
                        break;
                    case ActionType.GlobalMessage:
                        {
                            if (!bool.TryParse(param[1], out var result148))
                            {
                                result148 = false;
                            }
                            网络服务网关.发送公告(param[0], result148);
                            break;
                        }
                    case ActionType.LoadValue:
                        {
                            string key12;
                            key12 = param[0];
                            string fileName2;
                            fileName2 = param[1];
                            string section2;
                            section2 = param[2];
                            string key13;
                            key13 = param[3];
                            string text13;
                            text13 = new InIReader(fileName2).ReadString(section2, key13, "");
                            if (!(text13 == ""))
                            {
                                this.AddVariable(player, key12, text13);
                            }
                            break;
                        }
                    case ActionType.SaveValue:
                        {
                            string fileName;
                            fileName = param[0];
                            string section;
                            section = param[1];
                            string key11;
                            key11 = param[2];
                            string value40;
                            value40 = param[3];
                            new InIReader(fileName).Write(section, key11, value40);
                            break;
                        }
                    case ActionType.Break:
                        this.Page.BreakFromSegments = true;
                        break;
                    case ActionType.GetRandomText:
                        {
                            string text12;
                            text12 = Path.Combine(Settings.NPCPath, param[0]);
                            if (!File.Exists(text12))
                            {
                                主程.添加系统日志($"随机文本文件:{text12} does not exist.");
                            }
                            else
                            {
                                string[] array10;
                                array10 = File.ReadAllLines(text12);
                                this.AddVariable(value: array10[(!int.TryParse(param[2], out var result124) || result124 <= -1) ? 主程.随机数.Next(0, array10.Length) : result124], mapobject: player, key: param[1]);
                            }
                            break;
                        }
                    case ActionType.GiveTitle:
                        {
                            if (byte.TryParse(param[0], out var result122) && int.TryParse(param[1], out var result123))
                            {
                                player.玩家获得称号(result122, result123);
                            }
                            break;
                        }
                    case ActionType.UseTitle:
                        {
                            if (byte.TryParse(param[0], out var result113))
                            {
                                player.玩家使用称号(result113);
                            }
                            break;
                        }
                    case ActionType.DelTitle:
                        {
                            if (byte.TryParse(param[0], out var result111))
                            {
                                player.玩家称号到期(result111);
                            }
                            break;
                        }
                    case ActionType.RandomGiveItem:
                        {
                            string[] array7;
                            array7 = param[0].Split(',');
                            if (array7.Length == 0)
                            {
                                break;
                            }
                            if (param.Count < 2 || !int.TryParse(param[1], out var result62))
                            {
                                result62 = 1;
                            }
                            if (param.Count < 3 || !bool.TryParse(param[2], out var result63))
                            {
                                result63 = false;
                            }
                            string text4;
                            text4 = param[3];
                            物品数据 物品数据5;
                            物品数据5 = null;
                            int num15;
                            num15 = 0;
                            string key3;
                            key3 = array7[主程.随机数.Next(array7.Length)];
                            if (!游戏物品.检索表.TryGetValue(key3, out var value19) || value19.物品持久 == 0)
                            {
                                break;
                            }
                            int num16;
                            num16 = Math.Max(1, (value19.持久类型 == 物品持久分类.堆叠) ? 1 : result62);
                            if (player.角色数据.角色背包.Count + num16 > player.角色数据.背包大小.V)
                            {
                                break;
                            }
                            if (value19.持久类型 == 物品持久分类.堆叠)
                            {
                                byte b9;
                                b9 = byte.MaxValue;
                                byte b10;
                                b10 = 0;
                                while (b10 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色数据.角色背包.ContainsKey(b10))
                                    {
                                        b10++;
                                        continue;
                                    }
                                    b9 = b10;
                                    break;
                                }
                                player.角色数据.角色背包[b9] = new 物品数据(value19, player.角色数据, 1, b9, result62, result63, player.对象名字 + "-RandomGiveItem");
                                物品数据5 = player.角色数据.角色背包[b9];
                                num15 += result62;
                                player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = player.角色数据.角色背包[b9].字节描述()
                                });
                            }
                            else
                            {
                                for (int num17 = 0; num17 < num16; num17++)
                                {
                                    byte b11;
                                    b11 = byte.MaxValue;
                                    byte b12;
                                    b12 = 0;
                                    while (b12 < player.角色数据.背包大小.V)
                                    {
                                        if (player.角色数据.角色背包.ContainsKey(b12))
                                        {
                                            b12++;
                                            continue;
                                        }
                                        b11 = b12;
                                        break;
                                    }
                                    if (value19 is 游戏装备 模板3)
                                    {
                                        player.角色数据.角色背包[b11] = new 装备数据(模板3, player.角色数据, 1, b11, 随机生成: false, result63, player.对象名字 + "-RandomGiveItem");
                                    }
                                    else if (value19.持久类型 == 物品持久分类.容器)
                                    {
                                        player.角色数据.角色背包[b11] = new 物品数据(value19, player.角色数据, 1, b11, 0, result63, player.对象名字 + "-RandomGiveItem");
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b11] = new 物品数据(value19, player.角色数据, 1, b11, value19.物品持久, result63, player.对象名字 + "-RandomGiveItem");
                                    }
                                    物品数据5 = player.角色数据.角色背包[b11];
                                    num15++;
                                    if (player.角色数据.角色背包[b11].物品类型 == 物品使用分类.经验容器)
                                    {
                                        player.角色数据.角色背包[b11].当前持久.V = 0;
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b11].当前持久.V = player.角色数据.角色背包[b11].最大持久.V;
                                    }
                                    player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                    {
                                        物品描述 = player.角色数据.角色背包[b11].字节描述()
                                    });
                                }
                            }
                            if (物品数据5 != null && text4 != "")
                            {
                                主程.添加物品日志(player, text4, 物品数据5, num15, nPCActions.Type.ToString());
                            }
                            break;
                        }
                    case ActionType.RandomGiveItemIdx:
                        {
                            string[] array3;
                            array3 = param[0].Split(',');
                            if (array3.Length == 0 || !int.TryParse(array3[主程.随机数.Next(array3.Length)], out var result32))
                            {
                                break;
                            }
                            if (param.Count < 2 || !int.TryParse(param[1], out var result33))
                            {
                                result33 = 1;
                            }
                            if (param.Count < 3 || !bool.TryParse(param[2], out var result34))
                            {
                                result34 = false;
                            }
                            string text3;
                            text3 = param[3];
                            物品数据 物品数据2;
                            物品数据2 = null;
                            if (result32 == 0 || !游戏物品.数据表.TryGetValue(result32, out var value7) || value7.物品持久 == 0)
                            {
                                break;
                            }
                            int num8;
                            num8 = Math.Max(1, (value7.持久类型 == 物品持久分类.堆叠) ? 1 : result33);
                            if (player.角色数据.角色背包.Count + num8 > player.角色数据.背包大小.V)
                            {
                                break;
                            }
                            if (value7.持久类型 == 物品持久分类.堆叠)
                            {
                                byte b5;
                                b5 = byte.MaxValue;
                                byte b6;
                                b6 = 0;
                                while (b6 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色数据.角色背包.ContainsKey(b6))
                                    {
                                        b6++;
                                        continue;
                                    }
                                    b5 = b6;
                                    break;
                                }
                                player.角色数据.角色背包[b5] = new 物品数据(value7, player.角色数据, 1, b5, result33, result34, player.对象名字 + "-RandomGiveItemIdx");
                                物品数据2 = player.角色数据.角色背包[b5];
                                player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = player.角色数据.角色背包[b5].字节描述()
                                });
                            }
                            else
                            {
                                for (int num9 = 0; num9 < num8; num9++)
                                {
                                    byte b7;
                                    b7 = byte.MaxValue;
                                    byte b8;
                                    b8 = 0;
                                    while (b8 < player.角色数据.背包大小.V)
                                    {
                                        if (player.角色数据.角色背包.ContainsKey(b8))
                                        {
                                            b8++;
                                            continue;
                                        }
                                        b7 = b8;
                                        break;
                                    }
                                    if (value7 is 游戏装备 模板2)
                                    {
                                        player.角色数据.角色背包[b7] = new 装备数据(模板2, player.角色数据, 1, b7, 随机生成: true, result34, player.对象名字 + "-RandomGiveItemIdx");
                                    }
                                    else if (value7.持久类型 == 物品持久分类.容器)
                                    {
                                        player.角色数据.角色背包[b7] = new 物品数据(value7, player.角色数据, 1, b7, 0, result34, player.对象名字 + "-RandomGiveItemIdx");
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b7] = new 物品数据(value7, player.角色数据, 1, b7, value7.物品持久, result34, player.对象名字 + "-RandomGiveItemIdx");
                                    }
                                    物品数据2 = player.角色数据.角色背包[b7];
                                    if (player.角色数据.角色背包[b7].物品类型 == 物品使用分类.经验容器)
                                    {
                                        player.角色数据.角色背包[b7].当前持久.V = 0;
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b7].当前持久.V = player.角色数据.角色背包[b7].最大持久.V;
                                    }
                                    player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                    {
                                        物品描述 = player.角色数据.角色背包[b7].字节描述()
                                    });
                                }
                            }
                            if (物品数据2 != null && text3 != "")
                            {
                                主程.添加物品日志(player, text3, 物品数据2, num8, nPCActions.Type.ToString());
                            }
                            系统数据.数据.脚本数字[1000] = result32;
                            break;
                        }
                    case ActionType.GiveItemIdx:
                        {
                            if (!int.TryParse(param[0], out var result118))
                            {
                                return;
                            }
                            if (param.Count < 2 || !int.TryParse(param[1], out var result119))
                            {
                                result119 = 1;
                            }
                            if (param.Count < 3 || !bool.TryParse(param[2], out var result120))
                            {
                                result120 = false;
                            }
                            if (param.Count < 4 || !int.TryParse(param[3], out var result121))
                            {
                                result121 = 0;
                            }
                            string text11;
                            text11 = param[4];
                            物品数据 物品数据13;
                            物品数据13 = null;
                            if (result118 == 0 || !游戏物品.数据表.TryGetValue(result118, out var value32) || value32.物品持久 == 0)
                            {
                                return;
                            }
                            int num33;
                            num33 = Math.Max(1, (value32.持久类型 == 物品持久分类.堆叠) ? 1 : result119);
                            if (player.角色数据.角色背包.Count + num33 > player.角色数据.背包大小.V)
                            {
                                return;
                            }
                            if (value32.持久类型 == 物品持久分类.堆叠)
                            {
                                byte b23;
                                b23 = byte.MaxValue;
                                byte b24;
                                b24 = 0;
                                while (b24 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色数据.角色背包.ContainsKey(b24))
                                    {
                                        b24++;
                                        continue;
                                    }
                                    b23 = b24;
                                    break;
                                }
                                player.角色数据.角色背包[b23] = new 物品数据(value32, player.角色数据, 1, b23, result119, result120, player.对象名字 + "-GiveItemIdx");
                                物品数据13 = player.角色数据.角色背包[b23];
                                player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = player.角色数据.角色背包[b23].字节描述()
                                });
                            }
                            else
                            {
                                for (int num34 = 0; num34 < num33; num34++)
                                {
                                    byte b25;
                                    b25 = byte.MaxValue;
                                    byte b26;
                                    b26 = 0;
                                    while (b26 < player.角色数据.背包大小.V)
                                    {
                                        if (player.角色数据.角色背包.ContainsKey(b26))
                                        {
                                            b26++;
                                            continue;
                                        }
                                        b25 = b26;
                                        break;
                                    }
                                    if (value32 is 游戏装备 模板4)
                                    {
                                        装备数据 装备数据6;
                                        装备数据6 = new 装备数据(模板4, player.角色数据, 1, b25, 随机生成: false, result120, player.对象名字 + "-GiveItemIdx");
                                        if (装备数据6.随机属性.Count <= 3 && 随机属性.数据表.TryGetValue(result121, out var value33))
                                        {
                                            装备数据6.随机属性.Add(value33);
                                        }
                                        player.角色数据.角色背包[b25] = 装备数据6;
                                    }
                                    else if (value32.持久类型 == 物品持久分类.容器)
                                    {
                                        player.角色数据.角色背包[b25] = new 物品数据(value32, player.角色数据, 1, b25, 0, result120, player.对象名字 + "-GiveItemIdx");
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b25] = new 物品数据(value32, player.角色数据, 1, b25, value32.物品持久, result120, player.对象名字 + "-GiveItemIdx");
                                    }
                                    物品数据13 = player.角色数据.角色背包[b25];
                                    if (player.角色数据.角色背包[b25].物品类型 == 物品使用分类.经验容器)
                                    {
                                        player.角色数据.角色背包[b25].当前持久.V = 0;
                                    }
                                    else
                                    {
                                        player.角色数据.角色背包[b25].当前持久.V = player.角色数据.角色背包[b25].最大持久.V;
                                    }
                                    player.角色数据.网络连接?.发送封包(new 玩家物品变动
                                    {
                                        物品描述 = player.角色数据.角色背包[b25].字节描述()
                                    });
                                }
                            }
                            if (物品数据13 != null && text11 != "")
                            {
                                主程.添加物品日志(player, text11, 物品数据13, num33, nPCActions.Type.ToString());
                            }
                            break;
                        }
                    case ActionType.TakeItemIdx:
                        {
                            if (!int.TryParse(param[0], out var result98))
                            {
                                break;
                            }
                            if (param.Count < 2 || !int.TryParse(param[1], out var result99))
                            {
                                result99 = 1;
                            }
                            if (result98 == 0 || !游戏物品.数据表.TryGetValue(result98, out var value28) || value28.物品持久 == 0)
                            {
                                break;
                            }
                            string text8;
                            text8 = param[2];
                            物品数据 物品数据10;
                            物品数据10 = null;
                            int num28;
                            num28 = 0;
                            int num29;
                            num29 = result99;
                            if (value28.持久类型 == 物品持久分类.堆叠)
                            {
                                byte b19;
                                b19 = byte.MaxValue;
                                byte b20;
                                b20 = 0;
                                while (b20 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色背包.ContainsKey(b20) && player.角色背包[b20].物品编号 == result98)
                                    {
                                        物品数据 物品数据11;
                                        物品数据11 = player.角色背包[b20];
                                        物品数据10 = 物品数据11;
                                        if (num29 < 物品数据11.当前持久.V)
                                        {
                                            物品数据11.当前持久.V -= num29;
                                            num28 += 物品数据11.当前持久.V;
                                            player.网络连接?.发送封包(new 玩家物品变动
                                            {
                                                物品描述 = 物品数据11.字节描述()
                                            });
                                            break;
                                        }
                                        player.网络连接?.发送封包(new 删除玩家物品
                                        {
                                            背包类型 = 物品数据11.物品容器.V,
                                            物品位置 = 物品数据11.物品位置.V
                                        });
                                        player.角色背包.Remove(物品数据11.物品位置.V);
                                        物品数据11.删除数据();
                                        num29 -= 物品数据11.当前持久.V;
                                        num28 += 物品数据11.当前持久.V;
                                    }
                                    else
                                    {
                                        b20++;
                                    }
                                }
                                break;
                            }
                            for (int num30 = 0; num30 < num29; num30++)
                            {
                                byte b21;
                                b21 = byte.MaxValue;
                                byte b22;
                                b22 = 0;
                                while (b22 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色数据.角色背包.ContainsKey(b22) && player.角色背包[b22].物品编号 == result98)
                                    {
                                        物品数据 物品数据12;
                                        物品数据12 = player.角色背包[b22];
                                        物品数据10 = 物品数据12;
                                        player.网络连接?.发送封包(new 删除玩家物品
                                        {
                                            背包类型 = 物品数据12.物品容器.V,
                                            物品位置 = 物品数据12.物品位置.V
                                        });
                                        player.角色背包.Remove(物品数据12.物品位置.V);
                                        物品数据12.删除数据();
                                        num29--;
                                        num28++;
                                        if (num29 == 0)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        b22++;
                                    }
                                }
                            }
                            if (物品数据10 != null && text8 != "")
                            {
                                主程.添加物品日志(player, text8, 物品数据10, num28, nPCActions.Type.ToString());
                            }
                            break;
                        }
                    case ActionType.TakeItemLuckIdx:
                        {
                            if (!int.TryParse(param[0], out var result75))
                            {
                                return;
                            }
                            if (!int.TryParse(param[1], out var result76))
                            {
                                result76 = 1;
                            }
                            if (!int.TryParse(param[2], out var result77))
                            {
                                result77 = 0;
                            }
                            if (result75 == 0 || !游戏物品.数据表.TryGetValue(result75, out var value23) || value23.物品持久 == 0)
                            {
                                return;
                            }
                            int num19;
                            num19 = result76;
                            for (int num20 = 0; num20 < num19; num20++)
                            {
                                byte b13;
                                b13 = byte.MaxValue;
                                byte b14;
                                b14 = 0;
                                while (b14 < player.角色数据.背包大小.V)
                                {
                                    if (player.角色数据.角色背包.ContainsKey(b14) && player.角色背包[b14].物品编号 == result75 && player.GetItemValue(player.角色背包[b14], 9) >= result77)
                                    {
                                        物品数据 物品数据6;
                                        物品数据6 = player.角色背包[b14];
                                        player.网络连接?.发送封包(new 删除玩家物品
                                        {
                                            背包类型 = 物品数据6.物品容器.V,
                                            物品位置 = 物品数据6.物品位置.V
                                        });
                                        player.角色背包.Remove(物品数据6.物品位置.V);
                                        物品数据6.删除数据();
                                        num19--;
                                        if (num19 == 0)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        b14++;
                                    }
                                }
                            }
                            break;
                        }
                    case ActionType.ChangeItemValue:
                        {
                            if (!byte.TryParse(param[0], out var result69) || !byte.TryParse(param[1], out var result70) || !byte.TryParse(param[2], out var result71) || !int.TryParse(param[3], out var result72))
                            {
                                return;
                            }
                            物品数据 v4;
                            v4 = null;
                            装备数据 v5;
                            v5 = null;
                            switch (result69)
                            {
                                default:
                                    return;
                                case 7:
                                    if (!player.角色资源包.TryGetValue(result70, out v4))
                                    {
                                        return;
                                    }
                                    goto IL_494e;
                                case 0:
                                    if (!player.角色装备.TryGetValue(result70, out v5))
                                    {
                                        return;
                                    }
                                    goto IL_494e;
                                case 1:
                                    if (!player.角色背包.TryGetValue(result70, out v4))
                                    {
                                        return;
                                    }
                                    goto IL_494e;
                                case 2:
                                    {
                                        if (!player.角色仓库.TryGetValue(result70, out v4))
                                        {
                                            return;
                                        }
                                        goto IL_494e;
                                    }
                                IL_494e:
                                    if (v4 == null && v5 != null)
                                    {
                                        v4 = v5;
                                    }
                                    if (v4 is 装备数据 装备数据4)
                                    {
                                        switch (result71)
                                        {
                                            default:
                                                return;
                                            case 0:
                                                装备数据4.当前持久.V = result72;
                                                break;
                                            case 1:
                                                装备数据4.升级次数.V = (byte)result72;
                                                break;
                                            case 2:
                                                装备数据4.升级攻击.V = (byte)result72;
                                                break;
                                            case 3:
                                                装备数据4.升级魔法.V = (byte)result72;
                                                break;
                                            case 4:
                                                装备数据4.升级道术.V = (byte)result72;
                                                break;
                                            case 5:
                                                装备数据4.升级刺术.V = (byte)result72;
                                                break;
                                            case 6:
                                                装备数据4.升级弓术.V = (byte)result72;
                                                break;
                                            case 7:
                                                装备数据4.灵魂绑定.V = result72 != 0;
                                                break;
                                            case 8:
                                                装备数据4.祈祷次数.V = (byte)result72;
                                                break;
                                            case 9:
                                                装备数据4.幸运等级.V = (sbyte)result72;
                                                break;
                                            case 10:
                                                装备数据4.装备神佑.V = result72 != 0;
                                                break;
                                            case 11:
                                                装备数据4.神圣伤害.V = (byte)result72;
                                                break;
                                            case 12:
                                                装备数据4.圣石数量.V = (byte)result72;
                                                break;
                                            case 13:
                                                装备数据4.双铭文栏.V = result72 != 0;
                                                break;
                                            case 14:
                                                装备数据4.当前铭栏.V = (byte)result72;
                                                break;
                                            case 15:
                                                装备数据4.洗练数一.V = result72;
                                                break;
                                            case 16:
                                                装备数据4.洗练数二.V = result72;
                                                break;
                                            case 17:
                                                装备数据4.物品状态.V = (byte)result72;
                                                break;
                                            case 27:
                                                装备数据4.失败次数.V = (byte)result72;
                                                break;
                                            case 28:
                                                装备数据4.升级属性.V = (byte)result72;
                                                break;
                                            case 29:
                                                装备数据4.铸魂次数.V = (byte)result72;
                                                break;
                                            case 30:
                                                装备数据4.扣除持久.V = result72;
                                                break;
                                            case 31:
                                                装备数据4.开启精炼.V = (byte)result72;
                                                break;
                                            case 32:
                                                装备数据4.精炼值一.V = (ushort)result72;
                                                break;
                                            case 33:
                                                装备数据4.精炼值二.V = (ushort)result72;
                                                break;
                                            case 34:
                                                装备数据4.精炼值三.V = (ushort)result72;
                                                break;
                                            case 35:
                                                装备数据4.精炼次数.V = (ushort)result72;
                                                break;
                                            case 18:
                                            case 19:
                                            case 20:
                                            case 21:
                                            case 22:
                                            case 23:
                                            case 24:
                                            case 25:
                                            case 26:
                                                return;
                                        }
                                        player.战力加成[装备数据4] = 装备数据4.装备战力;
                                        player.属性加成[装备数据4] = 装备数据4.装备属性;
                                        player.更新对象属性();
                                        player.更新玩家战力();
                                        player.更新物品详情(装备数据4);
                                    }
                                    else
                                    {
                                        if (result71 != 0)
                                        {
                                            return;
                                        }
                                        v4.当前持久.V = result72;
                                        player.更新物品详情(v4);
                                    }
                                    break;
                            }
                            break;
                        }
                    case ActionType.AddBodyItemLuck:
                        {
                            if (!byte.TryParse(param[0], out var result46))
                            {
                                break;
                            }
                            物品数据 物品数据4;
                            物品数据4 = null;
                            装备数据 v2;
                            v2 = null;
                            if (!player.角色装备.TryGetValue(result46, out v2))
                            {
                                break;
                            }
                            if (物品数据4 == null && v2 != null)
                            {
                                物品数据4 = v2;
                            }
                            if (!(物品数据4 is 装备数据 装备数据2))
                            {
                                break;
                            }
                            if (装备数据2.随机属性.Count <= 3)
                            {
                                string[] array5;
                                array5 = param[1].Split(',');
                                if (!int.TryParse(array5[主程.随机数.Next(array5.Length)], out var result47))
                                {
                                    break;
                                }
                                if (随机属性.数据表.TryGetValue(result47, out var value16))
                                {
                                    装备数据2.随机属性.Add(value16);
                                }
                            }
                            player.战力加成[装备数据2] = 装备数据2.装备战力;
                            player.属性加成[装备数据2] = 装备数据2.装备属性;
                            player.更新对象属性();
                            player.更新玩家战力();
                            player.更新物品详情(装备数据2);
                            break;
                        }
                    case ActionType.DelBodyItemRandomAbil:
                        {
                            if (!byte.TryParse(param[0], out var result37))
                            {
                                break;
                            }
                            物品数据 物品数据3;
                            物品数据3 = null;
                            装备数据 v;
                            v = null;
                            if (!player.角色装备.TryGetValue(result37, out v))
                            {
                                break;
                            }
                            if (物品数据3 == null && v != null)
                            {
                                物品数据3 = v;
                            }
                            if (!(物品数据3 is 装备数据 装备数据))
                            {
                                break;
                            }
                            string[] array4;
                            array4 = param[1].Split(',');
                            if (array4.Length == 0)
                            {
                                装备数据.随机属性.Clear();
                            }
                            else
                            {
                                string[] array2;
                                array2 = array4;
                                int result38;
                                for (int k = 0; k < array2.Length && int.TryParse(array2[k], out result38); k++)
                                {
                                    if (随机属性.数据表.TryGetValue(result38, out var value11))
                                    {
                                        装备数据.随机属性.Remove(value11);
                                    }
                                }
                            }
                            player.战力加成[装备数据] = 装备数据.装备战力;
                            player.属性加成[装备数据] = 装备数据.装备属性;
                            player.更新对象属性();
                            player.更新玩家战力();
                            player.更新物品详情(装备数据);
                            break;
                        }
                    case ActionType.ErrorMessage:
                        {
                            if (int.TryParse(param[0], out var result20))
                            {
                                if (param.Count < 2 || !int.TryParse(param[1], out var result21))
                                {
                                    result21 = 0;
                                }
                                if (param.Count < 3 || !int.TryParse(param[2], out var result22))
                                {
                                    result22 = 0;
                                }
                                player.发送封包(new 游戏错误提示
                                {
                                    错误代码 = result20,
                                    第一参数 = result21,
                                    第二参数 = result22
                                });
                                break;
                            }
                            return;
                        }
                    case ActionType.StartQuest:
                        {
                            if (int.TryParse(param[0], out var result19))
                            {
                                player.StartQuest(result19);
                                break;
                            }
                            return;
                        }
                    case ActionType.CallLua:
                        游戏脚本.调用Lua(player, param.ToArray());
                        break;
                    case ActionType.ChangeGuildLv:
                        {
                            int.TryParse(param[0], out var result18);
                            player.行会等级 = (byte)result18;
                            break;
                        }
                    case ActionType.GameGold:
                        {
                            if (uint.TryParse(param[1], out var result17))
                            {
                                player.修改货币("=", 游戏货币.元宝, NPCSegment.Calculate(param[0], player.元宝数量, result17));
                                //主程.WebLog(LogDataType.ScriptLog, Settings.统计UUID代码, Settings.游戏区服名称, "GameGold", player.角色数据.角色名字.V, player.角色数据.所属账号.V.账号名字.V, player.元宝数量.ToString(), "元宝");
                                break;
                            }
                            return;
                        }
                    case ActionType.GameGoldEx:
                        {
                            if (uint.TryParse(param[1], out var result152))
                            {
                                string key14;
                                key14 = this.ReplaceValue(player, param[2]);
                                if (游戏数据网关.角色数据表.检索表.TryGetValue(key14, out var value41))
                                {
                                    角色数据 角色数据8;
                                    角色数据8 = value41 as 角色数据;
                                    if (角色数据8.网络连接?.绑定角色 != null)
                                    {
                                        角色数据8.网络连接.绑定角色.修改货币("=", 游戏货币.元宝, NPCSegment.Calculate(param[0], 角色数据8.元宝数量, result152));
                                    }
                                    else
                                    {
                                        角色数据8.元宝数量 += NPCSegment.Calculate(param[0], 角色数据8.元宝数量, result152);
                                    }
                                    //主程.WebLog(LogDataType.ScriptLog, Settings.统计UUID代码, Settings.游戏区服名称, "GameGoldEx", 角色数据8.角色名字.V, 角色数据8.所属账号.V.账号名字.V, 角色数据8.元宝数量.ToString(), "元宝");
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case ActionType.ChangeSevenCarnival:
                        {
                            if (!byte.TryParse(param[0], out var result146) || !int.TryParse(param[2], out var result147))
                            {
                                return;
                            }
                            if (player.角色数据.七天进度.TryGetValue(result146, out var v9))
                            {
                                result147 = NPCSegment.Calculate(param[1], v9, result147);
                                if (result147 < 常量类.七天最大进度表[result146])
                                {
                                    player.修改七天进度(result146, result147);
                                }
                            }
                            break;
                        }
                    case ActionType.ChangeSelfVar:
                        {
                            if (byte.TryParse(param[0], out result114) && int.TryParse(param[2], out var result139))
                            {
                                if (!player.角色数据.角色变量.TryGetValue(result114, out var v8))
                                {
                                    v8 = 0;
                                }
                                v8 = NPCSegment.Calculate(param[1], v8, result139);
                                player.修改角色变量(result114, v8);
                                break;
                            }
                            return;
                        }
                    case ActionType.SetOnTimer:
                        {
                            if (byte.TryParse(param[0], out result114) && int.TryParse(param[1], out var result128) && int.TryParse(param[2], out var result129))
                            {
                                player.开启脚本定时器(result114, result128, result129);
                                break;
                            }
                            return;
                        }
                    case ActionType.AutoTakeOnItem:
                        {
                            if (int.TryParse(param[1], out var result126))
                            {
                                player.脚本穿戴背包装备(param[0], (byte)result126);
                                break;
                            }
                            return;
                        }
                    case ActionType.SkillLevel:
                        {
                            if (int.TryParse(param[1], out var result116) && ushort.TryParse(param[0], out var result117) && 铭文技能.数据表.TryGetValue((ushort)(result117 * 10), out var value31) && value31 != null)
                            {
                                player.玩家移除技能(value31.技能编号);
                                player.玩家学习技能(value31.技能编号, (byte)result116);
                                break;
                            }
                            return;
                        }
                    case ActionType.SetOffTimer:
                        if (byte.TryParse(param[0], out result114))
                        {
                            player.关闭脚本定时器(result114);
                            break;
                        }
                        return;
                    case ActionType.StartCastleWar:
                        地图处理网关.沙巴克攻城战立即开始();
                        break;
                    case ActionType.OpenCastleDoor:
                        地图处理网关.沙巴克城门开启();
                        break;
                    case ActionType.StopCastleWar:
                        地图处理网关.攻城战结束时间 = 主程.当前时间.AddSeconds(2.0);
                        网络服务网关.发送公告("沙巴克攻城战结束");
                        break;
                    case ActionType.SendItemRestore:
                        {
                            if (int.TryParse(param[0], out var result110))
                            {
                                if (result110 == 0)
                                {
                                    player.发送封包(new 玩家重铸装备
                                    {
                                        通知结果 = 0,
                                        返回编号 = 0,
                                        未知参数 = 0
                                    });
                                }
                                else
                                {
                                    player.发送封包(new 玩家重铸装备
                                    {
                                        通知结果 = 1,
                                        返回编号 = result110,
                                        未知参数 = 8
                                    });
                                }
                                break;
                            }
                            return;
                        }
                    case ActionType.InputBox:
                        {
                            string text10;
                            text10 = param[0] + "/" + param[1];
                            byte[] bytes;
                            bytes = Encoding.UTF8.GetBytes(text10 + "\0");
                            player.SendCustomMessage(1, bytes);
                            break;
                        }
                    case ActionType.SetChestKeyName:
                        {
                            string key9;
                            key9 = param[0];
                            if (!int.TryParse(param[1], out var result104))
                            {
                                result104 = 1;
                            }
                            if (player.探索道具 != null && 游戏物品.检索表.TryGetValue(key9, out var value30))
                            {
                                player.探索道具.KeyId = value30.物品编号;
                                player.探索道具.KeyCost = result104;
                            }
                            break;
                        }
                    case ActionType.SpecialRepairAll:
                        player.随身修理全部();
                        break;
                    case ActionType.GetItemCount:
                        {
                            string key7;
                            key7 = param[0];
                            string key8;
                            key8 = this.ReplaceValue(player, param[1]);
                            if (游戏物品.检索表.TryGetValue(key8, out var value29))
                            {
                                int num31;
                                num31 = player.统计物品数量(1, value29.物品名字);
                                this.AddVariable(player, key7, num31.ToString());
                            }
                            else
                            {
                                this.AddVariable(player, key7, "0");
                            }
                            break;
                        }
                    case ActionType.GetItemIdxCount:
                        {
                            string key5;
                            key5 = param[0];
                            string text7;
                            text7 = this.ReplaceValue(player, param[1]);
                            int result91;
                            result91 = 0;
                            if (!string.IsNullOrWhiteSpace(text7) && !int.TryParse(text7, out result91))
                            {
                                if (游戏物品.数据表.TryGetValue(result91, out var value25))
                                {
                                    int num25;
                                    num25 = player.统计物品数量(1, value25.物品名字);
                                    this.AddVariable(player, key5, num25.ToString());
                                }
                                else
                                {
                                    this.AddVariable(player, key5, "0");
                                }
                            }
                            else
                            {
                                this.AddVariable(player, key5, "0");
                            }
                            break;
                        }
                    case ActionType.ClearAttribute:
                        {
                            if (!byte.TryParse(this.ReplaceValue(player, param[0]), out var result83) || !player.角色装备.TryGetValue(result83, out var v7) || v7.随机属性.Count == 0)
                            {
                                return;
                            }
                            for (int num24 = v7.随机属性.Count - 1; num24 >= 0; num24--)
                            {
                                if (v7.随机属性[num24].对应属性 != 游戏对象属性.幸运等级 && v7.随机属性[num24].对应属性 != 游戏对象属性.幸运等级系数)
                                {
                                    v7.随机属性.RemoveAt(num24);
                                }
                            }
                            break;
                        }
                    case ActionType.AddBagItemLuck:
                        {
                            if (!byte.TryParse(param[0], out var result73) || !player.角色背包.TryGetValue(result73, out var v6) || !(v6 is 装备数据 装备数据5))
                            {
                                break;
                            }
                            if (装备数据5.随机属性.Count <= 3)
                            {
                                string[] array9;
                                array9 = param[1].Split(',');
                                if (!int.TryParse(array9[主程.随机数.Next(array9.Length)], out var result74))
                                {
                                    break;
                                }
                                if (随机属性.数据表.TryGetValue(result74, out var value22))
                                {
                                    装备数据5.随机属性.Add(value22);
                                }
                            }
                            player.战力加成[装备数据5] = 装备数据5.装备战力;
                            player.属性加成[装备数据5] = 装备数据5.装备属性;
                            player.更新对象属性();
                            player.更新玩家战力();
                            player.更新物品详情(装备数据5);
                            break;
                        }
                    case ActionType.DelBagItemRandomAbil:
                        {
                            if (!byte.TryParse(param[0], out var result56) || !player.角色背包.TryGetValue(result56, out var v3) || !(v3 is 装备数据 装备数据3))
                            {
                                break;
                            }
                            string[] array6;
                            array6 = param[1].Split(',');
                            if (array6.Length == 0)
                            {
                                装备数据3.随机属性.Clear();
                            }
                            else
                            {
                                string[] array2;
                                array2 = array6;
                                int result57;
                                for (int k = 0; k < array2.Length && int.TryParse(array2[k], out result57); k++)
                                {
                                    if (随机属性.数据表.TryGetValue(result57, out var value18))
                                    {
                                        装备数据3.随机属性.Remove(value18);
                                    }
                                }
                            }
                            player.战力加成[装备数据3] = 装备数据3.装备战力;
                            player.属性加成[装备数据3] = 装备数据3.装备属性;
                            player.更新对象属性();
                            player.更新玩家战力();
                            player.更新物品详情(装备数据3);
                            break;
                        }
                    case ActionType.ThrowItem:
                        {
                            游戏地图 游戏地图;
                            游戏地图 = 游戏地图.数据表.Values.FirstOrDefault((游戏地图 m) => m.地图名字.Equals(param[0], StringComparison.OrdinalIgnoreCase));
                            if (游戏地图 == null)
                            {
                                break;
                            }
                            地图实例 地图实例2;
                            地图实例2 = 地图处理网关.已分配地图(游戏地图.地图编号);
                            if (地图实例2 == null || !int.TryParse(param[1], out var result4) || !int.TryParse(param[2], out var result5) || result4 == 0 || result5 == 0)
                            {
                                break;
                            }
                            Point pos;
                            pos = new Point(result4, result5);
                            if (!int.TryParse(param[3], out var range) || !游戏物品.检索表.TryGetValue(param[4], out var value2))
                            {
                                break;
                            }
                            range = Math.Max(1, range);
                            if (!int.TryParse(param[5], out var result6) || result6 == 0 || !int.TryParse(param[6], out var result7))
                            {
                                break;
                            }
                            bool num;
                            num = param.Count > 7 && param[7] == "1";
                            bool num2;
                            num2 = param.Count > 8 && param[8] == "1";
                            bool flag;
                            flag = param.Count > 9 && param[9] == "1";
                            List<Point> list;
                            list = 地图实例2.地图区域.FirstOrDefault((地图区域 o) => o.区域类型 == 地图区域类型.随机区域)?.范围坐标?.Where((Point p) => 计算类.网格距离(pos, p) < range && 地图实例2.阻塞数量(p) == 0).ToList();
                            HashSet<角色数据> 物品归属;
                            物品归属 = (num2 ? new HashSet<角色数据>() : new HashSet<角色数据>
                    {
                        new 角色数据()
                    });
                            DateTime 归属时间;
                            归属时间 = (num2 ? 主程.当前时间 : 主程.当前时间.AddSeconds(result7));
                            if (num)
                            {
                                网络服务网关.发送公告($"{value2.物品名字} 掉落在 {游戏地图.地图名字} {result4},{result5}");
                            }
                            if (value2.持久类型 == 物品持久分类.堆叠 && flag)
                            {
                                pos = list[主程.随机数.Next(list.Count)];
                                new 物品实例(value2, null, 地图实例2, pos, 物品归属, result6).归属时间 = 归属时间;
                                break;
                            }
                            for (int n = 0; n < result6; n++)
                            {
                                pos = list[主程.随机数.Next(list.Count)];
                                new 物品实例(value2, null, 地图实例2, pos, 物品归属, 1).归属时间 = 归属时间;
                            }
                            break;
                        }
                    case ActionType.Log:
                        if (!string.IsNullOrWhiteSpace(param[0]))
                        {
                            主程.添加系统日志(param[0], hardLog: true, showDiag: false);
                        }
                        break;
                }
            }
        }

        private void Act(IList<NPCActions> acts, 怪物实例 monster)
        {
            for (int i = 0; i < acts.Count; i++)
            {
                NPCActions nPCActions;
                nPCActions = acts[i];
                List<string> list;
                list = nPCActions.Params.Select((string t) => this.FindVariable(monster, t)).ToList();
                for (int j = 0; j < list.Count; j++)
                {
                    string[] array;
                    array = list[j].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (array.Length != 0)
                    {
                        string[] array2;
                        array2 = array;
                        foreach (string text in array2)
                        {
                            list[j] = list[j].Replace(text, this.ReplaceValue(monster, text));
                        }
                    }
                }
                switch (nPCActions.Type)
                {
                    case ActionType.Param1:
                        {
                            if (int.TryParse(list[1], out var result11))
                            {
                                this.Param1 = list[0];
                                this.Param1Instance = result11;
                                break;
                            }
                            return;
                        }
                    case ActionType.Param2:
                        {
                            if (int.TryParse(list[0], out var result3))
                            {
                                this.Param2 = result3;
                                break;
                            }
                            return;
                        }
                    case ActionType.Param3:
                        {
                            if (int.TryParse(list[0], out var result4))
                            {
                                this.Param3 = result4;
                                break;
                            }
                            return;
                        }
                    case ActionType.Mongen:
                        {
                            if (this.Param1 == null || this.Param2 == 0 || this.Param3 == 0 || !int.TryParse(list[1], out var result5))
                            {
                                return;
                            }
                            try
                            {
                                if (!游戏怪物.数据表.TryGetValue(list[0], out var value2) || !游戏地图.数据表.TryGetValue((byte)this.Param1Instance, out var value3))
                                {
                                    return;
                                }
                                地图实例 出生地图;
                                出生地图 = 地图处理网关.已分配地图(value3.地图编号);
                                for (int l = 0; l < result5; l++)
                                {
                                    new 怪物实例(value2, 出生地图, int.MaxValue, new Point(this.Param2, this.Param3), 禁止复活: true, 立即刷新: true).存活时间 = DateTime.MaxValue;
                                }
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到怪物[" + list[0] + "]");
                            }
                            break;
                        }
                    case ActionType.NpcGen:
                        {
                            if (!byte.TryParse(list[0], out var result6))
                            {
                                result6 = 0;
                            }
                            if (!int.TryParse(list[1], out var result7) || !int.TryParse(list[2], out var result8) || !ushort.TryParse(list[3], out var result9))
                            {
                                return;
                            }
                            if (!int.TryParse(list[4], out var result10))
                            {
                                result10 = int.MaxValue;
                            }
                            try
                            {
                                if (!地图守卫.数据表.TryGetValue(result9, out var value4) || (!游戏地图.数据表.TryGetValue(result6, out var value5) && result6 != 0))
                                {
                                    return;
                                }
                                new 守卫实例(出生地图: (result6 == 0) ? monster.当前地图 : 地图处理网关.已分配地图(value5.地图编号), 对应模板: value4, 出生方向: 游戏方向.上方, 出生坐标: new Point(result7, result8)).存活时间 = 主程.当前时间.AddSeconds(result10);
                            }
                            catch (Exception)
                            {
                                主程.添加命令日志("未找到地图[" + list[0] + "]");
                            }
                            break;
                        }
                    case ActionType.Break:
                        this.Page.BreakFromSegments = true;
                        break;
                    case ActionType.LoadValue:
                        {
                            string key3;
                            key3 = list[0];
                            string fileName2;
                            fileName2 = list[1];
                            string section2;
                            section2 = list[2];
                            string key4;
                            key4 = list[3];
                            string text2;
                            text2 = new InIReader(fileName2).ReadString(section2, key4, "");
                            if (!(text2 == ""))
                            {
                                this.AddVariable(monster, key3, text2);
                            }
                            break;
                        }
                    case ActionType.SaveValue:
                        {
                            string fileName;
                            fileName = list[0];
                            string section;
                            section = list[1];
                            string key2;
                            key2 = list[2];
                            string value;
                            value = list[3];
                            new InIReader(fileName).Write(section, key2, value);
                            break;
                        }
                    case ActionType.Mov:
                        this.AddVariable(key: list[0], mapobject: monster, value: list[1]);
                        break;
                    case ActionType.Calc:
                        {
                            if (int.TryParse(list[0], out var result) & int.TryParse(list[2], out var result2))
                            {
                                try
                                {
                                    int num;
                                    num = NPCSegment.Calculate(list[1], result, result2);
                                    this.AddVariable(monster, list[3].Replace("-", ""), num.ToString());
                                }
                                catch (ArgumentException)
                                {
                                    主程.添加系统日志($"不正确的操作员: {list[1]}, Page: {this.Key}");
                                }
                            }
                            else
                            {
                                this.AddVariable(monster, list[3].Replace("-", ""), list[0] + list[2]);
                            }
                            break;
                        }
                }
            }
        }

        private void Success(玩家实例 player)
        {
            this.Act(this.ActList, player);
            List<string> collection;
            collection = this.ParseSay(player, new List<string>(this.Say));
            player.NPCSpeech.AddRange(collection);
        }

        private void Failed(玩家实例 player)
        {
            this.Act(this.ElseActList, player);
            List<string> collection;
            collection = this.ParseSay(player, new List<string>(this.ElseSay));
            player.NPCSpeech.AddRange(collection);
        }

        private void Success(怪物实例 Monster)
        {
            this.Act(this.ActList, Monster);
        }

        private void Failed(怪物实例 Monster)
        {
            this.Act(this.ElseActList, Monster);
        }

        private void Success()
        {
            this.Act(this.ActList);
        }

        private void Failed()
        {
            this.Act(this.ElseActList);
        }

        public static bool Compare<T>(string op, T left, T right) where T : IComparable<T>
        {
            return op switch
            {
                "!=" => !left.Equals(right),
                "==" => left.Equals(right),
                ">=" => left.CompareTo(right) >= 0,
                "<=" => left.CompareTo(right) <= 0,
                ">" => left.CompareTo(right) > 0,
                "<" => left.CompareTo(right) < 0,
                _ => throw new ArgumentException("Invalid comparison operator: {0}", op),
            };
        }

        public static MemberInfo GetFieldOrPropertyInfo(Type tp, string name)
        {
            MemberInfo[] member;
            member = tp.GetMember(name);
            int num;
            num = 0;
            if (0 >= member.Length)
            {
                return null;
            }
            return member[num];
        }

        public static uint Calculate(string op, uint left, uint right)
        {
            long num;
            num = right;
            long num2;
            num2 = 0L;
            return (uint)Math.Min(Math.Max(op switch
            {
                "=" => num,
                "/" => left / num,
                "*" => left * num,
                "-" => left - num,
                "+" => left + num,
                _ => throw new ArgumentException("Invalid sum operator: {0}", op),
            }, 0L), 4294967295L);
        }

        public static int Calculate(string op, int left, int right)
        {
            long num;
            num = right;
            long num2;
            num2 = 0L;
            return (int)Math.Min(Math.Max(op switch
            {
                "=" => num,
                "/" => left / num,
                "*" => left * num,
                "-" => left - num,
                "+" => left + num,
                _ => throw new ArgumentException("Invalid sum operator: {0}", op),
            }, -2147483648L), 2147483647L);
        }
    }
}
