using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MoreLinq; //Adds .Batch used for indexing terms

namespace NCI.OCPL.Api.BestBets.Indexer.Services
{
    public class ESBBIndexerService : IBBIndexerService
    {
        private readonly IElasticClient _client;
        private readonly ESBBIndexerServiceOptions _config;
        private readonly ILogger<ESBBIndexerService> _logger;


        /// <summary>
        /// Creates a new instance of an IBBIndexerService with an Elasticsearch backend.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="config"></param>
        public ESBBIndexerService(IElasticClient client,
            IOptions<ESBBIndexerServiceOptions> config,
            ILogger<ESBBIndexerService> logger)
        {
            this._client = client;
            this._config = config.Value;
            this._logger = logger;
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
            {
                _logger.LogError("Elasticsearch Response is Not Valid creating timestamped index '{0}'.", indexName);
                _logger.LogError("Returned debug info: {0}.", response.DebugInformation);
                throw new Exception("Error creating Time Stamped Index, " + indexName);
            }

            return indexName;
        }

        /// <summary>
        /// This will remove any indices older than a given date
        /// </summary>
        /// <param name="olderThan">The date</param>
        /// <param name="minIndices">The minimum number of indices to keep regardless of date.</param>
        /// <returns>An array of indices that were deleted.</returns>
        public string[] DeleteOldIndices(DateTime olderThan, int minIndices)
        {

            Tuple<string[], string[]> indices = GetIndices(olderThan);
            string[] oldIndices = indices.Item1;
            string[] newIndices = indices.Item2;

            if (oldIndices.Length > 0)
            {
                //There are old candidates, but will we have minIndices left when finished.
                int diff = minIndices - newIndices.Length;
                if (diff > 0)
                {
                    //Skip past the old items.  P.S. If oldIndices is less than diff, then 
                    //Skip will return empty array.  yay!!
                    oldIndices = oldIndices.Skip(diff).ToArray();
                }


                if (oldIndices.Length > 0)
                {
                    var indicesToDelete = from index in oldIndices
                                          select new IndexName()
                                          {
                                              Name = index
                                          };

                    var response = _client.DeleteIndex(Indices.Index(indicesToDelete));

                    if (!response.IsValid)
                    {
                        _logger.LogError("Elasticsearch Response is Not Valid deleting indices '{0}'.", oldIndices);
                        _logger.LogError("Returned debug info: {0}.", response.DebugInformation);
                        throw new Exception(String.Format("Error deleting Indices, {0}", oldIndices));
                    }
                }
            }

            return oldIndices;
        }

        /// <summary>
        /// This will find all indices older than a given date
        /// </summary>
        /// <param name="olderThan">The date</param>
        /// <returns>Two arrays indices, the first older than the date, the second newer than the date.</returns>
        private Tuple<string[], string[]> GetIndices(DateTime olderThan)
        {
            var response = _client.GetIndexSettings(gis => gis
                   .Index(this._config.AliasName + "*")
                   .FlatSettings(true)
                   .Name(new Names(new string[] { "index.creation_date" }))
            );

            if (!response.IsValid)
            {
                _logger.LogError("Elasticsearch Response is not Valid when checking old indices for alias '{};", this._config.AliasName);
                _logger.LogError("Returned debug info: {0}.", response.DebugInformation);
                throw new Exception("Error Getting Index Settings, " + this._config.AliasName);
            }

            //The ElasticSearch index creation_date is the number of milliseconds since
            //the Unix Epoch (1/1/1970)
            double unixTime = olderThan.Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            if (response.Indices != null)
            {
                //Sort the Indices
                var sortedIndices = from indexKey in response.Indices.Keys                                    
                                    select new
                                    {
                                        IndexName = indexKey,
                                        CreateDate = double.Parse((string)response.Indices[indexKey].Settings["index.creation_date"])
                                    } into tmpIndex
                                    orderby tmpIndex.CreateDate descending
                                    select tmpIndex;

                //Pull old Indices
                var oldIndices = from index in sortedIndices
                                 where index.CreateDate < unixTime
                                 select index.IndexName;

                //Pull newer Indices
                var newIndices = from index in sortedIndices
                                 where index.CreateDate >= unixTime
                                 select index.IndexName;                                                 

                return Tuple.Create(oldIndices.ToArray(), newIndices.ToArray());

            }

            return Tuple.Create(new string[] { }, new string[] { });
        }


        public int IndexBestBetsMatches(string indexName, IEnumerable<BestBetsMatch> matches, int batchSize = 1000)
        {

            

            int totalIndexed = 0;

            //Note: IEnumerable<T>.Batch comes from moreLinq.
            foreach(IEnumerable<BestBetsMatch> matchGroup in matches.Batch(batchSize))
            {
                var res = _client.IndexMany<BestBetsMatch>(matchGroup, index: indexName);

                if (!res.IsValid)
                {
                    //You know what, there was an error.
                    //res.DebugInformation will tell us why
                    //res.ItemsWithErrors is a collection of the items, with an error message for each.
                    //for now, stuff broke, exit.
                    throw new Exception("Error occurred while indexing matches to " + indexName);
                }
                else
                {
                    //Update the number of items added.
                    totalIndexed += res.Items.Count();
                }
            }

            return totalIndexed;
        }

        public bool MakeIndexCurrentAlias(string indexName)
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

            if (!response.IsValid)
            {
                throw new Exception("Error swapping indices for alias: " + this._config.AliasName, response.OriginalException);
            }

            return true;
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

        public bool OptimizeIndex(string indexName)
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
                _logger.LogError("Error Optimizing index, '{0}'.", indexName);
                _logger.LogError("Returned debug info: {0}.", response.DebugInformation);
                throw new Exception("Error Optimizing index, " + indexName, response.OriginalException);
            }

            // An expected response has at least one shard and none that failed.
            if (
                response.Shards.Total < 1 
                || response.Shards.Successful != response.Shards.Total
                || response.Shards.Failed != 0
            )
            {
                _logger.LogError("Shard count: Total: {0}, Successful : {1}, Failed {2}",
                    response.Shards.Total, response.Shards.Successful, response.Shards.Failed);
                throw new Exception("Error Optimizing index," + indexName + " optimize finished unexpected response");
            }

            return true; //The task succeeded.
        }

    }
}
