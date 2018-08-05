using System;
using System.Data;

namespace HostsTool.Model
{
    public class Source
    {
        public Guid SourceGuid { get; set; }
        public String SourceTitle { get; set; }
        public Int32 SourceType { get; set; }
        public String SourceUrl { get; set; }
        public Boolean SourceEnable { get; set; }
        public String SourceContent { get; set; }

        public static Source Create(IDataRecord record)
        {
            return new Source
            {
                SourceGuid = record.GetGuid(record.GetOrdinal("sourceGuid")),
                SourceTitle = (String)record["sourceTitle"],
                SourceType = record.GetInt32(record.GetOrdinal("sourceType")),
                SourceUrl = record["sourceUrl"] is DBNull ? null : (String)record["sourceUrl"],
                SourceEnable = record.GetBoolean(record.GetOrdinal("sourceEnable")),
                SourceContent = record["sourceContent"] is DBNull ? null : (String)record["sourceContent"]
            };
        }
    }
}
