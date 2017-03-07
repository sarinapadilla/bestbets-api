using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Indexer.Services
{
    public class ElasticSearchOptions
    {
        public string Servers { get; set; }
        public string Userid { get; set; }
        public string Password { get; set; }
        public int MaximumRetries { get; set; }
    }
}
