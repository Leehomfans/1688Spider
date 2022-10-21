using _51Sole.DJG.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace _1688Spider
{
    public class IpHelper
    {
        public static List<string> GetAvilableIpList(out string errmsg)
        {

            List<string> ipList = new List<string>();
            int code = 200;
            errmsg = "";
            string apiUrl = "http://http.tiqu.letecs.com/getip3?num=50&type=2&pro=0&city=0&yys=100017&port=11&time=1&ts=0&ys=0&cs=0&lb=1&sb=0&pb=4&mr=1&regions=350000,430000,440000&gm=4";
            string res = HttpHelper.GetWebRequest(apiUrl, Encoding.UTF8, out code);
            if (string.IsNullOrEmpty(res))
            {
                errmsg = "接口数据返回为空!";
                return ipList;
            }
            RetModel ret = JsonConvert.DeserializeObject<RetModel>(res);
            if (ret != null && ret.data.Count() > 0)
            {
                foreach (var item in ret.data)
                {
                    if (PingIp(item.ip))
                    {
                        ipList.Add(item.ip + ":" + item.port);
                    }
                }
            }
            else
            {
                errmsg = ret.msg;
                if(ret.code==113)
                {
                    string ip = errmsg.Replace("请添加白名单","");
                    apiUrl = "https://wapi.http.linkudp.com/index/index/save_white?neek=&appkey=&white="+ip;
                    res = HttpHelper.GetWebRequest(apiUrl, Encoding.UTF8, out code);
                    if (string.IsNullOrEmpty(res))
                    {
                        errmsg = "添加白名单失败!";
                        return ipList;
                    }
                    apiUrl = "http://http.tiqu.letecs.com:81/getip3?num=50&type=2&pro=0&city=0&yys=100017&port=11&time=1&ts=0&ys=0&cs=0&lb=1&sb=0&pb=4&mr=1&regions=350000,430000,440000&gm=4";
                    res = HttpHelper.GetWebRequest(apiUrl, Encoding.UTF8, out code);
                    if (string.IsNullOrEmpty(res))
                    {
                        errmsg = "接口数据返回为空!";
                        return ipList;
                    }
                    ret = JsonConvert.DeserializeObject<RetModel>(res);
                    if (ret != null && ret.data.Count() > 0)
                    {
                        foreach (var item in ret.data)
                        {
                            if (PingIp(item.ip))
                            {
                                ipList.Add(item.ip + ":" + item.port);
                            }
                        }
                    }
                }
            }
            return ipList;
        }
        private static bool PingIp(string strIP)
        {
            bool bRet = false;
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(strIP, 30);
                if (reply.Status == IPStatus.Success)
                    bRet = true;
            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }
        public class RetModel
        {
            public int code { get; set; }
            public string msg { get; set; }
            public List<IpModel> data { get; set; }
        }
        public class IpModel
        {
            public string ip { get; set; }
            public int port { get; set; }
        }
    }
}
