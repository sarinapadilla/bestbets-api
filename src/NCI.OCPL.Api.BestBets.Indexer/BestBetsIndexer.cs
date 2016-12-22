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
        private readonly Logger<BestBetsIndexer> _logger = null;

        public BestBetsIndexer(
            IBBIndexerService indexerSvc, 
            IPublishedContentListingService listSvc,
            ITokenAnalyzerService tokenService,
            Logger<BestBetsIndexer> logger
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

                //Fetch bunch of BB
                IPublishedContentListing bestBetsList = _listSvc.GetItemsForPath("BestBets", "/");

                //Index the BB
                int numBBIndexed = _indexerSvc.IndexBestBetsMatches(indexName, GetBestBetMatchesFromListing(bestBetsList));

                //Test the collection?

                //Optimize
                bool didOptimize = _indexerSvc.OptimizeIndex(indexName);

                //Swap Alias
                bool didSwap = _indexerSvc.MakeIndexCurrentAlias(indexName);

                Console.WriteLine("Indexed " + numBBIndexed.ToString());
                //Clean Up old.
            }
            catch (Exception ex)
            {
                throw new Exception("We should really have a logger for this error.", ex);
            }
        }

        /// <summary>
        /// Gets the best bet matches from a IPublishedContentListing.
        /// </summary>
        /// <param name="bestBetsList">The best bets list.</param>
        /// <returns>IEnumerator&lt;BestBetsMatch&gt;.</returns>
        private IEnumerable<BestBetsMatch> GetBestBetMatchesFromListing(IPublishedContentListing bestBetsList)
        {
            foreach (IPublishedContentInfo item in bestBetsList.Files)
            {
                //Fetch Item
                CancerGovBestBet bbCategory = _listSvc.GetPublishedFile<CancerGovBestBet>(item.FullWebPath);

                //Do we really need a mapper that does this per category?  
                BestBetSynonymMapper mapper = new BestBetSynonymMapper(_tokenService, bbCategory);

                foreach (BestBetsMatch match in mapper)
                {
                    yield return match;
                }
            }
        }


    }
}
