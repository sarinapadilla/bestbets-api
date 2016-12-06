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
            if (String.IsNullOrWhiteSpace(this._config.AliasName))
            {
                throw new ArgumentNullException("CreateTimeStampedIndex: The name of the alias is required.");
            }

            //Get timestamp
            String timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            //Setup the index name
            String indexName = String.Join("", this._config.AliasName, timeStamp);

            //Create the index.  Since we are using a index template, there are no
            //actual parameters.
            var response = _client.CreateIndex(indexName);

            if (!response.IsValid)
                throw new Exception("Error creating Time Stamped Index, " + indexName);

            return indexName;
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
            //Get the Indices assigned to an Alias
            string[] indicesToRemove = GetIndicesForAlias();

            


            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the indices that are mapped to an alias.
        /// </summary>
        /// <param name="alias"></param>
        public string[] GetIndicesForAlias()
        {
            //Soo, if we ask for alias where the index name is that alias,
            //then it is like we asked for host/<alias>/_aliases and then
            //it will return only those indices mapped to that alias.
            var response = _client.GetAlias(ga => 
                ga
                .Index(_config.AliasName)
            );

            if (!response.IsValid)
            {
                throw new Exception("Error getting Indices for alias");
            }
            else
            {
                return response.Indices.Keys.ToArray();
            }
        }

        public void OptimizeIndex(string indexName)
        {
            throw new NotImplementedException();
        }
    }
}
