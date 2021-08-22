using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFS.GenericAutonumberSeries.Main.Variables
{
    public class Constants
    {
        public class AutonumberConfigurationEntity
        {
            public const string cfs_name = "cfs_name";
            public const string cfs_prefix = "cfs_prefix";
            public const string cfs_separator = "cfs_separator";
            public const string cfs_count = "cfs_counter";
            public const string cfs_autonumberfield = "cfs_autonumberfield";
            public const string cfs_entityname = "cfs_entityname";
            public const string cfs_incrementby = "cfs_incrementbynew";


        }
        public class Entities
        {
            public const string account = "account";
            public const string opportunity = "opportunity";
            public const string lead = "lead";
            public const string contact = "contact";
            public const string cfs_autonumberingconfiguration = "cfs_autonumberingconfiguration";


        }
        public class AutonumberFields
        {

            public const string cfs_accountnumber = "cfs_accountnumber";
            public const string cfs_opportunitynumber = "cfs_opportunitynumber";
            public const string cfs_contactnumber = "cfs_contactnumber";

        }
        public class Operations
        {
            public const string create = "create";
            //public const string update = "update";
            //public const string delete = "delete";
        }
    }
}
