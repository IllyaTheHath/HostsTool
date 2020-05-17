using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HostsTool.Util
{
    public class Utilities
    {
        public static void FlushDNS()
        {
            using Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/C ipconfig /flushdns";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }

        public static async Task<String> GetStringAsync(String url)
        {
            String result = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 15;

            try
            {
                using var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
                using var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static void MixHosts(ref String hosts, String data, String title)
        {
            hosts += $"######### {title} Start #########";
            hosts += "\n" + data + "\n";
            hosts += $"#########  {title} End  #########";
            hosts += "\n\n";
        }
    }
}