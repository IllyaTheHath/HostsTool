using System;
using System.IO;
using System.Reflection;

namespace HostsTool.Util
{
    public sealed class StaticInfo
    {
        public static String HostsPath => Environment.SystemDirectory + @"\drivers\etc\hosts";

        public static String TempFolderPath => Path.GetTempPath();

        public static String SoftPath => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static String DefaultHosts =>
@"# Localhost
127.0.0.1   localhost
::1         localhost";

        public static String Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
