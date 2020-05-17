using System;

namespace HostsTool.Model
{
    public sealed class Source
    {
        public Guid SourceGuid { get; set; }
        public String SourceTitle { get; set; }
        public SourceType SourceType { get; set; }
        public String SourceUrl { get; set; }
        public Boolean SourceEnable { get; set; }
        public String SourceContent { get; set; }
    }

    public enum SourceType
    {
        /// <summary>
        /// 本地源
        /// </summary>
        Local = 0,

        /// <summary>
        /// 网络源
        /// </summary>
        Web = 1
    }
}