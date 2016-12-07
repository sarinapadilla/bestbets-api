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

            //So the alias command is a bulk operation and
            //is not done on a single index or alias, but 
            //it is a series of add/remove commands of which
            //require an alias name and an index.  So we need
            //to do a bit more with our command descriptor than
            //we usually do.  Always remember that for the compiler
            //that 
            // command(x => x.Param()); 
            // is the same as
            // command(x => { x.Param(); return x });
            var response = _client.Alias(a =>
            {
                //Add the new index
                a.Add(add => add
                    .Index(indexName)
                    .Alias(this._config.AliasName)
                );

                //Remove any old ones
                foreach (string oldIndex in indicesToRemove)
                {
                    a.Remove(del => del
                        .Index(oldIndex)
                        .Alias(this._config.AliasName)
                    );
                }

                return a;
            });

            if (response.IsValid)
            {
                throw new Exception("Error swapping indices for alias: " + this._config.AliasName, response.OriginalException);
            }

            //This returns the acknowledge true thing...
        }

        /// <summary>
        /// Gets the indices that are mapped to an alias.
        /// </summary>
        /// <param name="alias"></param>
        public string[] GetIndicesForAlias()
        {
            if (_client.AliasExists(aed => aed.Name(this._config.AliasName)).Exists)
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
                    throw new Exception("Error getting Indices for alias: " + this._config.AliasName, response.OriginalException);
                }
                else
                {
                    return response.Indices.Keys.ToArray();
                }
            } 
            else
            {
                return new string[] { };
            }
        }

        public void OptimizeIndex(string indexName)
        {
            var response = _client.ForceMerge(
                Indices.Index(indexName), 
                fmd => fmd
                    .MaxNumSegments(1)
                    .WaitForMerge(true)
                    .Index(indexName)
                    .RequestConfiguration(rcd => rcd.RequestTimeout(TimeSpan.FromSeconds(90)))
            );

            if (!response.IsValid)
            {
                throw new Exception("Error Optimizing index, " + indexName, response.OriginalException);
            }

            //An expected response has 1 shard that was successful.
            if (
                response.Shards.Total != 1 
                || response.Shards.Successful != 1 
                || response.Shards.Failed != 0
            )
            {
                throw new Exception("Error Optimizing index," + indexName + " optimize finished unexpected response");
            }
        }
    }
}
