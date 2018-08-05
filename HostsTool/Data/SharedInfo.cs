using System;
using System.IO;
using System.Reflection;

namespace HostsTool.Data
{
    internal static class SharedInfo
    {
        public static String HostsPath =>
            Environment.SystemDirectory + @"\drivers\etc\hosts";

        public static String TempFolderPath => 
            Path.GetTempPath();

        public static String SoftPath =>
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static String DefaultHosts =>
            "# Localhost" + "\n" +
            "127.0.0.1	localhost" + "\n" +
            "::1	    localhost";
        
        public static String UpdateXMLUrl => 
            "https://raw.githubusercontent.com/IllyaTheHath/HostsTool/master/HostsTool/UpdateFile.xml";

        public static String UpdateUrl =>
            "https://github.com/IllyaTheHath/HostsTool/releases/latest";

        public static String Version =>
           Assembly.GetExecutingAssembly().GetName().Version.ToString();

    }
}
