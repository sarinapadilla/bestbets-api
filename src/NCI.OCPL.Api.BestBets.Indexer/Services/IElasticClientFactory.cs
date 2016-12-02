using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Indexer.Services
{
    public interface IElasticClientFactory
    {
        IElasticClient GetInstance();
    }
}
