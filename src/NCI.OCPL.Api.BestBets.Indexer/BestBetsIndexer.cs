using Microsoft.Extensions.Logging;
using NCI.OCPL.Api.BestBets.Indexer.Services;
using NCI.OCPL.Services.CDE.PublishedContentListing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Indexer
{
    /// <summary>
    /// This is the worker that does the indexing
    /// </summary>
    public class BestBetsIndexer
    {
        private readonly IBBIndexerService _indexerSvc = null;
        private readonly IPublishedContentListingService _listSvc = null;
        private readonly ITokenAnalyzerService _tokenService = null;
        private readonly ILogger<BestBetsIndexer> _logger = null;

        public BestBetsIndexer(
            IBBIndexerService indexerSvc, 
            IPublishedContentListingService listSvc,
            ITokenAnalyzerService tokenService,
            ILogger<BestBetsIndexer> logger
        ) {
            _indexerSvc = indexerSvc;
            _listSvc = listSvc;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// This actually runs the program.
        /// </summary>
        public void Run()
        {
            try
            {
                //This needs to be pulled into its own testable method.
                
                //Create new index
                string indexName = _indexerSvc.CreateTimeStampedIndex();
                _logger.LogInformation("Created Index {0}", indexName);

                //Fetch bunch of BB
                IPublishedContentListing bestBetsList = _listSvc.GetItemsForPath("BestBets", "/");

                //Index the BB
                int numBBIndexed = _indexerSvc.IndexBestBetsMatches(indexName, GetBestBetMatchesFromListing(indexName, bestBetsList));

                //Test the collection?

                //Optimize
                bool didOptimize = _indexerSvc.OptimizeIndex(indexName);
                _logger.LogInformation("Optimized Index {0}", indexName);

                //Swap Alias
                bool didSwap = _indexerSvc.MakeIndexCurrentAlias(indexName);
                _logger.LogInformation("Swapped Alias {0}");


                _logger.LogDebug("Indexed {0} Best Bets", numBBIndexed.ToString());
                //Clean Up old.
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while indexing. '{0}'", ex.Message);
                throw new Exception("We should really have a logger for this error.", ex);
            }
        }

        /// <summary>
        /// Gets the best bet matches from a IPublishedContentListing.
        /// </summary>
        /// <param name="analysisIndex">The index to use for token analysis</param>
        /// <param name="bestBetsList">The best bets list.</param>
        /// <returns>IEnumerator&lt;BestBetsMatch&gt;.</returns>
        private IEnumerable<BestBetsMatch> GetBestBetMatchesFromListing(string analysisIndex, IPublishedContentListing bestBetsList)
        {            
            try
            {
                this._tokenService.UseIndexName(analysisIndex);

                foreach (IPublishedContentInfo item in bestBetsList.Files)
                {
                    //Fetch Item
                    _logger.LogDebug("Fetching Category: {0}", item.FileName);
                    CancerGovBestBet bbCategory = _listSvc.GetPublishedFile<CancerGovBestBet>(item.FullWebPath);

                    _logger.LogDebug("Mapping Category: {0}", bbCategory.Name);

                    //Do we really need a mapper that does this per category?  
                    BestBetSynonymMapper mapper = new BestBetSynonymMapper(_tokenService, bbCategory);

                    foreach (BestBetsMatch match in mapper)
                    {
                        yield return match;
                    }
                }
            }
            finally
            {
                this._tokenService.UseDefaultIndexName();
            }
        }


    }
}
