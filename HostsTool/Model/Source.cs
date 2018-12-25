using System;
using System.Data;

namespace HostsTool.Model
{
    public sealed class Source
    {
        public Source() : base() { }

        public Source(IDataRecord record)
        {
            SourceGuid = record.GetGuid(record.GetOrdinal("sourceGuid"));
            SourceTitle = record.GetString(record.GetOrdinal("sourceTitle"));
            SourceType = (SourceType)record.GetInt32(record.GetOrdinal("sourceType"));
            SourceUrl = record["sourceUrl"] is DBNull ? null : (String)record["sourceUrl"];
            SourceEnable = record.GetBoolean(record.GetOrdinal("sourceEnable"));
            SourceContent = record["sourceContent"] is DBNull ? null : (String)record["sourceContent"];
        }

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
