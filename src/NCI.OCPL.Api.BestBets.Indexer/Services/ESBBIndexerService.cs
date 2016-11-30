using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nest;
using Microsoft.Extensions.Options;

namespace NCI.OCPL.Api.BestBets.Indexer.Services
{
    public class ESBBIndexerService : IBBIndexerService
    {
        private readonly IElasticClient _client;
        private readonly ESBBIndexerServiceOptions _config;


        /// <summary>
        /// Creates a new instance of an IBBIndexerService with an Elasticsearch backend.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="config"></param>
        public ESBBIndexerService(IElasticClient client, IOptions<ESBBIndexerServiceOptions> config)
        {
            this._client = client;
            this._config = config.Value;
        }


        public string CreateTimeStampedIndex()
        {
            throw new NotImplementedException();
        }

        public void DeleteOldIndices(DateTime olderThan)
        {
            throw new NotImplementedException();
        }

        public void IndexBestBetsMatches(string indexName, IEnumerable<BestBetsMatch> matches)
        {
            throw new NotImplementedException();
        }

        public void MakeIndexCurrentAlias(string indexName)
        {
            throw new NotImplementedException();
        }

        public void OptimizeIndex(string indexName)
        {
            throw new NotImplementedException();
        }
    }
}
