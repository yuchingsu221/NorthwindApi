using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Config
{
    public class AppSetting
    {
        public RelationDB RelationDB { get; set; }
        public bool EnableSwagger { get; set; }
        public string HttpWebRequestHost { get; set; }
    }

    public class RelationDB
    {       
        public string NorthwindApi_ConnectionString { get; set; }       
    }
}
