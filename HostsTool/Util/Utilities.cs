using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

using HostsTool.Data;

namespace HostsTool.Util
{
    internal static class Utilities
    {
        public static void FlushDNS()
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/C ipconfig /flushdns";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
        }

        public static async Task<String> GetStringAsync(String url)
        {
            String result;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 30;

            try
            {
                using (HttpWebResponse response = 
                    (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }catch (Exception)
            {
                throw;
            }
            
            return result;
        }

        public static void MixHosts(ref String hosts, String piece, String title)
        {
            hosts += String.Format("######### {0} Start #########",title);
            hosts += "\n" + piece + "\n";
            hosts += String.Format("#########  {0} End  #########", title);
            hosts += "\n\n";
        }

        public static async void CheckUpdateAsync()
        {
            String xmlString;
            try
            {
                xmlString = await GetStringAsync(SharedInfo.UpdateXMLUrl);
            }
            catch
            {
                return;
            }

            String date = String.Empty;
            String latestVersion = String.Empty;
            String note = String.Empty;
            String nowVersion = SharedInfo.Version;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            using (XmlNodeList nodeList = doc.ChildNodes)
            {
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "UpdateInfo")
                    {
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            if (child.Name == "UpdateTime")
                            {
                                date = child.InnerText;
                            }
                            if (child.Name == "Version")
                            {
                                latestVersion = child.InnerText;
                            }
                            if (child.Name == "Note")
                            {
                                note = child.InnerText;
                            }
                        }
                        break;
                    }
                }
            }

            if (new Version(latestVersion) > new Version(nowVersion))
            {
                var mbr = System.Windows.MessageBox.Show(
                    String.Format("发现新版本：\n" +
                                  "当前版本：{0}\n" + 
                                  "最新版本：{1}\n" + 
                                  "更新日期：{2}\n" + 
                                  "更新说明：{3}\n\n" + 
                                  "是否前往下载更新？",
                                  nowVersion,latestVersion,date,note),
                    "Hosts Tool",
                    System.Windows.MessageBoxButton.OKCancel);
                if (mbr == System.Windows.MessageBoxResult.OK)
                {
                    Process.Start(SharedInfo.UpdateUrl);
                }
            }
        }
    }
}
