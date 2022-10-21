using _51Sole.DJG.Common;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace _1688Spider
{
    class Program
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
        public static List<string> UAList = new List<string> { "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2226.0 Safari/537.36", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36", "Mozilla/5.0 (Windows NT 4.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36", "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.3319.102 Safari/537.36", "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1467.0 Safari/537.36", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.2 Safari/537.36", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.60 Safari/537.17", "Mozilla/5.0 (X11; Linux x86_64; rv:28.0) Gecko/20100101  Firefox/28.0", "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.93 Safari/537.36", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.8; rv:24.0) Gecko/20100101 Firefox/24.0", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:21.0) Gecko/20130514 Firefox/21.0", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_2) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1309.0 Safari/537.17", "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:21.0) Gecko/20100101 Firefox/21.0", "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_6; sv-se) AppleWebKit/533.20.25 (KHTML, like Gecko) Version/5.0.4 Safari/533.20.27", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.60 Safari/537.17", "Mozilla/5.0 (Windows NT 4.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36" };
        protected static string listPage = "https://s.1688.com/company/company_search.htm";
        static void Main(string[] args)
        {
            List<string> keywords = GetKeyWords();
            foreach (var keyword in keywords)
            {
                var d = Task.Run(() => TaskRun(listPage, keyword));
                d.Wait();
            }
            //ParallelLoopResult result = Parallel.For(0, 2, x =>
            //{
            //    foreach (var keyword in keywords.Where(k => keywords.IndexOf(k) % 2 == x))
            //    {
            //        var d = Task.Run(() => TaskRun(listPage, keyword));
            //        d.Wait();
            //    }
            //});
            Console.WriteLine("全部完成");
            Console.Read();
        }
        async static Task TaskRun(string url, string keyword)
        {
            try
            {
                //string ip = string.Empty;
                //string errmsg = "";
                //List<string> IpList = new List<string>();
                //while (string.IsNullOrEmpty(ip))
                //{
                //    IpList = IpHelper.GetAvilableIpList(out errmsg);
                //    if (!string.IsNullOrEmpty(errmsg))
                //    {
                //        Console.WriteLine(errmsg);
                //    }
                //    else
                //    {
                //        int index = new Random().Next(0, IpList.Count());
                //        ip = IpList[index];
                //        Console.WriteLine("当前代理IP:" + ip);
                //    }
                //    Thread.Sleep(new Random().Next(100, 5000));
                //}
                LaunchOptions options = new LaunchOptions
                {
                    Headless = true,
                    IgnoredDefaultArgs = new string[]
                    {
                        "--enable-automation"
                    },
                    ExecutablePath = @".local-chromium\Win64-901912\chrome-win\chrome.exe",
                    Args = new[] {
                    //string.Format("--proxy-server={0}",ip),
                    "--start-maximized",//最大窗口
                    "--disable-infobars",//--隐藏自动化标题
                    "--no-sandbox",
                    "–single-process",
                    "–disable-gpu",
                    "--disable-setuid-sandbox",
                    "--ignore-certificate-errors",
                    "--app=https://www.baidu.com/",
                    "--blink-settings=imagesEnabled=false"//不加载图片
                },
                    IgnoreHTTPSErrors = true
                };
                //var extra = new PuppeteerExtra();
                //extra.Use(new StealthPlugin());
                using (var browser = await Puppeteer.LaunchAsync(options))
                {
                    try
                    {
                        Console.WriteLine("开始处理关键词:" + keyword);
                        string userAgent = UAList[new Random().Next(0, UAList.Count())];
                        var newPage = await browser.NewPageAsync();
                        newPage.DefaultTimeout = 30000;
                        //await newPage.SetUserAgentAsync(userAgent);
                        ViewPortOptions vOptions = new ViewPortOptions
                        {
                            Width = 1920,
                            Height = 1080
                        };
                        //隐藏webdriver特征
                        //await newPage.EvaluateExpressionOnNewDocumentAsync("delete navigator.__proto__.webdriver;");
                        await newPage.SetViewportAsync(vOptions);
                        List<string> sqlList = new List<string>();
                        try
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            await newPage.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);
                            await newPage.TypeAsync("#alisearch-input", keyword);
                            Thread.Sleep(2000);
                            try
                            {
                                await newPage.ClickAsync(".baxia-dialog-close");
                            }
                            catch { }
                            
                            Thread.Sleep(1000);
                            await newPage.ClickAsync(".single");
                            //await newPage.WaitForNavigationAsync();
                            //url=newPage.Url + "&province=广东&city=汕头";
                            //await newPage.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);
                            Thread.Sleep(5000);
                            string pageStr = await newPage.EvaluateExpressionAsync<string>("document.querySelector('.fui-paging-num').innerText");
                            int totalPage = Dc.ToInt(pageStr);
                            totalPage = totalPage > 50 ? 50 : totalPage;
                            int page = 0;
                            while (true)
                            {
                                page++;
                                Console.WriteLine($"第{page}页");
                                await newPage.EvaluateExpressionAsync("window.scrollBy({top:1000,behavior:'smooth'})");
                                Thread.Sleep(2000);
                                await newPage.EvaluateExpressionAsync("window.scrollBy({top:1000,behavior:'smooth'})");
                                Thread.Sleep(2000);
                                var hrefSelector = @"Array.from(document.querySelectorAll('.title-container [class=\'company-name\']')).map(a=>a.href)";
                                var hrefs = await newPage.EvaluateExpressionAsync<string[]>(hrefSelector);
                                var nameSelector = @"Array.from(document.querySelectorAll('.title-container [class=\'company-name\']')).map(a=>a.innerText)";
                                var names = await newPage.EvaluateExpressionAsync<string[]>(nameSelector);
                                if (hrefs != null)
                                {
                                    string sql1 = @" select distinct CompanyName from Temp_1688CompanyInfo where CompanyName in ('{0}')";

                                    sql1 = string.Format(sql1, string.Join("','", names));
                                    List<string> newHrefs = new List<string>();
                                    DataSet ds1= DB.DataSet(ConnectionString, sql1);
                                    if(DataHelper.ExistsDataSet(ds1))
                                    {
                                       
                                        List<string> existList = (from d in ds1.Tables[0].AsEnumerable() select d.Field<string>("CompanyName")).ToList();
                                        
                                        List<string> notExistList = names.ToList().Except(existList).ToList();
                                        foreach(var name in notExistList)
                                        {
                                            int index = names.ToList().IndexOf(name);
                                            newHrefs.Add(hrefs[index]);
                                        }
                                    }
                                    else
                                    {
                                        newHrefs = hrefs.ToList();
                                    }
                                    using (var dPage = await browser.NewPageAsync())
                                    {
                                        dPage.DefaultTimeout = 15000;
                                        await dPage.SetViewportAsync(vOptions);
                                        foreach (var href in newHrefs.Distinct().ToList())
                                        {
                                            try
                                            {
                                                if (string.IsNullOrEmpty(href) || href.Contains("dj.1688.com"))
                                                {
                                                    continue;
                                                }
                                                string shopUrl = href.Substring(0, href.LastIndexOf('/')) + "/page/contactinfo.htm";
                                                Console.WriteLine(shopUrl);
                                                try
                                                {
                                                    await dPage.GoToAsync(shopUrl, WaitUntilNavigation.DOMContentLoaded);
                                                }
                                                catch
                                                {
                                                    continue;
                                                }
                                                if (dPage != null && !dPage.IsClosed)
                                                {
                                                    Thread.Sleep(300);
                                                    var companyNameSelector = @"document.querySelector('#bd_0_container_0 > div > div.module-wrapper > div:nth-child(1) > div:nth-child(2)').innerText";
                                                    string companyName = "";
                                                    try
                                                    {
                                                        companyName = await dPage.EvaluateExpressionAsync<string>(companyNameSelector);
                                                    }
                                                    catch
                                                    {
                                                        Console.WriteLine("获取公司名异常!");
                                                        continue;
                                                    }
                                                    if (companyName.Length < 5)
                                                    {
                                                        continue;
                                                    }
                                                    string existSql = "select ISNULL((select top 1 1  from Temp_1688CompanyInfo where CompanyName='" + companyName + "' and isdel=0),0)";
                                                    int cnt = Dc.ToInt(DB.Scalar(ConnectionString, existSql));
                                                    if (cnt == 0)
                                                    {
                                                        string addressSelector = "document.querySelector('#bd_0_container_0 > div > div.module-wrapper > div:nth-child(1) > div:nth-child(3) > div:nth-child(4) > div:nth-child(2)').innerText";
                                                        string address = string.Empty;
                                                        string contactMobile = string.Empty;
                                                        string contact = string.Empty;
                                                        try
                                                        {
                                                            address = await dPage.EvaluateExpressionAsync<string>(addressSelector);
                                                            contact = await dPage.EvaluateExpressionAsync<string>("document.querySelector('#bd_0_container_0 > div > div.module-wrapper > div:nth-child(1) > div:nth-child(4) > div:nth-child(3)').innerText");
                                                            contactMobile = await dPage.EvaluateExpressionAsync<string>("document.querySelector('#bd_0_container_0 > div > div.module-wrapper>div:nth-child(1)>div:nth-child(3)>div:nth-child(2)>div:nth-child(2)').innerText");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine(ex.Message);
                                                            continue;
                                                        }
                                                        //if (string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(contactMobile) || contactMobile == "暂无")
                                                        //{
                                                        //    continue;
                                                        //}
                                                        string provName = string.Empty;
                                                        string cityName = string.Empty;
                                                        //if (!string.IsNullOrEmpty(address))
                                                        //{
                                                        //    var res = Analysis(address);
                                                        //    provName = res.province;
                                                        //    cityName = res.city;
                                                        //}
                                                        string sql = "IF NOT EXISTS(select top 1 * from Temp_1688CompanyInfo where CompanyName=@CompanyName) INSERT INTO Temp_1688CompanyInfo(CompanyName,MainProduct,CompanyAddress,ProvName,CityName,ContactPerson,ContactMobile,CompanySetUpTime,TradeAmount,Area,TotalEmployees,keyword) values(@CompanyName,@MainProduct,@CompanyAddress,@ProvName,@CityName,@ContactPerson,@ContactMobile,@CompanySetUpTime,@TradeAmount,@Area,@TotalEmployees,@keyword);";
                                                        SqlParameter[] para = {
                                                                new SqlParameter("@CompanyName",SqlDbType.VarChar,500){ Value=companyName },
                                                                new SqlParameter("@MainProduct",SqlDbType.VarChar,50){ Value="" },
                                                                new SqlParameter("@CompanyAddress",SqlDbType.VarChar,50){ Value=address },
                                                                new SqlParameter("@ProvName",SqlDbType.VarChar,50){ Value=provName },
                                                                new SqlParameter("@CityName",SqlDbType.VarChar,50){ Value=cityName },
                                                                new SqlParameter("@ContactPerson",SqlDbType.VarChar){ Value=contact },
                                                                new SqlParameter("@ContactMobile",SqlDbType.VarChar,50){ Value=contactMobile },
                                                                new SqlParameter("@CompanySetUpTime",SqlDbType.VarChar,50){ Value="" },
                                                                new SqlParameter("@TradeAmount",SqlDbType.VarChar,50){ Value=""},
                                                                new SqlParameter("@Area",SqlDbType.VarChar,50){ Value="" },
                                                                new SqlParameter("@TotalEmployees",SqlDbType.VarChar,50){ Value="" },
                                                                new SqlParameter("@keyword",SqlDbType.VarChar,50) { Value=keyword },
                                                        };
                                                        try
                                                        {
                                                            DB.Query(ConnectionString, sql, para);
                                                            Console.WriteLine("插入成功："+companyName);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("插入公司信息异常:" + ex.Message);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //string contact = string.Empty;
                                                        //try
                                                        //{
                                                        //    contact = await dPage.EvaluateExpressionAsync<string>("document.querySelector('#bd_0_container_0 > div > div.module-wrapper > div:nth-child(1) > div:nth-child(4) > div:nth-child(3)').innerText");
                                                        //}
                                                        //catch (Exception ex)
                                                        //{
                                                        //    Console.WriteLine(ex.Message);
                                                        //    continue;
                                                        //}
                                                        string sql = "update Temp_1688CompanyInfo set keyword=keyword+'," + keyword + "' where companyName='" + companyName+"' and keyword not like '%"+keyword+"%'";
                                                        try
                                                        {
                                                            DB.Query(ConnectionString, sql);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("更新异常:" + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }
                                        }
                                    }

                                    try
                                    {
                                        //点击下一页
                                        await newPage.EvaluateExpressionAsync("document.querySelector('.fui-next').click()");
                                        Thread.Sleep(1000);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("点击下一页异常:" + ex.Message);
                                    }
                                    if (page >= totalPage)
                                    {
                                        break;
                                    }
                                }
                            }
                            try
                            {
                                DB.Query(ConnectionString, "update Temp_1688KeyWords set isprocess=1 where keyword='" + keyword + "';");
                                Console.WriteLine("关键词:" + keyword + "已处理");
                            }
                            catch
                            {

                            }
                            sw.Stop();
                            Console.WriteLine("耗时:" + sw.ElapsedMilliseconds + "ms");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("当前打开页面链接异常:" + ex.Message);
                            if (ex.Message.Contains("ERR_TUNNEL_CONNECTION_FAILED") || ex.Message.Contains("ERR_PROXY_CONNECTION_FAILED"))
                            {
                                if (browser != null && !browser.IsClosed)
                                {
                                    await browser.CloseAsync();//关闭浏览器
                                }                         //重新运行任务
                                var d = Task.Run(() => TaskRun(url, keyword));
                                d.Wait();
                                return;
                            }
                        }
                        if (newPage != null && !newPage.IsClosed)
                        {
                            await newPage.CloseAsync();
                        }
                        if (browser != null && !browser.IsClosed)
                        {
                            await browser.CloseAsync();//关闭浏览器
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("global:" + ex.Message);
                        if (browser != null && !browser.IsClosed)
                        {
                            await browser.CloseAsync();
                            var d = Task.Run(() => TaskRun(url, keyword));//递归执行
                            d.Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ";" + ex.InnerException);
            }
        }
        public static List<string> GetKeyWords()
        {
            List<string> keywordList = new List<string>();
            string sql = "select keyword  from  Temp_1688KeyWords where isprocess=0 order by keyword";
            DataSet ds = DB.DataSet(ConnectionString, sql);
            if (DataHelper.ExistsDataSet(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    keywordList.Add(Dc.ToStr(dr["keyword"]));
                }
            }
            return keywordList;
        }
        public static dynamic Analysis(string address)
        {
            address = ReplaceToProV(address);
            string regex = "(?<province>[^省]+自治区|.*?省|.*?行政区|.*?市)?(?<city>[^市]+自治州|.*?地区|.*?行政单位|.+盟|市辖区|.*?市|.*?县)?(?<county>[^县]+县|.+区|.+市|.+旗|.+海域|.+岛)?(?<town>[^区]+区|.+镇)?(?<village>.*)";
            var m = Regex.Match(address, regex, RegexOptions.IgnoreCase);
            var province = m.Groups["province"].Value;
            var city = m.Groups["city"].Value;
            if (!province.Contains("省") && !province.Contains("自治区"))
            {
                province = "";
            }
            else if (province.Contains("省") && province.Length > 4)
            {
                province = "";
            }
            if (!city.Contains("市"))
            {
                city = "";
            }
            else if (city.Length > 4)
            {
                city = "";
            }
            return new { province, city };
        }

        public static string ReplaceToProV(string address)
        {
            List<string> provList = new List<string>
            {
                "河北","辽宁","黑龙江","吉林","山东","山西","安徽","浙江","江苏","江西","广东","福建","海南","河南","湖北","湖南" ,"四川" ,"云南" ,"贵州" ,"陕西" ,"甘肃" ,"青海"
            };
            if (!address.Contains("省"))
            {
                foreach (var prov in provList)
                {
                    if (address.StartsWith(prov))
                    {
                        address = address.Replace(prov, prov + "省");
                        break;
                    }
                }
            }
            return address;
        }
    }
}
